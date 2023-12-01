using ITRIProject.Models;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using NPOI.POIFS.Crypt.Dsig;
using System.Collections;
using System.Linq;
using System.Text.Json;
using System.Text.Json.Nodes;

namespace ITRIProject.Controllers
{
    public class ResultController : CommonController
    {
        private MyDbContext _MyDbContext;
        public ResultController(MyDbContext MyDbContext)
        {
            _MyDbContext = MyDbContext;
        }
        public ActionResult OrganizeReport(int id)
        {
            try
            {
                var Workinghour = _MyDbContext.Workinghours.Where(a => a.BasicId == id);
                decimal? WorkinghourTotal = 0;
                foreach (var work in Workinghour)
                {
                    if ((work.Item == "0" || work.Item == "1") && work.TotalWorkHour != null)
                    {
                        WorkinghourTotal += work.TotalWorkHour * (work.DistributeRatio != null ? work.DistributeRatio / 100 : 1);
                    }
                }

                if (WorkinghourTotal != null)
                {
                    ViewBag.WorkinghourResult = WorkinghourTotal * (decimal)0.0000015938 * (decimal)28;
                }
                else
                {
                    ViewBag.WorkinghourResult = (decimal)0.00;
                }
                var Coefficient = _MyDbContext.Coefficients.ToList();
                var energyuseData = _MyDbContext.Set<Energyuse>()
                    .Where(e => e.BasicId == id)
                    .Select(e => new
                    {
                        Id = e.Id,
                        BasicId = e.BasicId,
                        dataname = e.EnergyName,
                        dataValue = (e.BeforeUnit == "14" ? 0.001m : e.BeforeUnit == "22" ? 0.001m : e.BeforeUnit == "25" ? 0.001m : 1 ) * e.BeforeTotal,
                        DistributeRatio = e.DistributeRatio
                    }).ToList();

                var refrigerantHaveData = _MyDbContext.Set<RefrigerantHave>()
                    .Where(r => r.BasicId == id)
                    .Select(r => new
                    {
                        Id = r.Id,
                        BasicId = r.BasicId,
                        dataname = r.RefrigerantType,
                        dataValue = (r.EquipmentType == "71" ? 0.003m : r.EquipmentType == "72" ? 0.08m : r.EquipmentType == "73" ? 0.225m : r.EquipmentType == "75" ? 0.085m : r.EquipmentType == "76" ? 0.055m : r.EquipmentType == "77" ? 0.15m : 1) * (r.Unit == "10" ? 0.001m : 1) * r.Total,
                        DistributeRatio = r.DistributeRatio
                    }).ToList();

                var refrigerantNoneData = _MyDbContext.Set<RefrigerantNone>()
                    .Where(r => r.BasicId == id)
                    .Select(r => new
                    {
                        Id = r.Id,
                        BasicId = r.BasicId,
                        dataname = r.RefrigerantType,
                        dataValue = (r.EquipmentType == "71" ? 0.003m : r.EquipmentType == "72" ? 0.08m : r.EquipmentType == "73" ? 0.225m : r.EquipmentType == "75" ? 0.085m : r.EquipmentType == "76" ? 0.055m : r.EquipmentType == "77" ? 0.15m : 1) * (r.Unit == "10" ? 0.001m : 1) * r.RefrigerantWeight,
                        DistributeRatio = r.DistributeRatio
                    }).ToList();

                var fireequipmentData = _MyDbContext.Set<Fireequipment>()
                    .Where(f => f.BasicId == id)
                    .Select(f => new
                    {
                        Id = f.Id,
                        BasicId = f.BasicId,
                        dataname = f.EquipmentName,
                        dataValue = (f.BeforeUnit == "10" ? 0.001m : 1) * f.BeforeTotal,
                        DistributeRatio = f.DistributeRatio
                    }).ToList();

                var result = energyuseData
                    .Union(refrigerantHaveData)
                    .Union(refrigerantNoneData)
                    .Union(fireequipmentData)
                    .AsEnumerable() // 将查询结果加载到内存中
                        .Join(
                            _MyDbContext.Coefficients,
                            a => a.dataname,
                            b => b.Id.ToString(),
                        (a, b) => new { TableA = a, TableB = b } // 使用匿名类型包装两个表的列

                        )
                    .GroupBy(e => new { e.TableA.BasicId, e.TableB.EmissionSource })
                    .Select(g => new
                    {
                        BasicId = g.Key.BasicId,
                        EmissionSource = g.Key.EmissionSource,
                        dataValue = g.Sum(e => e.TableA.dataValue * e.TableA.DistributeRatio / 100
                        * ((e.TableB.Co2coefficient.HasValue ? e.TableB.Co2coefficient : 0) +
                        ((e.TableB.Ch4coefficient.HasValue && e.TableB.Ch4gwp.HasValue) ? (e.TableB.Ch4coefficient * e.TableB.Ch4gwp) : 0) +
                        ((e.TableB.N2ocoefficient.HasValue && e.TableB.N2ogwp.HasValue) ? (e.TableB.N2ocoefficient * e.TableB.N2ogwp) : 0) +
                        (e.TableB.HfcsGwp.HasValue ? e.TableB.HfcsGwp : 0) +
                        (e.TableB.PfcsGwp.HasValue ? e.TableB.PfcsGwp : 0) +
                        (e.TableB.Sf6gwp.HasValue ? e.TableB.Sf6gwp : 0))
                        )
                    })
                    .ToList();
                ViewBag.result = result;

                var ResourceuseData = _MyDbContext.Set<Resourceuse>()
                .Where(e => e.BasicId == id)
                .Select(e => new
                {
                    Id = e.Id,
                    BasicId = e.BasicId,
                    dataname = e.EnergyName,
                    dataValue = (e.BeforeUnit == "22" ? 0.001m : 1) * e.BeforeTotal,
                    DistributeRatio = e.DistributeRatio
                }).ToList();

                var ResourceuseResult = ResourceuseData
                .AsEnumerable() // 将查询结果加载到内存中
                    .Join(
                        _MyDbContext.Coefficients,
                        a => a.dataname,
                        b => b.Id.ToString(),
                    (a, b) => new { TableA = a, TableB = b } // 使用匿名类型包装两个表的列
                    )
                .GroupBy(e => new { e.TableA.BasicId })
                .Select(g => new
                {
                    BasicId = g.Key.BasicId,
                    dataValue = g.Sum(e => e.TableA.dataValue * e.TableA.DistributeRatio / 100
                    * ((e.TableB.Co2coefficient.HasValue ? e.TableB.Co2coefficient : 0) +
                    ((e.TableB.Ch4coefficient.HasValue && e.TableB.Ch4gwp.HasValue) ? (e.TableB.Ch4coefficient * e.TableB.Ch4gwp) : 0) +
                    ((e.TableB.N2ocoefficient.HasValue && e.TableB.N2ogwp.HasValue) ? (e.TableB.N2ocoefficient * e.TableB.N2ogwp) : 0) +
                    ((e.TableB.N2ocoefficient.HasValue && e.TableB.N2ogwp.HasValue) ? (e.TableB.N2ocoefficient * e.TableB.N2ogwp) : 0) +
                    (e.TableB.HfcsGwp.HasValue ? e.TableB.HfcsGwp : 0) +
                    (e.TableB.PfcsGwp.HasValue ? e.TableB.PfcsGwp : 0) +
                    (e.TableB.Sf6gwp.HasValue ? e.TableB.Sf6gwp : 0))
                    )
                })
                .ToList();
                ViewBag.ResourceuseResult = ResourceuseResult;

                var EnergyuseTransportation = _MyDbContext.Set<Energyuse>()
                .Where(e => e.BasicId == id)
                .Join(_MyDbContext.DTransportations.Where(d => d.BindWhere == "EnergyUse"),
                a => a.Id,
                b => b.BindId,
                (a, b) => new { TableA = a, TableB = b })
                .Select(e => new
                {
                    Id = e.TableA.Id,
                    BasicId = e.TableA.BasicId,
                    dataValue = (e.TableA.EnergyName=="4" ? 0.7573m : e.TableA.EnergyName == "5" ? 0.8325m : 1) * (e.TableA.BeforeUnit=="14" ? 0.001m:1) *e.TableA.BeforeTotal,
                    DistributeRatio = e.TableA.DistributeRatio,
                    Method = e.TableB.Method,
                    Car = e.TableB.Car,
                    Land = e.TableB.Land,
                    Sea = e.TableB.Sea,
                    Air = e.TableB.Air,
                }).ToList();

                var ResourceuseTransportation = _MyDbContext.Set<Resourceuse>()
                .Where(e => e.BasicId == id)
                .Join(_MyDbContext.DTransportations.Where(d => d.BindWhere == "ResourceUse"),
                a => a.Id,
                b => b.BindId,
                (a, b) => new { TableA = a, TableB = b })
                .Select(e => new
                {
                    Id = e.TableA.Id,
                    BasicId = e.TableA.BasicId,
                    dataValue = e.TableA.BeforeTotal,
                    DistributeRatio = e.TableA.DistributeRatio,
                    Method = e.TableB.Method,
                    Car = e.TableB.Car,
                    Land = e.TableB.Land,
                    Sea = e.TableB.Sea,
                    Air = e.TableB.Air,
                }).ToList();

                var DumptreatmentTransportation = _MyDbContext.Set<DumptreatmentOutsourcing>()
                .Where(e => e.BasicId == id)
                .Join(_MyDbContext.DTransportations.Where(d => d.BindWhere == "DumptreatmentOutsourcing"),
                a => a.Id,
                b => b.BindId,
                (a, b) => new { TableA = a, TableB = b })
                .Select(e => new
                {
                    Id = e.TableA.Id,
                    BasicId = e.TableA.BasicId,
                    dataValue = (e.TableA.BeforeUnit == "10" ? 0.001m : 1) * e.TableA.BeforeTotal,
                    DistributeRatio = e.TableA.DistributeRatio,
                    Method = e.TableB.Method,
                    Car = e.TableB.Car,
                    Land = e.TableB.Land,
                    Sea = e.TableB.Sea,
                    Air = e.TableB.Air,
                }).ToList();

                var TransportationResult = EnergyuseTransportation
                    .Union(ResourceuseTransportation)
                    .Union(DumptreatmentTransportation)
                .AsEnumerable() // 将查询结果加载到内存中
                .GroupBy(e => new { e.BasicId })
                .Select(g => new
                {
                    BasicId = g.Key.BasicId,
                    dataValue = g.Sum(e => e.dataValue * e.DistributeRatio / 100
                    * ((e.Method != "61" && e.Method != "62" && e.Land != null ? e.Land * _MyDbContext.Coefficients.FirstOrDefault(a => a.Type == "Transportation" && a.Name == "陸運(km)").Co2coefficient * (e.Car == "63" ? (decimal)0.131 : e.Car == "64" ? (decimal)0.587 : 1) : 0)//大貨車63,小貨車64
                    + (e.Method == "59" && e.Sea != null ? e.Sea * _MyDbContext.Coefficients.FirstOrDefault(a => a.Type == "Transportation" && a.Name == "海運(km)").Co2coefficient : 0)
                    + (e.Method == "60" && e.Air != null ? e.Air * _MyDbContext.Coefficients.FirstOrDefault(a => a.Type == "Transportation" && a.Name == "空運(km)").Co2coefficient : 0))
                    )
                })
                .ToList();
                ViewBag.TransportationResult = TransportationResult;

                var DumptreatmentData = _MyDbContext.Set<DumptreatmentOutsourcing>()
                .Where(d => d.BasicId == id)
                .Select(d => new
                {
                    Id = d.Id,
                    BasicId = d.BasicId,
                    dataname = d.WasteId.ToString(),
                    dataValue = (d.BeforeUnit == "10" ? 0.001m : 1) * d.BeforeTotal,
                    DistributeRatio = d.DistributeRatio
                }).ToList();

                var DumptreatmentResult = DumptreatmentData
                .AsEnumerable() // 将查询结果加载到内存中
                    .Join(
                        _MyDbContext.Coefficients,
                        a => a.dataname,
                        b => b.Id.ToString(),
                    (a, b) => new { TableA = a, TableB = b } // 使用匿名类型包装两个表的列
                    )
                .GroupBy(e => new { e.TableA.BasicId })
                .Select(g => new
                {
                    BasicId = g.Key.BasicId,
                    dataValue = g.Sum(e => e.TableA.dataValue * e.TableA.DistributeRatio / 100
                    * ((e.TableB.Co2coefficient.HasValue ? e.TableB.Co2coefficient : 0) +
                    ((e.TableB.Ch4coefficient.HasValue && e.TableB.Ch4gwp.HasValue) ? (e.TableB.Ch4coefficient * e.TableB.Ch4gwp) : 0) +
                    ((e.TableB.N2ocoefficient.HasValue && e.TableB.N2ogwp.HasValue) ? (e.TableB.N2ocoefficient * e.TableB.N2ogwp) : 0) +
                    (e.TableB.HfcsGwp.HasValue ? e.TableB.HfcsGwp : 0) +
                    (e.TableB.PfcsGwp.HasValue ? e.TableB.PfcsGwp : 0) +
                    (e.TableB.Sf6gwp.HasValue ? e.TableB.Sf6gwp : 0))
                    )
                })
                .ToList();
                ViewBag.DumptreatmentResult = DumptreatmentResult;

                return View();
            }
            catch (Exception ex)
            {
                var Log = new Log()
                {
                    WhereFunction = "Result_OrganizeReport_get",
                    Exception = ex.ToString(),
                    DateTime = DateTime.Now,
                };
                _MyDbContext.Logs.Add(Log);
                _MyDbContext.SaveChanges();
                Log = null;
                return Error("內部伺服器錯誤"); // 或其他適當的錯誤訊息
            }
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult OrganizeReport(int basicid, int id, string type, IFormCollection? collection)
        {
            try
            {
                return RedirectToAction("EPAReport", "Result", new { id = id });//有下一個頁面時，要改為導向下一個頁面
            }
            catch (Exception ex)
            {
                var Log = new Log()
                {
                    WhereFunction = "Result_OrganizeReport_post",
                    Exception = ex.ToString(),
                    DateTime = DateTime.Now,
                };
                _MyDbContext.Logs.Add(Log);
                _MyDbContext.SaveChanges();
                Log = null;
                return Error("內部伺服器錯誤"); // 或其他適當的錯誤訊息
            }
        }
        public ActionResult EPAReport(int id)
        {
            try
            {
                var Workinghour = _MyDbContext.Workinghours.Where(a => a.BasicId == id);
                decimal? WorkinghourTotal = 0;
                foreach (var work in Workinghour)
                {
                    if ((work.Item == "0" || work.Item == "1") && work.TotalWorkHour != null)
                    {
                        WorkinghourTotal += work.TotalWorkHour * (work.DistributeRatio != null ? work.DistributeRatio / 100 : 1);
                    }
                }

                if (WorkinghourTotal != null)
                {
                    ViewBag.WorkinghourResult = WorkinghourTotal * (decimal)0.0000015938 * (decimal)28;
                }
                else
                {
                    ViewBag.WorkinghourResult = (decimal)0.00;
                }

                var EnergyuseTransportation = _MyDbContext.Set<Energyuse>()
                .Where(e => e.BasicId == id)
                .Join(_MyDbContext.DTransportations.Where(d => d.BindWhere == "EnergyUse"),
                a => a.Id,
                b => b.BindId,
                (a, b) => new { TableA = a, TableB = b })
                .Select(e => new
                {
                    Id = e.TableA.Id,
                    BasicId = e.TableA.BasicId,
                    dataValue = (e.TableA.EnergyName == "4" ? 0.7573m : e.TableA.EnergyName == "5" ? 0.8325m : 1) * (e.TableA.BeforeUnit == "14" ? 0.001m : 1) * e.TableA.BeforeTotal,
                    DistributeRatio = e.TableA.DistributeRatio,
                    Method = e.TableB.Method,
                    Car = e.TableB.Car,
                    Land = e.TableB.Land,
                    Sea = e.TableB.Sea,
                    Air = e.TableB.Air,
                }).ToList();

                var ResourceuseTransportation = _MyDbContext.Set<Resourceuse>()
                .Where(e => e.BasicId == id)
                .Join(_MyDbContext.DTransportations.Where(d => d.BindWhere == "ResourceUse"),
                a => a.Id,
                b => b.BindId,
                (a, b) => new { TableA = a, TableB = b })
                .Select(e => new
                {
                    Id = e.TableA.Id,
                    BasicId = e.TableA.BasicId,
                    dataValue = e.TableA.BeforeTotal,
                    DistributeRatio = e.TableA.DistributeRatio,
                    Method = e.TableB.Method,
                    Car = e.TableB.Car,
                    Land = e.TableB.Land,
                    Sea = e.TableB.Sea,
                    Air = e.TableB.Air,
                }).ToList();

                var DumptreatmentTransportation = _MyDbContext.Set<DumptreatmentOutsourcing>()
                .Where(e => e.BasicId == id)
                .Join(_MyDbContext.DTransportations.Where(d => d.BindWhere == "DumptreatmentOutsourcing"),
                a => a.Id,
                b => b.BindId,
                (a, b) => new { TableA = a, TableB = b })
                .Select(e => new
                {
                    Id = e.TableA.Id,
                    BasicId = e.TableA.BasicId,
                    dataValue = (e.TableA.BeforeUnit == "10" ? 0.001m : 1) * e.TableA.BeforeTotal,
                    DistributeRatio = e.TableA.DistributeRatio,
                    Method = e.TableB.Method,
                    Car = e.TableB.Car,
                    Land = e.TableB.Land,
                    Sea = e.TableB.Sea,
                    Air = e.TableB.Air,
                }).ToList();

                var TransportationResult = EnergyuseTransportation
                    .Union(ResourceuseTransportation)
                    .Union(DumptreatmentTransportation)
                .AsEnumerable() // 将查询结果加载到内存中
                .GroupBy(e => new { e.BasicId })
                .Select(g => new
                {
                    BasicId = g.Key.BasicId,
                    dataValue = g.Sum(e => e.dataValue * e.DistributeRatio / 100
                    * ((e.Method != "61" && e.Method != "62" && e.Land != null ? e.Land * _MyDbContext.Coefficients.FirstOrDefault(a => a.Type == "Transportation" && a.Name == "陸運(km)").Co2coefficient * (e.Car == "63" ? (decimal)0.131 : e.Car == "64" ? (decimal)0.587 : 1) : 0)//大貨車63,小貨車64
                    + (e.Method == "59" && e.Sea != null ? e.Sea * _MyDbContext.Coefficients.FirstOrDefault(a => a.Type == "Transportation" && a.Name == "海運(km)").Co2coefficient : 0)
                    + (e.Method == "60" && e.Air != null ? e.Air * _MyDbContext.Coefficients.FirstOrDefault(a => a.Type == "Transportation" && a.Name == "空運(km)").Co2coefficient : 0))
                    )
                })
                .ToList();

                decimal? TransportationResultData = TransportationResult.Sum(item => item.dataValue);
                ViewBag.TransportationResultData = Math.Round((decimal)TransportationResultData, 6);
                var energyuseData = _MyDbContext.Set<Energyuse>()
                    .Where(e => e.BasicId == id)
                    .Select(e => new
                    {
                        Id = e.Id,
                        BasicId = e.BasicId,
                        dataname = e.EnergyName,
                        dataValue = (e.BeforeUnit == "14" ? 0.001m : e.BeforeUnit == "22" ? 0.001m : e.BeforeUnit == "25" ? 0.001m : 1) * e.BeforeTotal,
                        DistributeRatio = e.DistributeRatio
                    }).ToList();

                var ResourceuseData = _MyDbContext.Set<Resourceuse>()
                    .Where(e => e.BasicId == id)
                    .Select(e => new
                    {
                        Id = e.Id,
                        BasicId = e.BasicId,
                        dataname = e.EnergyName,
                        dataValue = (e.BeforeUnit == "22" ? 0.001m : 1) * e.BeforeTotal,
                        DistributeRatio = e.DistributeRatio
                    }).ToList();

                var refrigerantHaveData = _MyDbContext.Set<RefrigerantHave>()
                    .Where(r => r.BasicId == id)
                    .Select(r => new
                    {
                        Id = r.Id,
                        BasicId = r.BasicId,
                        dataname = r.RefrigerantType,
                        dataValue = (r.EquipmentType == "71" ? 0.003m : r.EquipmentType == "72" ? 0.08m : r.EquipmentType == "73" ? 0.225m : r.EquipmentType == "75" ? 0.085m : r.EquipmentType == "76" ? 0.055m : r.EquipmentType == "77" ? 0.15m : 1) * (r.Unit == "10" ? 0.001m : 1) * r.Total,
                        DistributeRatio = r.DistributeRatio
                    }).ToList();

                var refrigerantNoneData = _MyDbContext.Set<RefrigerantNone>()
                    .Where(r => r.BasicId == id)
                    .Select(r => new
                    {
                        Id = r.Id,
                        BasicId = r.BasicId,
                        dataname = r.RefrigerantType,
                        dataValue = (r.EquipmentType == "71" ? 0.003m : r.EquipmentType == "72" ? 0.08m : r.EquipmentType == "73" ? 0.225m : r.EquipmentType == "75" ? 0.085m : r.EquipmentType == "76" ? 0.055m : r.EquipmentType == "77" ? 0.15m : 1) * (r.Unit == "10" ? 0.001m : 1) * r.RefrigerantWeight,
                        DistributeRatio = r.DistributeRatio
                    }).ToList();

                var fireequipmentData = _MyDbContext.Set<Fireequipment>()
                    .Where(f => f.BasicId == id)
                    .Select(f => new
                    {
                        Id = f.Id,
                        BasicId = f.BasicId,
                        dataname = f.EquipmentName,
                        dataValue = (f.BeforeUnit == "10" ? 0.001m : 1) * f.BeforeTotal,
                        DistributeRatio = f.DistributeRatio
                    }).ToList();

                var DumptreatmentData = _MyDbContext.Set<DumptreatmentOutsourcing>()
                    .Where(d => d.BasicId == id)
                    .Select(d => new
                    {
                        Id = d.Id,
                        BasicId = d.BasicId,
                        dataname = d.WasteId.ToString(),
                        dataValue = (d.BeforeUnit == "10" ? 0.001m : 1) * d.BeforeTotal,
                        DistributeRatio = d.DistributeRatio
                    }).ToList();

                var AllGHG = energyuseData
                    .Union(ResourceuseData)
                    .Union(refrigerantHaveData)
                    .Union(refrigerantNoneData)
                    .Union(fireequipmentData)
                    .Union(DumptreatmentData)
                    .AsEnumerable() // 将查询结果加载到内存中
                        .Join(
                            _MyDbContext.Coefficients,
                            a => a.dataname,
                            b => b.Id.ToString(),
                        (a, b) => new { TableA = a, TableB = b } // 使用匿名类型包装两个表的列

                        )
                    .GroupBy(e => new { e.TableA.BasicId })
                    .Select(g => new
                    {
                        BasicId = g.Key.BasicId,
                        CO2 = g.Sum(e => e.TableA.dataValue * e.TableA.DistributeRatio / 100 * (e.TableB.Co2coefficient.HasValue ? e.TableB.Co2coefficient : 0)) + TransportationResultData,
                        CH4 = g.Sum(e => e.TableA.dataValue * e.TableA.DistributeRatio / 100 * ((e.TableB.Ch4coefficient.HasValue && e.TableB.Ch4gwp.HasValue) ? (e.TableB.Ch4coefficient * e.TableB.Ch4gwp) : 0)),
                        N2O = g.Sum(e => e.TableA.dataValue * e.TableA.DistributeRatio / 100 * ((e.TableB.N2ocoefficient.HasValue && e.TableB.N2ogwp.HasValue) ? (e.TableB.N2ocoefficient * e.TableB.N2ogwp) : 0)),
                        HFCs = g.Sum(e => e.TableA.dataValue * e.TableA.DistributeRatio / 100 * (e.TableB.HfcsGwp.HasValue ? e.TableB.HfcsGwp : 0)),
                        PFCs = g.Sum(e => e.TableA.dataValue * e.TableA.DistributeRatio / 100 * (e.TableB.PfcsGwp.HasValue ? e.TableB.PfcsGwp : 0)),
                        SF6 = g.Sum(e => e.TableA.dataValue * e.TableA.DistributeRatio / 100 * (e.TableB.Sf6gwp.HasValue ? e.TableB.Sf6gwp : 0)),
                    })
                    .ToList();
                ViewBag.AllGHG = AllGHG;

                var Category1GHG = energyuseData
                    .Union(ResourceuseData)
                    .Union(refrigerantHaveData)
                    .Union(refrigerantNoneData)
                    .Union(fireequipmentData)
                    .Union(DumptreatmentData)
                    .AsEnumerable() // 将查询结果加载到内存中
                        .Join(
                            _MyDbContext.Coefficients.Where(a => a.Category == "113"),//範疇一
                            a => a.dataname,
                            b => b.Id.ToString(),
                        (a, b) => new { TableA = a, TableB = b } // 使用匿名类型包装两个表的列

                        )
                    .GroupBy(e => new { e.TableA.BasicId })
                    .Select(g => new
                    {
                        BasicId = g.Key.BasicId,
                        CO2 = g.Sum(e => e.TableA.dataValue * e.TableA.DistributeRatio / 100 * (e.TableB.Co2coefficient.HasValue ? e.TableB.Co2coefficient : 0)) + TransportationResultData,
                        CH4 = g.Sum(e => e.TableA.dataValue * e.TableA.DistributeRatio / 100 * ((e.TableB.Ch4coefficient.HasValue && e.TableB.Ch4gwp.HasValue) ? (e.TableB.Ch4coefficient * e.TableB.Ch4gwp) : 0)),
                        N2O = g.Sum(e => e.TableA.dataValue * e.TableA.DistributeRatio / 100 * ((e.TableB.N2ocoefficient.HasValue && e.TableB.N2ogwp.HasValue) ? (e.TableB.N2ocoefficient * e.TableB.N2ogwp) : 0)),
                        HFCs = g.Sum(e => e.TableA.dataValue * e.TableA.DistributeRatio / 100 * (e.TableB.HfcsGwp.HasValue ? e.TableB.HfcsGwp : 0)),
                        PFCs = g.Sum(e => e.TableA.dataValue * e.TableA.DistributeRatio / 100 * (e.TableB.PfcsGwp.HasValue ? e.TableB.PfcsGwp : 0)),
                        SF6 = g.Sum(e => e.TableA.dataValue * e.TableA.DistributeRatio / 100 * (e.TableB.Sf6gwp.HasValue ? e.TableB.Sf6gwp : 0)),
                    })
                    .ToList();
                ViewBag.Category1GHG = Category1GHG;

                var EmissionSource = energyuseData
                    .Union(ResourceuseData)
                    .Union(refrigerantHaveData)
                    .Union(refrigerantNoneData)
                    .Union(fireequipmentData)
                    .Union(DumptreatmentData)
                    .AsEnumerable() // 将查询结果加载到内存中
                        .Join(
                            _MyDbContext.Coefficients,//範疇一
                            a => a.dataname,
                            b => b.Id.ToString(),
                        (a, b) => new { TableA = a, TableB = b } // 使用匿名类型包装两个表的列

                        )
                    .GroupBy(e => new { e.TableA.BasicId, e.TableB.EmissionSource })
                    .Select(g => new
                    {
                        BasicId = g.Key.BasicId,
                        EmissionSource = g.Key.EmissionSource,
                        dataValue = g.Sum(e => e.TableA.dataValue * e.TableA.DistributeRatio / 100
                        * ((e.TableB.Co2coefficient.HasValue ? e.TableB.Co2coefficient : 0) +
                        ((e.TableB.Ch4coefficient.HasValue && e.TableB.Ch4gwp.HasValue) ? (e.TableB.Ch4coefficient * e.TableB.Ch4gwp) : 0) +
                        ((e.TableB.N2ocoefficient.HasValue && e.TableB.N2ogwp.HasValue) ? (e.TableB.N2ocoefficient * e.TableB.N2ogwp) : 0) +
                        (e.TableB.HfcsGwp.HasValue ? e.TableB.HfcsGwp : 0) +
                        (e.TableB.PfcsGwp.HasValue ? e.TableB.PfcsGwp : 0) +
                        (e.TableB.Sf6gwp.HasValue ? e.TableB.Sf6gwp : 0))
                        )
                    })
                    .ToList();
                ViewBag.EmissionSource = EmissionSource;

                return View();
            }
            catch (Exception ex)
            {
                var Log = new Log()
                {
                    WhereFunction = "Result_EPAReport_get",
                    Exception = ex.ToString(),
                    DateTime = DateTime.Now,
                };
                _MyDbContext.Logs.Add(Log);
                _MyDbContext.SaveChanges();
                Log = null;
                return Error("內部伺服器錯誤"); // 或其他適當的錯誤訊息
            }
        }

        public ActionResult Chart(int id)
        {
            try
            {
                var Workinghour = _MyDbContext.Workinghours.Where(a => a.BasicId == id);
                decimal? WorkinghourTotal = 0, WorkinghourTotalResult = 0;
                foreach (var work in Workinghour)
                {
                    if ((work.Item == "0" || work.Item == "1") && work.TotalWorkHour != null)
                    {
                        WorkinghourTotal += work.TotalWorkHour * (work.DistributeRatio != null ? work.DistributeRatio / 100 : 1);
                    }
                }

                if (WorkinghourTotal != null)
                {
                    WorkinghourTotalResult = WorkinghourTotal * (decimal)0.0000015938 * (decimal)28;
                }
                else
                {
                    WorkinghourTotalResult = (decimal)0.00;
                }
                var EnergyuseTransportation = _MyDbContext.Set<Energyuse>()
                .Where(e => e.BasicId == id)
                .Join(_MyDbContext.DTransportations.Where(d => d.BindWhere == "EnergyUse"),
                a => a.Id,
                b => b.BindId,
                (a, b) => new { TableA = a, TableB = b })
                .Select(e => new
                {
                    Id = e.TableA.Id,
                    BasicId = e.TableA.BasicId,
                    dataValue = (e.TableA.EnergyName == "4" ? 0.7573m : e.TableA.EnergyName == "5" ? 0.8325m : 1) * (e.TableA.BeforeUnit == "14" ? 0.001m : 1) * e.TableA.BeforeTotal,
                    DistributeRatio = e.TableA.DistributeRatio,
                    Method = e.TableB.Method,
                    Car = e.TableB.Car,
                    Land = e.TableB.Land,
                    Sea = e.TableB.Sea,
                    Air = e.TableB.Air,
                }).ToList();

                var ResourceuseTransportation = _MyDbContext.Set<Resourceuse>()
                .Where(e => e.BasicId == id)
                .Join(_MyDbContext.DTransportations.Where(d => d.BindWhere == "ResourceUse"),
                a => a.Id,
                b => b.BindId,
                (a, b) => new { TableA = a, TableB = b })
                .Select(e => new
                {
                    Id = e.TableA.Id,
                    BasicId = e.TableA.BasicId,
                    dataValue = e.TableA.BeforeTotal,
                    DistributeRatio = e.TableA.DistributeRatio,
                    Method = e.TableB.Method,
                    Car = e.TableB.Car,
                    Land = e.TableB.Land,
                    Sea = e.TableB.Sea,
                    Air = e.TableB.Air,
                }).ToList();

                var DumptreatmentTransportation = _MyDbContext.Set<DumptreatmentOutsourcing>()
                .Where(e => e.BasicId == id)
                .Join(_MyDbContext.DTransportations.Where(d => d.BindWhere == "DumptreatmentOutsourcing"),
                a => a.Id,
                b => b.BindId,
                (a, b) => new { TableA = a, TableB = b })
                .Select(e => new
                {
                    Id = e.TableA.Id,
                    BasicId = e.TableA.BasicId,
                    dataValue = (e.TableA.BeforeUnit == "10" ? 0.001m : 1) * e.TableA.BeforeTotal,
                    DistributeRatio = e.TableA.DistributeRatio,
                    Method = e.TableB.Method,
                    Car = e.TableB.Car,
                    Land = e.TableB.Land,
                    Sea = e.TableB.Sea,
                    Air = e.TableB.Air,
                }).ToList();

                var TransportationResult = EnergyuseTransportation
                    .Union(ResourceuseTransportation)
                    .Union(DumptreatmentTransportation)
                .AsEnumerable() // 将查询结果加载到内存中
                .GroupBy(e => new { e.BasicId })
                .Select(g => new
                {
                    BasicId = g.Key.BasicId,
                    dataValue = g.Sum(e => e.dataValue * e.DistributeRatio / 100
                    * ((e.Method != "61" && e.Method != "62" && e.Land != null ? e.Land * _MyDbContext.Coefficients.FirstOrDefault(a => a.Type == "Transportation" && a.Name == "陸運(km)").Co2coefficient * (e.Car == "63" ? (decimal)0.131 : e.Car == "64" ? (decimal)0.587 : 1) : 0)//大貨車63,小貨車64
                    + (e.Method == "59" && e.Sea != null ? e.Sea * _MyDbContext.Coefficients.FirstOrDefault(a => a.Type == "Transportation" && a.Name == "海運(km)").Co2coefficient : 0)
                    + (e.Method == "60" && e.Air != null ? e.Air * _MyDbContext.Coefficients.FirstOrDefault(a => a.Type == "Transportation" && a.Name == "空運(km)").Co2coefficient : 0))
                    )
                })
                .ToList();

                decimal? TransportationResultData = TransportationResult.Sum(item => item.dataValue);
                var energyuseData = _MyDbContext.Set<Energyuse>()
                    .Where(e => e.BasicId == id)
                    .Select(e => new
                    {
                        Id = e.Id,
                        BasicId = e.BasicId,
                        dataname = e.EnergyName,
                        dataValue = (e.BeforeUnit == "14" ? 0.001m : e.BeforeUnit == "22" ? 0.001m : e.BeforeUnit == "25" ? 0.001m : 1) * e.BeforeTotal,
                        DistributeRatio = e.DistributeRatio
                    }).ToList();

                var ResourceuseData = _MyDbContext.Set<Resourceuse>()
                    .Where(e => e.BasicId == id)
                    .Select(e => new
                    {
                        Id = e.Id,
                        BasicId = e.BasicId,
                        dataname = e.EnergyName,
                        dataValue = (e.BeforeUnit == "22" ? 0.001m : 1) * e.BeforeTotal,
                        DistributeRatio = e.DistributeRatio
                    }).ToList();

                var refrigerantHaveData = _MyDbContext.Set<RefrigerantHave>()
                    .Where(r => r.BasicId == id)
                    .Select(r => new
                    {
                        Id = r.Id,
                        BasicId = r.BasicId,
                        dataname = r.RefrigerantType,
                        dataValue = (r.EquipmentType == "71" ? 0.003m : r.EquipmentType == "72" ? 0.08m : r.EquipmentType == "73" ? 0.225m : r.EquipmentType == "75" ? 0.085m : r.EquipmentType == "76" ? 0.055m : r.EquipmentType == "77" ? 0.15m : 1) * (r.Unit == "10" ? 0.001m : 1) * r.Total,
                        DistributeRatio = r.DistributeRatio
                    }).ToList();

                var refrigerantNoneData = _MyDbContext.Set<RefrigerantNone>()
                    .Where(r => r.BasicId == id)
                    .Select(r => new
                    {
                        Id = r.Id,
                        BasicId = r.BasicId,
                        dataname = r.RefrigerantType,
                        dataValue = (r.EquipmentType == "71" ? 0.003m : r.EquipmentType == "72" ? 0.08m : r.EquipmentType == "73" ? 0.225m : r.EquipmentType == "75" ? 0.085m : r.EquipmentType == "76" ? 0.055m : r.EquipmentType == "77" ? 0.15m : 1) * (r.Unit == "10" ? 0.001m : 1) * r.RefrigerantWeight,
                        DistributeRatio = r.DistributeRatio
                    }).ToList();

                var fireequipmentData = _MyDbContext.Set<Fireequipment>()
                    .Where(f => f.BasicId == id)
                    .Select(f => new
                    {
                        Id = f.Id,
                        BasicId = f.BasicId,
                        dataname = f.EquipmentName,
                        dataValue = (f.BeforeUnit == "10" ? 0.001m : 1) * f.BeforeTotal,
                        DistributeRatio = f.DistributeRatio
                    }).ToList();

                var DumptreatmentData = _MyDbContext.Set<DumptreatmentOutsourcing>()
                    .Where(d => d.BasicId == id)
                    .Select(d => new
                    {
                        Id = d.Id,
                        BasicId = d.BasicId,
                        dataname = d.WasteId.ToString(),
                        dataValue = (d.BeforeUnit == "10" ? 0.001m : 1) * d.BeforeTotal,
                        DistributeRatio = d.DistributeRatio
                    }).ToList();
                
                var AllGHG = energyuseData
                    .Union(ResourceuseData)
                    .Union(refrigerantHaveData)
                    .Union(refrigerantNoneData)
                    .Union(fireequipmentData)
                    .Union(DumptreatmentData)
                    .AsEnumerable() // 将查询结果加载到内存中
                        .Join(
                            _MyDbContext.Coefficients,
                            a => a.dataname,
                            b => b.Id.ToString(),
                        (a, b) => new { TableA = a, TableB = b } // 使用匿名类型包装两个表的列

                        )
                    .GroupBy(e => new { e.TableA.BasicId })
                    .Select(g => new
                    {
                        CO2 = g.Sum(e => e.TableA.dataValue * e.TableA.DistributeRatio / 100 * (e.TableB.Co2coefficient.HasValue ? e.TableB.Co2coefficient : 0)) + TransportationResultData,
                        CH4 = g.Sum(e => e.TableA.dataValue * e.TableA.DistributeRatio / 100 * ((e.TableB.Ch4coefficient.HasValue && e.TableB.Ch4gwp.HasValue) ? (e.TableB.Ch4coefficient * e.TableB.Ch4gwp) : 0)) + WorkinghourTotalResult,
                        N2O = g.Sum(e => e.TableA.dataValue * e.TableA.DistributeRatio / 100 * ((e.TableB.N2ocoefficient.HasValue && e.TableB.N2ogwp.HasValue) ? (e.TableB.N2ocoefficient * e.TableB.N2ogwp) : 0)),
                        HFCs = g.Sum(e => e.TableA.dataValue * e.TableA.DistributeRatio / 100 * (e.TableB.HfcsGwp.HasValue ? e.TableB.HfcsGwp : 0)),
                        PFCs = g.Sum(e => e.TableA.dataValue * e.TableA.DistributeRatio / 100 * (e.TableB.PfcsGwp.HasValue ? e.TableB.PfcsGwp : 0)),
                        SF6 = g.Sum(e => e.TableA.dataValue * e.TableA.DistributeRatio / 100 * (e.TableB.Sf6gwp.HasValue ? e.TableB.Sf6gwp : 0)),
                        NF3 = 0,
                    })
                    .ToList();



                ViewBag.AllGHG = JsonConvert.SerializeObject(AllGHG);

                // 只有工時資料
                if (AllGHG.Count() == 0)
                {
                    var NewAllGHG = new[]
                    {
                          new
                        {
                            CO2 = TransportationResultData,
                            CH4 = WorkinghourTotalResult,
                            N2O = 0,
                            HFCs = 0,
                            PFCs = 0,
                            SF6 = 0,
                            NF3 = 0
                        }
                    };
                    ViewBag.AllGHG = JsonConvert.SerializeObject(NewAllGHG);
                }

                var Category1GHG = energyuseData
                    .Union(ResourceuseData)
                    .Union(refrigerantHaveData)
                    .Union(refrigerantNoneData)
                    .Union(fireequipmentData)
                    .Union(DumptreatmentData)
                    .AsEnumerable() // 将查询结果加载到内存中
                        .Join(
                            _MyDbContext.Coefficients.Where(a => a.Category == "113"),//範疇一
                            a => a.dataname,
                            b => b.Id.ToString(),
                        (a, b) => new { TableA = a, TableB = b } // 使用匿名类型包装两个表的列

                        )
                    .GroupBy(e => new { e.TableA.BasicId })
                    .Select(g => new
                    {
                        CO2 = g.Sum(e => e.TableA.dataValue * e.TableA.DistributeRatio / 100 * (e.TableB.Co2coefficient.HasValue ? e.TableB.Co2coefficient : 0)) + TransportationResultData,
                        CH4 = g.Sum(e => e.TableA.dataValue * e.TableA.DistributeRatio / 100 * ((e.TableB.Ch4coefficient.HasValue && e.TableB.Ch4gwp.HasValue) ? (e.TableB.Ch4coefficient * e.TableB.Ch4gwp) : 0)) + WorkinghourTotalResult,
                        N2O = g.Sum(e => e.TableA.dataValue * e.TableA.DistributeRatio / 100 * ((e.TableB.N2ocoefficient.HasValue && e.TableB.N2ogwp.HasValue) ? (e.TableB.N2ocoefficient * e.TableB.N2ogwp) : 0)),
                        HFCs = g.Sum(e => e.TableA.dataValue * e.TableA.DistributeRatio / 100 * (e.TableB.HfcsGwp.HasValue ? e.TableB.HfcsGwp : 0)),
                        PFCs = g.Sum(e => e.TableA.dataValue * e.TableA.DistributeRatio / 100 * (e.TableB.PfcsGwp.HasValue ? e.TableB.PfcsGwp : 0)),
                        SF6 = g.Sum(e => e.TableA.dataValue * e.TableA.DistributeRatio / 100 * (e.TableB.Sf6gwp.HasValue ? e.TableB.Sf6gwp : 0)),
                        NF3 = 0,
                    })
                    .ToList();
                ViewBag.Category1GHG = JsonConvert.SerializeObject(Category1GHG);
                

                // 只有工時資料
                if (AllGHG.Count() == 0)
                {
                    var NewCategory1GHG = new[]
                    {
                      new
                    {
                        CO2 = TransportationResultData,
                        CH4 = WorkinghourTotalResult,
                        N2O = 0,
                        HFCs = 0,
                        PFCs = 0,
                        SF6 = 0,
                        NF3 = 0
                    }
                };
                    ViewBag.Category1GHG = JsonConvert.SerializeObject(NewCategory1GHG);
                    NewCategory1GHG = null;
                }
                
                Category1GHG = null;

                var EmissionSource = energyuseData
                    .Union(ResourceuseData)
                    .Union(refrigerantHaveData)
                    .Union(refrigerantNoneData)
                    .Union(fireequipmentData)
                    .Union(DumptreatmentData)
                    .AsEnumerable() // 将查询结果加载到内存中
                        .Join(
                            _MyDbContext.Coefficients,//範疇一
                            a => a.dataname,
                            b => b.Id.ToString(),
                        (a, b) => new { TableA = a, TableB = b } // 使用匿名类型包装两个表的列
                        )
                    .GroupBy(e => new { e.TableA.BasicId, e.TableB.EmissionSource })
                    .Select(g => new
                    {
                        EmissionSource = g.Key.EmissionSource == "107" ? "固定排放" : g.Key.EmissionSource == "108" ? "製程排放" : g.Key.EmissionSource == "109" ? "移動排放" : g.Key.EmissionSource == "110" ? "逸散排放" : g.Key.EmissionSource == "111" ? "能源間接排放" : g.Key.EmissionSource == "112" ? "其他間接排放" : "",
                        dataValue = g.Sum(e => e.TableA.dataValue * e.TableA.DistributeRatio / 100
                        * ((e.TableB.Co2coefficient.HasValue ? e.TableB.Co2coefficient : 0) +
                        ((e.TableB.Ch4coefficient.HasValue && e.TableB.Ch4gwp.HasValue) ? (e.TableB.Ch4coefficient * e.TableB.Ch4gwp) : 0) +
                        ((e.TableB.N2ocoefficient.HasValue && e.TableB.N2ogwp.HasValue) ? (e.TableB.N2ocoefficient * e.TableB.N2ogwp) : 0) +
                        (e.TableB.HfcsGwp.HasValue ? e.TableB.HfcsGwp : 0) +
                        (e.TableB.PfcsGwp.HasValue ? e.TableB.PfcsGwp : 0) +
                        (e.TableB.Sf6gwp.HasValue ? e.TableB.Sf6gwp : 0))
                        ) + (g.Key.EmissionSource == "110" ? WorkinghourTotalResult : 0)
                        + (g.Key.EmissionSource == "109" ? TransportationResultData : 0)
                    })
                    .ToList();
                ViewBag.EmissionSource = JsonConvert.SerializeObject(EmissionSource);

                // 检查是否包含 "移動排放" 的对象
                bool hasEmissionSource_109 = EmissionSource.Any(item => item.EmissionSource == "移動排放");
                // 检查是否包含 "逸散排放" 的对象
                bool hasEmissionSource_110 = EmissionSource.Any(item => item.EmissionSource == "逸散排放");

                string existingJson = ViewBag.EmissionSource; // 获取已有的 JSON 字符串
                JArray combinedJsonArray = JArray.Parse(existingJson); // 将已有的 JSON 字符串解析成 JArray

                if (!hasEmissionSource_109)
                {
                    JObject newObjectTrans = new JObject();
                    newObjectTrans["EmissionSource"] = "移動排放";
                    newObjectTrans["dataValue"] = TransportationResultData;

                    // 添加到 JSON 数组
                    combinedJsonArray.Add(newObjectTrans);
                }
                if (!hasEmissionSource_110)
                {
                    JObject newObjectWork = new JObject();
                    newObjectWork["EmissionSource"] = "逸散排放";
                    newObjectWork["dataValue"] = WorkinghourTotalResult;

                    // 添加到 JSON 数组
                    combinedJsonArray.Add(newObjectWork);
                }
                // 将 JSON 数组转换为字符串
                string finalJsonArray = combinedJsonArray.ToString();
                ViewBag.EmissionSource = finalJsonArray;


                var GHGType = energyuseData
                    .Union(ResourceuseData)
                    .Union(refrigerantHaveData)
                    .Union(refrigerantNoneData)
                    .Union(fireequipmentData)
                    .Union(DumptreatmentData)
                    .AsEnumerable() // 将查询结果加载到内存中
                        .Join(
                            _MyDbContext.Coefficients,//範疇一
                            a => a.dataname,
                            b => b.Id.ToString(),
                        (a, b) => new { TableA = a, TableB = b } // 使用匿名类型包装两个表的列
                        )
                    .GroupBy(e => new { e.TableA.BasicId, e.TableA.dataname })
                    .Select(g => new
                    {
                        GHGType = _MyDbContext.Coefficients.Where(a => a.Id.ToString() == g.Key.dataname).First().Name,
                        dataValue = g.Sum(e => e.TableA.dataValue * e.TableA.DistributeRatio / 100
                        * ((e.TableB.Co2coefficient.HasValue ? e.TableB.Co2coefficient : 0) +
                        ((e.TableB.Ch4coefficient.HasValue && e.TableB.Ch4gwp.HasValue) ? (e.TableB.Ch4coefficient * e.TableB.Ch4gwp) : 0) +
                        ((e.TableB.N2ocoefficient.HasValue && e.TableB.N2ogwp.HasValue) ? (e.TableB.N2ocoefficient * e.TableB.N2ogwp) : 0) +
                        (e.TableB.HfcsGwp.HasValue ? e.TableB.HfcsGwp : 0) +
                        (e.TableB.PfcsGwp.HasValue ? e.TableB.PfcsGwp : 0) +
                        (e.TableB.Sf6gwp.HasValue ? e.TableB.Sf6gwp : 0))
                        )
                    })
                    .ToList();
                string jsonArray = JsonConvert.SerializeObject(GHGType);
                JArray jsonArrayObject = JArray.Parse(jsonArray);

                JObject newObject = new JObject();
                newObject["GHGType"] = "運輸";
                newObject["dataValue"] = TransportationResultData;

                JObject newObject2 = new JObject();
                newObject2["GHGType"] = "工時";
                newObject2["dataValue"] = WorkinghourTotalResult;

                // 將新的 JSON 物件添加到陣列中
                jsonArrayObject.Add(newObject);
                jsonArrayObject.Add(newObject2);

                string newJsonArray = jsonArrayObject.ToString();

                ViewBag.GHGType = newJsonArray;

                newJsonArray = null;


                return View();
            }
            catch (Exception ex)
            {
                var Log = new Log()
                {
                    WhereFunction = "Result_Chart_get",
                    Exception = ex.ToString(),
                    DateTime = DateTime.Now,
                };
                _MyDbContext.Logs.Add(Log);
                _MyDbContext.SaveChanges();
                Log = null;
                return Error("內部伺服器錯誤"); // 或其他適當的錯誤訊息
            }
        }
    }
}
