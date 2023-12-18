using Project.Models;
using Microsoft.AspNetCore.Mvc;
using Project.Models.View;
using X.PagedList;
using Newtonsoft.Json;

namespace Project.Controllers
{
    public class OrganizeDetailController : CommonController
    {
        private MyDbContext _MyDbContext;
        public OrganizeDetailController(MyDbContext MyDbContext)
        {
            _MyDbContext = MyDbContext;
        }
        public ActionResult EnergyUsedetail(int basicid, int id,string energyname)
        {
            try
            {
                ViewBag.basicid = basicid;//給input存取    
                var a = _MyDbContext.Basicdatas.Find(basicid);
                int monthcount = (a.EndTime.Year - a.StartTime.Year) * 12 + a.EndTime.Month - a.StartTime.Month + 1;//相差多少月份
                int year = a.StartTime.Year;//系統從盤查區間自動帶出年
                int month = a.StartTime.Month;//系統自動帶出樂
                ViewBag.EndYear = a.EndTime.Year;
                ViewBag.EndMonth = a.EndTime.Month;
                ViewBag.monthcount = monthcount;//月份級距
                ViewBag.year = year;
                ViewBag.month = month;
                //單位設定
                if (_MyDbContext.Energyuses.Any(a => a.Id == id && a.EnergyName == "1")) //電力
                {
                    ViewBag.Unit = _MyDbContext.Selectdata.Where(a => a.Code == "25"||a.Code == "117").ToList(); //度,千度
                }
                else if (_MyDbContext.Energyuses.Any(a => a.Id == id && a.EnergyName == "2"))//天然氣
                {
                    ViewBag.Unit = _MyDbContext.Selectdata.Where(a => a.Code == "22").ToList(); //立方公尺
                }
                else if (_MyDbContext.Energyuses.Any(a => a.Id == id &&( a.EnergyName == "3" || a.EnergyName == "4"|| a.EnergyName == "5"|| a.EnergyName == "6" || a.EnergyName == "7"))) //車用柴油,車用汽油,柴油,汽油,液化石油氣
                {
                    ViewBag.Unit = _MyDbContext.Selectdata.Where(a => a.Code == "14" || a.Code == "15").ToList(); //公升,公秉
                }
                


                ViewBag.Method = _MyDbContext.Selectdata.Where(a => a.Type == "Transportation");
                ViewBag.Car = _MyDbContext.Selectdata.Where(a => a.Type == "Car");
                var organize = _MyDbContext.Organizes.Where(a => a.BasicId == basicid).First();
                ViewBag.BasicDataStatus = organize.Status;
                ViewBag.AddMonth=_MyDbContext.DIntervalusetotals.FirstOrDefault(a => a.BindId == id && a.Type == "AddMonth" && a.BindWhere == "EnergyUse");
                //viewmodel資料設定
                var DTransportation = _MyDbContext.DTransportations.FirstOrDefault(a => a.BindId == id && a.BindWhere == "EnergyUse");
                var DIntervalusetotals_DateTime = _MyDbContext.DIntervalusetotals.Where(a => a.BindId == id && a.Type == "DateTime" && a.BindWhere == "EnergyUse").ToList();
                var DIntervalusetotals_StartTime = _MyDbContext.DIntervalusetotals.Where(a => a.BindId == id && a.Type == "StartTime" && a.BindWhere == "EnergyUse").ToList();
                var DIntervalusetotals_EndTime = _MyDbContext.DIntervalusetotals.Where(a => a.BindId == id && a.Type == "EndTime" && a.BindWhere == "EnergyUse").ToList();
                var DIntervalusetotals_Num = _MyDbContext.DIntervalusetotals.Where(a => a.BindId == id && a.Type == "Num" && a.BindWhere == "EnergyUse").ToList();
                var Unit = _MyDbContext.DIntervalusetotals.FirstOrDefault(a => a.BindId == id && a.Type == "Unit" && a.BindWhere == "EnergyUse"); 
                var SelMonth = _MyDbContext.DIntervalusetotals.FirstOrDefault(a => a.BindId == id && a.Type == "SelMonth" && a.BindWhere == "EnergyUse");
                var DDatasources = _MyDbContext.DDatasources.FirstOrDefault(a => a.BindId == id && a.BindWhere == "EnergyUse");

                if (DDatasources?.EvidenceFile != null)//檔案不為空時，給予前端黨名
                {
                    ViewBag.EvidenceFile = AccountId() + "/" + a.BasicId + "/" + DDatasources.EvidenceFile;
                }
                //把資料塞進viewmodel
                var viewModel = new EnergyUsedetail
                {
                    DTransportation = DTransportation,
                    DIntervalusetotals_DateTime = DIntervalusetotals_DateTime,
                    DIntervalusetotals_StartTime = DIntervalusetotals_StartTime,
                    DIntervalusetotals_EndTime = DIntervalusetotals_EndTime,
                    DIntervalusetotals_Num = DIntervalusetotals_Num,
                    Unit = Unit,
                    SelMonth = SelMonth,
                    DDatasource = DDatasources
                };
                DTransportation = null;
                DIntervalusetotals_DateTime = null;
                DIntervalusetotals_StartTime = null;
                DIntervalusetotals_EndTime = null;
                DIntervalusetotals_Num = null;
                Unit = null;
                DDatasources = null;

                return View(viewModel);
            }
            catch (Exception ex)
            {
                var Log = new Log()
                {
                    WhereFunction = "OrganizeDetail_EnergyUsedetail_get",
                    Exception = ex.ToString(),
                    DateTime = DateTime.Now,
                };
                _MyDbContext.Logs.Add(Log);
                _MyDbContext.SaveChanges();
                Log = null;
                return Error("內部伺服器錯誤"); // 或其他適當的錯誤訊息
            }
        }
        // POST: ProductController/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult EnergyUsedetail(int basicid, int id, IFormCollection? collection, decimal? Tonnes, decimal? Land, decimal? Sea, decimal? Air,
            string[]? DateTimes, string[]? StartTime, string[]? EndTime, string[]? Num, string SelMonth, List<IFormFile> EvidenceFile,int? AddMonth)
        {
            try
            {
                if (AccountId() == null)//帳號為空時，登出
                {
                    return Nologin();
                }
                var entity = _MyDbContext.DTransportations.FirstOrDefault(e => e.BindId == id && e.BindWhere == "EnergyUse");
                var a = _MyDbContext.Basicdatas.Find(basicid);
                //string[] parts = DateTimes[1].Split('/');

                //int AddMonth = int.Parse(parts[1]) - (a.StartTime.Month);

                //如果沒資料則新增資料
                if (entity == null)
                {
                    var DTransportation = new DTransportation()
                    {
                        BindId = id,
                        BindWhere = "EnergyUse",
                        Method = collection["Method"],
                        StartLocation = collection["StartLocation"],
                        EndLocation = collection["EndLocation"],
                        Car = collection["Car"],
                        Tonnes = Tonnes,
                        Fuel = collection["Fuel"],
                        Land = Land,
                        Sea = Sea,
                        Air = Air,
                    };
                    //新增運輸資訊
                    _MyDbContext.DTransportations.Add(DTransportation);
                    //依照多少欄位新增數據統計
                    DIntervalusetotal DIntervalusetotal;
                    for (int i = 0; i < DateTimes.Length; i++)
                    {
                        DIntervalusetotal = new DIntervalusetotal()
                        {
                            BindId = id,
                            BindWhere = "EnergyUse",//設定屬於能源使用的表
                            Num = DateTimes[i],
                            Type = "DateTime",
                            ArraySort = i,
                        };
                        _MyDbContext.DIntervalusetotals.Add(DIntervalusetotal);
                        DIntervalusetotal = new DIntervalusetotal()
                        {
                            BindId = id,
                            BindWhere = "EnergyUse",
                            Num = StartTime[i] != null ? StartTime[i] : "",//為空時，給予空值
                            Type = "StartTime",
                            ArraySort = i,
                        };
                        _MyDbContext.DIntervalusetotals.Add(DIntervalusetotal);
                        DIntervalusetotal = new DIntervalusetotal()
                        {
                            BindId = id,
                            BindWhere = "EnergyUse",
                            Num = EndTime[i] != null ? EndTime[i] : "",
                            Type = "EndTime",
                            ArraySort = i,
                        };
                        _MyDbContext.DIntervalusetotals.Add(DIntervalusetotal);
                        DIntervalusetotal = new DIntervalusetotal()
                        {
                            BindId = id,
                            BindWhere = "EnergyUse",
                            Num = Num[i] != null ? Num[i] : "",
                            Type = "Num",
                            ArraySort = i,
                        };
                        _MyDbContext.DIntervalusetotals.Add(DIntervalusetotal);
                    }
                    DIntervalusetotal = new DIntervalusetotal()
                    {
                        BindId = id,
                        BindWhere = "EnergyUse",
                        Num = collection["Unit"],
                        Type = "Unit",
                    };
                    _MyDbContext.DIntervalusetotals.Add(DIntervalusetotal);

                    DIntervalusetotal = new DIntervalusetotal()
                    {
                        BindId = id,
                        BindWhere = "EnergyUse",
                        Num = SelMonth,
                        Type = "SelMonth",
                    };
                    _MyDbContext.DIntervalusetotals.Add(DIntervalusetotal);
                    DIntervalusetotal = new DIntervalusetotal()
                    {
                        BindId = id,
                        BindWhere = "EnergyUse",
                        Num = AddMonth.ToString(),
                        Type = "AddMonth",
                    };
                    //新增單位
                    _MyDbContext.DIntervalusetotals.Add(DIntervalusetotal);
                    DIntervalusetotal = null;

                    //新增數據來源資訊
                    var DDatasource = new DDatasource()
                    {
                        BindId = id,
                        BindWhere = "EnergyUse",
                        Management = collection["management"],
                        Principal = collection["Principal"],
                        Datasource = collection["Datasource"],
                    };
                    _MyDbContext.DDatasources.Add(DDatasource);
                    DDatasource = null;
                }
                else//有資料時則更新資料
                {
                    //更新運輸資訊
                    var DTransportations = _MyDbContext.DTransportations.FirstOrDefault(e => e.BindId == id && e.BindWhere == "EnergyUse");
                    DTransportations.Method = collection["Method"];
                    DTransportations.StartLocation = collection["StartLocation"];
                    DTransportations.EndLocation = collection["EndLocation"];
                    DTransportations.Car = collection["Car"];
                    DTransportations.Tonnes = Tonnes;
                    DTransportations.Fuel = collection["Fuel"];
                    DTransportations.Land = Land;
                    DTransportations.Sea = Sea;
                    DTransportations.Air = Air;
                    _MyDbContext.DTransportations.Update(DTransportations);
                    DTransportations = null;
                    _MyDbContext.DIntervalusetotals.RemoveRange(_MyDbContext.DIntervalusetotals.Where(a => a.BindId == id && a.BindWhere == "EnergyUse").ToList());//數據統計資料，全部刪除再塞新的進來

                    DIntervalusetotal DIntervalusetotal;
                    for (int i = 0; i < DateTimes.Length; i++)
                    {
                        DIntervalusetotal = new DIntervalusetotal()
                        {
                            BindId = id,
                            BindWhere = "EnergyUse",//設定屬於能源使用的表
                            Num = DateTimes[i],
                            Type = "DateTime",
                            ArraySort = i,
                        };
                        _MyDbContext.DIntervalusetotals.Add(DIntervalusetotal);
                        DIntervalusetotal = new DIntervalusetotal()
                        {
                            BindId = id,
                            BindWhere = "EnergyUse",
                            Num = StartTime[i] != null ? StartTime[i] : "",//為空時，給予空值
                            Type = "StartTime",
                            ArraySort = i,
                        };
                        _MyDbContext.DIntervalusetotals.Add(DIntervalusetotal);
                        DIntervalusetotal = new DIntervalusetotal()
                        {
                            BindId = id,
                            BindWhere = "EnergyUse",
                            Num = EndTime[i] != null ? EndTime[i] : "",
                            Type = "EndTime",
                            ArraySort = i,
                        };
                        _MyDbContext.DIntervalusetotals.Add(DIntervalusetotal);
                        DIntervalusetotal = new DIntervalusetotal()
                        {
                            BindId = id,
                            BindWhere = "EnergyUse",
                            Num = Num[i] != null ? Num[i] : "",
                            Type = "Num",
                            ArraySort = i,
                        };
                        _MyDbContext.DIntervalusetotals.Add(DIntervalusetotal);
                    }
                    DIntervalusetotal = new DIntervalusetotal()
                    {
                        BindId = id,
                        BindWhere = "EnergyUse",
                        Num = collection["Unit"],
                        Type = "Unit",
                    };
                    _MyDbContext.DIntervalusetotals.Add(DIntervalusetotal);
                    DIntervalusetotal = new DIntervalusetotal()
                    {
                        BindId = id,
                        BindWhere = "EnergyUse",
                        Num = AddMonth.ToString(),
                        Type = "AddMonth",
                    };
                    //新增單位
                    _MyDbContext.DIntervalusetotals.Add(DIntervalusetotal);
                    DIntervalusetotal = new DIntervalusetotal()
                    {
                        BindId = id,
                        BindWhere = "EnergyUse",
                        Num = SelMonth,
                        Type = "SelMonth",
                    };
                    _MyDbContext.DIntervalusetotals.Add(DIntervalusetotal);

                    DIntervalusetotal = null;
                    //更新數據來源資訊
                    var DDatasources = _MyDbContext.DDatasources.FirstOrDefault(e => e.BindId == id && e.BindWhere == "EnergyUse");
                    DDatasources.Management = collection["management"];
                    DDatasources.Principal = collection["Principal"];
                    DDatasources.Datasource = collection["Datasource"];
                    _MyDbContext.DDatasources.Update(DDatasources);
                    DDatasources = null;
                }

                //塞入總量，單位，檔案到能源使用的table
                var Energyuses = _MyDbContext.Energyuses.Find(id);
                Energyuses.BeforeTotal = decimal.Parse(collection["TotalNum"]);
                Energyuses.BeforeUnit = collection["Unit"];

                string directoryPath = FileSave.Path + a.BasicId + "/能源使用";
                //查詢有無此黨名的資料夾，沒有則新建
                if (!Directory.Exists(directoryPath))
                {
                    Directory.CreateDirectory(directoryPath);
                }
                foreach (var file in EvidenceFile)
                {
                    if (file?.Length > 0)
                    {
                        var fileName = id.ToString() + "_" + file.FileName;

                        // 檢查檔案是否已存在
                        var existingData = _MyDbContext.Evidencefilemanages
                            .FirstOrDefault(e =>
                                e.BasicId == basicid &&
                                e.ItemId == id &&
                                e.FileName == fileName &&
                                e.WhereForm == "能源使用");

                        if (existingData != null)
                        {
                            return Error("此檔名已經存在：" + file.FileName);
                        }

                        // 上傳檔案並新增至資料庫
                        var path = Path.Combine(directoryPath, fileName);
                        using (var stream = new FileStream(path, FileMode.Create))
                        {
                            file.CopyTo(stream);
                        }

                        var evidencefilemanage = new Evidencefilemanage()
                        {
                            BasicId = basicid,
                            ItemId = id,
                            FileName = fileName,
                            WhereForm = "能源使用",
                            Time = DateTime.Now
                        };
                        _MyDbContext.Evidencefilemanages.Add(evidencefilemanage);
                        evidencefilemanage = null;
                    }
                }

                _MyDbContext.SaveChanges();


                return Sucess();
            }
            catch (Exception ex)
            {
                var Log = new Log()
                {
                    WhereFunction = "OrganizeDetail_EnergyUsedetail_post",
                    Exception = ex.ToString(),
                    DateTime = DateTime.Now,
                };
                _MyDbContext.Logs.Add(Log);
                _MyDbContext.SaveChanges();
                Log = null;
                return Error("內部伺服器錯誤"); // 或其他適當的錯誤訊息
            }
        }

