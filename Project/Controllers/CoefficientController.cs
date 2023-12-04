using Project.Models;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace Project.Controllers
{
    public class CoefficientController : CommonController
    {
        private MyDbContext _MyDbContext;
        public CoefficientController(MyDbContext MyDbContext)
        {
            _MyDbContext = MyDbContext;
        }
        public ActionResult EnergyUse()
        {
            try
            {
                var Energycoefficients = _MyDbContext.Coefficients.Where(a => a.Type == "EnergyName").ToList();//抓取該id的能源使用

                var pagedList = Energycoefficients.OrderByDescending(a => a.Id).ToList();
                int count = 1;//jsgrid sort遞增
                              //選擇前端jsgrid需顯示的資料
                var result = pagedList.Select((item, index) => new
                {
                    sort = count + index,
                    id = item.Id,
                    emissionsource = item.EmissionSource,
                    name = item.Name,
                    unit = item.Unit,
                    category = item.Category,
                    co2coefficient = item.Co2coefficient,
                    co2unit = "KgCO2/" + _MyDbContext.Selectdata.Where(a => a.Code == item.Unit).Select(a => a.Name).FirstOrDefault(),
                    ch4coefficient = item.Ch4coefficient,
                    ch4unit = "KgCH4/" + _MyDbContext.Selectdata.Where(a => a.Code == item.Unit).Select(a => a.Name).FirstOrDefault(),
                    ch4gwp=item.Ch4gwp,
                    n2ocoefficient = item.N2ocoefficient,
                    n2ounit = "KgN2O/" + _MyDbContext.Selectdata.Where(a => a.Code == item.Unit).Select(a => a.Name).FirstOrDefault(),
                    n2ogwp=item.N2ogwp,
                });
                ViewBag.DeatilsJson = JsonConvert.SerializeObject(result);//資料傳回前端
                ViewBag.EmissionSource = JsonConvert.SerializeObject(_MyDbContext.Selectdata.Where(a => a.Type == "EmissionSource").Select(a => new { a.Code, a.Name, a.Sort }).OrderBy(a => a.Sort).ToList());//單位資料
                ViewBag.Unit = JsonConvert.SerializeObject(_MyDbContext.Selectdata.Where(a => a.Type == "Unit").Select(a => new { a.Code, a.Name, a.Sort }).OrderBy(a => a.Sort).ToList());//單位資料
                ViewBag.Category = JsonConvert.SerializeObject(_MyDbContext.Selectdata.Where(a => a.Type == "Category").Select(a => new { a.Code, a.Name, a.Sort }).OrderBy(a => a.Sort).ToList());//單位資料

                result = null;
                pagedList = null;
                return View();
            }
            catch (Exception ex)
            {
                var Log = new Log()
                {
                    WhereFunction = "Coefficient_EnergyUse_get",
                    Exception = ex.ToString(),
                    DateTime = DateTime.Now,
                };
                _MyDbContext.Logs.Add(Log);
                _MyDbContext.SaveChanges();
                Log = null;
                return Error("內部伺服器錯誤"); // 或其他適當的錯誤訊息
            }

        }
        public ActionResult ResourceUse()
        {
            try
            {
                var ResourceUsecoefficients = _MyDbContext.Coefficients.Where(a => a.Type == "ResourceName").ToList();//抓取該id的能源使用

                var pagedList = ResourceUsecoefficients.OrderByDescending(a => a.Id).ToList();
                int count = 1;//jsgrid sort遞增
                              //選擇前端jsgrid需顯示的資料
                var result = pagedList.Select((item, index) => new
                {
                    sort = count + index,
                    id = item.Id,
                    emissionsource = item.EmissionSource,
                    name = item.Name,
                    unit = item.Unit,
                    category = item.Category,
                    co2coefficient = item.Co2coefficient,
                    co2unit = "KgCO2/" + _MyDbContext.Selectdata.Where(a => a.Code == item.Unit).Select(a => a.Name).FirstOrDefault(),
                });
                ViewBag.DeatilsJson = JsonConvert.SerializeObject(result);//資料傳回前端
                ViewBag.EmissionSource = JsonConvert.SerializeObject(_MyDbContext.Selectdata.Where(a => a.Type == "EmissionSource").Select(a => new { a.Code, a.Name, a.Sort }).OrderBy(a => a.Sort).ToList());//單位資料
                ViewBag.Unit = JsonConvert.SerializeObject(_MyDbContext.Selectdata.Where(a => a.Type == "Unit").Select(a => new { a.Code, a.Name, a.Sort }).OrderBy(a => a.Sort).ToList());//單位資料
                ViewBag.Category = JsonConvert.SerializeObject(_MyDbContext.Selectdata.Where(a => a.Type == "Category").Select(a => new { a.Code, a.Name, a.Sort }).OrderBy(a => a.Sort).ToList());//單位資料

                result = null;
                pagedList = null;
                return View();
            }
            catch (Exception ex)
            {
                var Log = new Log()
                {
                    WhereFunction = "Coefficient_ResourceUse_get",
                    Exception = ex.ToString(),
                    DateTime = DateTime.Now,
                };
                _MyDbContext.Logs.Add(Log);
                _MyDbContext.SaveChanges();
                Log = null;
                return Error("內部伺服器錯誤"); // 或其他適當的錯誤訊息
            }
        }
        public ActionResult FireEquipment()
        {
            try
            {
                var FireEquipmentcoefficients = _MyDbContext.Coefficients.Where(a => a.Type == "FireEquipmentName").ToList();//抓取該id的能源使用

                var pagedList = FireEquipmentcoefficients.OrderByDescending(a => a.Id).ToList();
                int count = 1;//jsgrid sort遞增
                              //選擇前端jsgrid需顯示的資料
                var result = pagedList.Select((item, index) => new
                {
                    sort = count + index,
                    id = item.Id,
                    emissionsource = item.EmissionSource,
                    name = item.Name,
                    unit = item.Unit,
                    category = item.Category,
                    co2coefficient = item.Co2coefficient,
                    hfcsgwp = item.HfcsGwp,
                    sf6gwp=item.Sf6gwp,
                });
                ViewBag.DeatilsJson = JsonConvert.SerializeObject(result);//資料傳回前端
                ViewBag.EmissionSource = JsonConvert.SerializeObject(_MyDbContext.Selectdata.Where(a => a.Type == "EmissionSource").Select(a => new { a.Code, a.Name, a.Sort }).OrderBy(a => a.Sort).ToList());//單位資料
                ViewBag.Unit = JsonConvert.SerializeObject(_MyDbContext.Selectdata.Where(a => a.Type == "Unit").Select(a => new { a.Code, a.Name, a.Sort }).OrderBy(a => a.Sort).ToList());//單位資料
                ViewBag.Category = JsonConvert.SerializeObject(_MyDbContext.Selectdata.Where(a => a.Type == "Category").Select(a => new { a.Code, a.Name, a.Sort }).OrderBy(a => a.Sort).ToList());//單位資料

                result = null;
                pagedList = null;
                return View();
            }
            catch (Exception ex)
            {
                var Log = new Log()
                {
                    WhereFunction = "Coefficient_FireEquipment_get",
                    Exception = ex.ToString(),
                    DateTime = DateTime.Now,
                };
                _MyDbContext.Logs.Add(Log);
                _MyDbContext.SaveChanges();
                Log = null;
                return Error("內部伺服器錯誤"); // 或其他適當的錯誤訊息
            }
        }
        public ActionResult Refrigerant()
        {
            try
            {
                var Energycoefficients = _MyDbContext.Coefficients.Where(a => a.Type == "RefrigerantName").ToList();//抓取該id的能源使用

                var pagedList = Energycoefficients.OrderByDescending(a => a.Id).ToList();
                int count = 1;//jsgrid sort遞增
                              //選擇前端jsgrid需顯示的資料
                var result = pagedList.Select((item, index) => new
                {
                    sort = count + index,
                    id = item.Id,
                    emissionsource = item.EmissionSource,
                    name = item.Name,
                    unit = item.Unit,
                    category = item.Category,
                    hfcsgwp = item.HfcsGwp,
                    pfcsgwp = item.PfcsGwp,
                });
                ViewBag.DeatilsJson = JsonConvert.SerializeObject(result);//資料傳回前端
                ViewBag.EmissionSource = JsonConvert.SerializeObject(_MyDbContext.Selectdata.Where(a => a.Type == "EmissionSource").Select(a => new { a.Code, a.Name, a.Sort }).OrderBy(a => a.Sort).ToList());//單位資料
                ViewBag.Unit = JsonConvert.SerializeObject(_MyDbContext.Selectdata.Where(a => a.Type == "Unit").Select(a => new { a.Code, a.Name, a.Sort }).OrderBy(a => a.Sort).ToList());//單位資料
                ViewBag.Category = JsonConvert.SerializeObject(_MyDbContext.Selectdata.Where(a => a.Type == "Category").Select(a => new { a.Code, a.Name, a.Sort }).OrderBy(a => a.Sort).ToList());//單位資料

                result = null;
                pagedList = null;
                return View();
            }
            catch (Exception ex)
            {
                var Log = new Log()
                {
                    WhereFunction = "Coefficient_Refrigerant_get",
                    Exception = ex.ToString(),
                    DateTime = DateTime.Now,
                };
                _MyDbContext.Logs.Add(Log);
                _MyDbContext.SaveChanges();
                Log = null;
                return Error("內部伺服器錯誤"); // 或其他適當的錯誤訊息
            }
        }
        public ActionResult Transportation()
        {
            try
            {
                var Transportationcoefficients = _MyDbContext.Coefficients.Where(a => a.Type == "Transportation").ToList();//抓取該id的能源使用

                var pagedList = Transportationcoefficients.OrderByDescending(a => a.Id).ToList();
                int count = 1;//jsgrid sort遞增
                              //選擇前端jsgrid需顯示的資料
                var result = pagedList.Select((item, index) => new
                {
                    sort = count + index,
                    id = item.Id,
                    emissionsource = item.EmissionSource,
                    name = item.Name,
                    wastemethod=item.WasteMethod,
                    unit = item.Unit,
                    category = item.Category,
                    co2coefficient = item.Co2coefficient,
                    co2unit = "KgCO2/" + _MyDbContext.Selectdata.Where(a => a.Code == item.Unit).Select(a => a.Name).FirstOrDefault(),
                });
                ViewBag.DeatilsJson = JsonConvert.SerializeObject(result);//資料傳回前端
                ViewBag.EmissionSource = JsonConvert.SerializeObject(_MyDbContext.Selectdata.Where(a => a.Type == "EmissionSource").Select(a => new { a.Code, a.Name, a.Sort }).OrderBy(a => a.Sort).ToList());//單位資料
                ViewBag.Unit = JsonConvert.SerializeObject(_MyDbContext.Selectdata.Where(a => a.Type == "Unit").Select(a => new { a.Code, a.Name, a.Sort }).OrderBy(a => a.Sort).ToList());//單位資料
                ViewBag.Category = JsonConvert.SerializeObject(_MyDbContext.Selectdata.Where(a => a.Type == "Category").Select(a => new { a.Code, a.Name, a.Sort }).OrderBy(a => a.Sort).ToList());//單位資料

                result = null;
                pagedList = null;
                return View();
            }
            catch (Exception ex)
            {
                var Log = new Log()
                {
                    WhereFunction = "Coefficient_Refrigerant_get",
                    Exception = ex.ToString(),
                    DateTime = DateTime.Now,
                };
                _MyDbContext.Logs.Add(Log);
                _MyDbContext.SaveChanges();
                Log = null;
                return Error("內部伺服器錯誤"); // 或其他適當的錯誤訊息
            }
        }
        public ActionResult dumptreatment_outsourcing()
        {
            try
            {
                var Energycoefficients = _MyDbContext.Coefficients.Where(a => a.Type == "dumptreatment_outsourcing").ToList();//抓取該id的能源使用

                var pagedList = Energycoefficients.OrderByDescending(a => a.Id).ToList();
                int count = 1;//jsgrid sort遞增
                              //選擇前端jsgrid需顯示的資料
                var result = pagedList.Select((item, index) => new
                {
                    sort = count + index,
                    id = item.Id,
                    emissionsource = item.EmissionSource,
                    name = item.Name,
                    wastemethod = item.WasteMethod,
                    unit = item.Unit,
                    category = item.Category,
                    co2coefficient = item.Co2coefficient,
                    co2unit = "KgCO2/" + _MyDbContext.Selectdata.Where(a => a.Code == item.Unit).Select(a => a.Name).FirstOrDefault(),
                });
                ViewBag.DeatilsJson = JsonConvert.SerializeObject(result);//資料傳回前端
                ViewBag.EmissionSource = JsonConvert.SerializeObject(_MyDbContext.Selectdata.Where(a => a.Type == "EmissionSource").Select(a => new { a.Code, a.Name, a.Sort }).OrderBy(a => a.Sort).ToList());//單位資料
                ViewBag.Unit = JsonConvert.SerializeObject(_MyDbContext.Selectdata.Where(a => a.Type == "Unit").Select(a => new { a.Code, a.Name, a.Sort }).OrderBy(a => a.Sort).ToList());//單位資料
                ViewBag.Category = JsonConvert.SerializeObject(_MyDbContext.Selectdata.Where(a => a.Type == "Category").Select(a => new { a.Code, a.Name, a.Sort }).OrderBy(a => a.Sort).ToList());//單位資料

                result = null;
                pagedList = null;
                return View();
            }
            catch (Exception ex)
            {
                var Log = new Log()
                {
                    WhereFunction = "Coefficient_Refrigerant_get",
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