        public ActionResult ResourceUsedetail(int basicid, int id,int energyname)
        {
            try
            {
                ViewBag.basicid = basicid;//給input存取    
                var a = _MyDbContext.Basicdatas.Find(basicid);
                int monthcount = (a.EndTime.Year - a.StartTime.Year) * 12 + a.EndTime.Month - a.StartTime.Month + 1;//相差多少月份
                int year = a.StartTime.Year;//系統從盤查區間自動帶出年
                int month = a.StartTime.Month;//系統自動帶出樂
                ViewBag.EndYear = a.EndTime.Year;
                ViewBag.EndMonth = a.EndTime.Month;
                ViewBag.monthcount = monthcount;//月份級距
                ViewBag.year = year;
                ViewBag.month = month;

                var Units = _MyDbContext.Coefficients
                     .Where(a => a.Type == "ResourceName")
                     .Join(
                         _MyDbContext.Selectdata,
                         c => c.Unit,
                         s => s.Code,
                         (c, s) => new { Name = s.Name, Code = s.Code, Sort = s.Sort }
                     )
                     .OrderBy(r => r.Sort) // 根據 Sort 欄位進行升序排序
                     .ToList();

                ViewBag.Unit = Units;
                ViewBag.Method = _MyDbContext.Selectdata.Where(a => a.Type == "Transportation");
                ViewBag.Car = _MyDbContext.Selectdata.Where(a => a.Type == "Car");
                var organize = _MyDbContext.Organizes.Where(a => a.BasicId == basicid).First();
                ViewBag.BasicDataStatus = organize.Status;
                ViewBag.AddMonth = _MyDbContext.DIntervalusetotals.FirstOrDefault(a => a.BindId == id && a.Type == "AddMonth" && a.BindWhere == "ResourceUse");

                //viewmodel資料設定
                var DTransportation = _MyDbContext.DTransportations.FirstOrDefault(a => a.BindId == id && a.BindWhere == "ResourceUse");
                var DIntervalusetotals_DateTime = _MyDbContext.DIntervalusetotals.Where(a => a.BindId == id && a.Type == "DateTime" && a.BindWhere == "ResourceUse").ToList();
                var DIntervalusetotals_StartTime = _MyDbContext.DIntervalusetotals.Where(a => a.BindId == id && a.Type == "StartTime" && a.BindWhere == "ResourceUse").ToList();
                var DIntervalusetotals_EndTime = _MyDbContext.DIntervalusetotals.Where(a => a.BindId == id && a.Type == "EndTime" && a.BindWhere == "ResourceUse").ToList();
                var DIntervalusetotals_Num = _MyDbContext.DIntervalusetotals.Where(a => a.BindId == id && a.Type == "Num" && a.BindWhere == "ResourceUse").ToList();
                var Unit = _MyDbContext.DIntervalusetotals.FirstOrDefault(a => a.BindId == id && a.Type == "Unit" && a.BindWhere == "ResourceUse");
                var SelMonth = _MyDbContext.DIntervalusetotals.FirstOrDefault(a => a.BindId == id && a.Type == "SelMonth" && a.BindWhere == "ResourceUse");

                var DDatasources = _MyDbContext.DDatasources.FirstOrDefault(a => a.BindId == id && a.BindWhere == "ResourceUse");
                if (DDatasources?.EvidenceFile != null)//檔案不為空時，給予前端黨名
                {
                    ViewBag.EvidenceFile = AccountId() + "/" + a.BasicId + "/" + DDatasources.EvidenceFile;
                }
                //把資料塞進viewmodel
                var viewModel = new ResourceUsedetail
                {
                    DTransportation = DTransportation,
                    DIntervalusetotals_DateTime = DIntervalusetotals_DateTime,
                    DIntervalusetotals_StartTime = DIntervalusetotals_StartTime,
                    DIntervalusetotals_EndTime = DIntervalusetotals_EndTime,
                    DIntervalusetotals_Num = DIntervalusetotals_Num,
                    Unit = Unit,
                    SelMonth = SelMonth,
                    DDatasource = DDatasources
                };
                DTransportation = null;
                DIntervalusetotals_DateTime = null;
                DIntervalusetotals_StartTime = null;
                DIntervalusetotals_EndTime = null;
                DIntervalusetotals_Num = null;
                Unit = null;
                DDatasources = null;

                return View(viewModel);
            }
            catch (Exception ex)
            {
                var Log = new Log()
                {
                    WhereFunction = "OrganizeDetail_ResourceUsedetail_get",
                    Exception = ex.ToString(),
                    DateTime = DateTime.Now,
                };
                _MyDbContext.Logs.Add(Log);
                _MyDbContext.SaveChanges();
                Log = null;
                return Error("內部伺服器錯誤"); // 或其他適當的錯誤訊息
            }
        }
        // POST: ProductController/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult ResourceUsedetail(int basicid, int id, IFormCollection? collection, decimal? Tonnes, decimal? Land, decimal? Sea, decimal? Air,
            string[]? DateTimes, string[]? StartTime, string[]? EndTime, string[]? Num, string SelMonth, List<IFormFile> EvidenceFile, int? AddMonth)
        {
            try
            {
                if (AccountId() == null)//帳號為空時，登出
                {
                    return Nologin();
                }
                var entity = _MyDbContext.DTransportations.FirstOrDefault(e => e.BindId == id && e.BindWhere == "ResourceUse");
                var a = _MyDbContext.Basicdatas.Find(basicid);

                //如果沒資料則新增資料
                if (entity == null)
                {
                    var DTransportation = new DTransportation()
                    {
                        BindId = id,
                        BindWhere = "ResourceUse",
                        Method = collection["Method"],
                        StartLocation = collection["StartLocation"],
                        EndLocation = collection["EndLocation"],
                        Car = collection["Car"],
                        Tonnes = Tonnes,
                        Fuel = collection["Fuel"],
                        Land = Land,
                        Sea = Sea,
                        Air = Air,
                    };
                    //新增運輸資訊
                    _MyDbContext.DTransportations.Add(DTransportation);
                    //依照多少欄位新增數據統計
                    DIntervalusetotal DIntervalusetotal;
                    for (int i = 0; i < DateTimes.Length; i++)
                    {
                        DIntervalusetotal = new DIntervalusetotal()
                        {
                            BindId = id,
                            BindWhere = "ResourceUse",//設定屬於資源使用的表
                            Num = DateTimes[i],
                            Type = "DateTime",
                            ArraySort = i,
                        };
                        _MyDbContext.DIntervalusetotals.Add(DIntervalusetotal);
                        DIntervalusetotal = new DIntervalusetotal()
                        {
                            BindId = id,
                            BindWhere = "ResourceUse",
                            Num = StartTime[i] != null ? StartTime[i] : "",//為空時，給予空值
                            Type = "StartTime",
                            ArraySort = i,
                        };
                        _MyDbContext.DIntervalusetotals.Add(DIntervalusetotal);
                        DIntervalusetotal = new DIntervalusetotal()
                        {
                            BindId = id,
                            BindWhere = "ResourceUse",
                            Num = EndTime[i] != null ? EndTime[i] : "",
                            Type = "EndTime",
                            ArraySort = i,
                        };
                        _MyDbContext.DIntervalusetotals.Add(DIntervalusetotal);
                        DIntervalusetotal = new DIntervalusetotal()
                        {
                            BindId = id,
                            BindWhere = "ResourceUse",
                            Num = Num[i] != null ? Num[i] : "",
                            Type = "Num",
                            ArraySort = i,
                        };
                        _MyDbContext.DIntervalusetotals.Add(DIntervalusetotal);
                    }
                    DIntervalusetotal = new DIntervalusetotal()
                    {
                        BindId = id,
                        BindWhere = "ResourceUse",
                        Num = collection["Unit"],
                        Type = "Unit",
                    };
                    //新增單位
                    _MyDbContext.DIntervalusetotals.Add(DIntervalusetotal);
                    DIntervalusetotal = new DIntervalusetotal()
                    {
                        BindId = id,
                        BindWhere = "ResourceUse",
                        Num = SelMonth,
                        Type = "SelMonth",
                    };
                    _MyDbContext.DIntervalusetotals.Add(DIntervalusetotal);
                    DIntervalusetotal = new DIntervalusetotal()
                    {
                        BindId = id,
                        BindWhere = "ResourceUse",
                        Num = AddMonth.ToString(),
                        Type = "AddMonth",
                    };
                    //新增單位
                    _MyDbContext.DIntervalusetotals.Add(DIntervalusetotal);
                    DIntervalusetotal = null;

                    //新增數據來源資訊
                    var DDatasource = new DDatasource()
                    {
                        BindId = id,
                        BindWhere = "ResourceUse",
                        Management = collection["management"],
                        Principal = collection["Principal"],
                        Datasource = collection["Datasource"],
                    };
                    _MyDbContext.DDatasources.Add(DDatasource);
                    DDatasource = null;
                }
                else//有資料時則更新資料
                {
                    //更新運輸資訊
                    var DTransportations = _MyDbContext.DTransportations.FirstOrDefault(e => e.BindId == id && e.BindWhere == "ResourceUse");
                    DTransportations.Method = collection["Method"];
                    DTransportations.StartLocation = collection["StartLocation"];
                    DTransportations.EndLocation = collection["EndLocation"];
                    DTransportations.Car = collection["Car"];
                    DTransportations.Tonnes = Tonnes;
                    DTransportations.Fuel = collection["Fuel"];
                    DTransportations.Land = Land;
                    DTransportations.Sea = Sea;
                    DTransportations.Air = Air;
                    _MyDbContext.DTransportations.Update(DTransportations);
                    DTransportations = null;
                    _MyDbContext.DIntervalusetotals.RemoveRange(_MyDbContext.DIntervalusetotals.Where(a => a.BindId == id && a.BindWhere == "ResourceUse").ToList());//數據統計資料，全部刪除再塞新的進來

                    DIntervalusetotal DIntervalusetotal;
                    for (int i = 0; i < DateTimes.Length; i++)
                    {
                        DIntervalusetotal = new DIntervalusetotal()
                        {
                            BindId = id,
                            BindWhere = "ResourceUse",//設定屬於資源使用的表
                            Num = DateTimes[i],
                            Type = "DateTime",
                            ArraySort = i,
                        };
                        _MyDbContext.DIntervalusetotals.Add(DIntervalusetotal);
                        DIntervalusetotal = new DIntervalusetotal()
                        {
                            BindId = id,
                            BindWhere = "ResourceUse",
                            Num = StartTime[i] != null ? StartTime[i] : "",//為空時，給予空值
                            Type = "StartTime",
                            ArraySort = i,
                        };
                        _MyDbContext.DIntervalusetotals.Add(DIntervalusetotal);
                        DIntervalusetotal = new DIntervalusetotal()
                        {
                            BindId = id,
                            BindWhere = "ResourceUse",
                            Num = EndTime[i] != null ? EndTime[i] : "",
                            Type = "EndTime",
                            ArraySort = i,
                        };
                        _MyDbContext.DIntervalusetotals.Add(DIntervalusetotal);
                        DIntervalusetotal = new DIntervalusetotal()
                        {
                            BindId = id,
                            BindWhere = "ResourceUse",
                            Num = Num[i] != null ? Num[i] : "",
                            Type = "Num",
                            ArraySort = i,
                        };
                        _MyDbContext.DIntervalusetotals.Add(DIntervalusetotal);
                    }
                    DIntervalusetotal = new DIntervalusetotal()
                    {
                        BindId = id,
                        BindWhere = "ResourceUse",
                        Num = collection["Unit"],
                        Type = "Unit",
                    };
                    //新增單位
                    _MyDbContext.DIntervalusetotals.Add(DIntervalusetotal);
                    DIntervalusetotal = new DIntervalusetotal()
                    {
                        BindId = id,
                        BindWhere = "ResourceUse",
                        Num = SelMonth,
                        Type = "SelMonth",
                    };
                    _MyDbContext.DIntervalusetotals.Add(DIntervalusetotal);
                    DIntervalusetotal = new DIntervalusetotal()
                    {
                        BindId = id,
                        BindWhere = "ResourceUse",
                        Num = AddMonth.ToString(),
                        Type = "AddMonth",
                    };
                    //新增單位
                    _MyDbContext.DIntervalusetotals.Add(DIntervalusetotal);
                    DIntervalusetotal = null;
                    //更新數據來源資訊
                    var DDatasources = _MyDbContext.DDatasources.FirstOrDefault(e => e.BindId == id && e.BindWhere == "ResourceUse");
                    DDatasources.Management = collection["management"];
                    DDatasources.Principal = collection["Principal"];
                    DDatasources.Datasource = collection["Datasource"];
                    _MyDbContext.DDatasources.Update(DDatasources);
                    DDatasources = null;
                }

                //塞入總量，單位，檔案到資源使用的table
                var Resourceuses = _MyDbContext.Resourceuses.Find(id);
                Resourceuses.BeforeTotal = decimal.Parse(collection["TotalNum"]);
                Resourceuses.BeforeUnit = collection["Unit"];

                string directoryPath = FileSave.Path + a.BasicId + "/資源使用";
                //查詢有無此黨名的資料夾，沒有則新建
                if (!Directory.Exists(directoryPath))
                {
                    Directory.CreateDirectory(directoryPath);
                }
                foreach (var file in EvidenceFile)
                {
                    if (file?.Length > 0)
                    {
                        var fileName = id.ToString() + "_" + file.FileName;

                        // 檢查檔案是否已存在
                        var existingData = _MyDbContext.Evidencefilemanages
                            .FirstOrDefault(e =>
                                e.BasicId == basicid &&
                                e.ItemId == id &&
                                e.FileName == fileName &&
                                e.WhereForm == "資源使用");

                        if (existingData != null)
                        {
                            return Error("此檔名已經存在：" + file.FileName);
                        }

                        // 上傳檔案並新增至資料庫
                        var path = Path.Combine(directoryPath, fileName);
                        using (var stream = new FileStream(path, FileMode.Create))
                        {
                            file.CopyTo(stream);
                        }

                        var evidencefilemanage = new Evidencefilemanage()
                        {
                            BasicId = basicid,
                            ItemId = id,
                            FileName = fileName,
                            WhereForm = "資源使用",
                            Time = DateTime.Now
                        };
                        _MyDbContext.Evidencefilemanages.Add(evidencefilemanage);
                        evidencefilemanage = null;
                    }
                }

                _MyDbContext.SaveChanges();


                return Sucess();
            }
            catch (Exception ex)
            {
                var Log = new Log()
                {
                    WhereFunction = "OrganizeDetail_ResourceUsedetail_post",
                    Exception = ex.ToString(),
                    DateTime = DateTime.Now,
                };
                _MyDbContext.Logs.Add(Log);
                _MyDbContext.SaveChanges();
                Log = null;
                return Error("內部伺服器錯誤"); // 或其他適當的錯誤訊息
            }
        }

        public ActionResult Fireequipmentdetail(int basicid, int id)
        {
            try
            {
                ViewBag.basicid = basicid;//給input存取

                var a = _MyDbContext.Basicdatas.Find(basicid);
                int monthcount = (a.EndTime.Year - a.StartTime.Year) * 12 + a.EndTime.Month - a.StartTime.Month + 1;
                int year = a.StartTime.Year;
                int month = a.StartTime.Month;
                ViewBag.monthcount = monthcount;
                ViewBag.year = year;
                ViewBag.month = month;
                ViewBag.Unit = _MyDbContext.Selectdata.Where(a => a.Code == "10").Select(a => new { a.Code, a.Name, a.Sort }).OrderBy(a => a.Sort).ToList();//單位資料



                //viewmodel
                var DIntervalusetotals_DateTime = _MyDbContext.DIntervalusetotals.Where(a => a.BindId == id && a.Type == "DateTime" && a.BindWhere == "Fireequipment").ToList();
                var DIntervalusetotals_Num = _MyDbContext.DIntervalusetotals.Where(a => a.BindId == id && a.Type == "Num" && a.BindWhere == "Fireequipment").ToList();
                var Unit = _MyDbContext.DIntervalusetotals.FirstOrDefault(a => a.BindId == id && a.Type == "Unit" && a.BindWhere == "Fireequipment");

                var DDatasources = _MyDbContext.DDatasources.FirstOrDefault(a => a.BindId == id && a.BindWhere == "Fireequipment");
                if (DDatasources?.EvidenceFile != null)
                {
                    ViewBag.EvidenceFile = AccountId() + "/" + a.BasicId + "/" + DDatasources.EvidenceFile;

                }

                var viewModel = new Fireequipmentdetail
                {

                    DIntervalusetotals_DateTime = DIntervalusetotals_DateTime,
                    DIntervalusetotals_Num = DIntervalusetotals_Num,
                    Unit = Unit,
                    DDatasource = DDatasources
                };

                DIntervalusetotals_DateTime = null;
                DIntervalusetotals_Num = null;
                Unit = null;
                DDatasources = null;

                return View(viewModel);
            }
            catch (Exception ex)
            {
                var Log = new Log()
                {
                    WhereFunction = "OrganzieDetail_Fireequipmentdetail_get",
                    Exception = ex.ToString(),
                    DateTime = DateTime.Now,
                };
                _MyDbContext.Logs.Add(Log);
                _MyDbContext.SaveChanges();
                Log = null;
                return Error("內部伺服器錯誤"); // 或其他適當的錯誤訊息
            }

        }

        // POST: ProductController/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Fireequipmentdetail(int basicid, int id, IFormCollection? collection, string[]? DateTimes, string[]? Num, List<IFormFile> EvidenceFile)

        {
            try
            {
                if (AccountId() == null)
                {
                    return Nologin();
                }
                var entity = _MyDbContext.DDatasources.FirstOrDefault(e => e.BindId == id && e.BindWhere == "Fireequipment");
                Console.Write(entity);
                var a = _MyDbContext.Basicdatas.Find(basicid);
                /**/
                //如果沒資料則新增資料
                if (entity == null)
                {

                    DIntervalusetotal DIntervalusetotal;
                    for (int i = 0; i < DateTimes.Length; i++)
                    {
                        DIntervalusetotal = new DIntervalusetotal()
                        {
                            BindId = id,
                            BindWhere = "Fireequipment",
                            Num = DateTimes[i],
                            Type = "DateTime",
                            ArraySort = i,
                        };
                        _MyDbContext.DIntervalusetotals.Add(DIntervalusetotal);
                        DIntervalusetotal = new DIntervalusetotal()
                        {
                            BindId = id,
                            BindWhere = "Fireequipment",
                            Num = Num[i] != null ? Num[i] : "",
                            Type = "Num",
                            ArraySort = i,
                        };
                        _MyDbContext.DIntervalusetotals.Add(DIntervalusetotal);
                    }
                    DIntervalusetotal = new DIntervalusetotal()
                    {
                        BindId = id,
                        BindWhere = "Fireequipment",
                        Num = collection["Unit"],
                        Type = "Unit",
                    };
                    _MyDbContext.DIntervalusetotals.Add(DIntervalusetotal);
                    DIntervalusetotal = null;
                    var DDatasource = new DDatasource()
                    {
                        BindId = id,
                        BindWhere = "Fireequipment",
                        Management = collection["management"],
                        Principal = collection["Principal"],
                        Datasource = collection["Datasource"],
                    };
                    _MyDbContext.DDatasources.Add(DDatasource);
                }
                else
                {

                    _MyDbContext.DIntervalusetotals.RemoveRange(_MyDbContext.DIntervalusetotals.Where(a => a.BindId == id && a.BindWhere == "Fireequipment").ToList());

                    DIntervalusetotal DIntervalusetotal;
                    for (int i = 0; i < DateTimes.Length; i++)
                    {
                        DIntervalusetotal = new DIntervalusetotal()
                        {
                            BindId = id,
                            BindWhere = "Fireequipment",
                            Num = DateTimes[i],
                            Type = "DateTime",
                            ArraySort = i,
                        };
                        _MyDbContext.DIntervalusetotals.Add(DIntervalusetotal);
                        DIntervalusetotal = new DIntervalusetotal()
                        {
                            BindId = id,
                            BindWhere = "Fireequipment",
                            Num = Num[i] != null ? Num[i] : "",
                            Type = "Num",
                            ArraySort = i,
                        };
                        _MyDbContext.DIntervalusetotals.Add(DIntervalusetotal);
                    }
                    DIntervalusetotal = new DIntervalusetotal()
                    {
                        BindId = id,
                        BindWhere = "Fireequipment",
                        Num = collection["Unit"],
                        Type = "Unit",
                    };
                    _MyDbContext.DIntervalusetotals.Add(DIntervalusetotal);
                    DIntervalusetotal = null;

                    var DDatasources = _MyDbContext.DDatasources.FirstOrDefault(e => e.BindId == id && e.BindWhere == "Fireequipment");
                    Console.Write(DDatasources);
                    DDatasources.Management = collection["management"];
                    DDatasources.Principal = collection["Principal"];
                    DDatasources.Datasource = collection["Datasource"];

                    _MyDbContext.DDatasources.Update(DDatasources);
                    DDatasources = null;
                }

                var Fireequipment = _MyDbContext.Fireequipments.Find(id);

                Fireequipment.BeforeTotal = decimal.Parse(collection["TotalNum"]);
                Fireequipment.BeforeUnit = collection["Unit"];
                _MyDbContext.Fireequipments.Update(Fireequipment);
                Fireequipment = null;


                string directoryPath = FileSave.Path + a.BasicId + "/消防設備";
                //查詢有無此黨名的資料夾，沒有則新建
                if (!Directory.Exists(directoryPath))
                {
                    Directory.CreateDirectory(directoryPath);
                }
                foreach (var file in EvidenceFile)
                {
                    if (file?.Length > 0)
                    {
                        var fileName = id.ToString() + "_" + file.FileName;

                        // 檢查檔案是否已存在
                        var existingData = _MyDbContext.Evidencefilemanages
                            .FirstOrDefault(e =>
                                e.BasicId == basicid &&
                                e.ItemId == id &&
                                e.FileName == fileName &&
                                e.WhereForm == "消防設備");

                        if (existingData != null)
                        {
                            return Error("此檔名已經存在：" + file.FileName);
                        }

                        // 上傳檔案並新增至資料庫
                        var path = Path.Combine(directoryPath, fileName);
                        using (var stream = new FileStream(path, FileMode.Create))
                        {
                            file.CopyTo(stream);
                        }

                        var evidencefilemanage = new Evidencefilemanage()
                        {
                            BasicId = basicid,
                            ItemId = id,
                            FileName = fileName,
                            WhereForm = "消防設備",
                            Time = DateTime.Now
                        };
                        _MyDbContext.Evidencefilemanages.Add(evidencefilemanage);
                        evidencefilemanage = null;
                    }
                }
                _MyDbContext.SaveChanges();


                return Sucess();


            }
            catch (Exception ex)
            {
                var Log = new Log()
                {
                    WhereFunction = "OrganzieDetail_Fireequipmentdetail_post",
                    Exception = ex.ToString(),
                    DateTime = DateTime.Now,
                };
                _MyDbContext.Logs.Add(Log);
                _MyDbContext.SaveChanges();
                Log = null;
                return Error("內部伺服器錯誤"); // 或其他適當的錯誤訊息
            }
        }


        public ActionResult DumptreatmentOutsourcingdetail(int basicid, int id)
        {
            try
            {
                ViewBag.basicid = basicid;//給input存取    
                var a = _MyDbContext.Basicdatas.Find(basicid);
                int monthcount = (a.EndTime.Year - a.StartTime.Year) * 12 + a.EndTime.Month - a.StartTime.Month + 1;//相差多少月份
                int year = a.StartTime.Year;//系統從盤查區間自動帶出年
                int month = a.StartTime.Month;//系統自動帶出樂
                ViewBag.EndYear = a.EndTime.Year;
                ViewBag.EndMonth = a.EndTime.Month;
                ViewBag.monthcount = monthcount;//月份級距
                ViewBag.year = year;
                ViewBag.month = month;
                ViewBag.Unit = _MyDbContext.Selectdata.Where(a => a.Code == "10" || a.Code == "11");
                ViewBag.Method = _MyDbContext.Selectdata.Where(a => a.Type == "Transportation");
                ViewBag.Car = _MyDbContext.Selectdata.Where(a => a.Type == "Car");
                var organize = _MyDbContext.Organizes.Where(a => a.BasicId == basicid).First();
                ViewBag.BasicDataStatus = organize.Status;
                ViewBag.AddMonth = _MyDbContext.DIntervalusetotals.FirstOrDefault(a => a.BindId == id && a.Type == "AddMonth" && a.BindWhere == "DumptreatmentOutsourcing");

                //viewmodel資料設定
                var DTransportation = _MyDbContext.DTransportations.FirstOrDefault(a => a.BindId == id && a.BindWhere == "DumptreatmentOutsourcing");
                var DIntervalusetotals_DateTime = _MyDbContext.DIntervalusetotals.Where(a => a.BindId == id && a.Type == "DateTime" && a.BindWhere == "DumptreatmentOutsourcing").ToList();
                var DIntervalusetotals_StartTime = _MyDbContext.DIntervalusetotals.Where(a => a.BindId == id && a.Type == "StartTime" && a.BindWhere == "DumptreatmentOutsourcing").ToList();
                var DIntervalusetotals_EndTime = _MyDbContext.DIntervalusetotals.Where(a => a.BindId == id && a.Type == "EndTime" && a.BindWhere == "DumptreatmentOutsourcing").ToList();
                var DIntervalusetotals_Num = _MyDbContext.DIntervalusetotals.Where(a => a.BindId == id && a.Type == "Num" && a.BindWhere == "DumptreatmentOutsourcing").ToList();
                var Unit = _MyDbContext.DIntervalusetotals.FirstOrDefault(a => a.BindId == id && a.Type == "Unit" && a.BindWhere == "DumptreatmentOutsourcing");
                var SelMonth = _MyDbContext.DIntervalusetotals.FirstOrDefault(a => a.BindId == id && a.Type == "SelMonth" && a.BindWhere == "DumptreatmentOutsourcing");

                var DDatasources = _MyDbContext.DDatasources.FirstOrDefault(a => a.BindId == id && a.BindWhere == "DumptreatmentOutsourcing");
                if (DDatasources?.EvidenceFile != null)//檔案不為空時，給予前端黨名
                {
                    ViewBag.EvidenceFile = AccountId() + "/" + a.BasicId + "/" + DDatasources.EvidenceFile;
                }
                //把資料塞進viewmodel
                var viewModel = new DumptreatmentOutsourcingdetail
                {
                    DTransportation = DTransportation,
                    DIntervalusetotals_DateTime = DIntervalusetotals_DateTime,
                    DIntervalusetotals_StartTime = DIntervalusetotals_StartTime,
                    DIntervalusetotals_EndTime = DIntervalusetotals_EndTime,
                    DIntervalusetotals_Num = DIntervalusetotals_Num,
                    Unit = Unit,
                    SelMonth= SelMonth,
                    DDatasource = DDatasources
                };
                DTransportation = null;
                DIntervalusetotals_DateTime = null;
                DIntervalusetotals_StartTime = null;
                DIntervalusetotals_EndTime = null;
                DIntervalusetotals_Num = null;
                Unit = null;
                DDatasources = null;

                return View(viewModel);
            }
            catch (Exception ex)
            {
                var Log = new Log()
                {
                    WhereFunction = "OrganizeDetail_DumptreatmentOutsourcingdetail_get",
                    Exception = ex.ToString(),
                    DateTime = DateTime.Now,
                };
                _MyDbContext.Logs.Add(Log);
                _MyDbContext.SaveChanges();
                Log = null;
                return Error("內部伺服器錯誤"); // 或其他適當的錯誤訊息
            }
        }
        // POST: OrganizeController/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult DumptreatmentOutsourcingdetail(int basicid, int id, IFormCollection? collection, decimal? Tonnes, decimal? Land, decimal? Sea, decimal? Air,
            string[]? DateTimes, string[]? StartTime, string[]? EndTime, string[]? Num,string SelMonth, List<IFormFile> EvidenceFile, int? AddMonth)
        {
            try
            {
                if (AccountId() == null)//帳號為空時，登出
                {
                    return Nologin();
                }
                var entity = _MyDbContext.DTransportations.FirstOrDefault(e => e.BindId == id && e.BindWhere == "DumptreatmentOutsourcing");
                var a = _MyDbContext.Basicdatas.Find(basicid);

                //如果沒資料則新增資料
                if (entity == null)
                {
                    var DTransportation = new DTransportation()
                    {
                        BindId = id,
                        BindWhere = "DumptreatmentOutsourcing",
                        Method = collection["Method"],
                        StartLocation = collection["StartLocation"],
                        EndLocation = collection["EndLocation"],
                        Car = collection["Car"],
                        Tonnes = Tonnes,
                        Fuel = collection["Fuel"],
                        Land = Land,
                        Sea = Sea,
                        Air = Air,
                    };
                    //新增運輸資訊
                    _MyDbContext.DTransportations.Add(DTransportation);
                    //依照多少欄位新增數據統計
                    DIntervalusetotal DIntervalusetotal;
                    for (int i = 0; i < DateTimes.Length; i++)
                    {
                        DIntervalusetotal = new DIntervalusetotal()
                        {
                            BindId = id,
                            BindWhere = "DumptreatmentOutsourcing",//設定屬於空水廢委外處理的表
                            Num = DateTimes[i],
                            Type = "DateTime",
                            ArraySort = i,
                        };
                        _MyDbContext.DIntervalusetotals.Add(DIntervalusetotal);
                        DIntervalusetotal = new DIntervalusetotal()
                        {
                            BindId = id,
                            BindWhere = "DumptreatmentOutsourcing",
                            Num = StartTime[i] != null ? StartTime[i] : "",//為空時，給予空值
                            Type = "StartTime",
                            ArraySort = i,
                        };
                        _MyDbContext.DIntervalusetotals.Add(DIntervalusetotal);
                        DIntervalusetotal = new DIntervalusetotal()
                        {
                            BindId = id,
                            BindWhere = "DumptreatmentOutsourcing",
                            Num = EndTime[i] != null ? EndTime[i] : "",
                            Type = "EndTime",
                            ArraySort = i,
                        };
                        _MyDbContext.DIntervalusetotals.Add(DIntervalusetotal);
                        DIntervalusetotal = new DIntervalusetotal()
                        {
                            BindId = id,
                            BindWhere = "DumptreatmentOutsourcing",
                            Num = Num[i] != null ? Num[i] : "",
                            Type = "Num",
                            ArraySort = i,
                        };

                        _MyDbContext.DIntervalusetotals.Add(DIntervalusetotal);
                    }
                    DIntervalusetotal = new DIntervalusetotal()
                    {
                        BindId = id,
                        BindWhere = "DumptreatmentOutsourcing",
                        Num = collection["Unit"],
                        Type = "Unit",
                    };
                    //新增單位
                    _MyDbContext.DIntervalusetotals.Add(DIntervalusetotal);
                    DIntervalusetotal = new DIntervalusetotal()
                    {
                        BindId = id,
                        BindWhere = "DumptreatmentOutsourcing",
                        Num = SelMonth,
                        Type = "SelMonth",
                    };
                    _MyDbContext.DIntervalusetotals.Add(DIntervalusetotal);
                    DIntervalusetotal = new DIntervalusetotal()
                    {
                        BindId = id,
                        BindWhere = "DumptreatmentOutsourcing",
                        Num = AddMonth.ToString(),
                        Type = "AddMonth",
                    };
                    //新增單位
                    _MyDbContext.DIntervalusetotals.Add(DIntervalusetotal);
                    DIntervalusetotal = null;

                    //新增數據來源資訊
                    var DDatasource = new DDatasource()
                    {
                        BindId = id,
                        BindWhere = "DumptreatmentOutsourcing",
                        Management = collection["management"],
                        Principal = collection["Principal"],
                        Datasource = collection["Datasource"],
                    };
                    _MyDbContext.DDatasources.Add(DDatasource);
                    DDatasource = null;
                }
                else//有資料時則更新資料
                {
                    //更新運輸資訊
                    var DTransportations = _MyDbContext.DTransportations.FirstOrDefault(e => e.BindId == id && e.BindWhere == "DumptreatmentOutsourcing");
                    DTransportations.Method = collection["Method"];
                    DTransportations.StartLocation = collection["StartLocation"];
                    DTransportations.EndLocation = collection["EndLocation"];
                    DTransportations.Car = collection["Car"];
                    DTransportations.Tonnes = Tonnes;
                    DTransportations.Fuel = collection["Fuel"];
                    DTransportations.Land = Land;
                    DTransportations.Sea = Sea;
                    DTransportations.Air = Air;
                    _MyDbContext.DTransportations.Update(DTransportations);
                    DTransportations = null;
                    _MyDbContext.DIntervalusetotals.RemoveRange(_MyDbContext.DIntervalusetotals.Where(a => a.BindId == id && a.BindWhere == "DumptreatmentOutsourcing").ToList());//數據統計資料，全部刪除再塞新的進來

                    DIntervalusetotal DIntervalusetotal;
                    for (int i = 0; i < DateTimes.Length; i++)
                    {
                        DIntervalusetotal = new DIntervalusetotal()
                        {
                            BindId = id,
                            BindWhere = "DumptreatmentOutsourcing",//設定屬於能源使用的表
                            Num = DateTimes[i],
                            Type = "DateTime",
                            ArraySort = i,
                        };
                        _MyDbContext.DIntervalusetotals.Add(DIntervalusetotal);
                        DIntervalusetotal = new DIntervalusetotal()
                        {
                            BindId = id,
                            BindWhere = "DumptreatmentOutsourcing",
                            Num = StartTime[i] != null ? StartTime[i] : "",//為空時，給予空值
                            Type = "StartTime",
                            ArraySort = i,
                        };
                        _MyDbContext.DIntervalusetotals.Add(DIntervalusetotal);
                        DIntervalusetotal = new DIntervalusetotal()
                        {
                            BindId = id,
                            BindWhere = "DumptreatmentOutsourcing",
                            Num = EndTime[i] != null ? EndTime[i] : "",
                            Type = "EndTime",
                            ArraySort = i,
                        };
                        _MyDbContext.DIntervalusetotals.Add(DIntervalusetotal);
                        DIntervalusetotal = new DIntervalusetotal()
                        {
                            BindId = id,
                            BindWhere = "DumptreatmentOutsourcing",
                            Num = Num[i] != null ? Num[i] : "",
                            Type = "Num",
                            ArraySort = i,
                        };
                        _MyDbContext.DIntervalusetotals.Add(DIntervalusetotal);
                    }
                    DIntervalusetotal = new DIntervalusetotal()
                    {
                        BindId = id,
                        BindWhere = "DumptreatmentOutsourcing",
                        Num = collection["Unit"],
                        Type = "Unit",
                    };
                    //新增單位
                    _MyDbContext.DIntervalusetotals.Add(DIntervalusetotal);
                    DIntervalusetotal = new DIntervalusetotal()
                    {
                        BindId = id,
                        BindWhere = "DumptreatmentOutsourcing",
                        Num = SelMonth,
                        Type = "SelMonth",
                    };
                    _MyDbContext.DIntervalusetotals.Add(DIntervalusetotal);
                    DIntervalusetotal = new DIntervalusetotal()
                    {
                        BindId = id,
                        BindWhere = "DumptreatmentOutsourcing",
                        Num = AddMonth.ToString(),
                        Type = "AddMonth",
                    };
                    //新增單位
                    _MyDbContext.DIntervalusetotals.Add(DIntervalusetotal);
                    DIntervalusetotal = null;
                    //更新數據來源資訊
                    var DDatasources = _MyDbContext.DDatasources.FirstOrDefault(e => e.BindId == id && e.BindWhere == "DumptreatmentOutsourcing");
                    DDatasources.Management = collection["management"];
                    DDatasources.Principal = collection["Principal"];
                    DDatasources.Datasource = collection["Datasource"];
                    _MyDbContext.DDatasources.Update(DDatasources);
                    DDatasources = null;
                }

                //塞入總量，單位，檔案到能源使用的table
                var DumptreatmentOutsourcings = _MyDbContext.DumptreatmentOutsourcings.Find(id);
                DumptreatmentOutsourcings.BeforeTotal = decimal.Parse(collection["TotalNum"]);
                DumptreatmentOutsourcings.BeforeUnit = collection["Unit"];

                string directoryPath = FileSave.Path + a.BasicId + "/廢棄物處理";
                //查詢有無此黨名的資料夾，沒有則新建
                if (!Directory.Exists(directoryPath))
                {
                    Directory.CreateDirectory(directoryPath);
                }
                foreach (var file in EvidenceFile)
                {
                    if (file?.Length > 0)
                    {
                        var fileName = id.ToString() + "_" + file.FileName;

                        // 檢查檔案是否已存在
                        var existingData = _MyDbContext.Evidencefilemanages
                            .FirstOrDefault(e =>
                                e.BasicId == basicid &&
                                e.ItemId == id &&
                                e.FileName == fileName &&
                                e.WhereForm == "廢棄物處理");

                        if (existingData != null)
                        {
                            return Error("此檔名已經存在：" + file.FileName);
                        }

                        // 上傳檔案並新增至資料庫
                        var path = Path.Combine(directoryPath, fileName);
                        using (var stream = new FileStream(path, FileMode.Create))
                        {
                            file.CopyTo(stream);
                        }

                        var evidencefilemanage = new Evidencefilemanage()
                        {
                            BasicId = basicid,
                            ItemId = id,
                            FileName = fileName,
                            WhereForm = "廢棄物處理",
                            Time = DateTime.Now
                        };
                        _MyDbContext.Evidencefilemanages.Add(evidencefilemanage);
                        evidencefilemanage = null;
                    }
                }

                _MyDbContext.SaveChanges();


                return Sucess();
            }
            catch (Exception ex)
            {
                var Log = new Log()
                {
                    WhereFunction = "OrganizeDetail_DumptreatmentOutsourcingdetail_post",
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
