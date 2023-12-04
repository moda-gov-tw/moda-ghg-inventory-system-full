using Project.Common;
using Project.Models;
using Project.Models.View;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Org.BouncyCastle.Crypto;
using System.IO;
using System.Linq.Expressions;
using X.PagedList;

namespace Project.Controllers
{
    public class OrganizeController : CommonController
    {
        private MyDbContext _MyDbContext;
        private readonly IConfiguration _configuration;
        public OrganizeController(MyDbContext MyDbContext, IConfiguration configuration)
        {
            _MyDbContext = MyDbContext;
            _configuration = configuration;

        }
        // GET: ProductController/Details/5
        //盤查表detail
        public ActionResult Details(int id, int? page, string? productname = "", DateTime? starttime = null, DateTime? endtime = null)
        {
            try
            {
                if (string.IsNullOrEmpty(HttpContext.Session.GetString("AccountId")))
                {

                }


                int pageSize = 10; // 每頁顯示的項目數量
                int pageNumber = (page ?? 1); // 當前頁數（如果未指定，預設為第1頁）

                var a = _MyDbContext.Organizes.AsQueryable().Where(a => a.Account == AccountId());
                //關鍵字搜尋條件
                if (!string.IsNullOrEmpty(productname))
                {
                    a = a.Where(p => p.Inventory.Contains(productname));
                }
                if (starttime.HasValue)
                {
                    a = a.Where(a => a.StartTime >= starttime.Value.Date);
                }
                if (endtime.HasValue)
                {
                    a = a.Where(a => a.EndTime <= endtime.Value.Date);
                }
                var pagedList = a.OrderByDescending(a => a.Id).ToPagedList(pageNumber, pageSize);
                return View(pagedList);
            }
            catch (Exception ex)
            {
                var Log = new Log()
                {
                    WhereFunction = "Organize_Details_get",
                    Exception = ex.ToString(),
                    DateTime = DateTime.Now,
                };
                _MyDbContext.Logs.Add(Log);
                _MyDbContext.SaveChanges();
                Log = null;
                return Error("內部伺服器錯誤"); // 或其他適當的錯誤訊息
            }
        }
        public ActionResult Group(int id, int? page, string? productname = "", DateTime? starttime = null, DateTime? endtime = null)
        {
            try
            {
                int pageSize = 10; // 每頁顯示的項目數量
                int pageNumber = (page ?? 1); // 當前頁數（如果未指定，預設為第1頁）

                var a = _MyDbContext.Organizes.Where(a => a.BasicId == id).ToList();
                var Member = _MyDbContext.Members.ToList();
                ViewBag.Member = Member;

                return View(a);
            }
            catch (Exception ex)
            {
                var Log = new Log()
                {
                    WhereFunction = "Organize_Group_get",
                    Exception = ex.ToString(),
                    DateTime = DateTime.Now,
                };
                _MyDbContext.Logs.Add(Log);
                _MyDbContext.SaveChanges();
                Log = null;
                return Error("內部伺服器錯誤"); // 或其他適當的錯誤訊息
            }
        }
        /// <summary>
        /// 基本資料表
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public ActionResult BasicData(int? id)
        {
            try
            {
                var Member = _MyDbContext.Members.ToList();
                ViewBag.Member = JsonConvert.SerializeObject(_MyDbContext.Members.Select(a => new { MemberId = a.MemberId.ToString(), a.Name }).ToList());//單位資料

                if (id != null)
                {
                    var a = _MyDbContext.Basicdatas.Find(id);
                    var organize = _MyDbContext.Organizes.Where(a => a.BasicId == id).First();
                    var factory = _MyDbContext.BasicdataFactoryaddresses.Where(a => a.BasicId == id && a.WherePlace == "CheckField").Select(a => new { a.Name, a.Address, a.Id }).ToList(); // 工廠地址陣列
                    var CheckFieldoutside = _MyDbContext.BasicdataFactoryaddresses.Where(a => a.BasicId == id && a.WherePlace == "OutsideCheckField").Select(a => new { a.Name, a.Address, a.Id }).ToList(); // 工廠地址陣列
                    var InDeduction = _MyDbContext.BasicdataFactoryaddresses.Where(a => a.BasicId == id && a.WherePlace == "InDeduction").Select(a => new { a.Name, a.Address, a.Id }).ToList(); // 工廠地址陣列
                    var Group = _MyDbContext.Organizes.Where(b => b.BasicId == id && b.Account != a.Account) // 添加额外的 where 条件
                            .Join(
                                _MyDbContext.Members,
                                a => a.Account,
                                b => b.MemberId.ToString(),
                                (a, b) => new
                                {
                                    MemberId = a.Account,
                                    Id = a.Id,
                                    Name = b.Name
                                })
                            .ToList();
                    string aaa = AccountId();


                    ViewBag.factory = JsonConvert.SerializeObject(factory);
                    ViewBag.CheckFieldoutside = JsonConvert.SerializeObject(CheckFieldoutside);
                    ViewBag.InDeduction = JsonConvert.SerializeObject(InDeduction);
                    ViewBag.Group = JsonConvert.SerializeObject(Group);
                    return View(a);

                }
                return View();

            }
            catch (Exception ex)
            {
                var Log = new Log()
                {
                    WhereFunction = "Organize_BasicData_get",
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
        public async Task<ActionResult> BasicDataAsync(int? id, IFormCollection? collection, DateTime StartTime, DateTime EndTime)
        {
            try
            {
                if (AccountId() == null)
                {
                    return Nologin();
                }
                //新資料跑這
                if (id == null)
                {

                    var Basicdata = new Basicdata()
                    {
                        OrganName = collection["OrganName"],
                        OrganNumber = collection["OrganNumber"],
                        ContactPerson = collection["ContactPerson"],
                        ContactPhone = collection["ContactPhone"],
                        ContactEmail = collection["ContactEmail"],
                        StartTime = StartTime,
                        EndTime = EndTime,
                        Account = AccountId(),
                    };
                    //基本資料
                    var Basicdatas = _MyDbContext.Basicdatas.Add(Basicdata);
                    _MyDbContext.SaveChanges();
                    int BasicId = Basicdatas.Entity.BasicId;//該筆基本資料的Id
                    string directoryPath = FileSave.Path + AccountId() + "/" + BasicId;
                    //查詢有無此黨名的資料夾，沒有則新建
                    if (!Directory.Exists(directoryPath))
                    {
                        Directory.CreateDirectory(directoryPath);
                    }

                    Basicdata = null;
                    BasicdataFactoryaddress? BasicdataFactoryaddress;
                    Basicdatagroup Basicdatagroup;
                    Workinghour? Workinghour;
                    Organize Organize;
                    //儲存生產地點
                    for (int i = 1; ; i++)
                    {
                        string name = "name" + i;
                        string address = "address" + i;
                        string namevalue = collection[name];
                        string addressvalue = collection[address];
                        //陣列沒資料跳出
                        if (string.IsNullOrEmpty(namevalue))
                        {
                            break;
                        }

                        BasicdataFactoryaddress = new BasicdataFactoryaddress()
                        {
                            BasicId = BasicId,
                            Name = namevalue,
                            Address = addressvalue,
                            WherePlace = "CheckField",
                            Sort = i,
                        };

                        var factoryid = _MyDbContext.BasicdataFactoryaddresses.Add(BasicdataFactoryaddress);
                        _MyDbContext.SaveChanges();
                        //塞入工時資料
                        int newfactoryid = factoryid.Entity.Id;
                        for (int k = 0; k < 2; k++)
                        {
                            Workinghour = new Workinghour()
                            {
                                BasicId = _MyDbContext.Basicdatas.OrderBy(a => a.BasicId).Last().BasicId,
                                FactoryId = newfactoryid,
                                Item = k.ToString(),
                            };
                            _MyDbContext.Workinghours.Add(Workinghour);

                        }

                    }
                    for (int i = 1; ; i++)
                    {
                        string name = "factoryoutsidename" + i;
                        string address = "factoryoutsideaddress" + i;
                        string namevalue = collection[name];
                        string addressvalue = collection[address];
                        //陣列沒資料跳出
                        if (string.IsNullOrEmpty(namevalue))
                        {
                            break;
                        }

                        BasicdataFactoryaddress = new BasicdataFactoryaddress()
                        {
                            BasicId = BasicId,
                            Name = namevalue,
                            Address = addressvalue,
                            WherePlace = "OutsideCheckField",
                            Sort = i,
                        };

                        var factoryid = _MyDbContext.BasicdataFactoryaddresses.Add(BasicdataFactoryaddress);
                        _MyDbContext.SaveChanges();
                        //塞入工時資料
                        int newfactoryid = factoryid.Entity.Id;
                        for (int k = 0; k < 2; k++)
                        {
                            Workinghour = new Workinghour()
                            {
                                BasicId = _MyDbContext.Basicdatas.OrderBy(a => a.BasicId).Last().BasicId,
                                FactoryId = newfactoryid,
                                Item = k.ToString(),
                            };
                            _MyDbContext.Workinghours.Add(Workinghour);

                        }

                    }
                    for (int i = 1; ; i++)
                    {
                        string name = "factoryInDeductionname" + i;
                        string address = "factoryInDeductionaddress" + i;
                        string namevalue = collection[name];
                        string addressvalue = collection[address];
                        //陣列沒資料跳出
                        if (string.IsNullOrEmpty(namevalue))
                        {
                            break;
                        }

                        BasicdataFactoryaddress = new BasicdataFactoryaddress()
                        {
                            BasicId = BasicId,
                            Name = namevalue,
                            Address = addressvalue,
                            WherePlace = "InDeduction",
                            Sort = i,
                        };

                        var factoryid = _MyDbContext.BasicdataFactoryaddresses.Add(BasicdataFactoryaddress);
                        _MyDbContext.SaveChanges();
                    }
                    for (int i = 1; ; i++)
                    {
                        string name = "PersonName" + i;
                        string namevalue = collection[name];
                        //陣列沒資料跳出
                        if (string.IsNullOrEmpty(namevalue))
                        {
                            break;
                        }

                        Organize = new Organize()
                        {
                            BasicId = BasicId,
                            Inventory = collection["OrganName"],
                            ContactName = collection["ContactPerson"],
                            StartTime = StartTime,
                            EndTime = EndTime,
                            UpdateTime = DateTime.Parse(DateTime.Now.ToString("yyyy-MM-dd")),
                            Status = "盤查中",
                            Account = collection[name]
                        };
                        _MyDbContext.Organizes.Add(Organize);

                        _MyDbContext.SaveChanges();
                    }

                    Organize = new Organize()
                    {
                        BasicId = BasicId,
                        Inventory = collection["OrganName"],
                        ContactName = collection["ContactPerson"],
                        StartTime = StartTime,
                        EndTime = EndTime,
                        UpdateTime = DateTime.Parse(DateTime.Now.ToString("yyyy-MM-dd")),
                        Status = "盤查中",
                        Account = AccountId()
                    };
                    //儲存碳足跡盤查表
                    _MyDbContext.Organizes.Add(Organize);
                    _MyDbContext.SaveChanges();
                    Organize = null;
                    BasicdataFactoryaddress = null;

                    HttpContext.Session.SetInt32("basicid", BasicId);

                    return Sucess(BasicId);

                }
                //原本已經有資料
                else
                {
                    var Basicdatas = _MyDbContext.Basicdatas.Find(id);


                    string directoryPath = FileSave.Path + AccountId() + "/" + id;
                    Basicdatas.OrganName = collection["OrganName"];
                    Basicdatas.OrganNumber = collection["OrganNumber"];
                    Basicdatas.ContactPerson = collection["ContactPerson"];
                    Basicdatas.ContactPhone = collection["ContactPhone"];
                    Basicdatas.ContactEmail = collection["ContactEmail"];
                    Basicdatas.StartTime = StartTime;
                    Basicdatas.EndTime = EndTime;
                    _MyDbContext.Basicdatas.Update(Basicdatas);


                    var Organizes = _MyDbContext.Organizes.Where(a => a.BasicId == id).FirstOrDefault();
                    Organizes.Inventory = collection["OrganName"];
                    Organizes.StartTime = StartTime;
                    Organizes.EndTime = EndTime;
                    Organizes.UpdateTime = DateTime.Parse(DateTime.Now.ToString("yyyy-MM-dd"));
                    Organizes.Status = "盤查中";
                    _MyDbContext.Organizes.Update(Organizes);


                    //_MyDbContext.BasicdataFactoryaddresses.RemoveRange(_MyDbContext.BasicdataFactoryaddresses.Where(a => a.BasicId == id).ToList());

                    BasicdataFactoryaddress BasicdataFactoryaddress;
                    Basicdatagroup Basicdatagroup;
                    Workinghour Workinghour;
                    Organize Organize;
                    // 记录要删除的 ID 列表
                    List<int> idsToDelete = new List<int>();
                    for (int i = 1; ; i++)
                    {
                        string factoryID = "factoryID" + i;//給予陣列名稱
                        string name = "name" + i;//給予陣列名
                        string address = "address" + i;
                        string namevalue = collection[name];//依哪個陣列名稱抓取前端的值
                        string addressvalue = collection[address];
                        string factoryvalue = collection[factoryID];
                        //陣列沒資料跳出
                        if (string.IsNullOrEmpty(namevalue))
                        {
                            break;
                        }
                        if (factoryvalue == "")
                        {
                            BasicdataFactoryaddress = new BasicdataFactoryaddress()
                            {
                                BasicId = (int)id,
                                Name = namevalue,
                                Address = addressvalue,
                                WherePlace = "CheckField",
                                Sort = i,
                            };

                            var factoryid = _MyDbContext.BasicdataFactoryaddresses.Add(BasicdataFactoryaddress);
                            _MyDbContext.SaveChanges();
                            int newfactoryid = factoryid.Entity.Id;
                            idsToDelete.Add(newfactoryid);

                            //工時資料
                            for (int k = 0; k < 2; k++)
                            {
                                Workinghour = new Workinghour()
                                {
                                    BasicId = (int)id,
                                    FactoryId = newfactoryid,
                                    Item = k.ToString(),
                                };
                                _MyDbContext.Workinghours.Add(Workinghour);

                            }
                        }
                        else
                        {
                            var BasicdataFactoryaddresses = _MyDbContext.BasicdataFactoryaddresses.Where(a => a.BasicId == id && a.Id.ToString() == factoryvalue).FirstOrDefault();
                            BasicdataFactoryaddresses.Name = namevalue;
                            BasicdataFactoryaddresses.Address = addressvalue;
                            _MyDbContext.BasicdataFactoryaddresses.Update(BasicdataFactoryaddresses);
                            idsToDelete.Add(int.Parse(factoryvalue));
                        }

                    }
                    List<int> idsToDelete1 = new List<int>();
                    for (int i = 1; ; i++)
                    {
                        string factoryID = "factoryoutsideID" + i;//給予陣列名稱
                        string name = "factoryoutsidename" + i;//給予陣列名稱
                        string address = "factoryoutsideaddress" + i;
                        string namevalue = collection[name];//依哪個陣列名稱抓取前端的值
                        string addressvalue = collection[address];
                        string factoryvalue = collection[factoryID];
                        //陣列沒資料跳出
                        if (string.IsNullOrEmpty(namevalue))
                        {
                            break;
                        }
                        if (factoryvalue == "")
                        {
                            BasicdataFactoryaddress = new BasicdataFactoryaddress()
                            {
                                BasicId = (int)id,
                                Name = namevalue,
                                Address = addressvalue,
                                WherePlace = "OutsideCheckField",
                                Sort = i,
                            };

                            var factoryid = _MyDbContext.BasicdataFactoryaddresses.Add(BasicdataFactoryaddress);
                            _MyDbContext.SaveChanges();
                            int newfactoryid = factoryid.Entity.Id;
                            idsToDelete1.Add(newfactoryid);

                            //工時資料
                            for (int k = 0; k < 2; k++)
                            {
                                Workinghour = new Workinghour()
                                {
                                    BasicId = (int)id,
                                    FactoryId = newfactoryid,
                                    Item = k.ToString(),
                                };
                                _MyDbContext.Workinghours.Add(Workinghour);

                            }
                        }
                        else
                        {
                            var BasicdataFactoryaddresses = _MyDbContext.BasicdataFactoryaddresses.Where(a => a.BasicId == id && a.Id.ToString() == factoryvalue).FirstOrDefault();
                            BasicdataFactoryaddresses.Name = namevalue;
                            BasicdataFactoryaddresses.Address = addressvalue;
                            _MyDbContext.BasicdataFactoryaddresses.Update(BasicdataFactoryaddresses);
                            idsToDelete1.Add(int.Parse(factoryvalue));
                        }

                    }
                    List<int> idsToDelete2 = new List<int>();
                    for (int i = 1; ; i++)
                    {
                        string factoryID = "factoryInDeductionID" + i;//給予陣列名稱
                        string name = "factoryInDeductionname" + i;//給予陣列名稱
                        string address = "factoryInDeductionaddress" + i;
                        string namevalue = collection[name];//依哪個陣列名稱抓取前端的值
                        string addressvalue = collection[address];
                        string factoryvalue = collection[factoryID];
                        //陣列沒資料跳出
                        if (string.IsNullOrEmpty(namevalue))
                        {
                            break;
                        }
                        if (factoryvalue == "")
                        {
                            BasicdataFactoryaddress = new BasicdataFactoryaddress()
                            {
                                BasicId = (int)id,
                                Name = namevalue,
                                Address = addressvalue,
                                WherePlace = "InDeduction",
                                Sort = i,
                            };

                            var factoryid = _MyDbContext.BasicdataFactoryaddresses.Add(BasicdataFactoryaddress);
                            _MyDbContext.SaveChanges();
                            int newfactoryid = factoryid.Entity.Id;
                            idsToDelete2.Add(newfactoryid);

                            //工時資料
                            //for (int k = 0; k < 5; k++)
                            //{
                            //    Workinghour = new Workinghour()
                            //    {
                            //        BasicId = (int)id,
                            //        FactoryId = newfactoryid,
                            //        Item = k.ToString(),
                            //    };
                            //    _MyDbContext.Workinghours.Add(Workinghour);

                            //}
                        }
                        else
                        {
                            var BasicdataFactoryaddresses = _MyDbContext.BasicdataFactoryaddresses.Where(a => a.BasicId == id && a.Id.ToString() == factoryvalue).FirstOrDefault();
                            BasicdataFactoryaddresses.Name = namevalue;
                            BasicdataFactoryaddresses.Address = addressvalue;
                            _MyDbContext.BasicdataFactoryaddresses.Update(BasicdataFactoryaddresses);
                            idsToDelete2.Add(int.Parse(factoryvalue));
                        }

                    }
                    // 找出要删除的记录并删除
                    var factoriesToDelete = _MyDbContext.BasicdataFactoryaddresses
                        .Where(a => a.BasicId == id && !idsToDelete.Contains(a.Id) && !idsToDelete1.Contains(a.Id) && !idsToDelete2.Contains(a.Id))
                        .ToList();

                    var Workinghours = _MyDbContext.Workinghours
                        .Where(a => a.BasicId == id && !idsToDelete.Contains(a.FactoryId) && !idsToDelete1.Contains(a.FactoryId) && !idsToDelete2.Contains(a.FactoryId))
                        .ToList();
                    _MyDbContext.BasicdataFactoryaddresses.RemoveRange(factoriesToDelete);
                    _MyDbContext.Workinghours.RemoveRange(Workinghours);

                    List<int> idsToDelete3 = new List<int>();
                    for (int i = 1; ; i++)
                    {
                        string PersonID = "PersonID" + i;//給予陣列名稱
                        string name = "PersonName" + i;//給予陣列名稱
                        string namevalue = collection[name];//依哪個陣列名稱抓取前端的值
                        string Personvalue = collection[PersonID];
                        //陣列沒資料跳出
                        if (string.IsNullOrEmpty(namevalue))
                        {
                            break;
                        }
                        if (Personvalue == "")
                        {
                            //Basicdatagroup = new Basicdatagroup()
                            //{
                            //    BasicId = (int)id,
                            //    Account = namevalue,
                            //};
                            Organize = new Organize()
                            {
                                BasicId = (int)id,
                                Inventory = collection["OrganName"],
                                ContactName = collection["ContactPerson"],
                                StartTime = StartTime,
                                EndTime = EndTime,
                                UpdateTime = DateTime.Parse(DateTime.Now.ToString("yyyy-MM-dd")),
                                Status = "盤查中",
                                Account = namevalue
                            };
                            var Group = _MyDbContext.Organizes.Add(Organize);

                            _MyDbContext.SaveChanges();
                            int GroupId = Group.Entity.Id;
                            idsToDelete3.Add(GroupId);

                        }
                        else
                        {
                            var group = _MyDbContext.Organizes.Where(a => a.BasicId == id && a.Id.ToString() == Personvalue).FirstOrDefault();
                            group.Account = namevalue;
                            _MyDbContext.Organizes.Update(group);
                            idsToDelete3.Add(int.Parse(Personvalue));
                        }
                    }
                    // 找出要删除的记录并删除
                    var groupToDelete = _MyDbContext.Organizes
                        .Where(a => a.BasicId == id && !idsToDelete3.Contains(a.Id) && a.Account != _MyDbContext.Basicdatas.First(a => a.BasicId == id).Account)
                        .ToList();
                    _MyDbContext.Organizes.RemoveRange(groupToDelete);


                    _MyDbContext.SaveChanges();
                    Basicdatas = null;
                    Organizes = null;
                    BasicdataFactoryaddress = null;
                    return Sucess(id);

                }
            }
            catch (Exception ex)
            {
                var Log = new Log()
                {
                    WhereFunction = "Organize_Basicid_post",
                    Exception = ex.ToString(),
                    DateTime = DateTime.Now,
                };
                _MyDbContext.Logs.Add(Log);
                _MyDbContext.SaveChanges();
                Log = null;
                return Error("內部伺服器錯誤"); // 或其他適當的錯誤訊息
            }

        }

        public ActionResult EnergyUse(int id, string? search)
        {
            try
            {
                var Energyuses = _MyDbContext.Energyuses.Where(a => a.BasicId == id);//抓取該id的能源使用
                var a = _MyDbContext.Basicdatas.Find(id);//依盤查表名稱設定檔案存取路徑

                if (!string.IsNullOrEmpty(search))
                {

                }

                var pagedList = Energyuses.OrderByDescending(a => a.Id).ToPagedList();
                int count = 1;//jsgrid sort遞增
                //選擇前端jsgrid需顯示的資料
                var result = pagedList.Select((item, index) => new
                {
                    sort = count + index,
                    id = item.Id,
                    factoryname = item.FactoryName,
                    energyname = item.EnergyName,
                    category = _MyDbContext.Selectdata.Where(a => a.Code == _MyDbContext.Coefficients.Where(a => a.Id.ToString() == item.EnergyName).Select(a => a.Category).FirstOrDefault()).Select(a => a.Name),
                    equipmentname = item.EquipmentName,
                    equipmentlocation = item.EquipmentLocation,
                    suppliername = _MyDbContext.Suppliermanages.Where(a => a.Id == item.SupplierId).Select(a => a.SupplierName).FirstOrDefault(),
                    supplieraddress = _MyDbContext.Suppliermanages.Where(a => a.Id == item.SupplierId).Select(a => a.SupplierAddress).FirstOrDefault(),
                    remark = item.Remark,
                    beforetotal = item.BeforeTotal,
                    beforeunit = _MyDbContext.Selectdata.Where(a => a.Code == item.BeforeUnit).Select(a => a.Name).FirstOrDefault(),
                    convertnum = item.ConvertNum,
                    convertunit = _MyDbContext.Selectdata.Where(a => a.Code == item.AfertUnit).Select(a => a.Name).FirstOrDefault() + "/" + _MyDbContext.Selectdata.Where(a => a.Code == item.BeforeUnit).Select(a => a.Name).FirstOrDefault(),
                    aferttotal = item.AfertTotal,
                    afertUnit = item.AfertUnit,
                    distributeratio = item.DistributeRatio,
                    name = _MyDbContext.Members.Where(a => a.MemberId.ToString() == item.Account).Select(a => a.Name).FirstOrDefault(),
                    edittime = item.EditTime,
                });
                ViewBag.DeatilsJson = JsonConvert.SerializeObject(result);//資料傳回前端
                ViewBag.Factory = JsonConvert.SerializeObject(_MyDbContext.BasicdataFactoryaddresses.Where(a => a.BasicId == id && (a.WherePlace == "CheckField" || a.WherePlace == "OutsideCheckField")).OrderByDescending(a => a.WherePlace == "CheckField").Select(a => new { a.Name, Id = a.Id.ToString() }).ToList());//工廠資料
                ViewBag.Unit = JsonConvert.SerializeObject(_MyDbContext.Selectdata.Where(a => a.Code == "14" || a.Code == "15" || a.Code == "22" || a.Code == "25" || a.Code == "117").Select(a => new { a.Code, a.Name, a.Sort }).OrderBy(a => a.Sort).ToList());//單位資料
                ViewBag.Transportation = JsonConvert.SerializeObject(_MyDbContext.Selectdata.Where(a => a.Type == "Transportation").Select(a => new { a.Code, a.Name, a.Sort }).OrderBy(a => a.Sort).ToList());//運輸方式資料
                ViewBag.Car = JsonConvert.SerializeObject(_MyDbContext.Selectdata.Where(a => a.Type == "Car").Select(a => new { a.Code, a.Name, a.Sort }).OrderBy(a => a.Sort).ToList());//運輸方式資料
                ViewBag.Suppliermanages = JsonConvert.SerializeObject(_MyDbContext.Suppliermanages.Where(a => a.Account == AccountId()).Select(a => new { a.SupplierName, Id = a.Id.ToString() }).ToList());//供應商資料
                ViewBag.EnergyName = JsonConvert.SerializeObject(_MyDbContext.Coefficients.Where(a => a.Type == "EnergyName").Select(a => new { Id = a.Id.ToString(), a.Name }).OrderBy(a => a.Id).ToList());//能源名稱資料



                int monthcount = (a.EndTime.Year - a.StartTime.Year) * 12 + a.EndTime.Month - a.StartTime.Month + 1;//相差多少月份
                int year = a.StartTime.Year;//系統從盤查區間自動帶出年
                int month = a.StartTime.Month;//系統自動帶出樂
                ViewBag.monthcount = monthcount;//月份級距
                ViewBag.year = year;
                ViewBag.month = month;

                ViewBag.supplier = JsonConvert.SerializeObject(_MyDbContext.Suppliermanages.Where(a => a.Account == AccountId()).Select(a => new { a.SupplierName, a.SupplierAddress, a.Id }).ToList());
                result = null;
                pagedList = null;
                Energyuses = null;
                a = null;
                return View();
            }
            catch (Exception ex)
            {
                var Log = new Log()
                {
                    WhereFunction = "Organize_EnergyUse_get",
                    Exception = ex.ToString(),
                    DateTime = DateTime.Now,
                };
                _MyDbContext.Logs.Add(Log);
                _MyDbContext.SaveChanges();
                Log = null;
                return Error("內部伺服器錯誤"); // 或其他適當的錯誤訊息
            }


        }

        //POST: ProductController/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult EnergyUse(int basicid, int id, string type, IFormCollection? collection)
        {
            try
            {
                return RedirectToAction("ResourceUse", "Organize", new { id = id });//有下一個頁面時，要改為導向下一個頁面

            }
            catch (Exception ex)
            {
                var Log = new Log()
                {
                    WhereFunction = "Organize_EnergyUse_post",
                    Exception = ex.ToString(),
                    DateTime = DateTime.Now,
                };
                _MyDbContext.Logs.Add(Log);
                _MyDbContext.SaveChanges();
                Log = null;
                return Error("內部伺服器錯誤"); // 或其他適當的錯誤訊息
            }
        }

        public ActionResult ResourceUse(int id, string? search)
        {
            try
            {
                var Resourceuses = _MyDbContext.Resourceuses.Where(a => a.BasicId == id);//抓取該id的能源使用
                var a = _MyDbContext.Basicdatas.Find(id);//依盤查表名稱設定檔案存取路徑

                if (!string.IsNullOrEmpty(search))
                {
                }

                var pagedList = Resourceuses.OrderByDescending(a => a.Id).ToPagedList();
                int count = 1;//jsgrid sort遞增
                //選擇前端jsgrid需顯示的資料
                var result = pagedList.Select((item, index) => new
                {
                    sort = count + index,
                    id = item.Id,
                    factoryname = item.FactoryName,
                    energyname = item.EnergyName,
                    category = _MyDbContext.Selectdata.Where(a => a.Code == _MyDbContext.Coefficients.Where(a => a.Id.ToString() == item.EnergyName).Select(a => a.Category).FirstOrDefault()).Select(a => a.Name),
                    equipmentname = item.EquipmentName,
                    equipmentlocation = item.EquipmentLocation,
                    suppliername = _MyDbContext.Suppliermanages.Where(a => a.Id == item.SupplierId).Select(a => a.SupplierName).FirstOrDefault(),
                    supplieraddress = _MyDbContext.Suppliermanages.Where(a => a.Id == item.SupplierId).Select(a => a.SupplierAddress).FirstOrDefault(),
                    remark = item.Remark,
                    beforetotal = item.BeforeTotal,
                    beforeunit = _MyDbContext.Selectdata.Where(a => a.Code == item.BeforeUnit).Select(a => a.Name).FirstOrDefault(),
                    convertnum = item.ConvertNum,
                    convertunit = _MyDbContext.Selectdata.Where(a => a.Code == item.AfertUnit).Select(a => a.Name).FirstOrDefault() + "/" + _MyDbContext.Selectdata.Where(a => a.Code == item.BeforeUnit).Select(a => a.Name).FirstOrDefault(),
                    aferttotal = item.AfertTotal,
                    afertUnit = item.AfertUnit,
                    distributeratio = item.DistributeRatio,
                });
                ViewBag.DeatilsJson = JsonConvert.SerializeObject(result);//資料傳回前端
                ViewBag.Factory = JsonConvert.SerializeObject(_MyDbContext.BasicdataFactoryaddresses.Where(a => a.BasicId == id && (a.WherePlace == "CheckField" || a.WherePlace == "OutsideCheckField")).OrderByDescending(a => a.WherePlace == "CheckField").Select(a => new { a.Name, Id = a.Id.ToString() }).ToList());//工廠資料              
                var Unit = _MyDbContext.Coefficients
                    .Where(a => a.Type == "ResourceName")
                    .Join(
                        _MyDbContext.Selectdata,
                        c => c.Unit,
                        s => s.Code,
                        (c, s) => new { Name = s.Name, Code = s.Code, Sort = s.Sort }
                    )
                    .OrderBy(r => r.Sort) // 根據 Sort 欄位進行升序排序
                    .ToList();
                var additionalData = _MyDbContext.Selectdata
    .Where(s => s.Code == "22")
    .Select(s => new { Name = s.Name, Code = s.Code, Sort = s.Sort })
    .ToList();
                Unit.AddRange(additionalData);

                ViewBag.Unit = JsonConvert.SerializeObject(Unit);
                ViewBag.supplier = JsonConvert.SerializeObject(_MyDbContext.Suppliermanages.Where(a => a.Account == AccountId()).Select(a => new { a.SupplierName, a.SupplierAddress, a.Id }).ToList());
                ViewBag.ResourceName = JsonConvert.SerializeObject(_MyDbContext.Coefficients.Where(a => a.Type == "ResourceName").Select(a => new { Id = a.Id.ToString(), a.Name }).OrderBy(a => a.Id).ToList());//資源名稱資料
                ViewBag.Suppliermanages = JsonConvert.SerializeObject(_MyDbContext.Suppliermanages.Where(a => a.Account == AccountId()).Select(a => new { a.SupplierName, Id = a.Id.ToString() }).ToList());//供應商資料
                ViewBag.Transportation = JsonConvert.SerializeObject(_MyDbContext.Selectdata.Where(a => a.Type == "Transportation").Select(a => new { a.Code, a.Name, a.Sort }).OrderBy(a => a.Sort).ToList());//運輸方式資料
                ViewBag.Car = JsonConvert.SerializeObject(_MyDbContext.Selectdata.Where(a => a.Type == "Car").Select(a => new { a.Code, a.Name, a.Sort }).OrderBy(a => a.Sort).ToList());//運輸方式資料

                int monthcount = (a.EndTime.Year - a.StartTime.Year) * 12 + a.EndTime.Month - a.StartTime.Month + 1;//相差多少月份
                int year = a.StartTime.Year;//系統從盤查區間自動帶出年
                int month = a.StartTime.Month;//系統自動帶出樂
                ViewBag.monthcount = monthcount;//月份級距
                ViewBag.year = year;
                ViewBag.month = month;

                result = null;
                pagedList = null;
                Resourceuses = null;
                a = null;
                return View();
            }
            catch (Exception ex)
            {
                var Log = new Log()
                {
                    WhereFunction = "Organize_ResourceUse_get",
                    Exception = ex.ToString(),
                    DateTime = DateTime.Now,
                };
                _MyDbContext.Logs.Add(Log);
                _MyDbContext.SaveChanges();
                Log = null;
                return Error("內部伺服器錯誤"); // 或其他適當的錯誤訊息
            }


        }

        //POST: ProductController/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult ResourceUse(int basicid, int id, string type, IFormCollection? collection)
        {
            try
            {
                return RedirectToAction("RefrigerantNone", "Organize", new { id = id });//有下一個頁面時，要改為導向下一個頁面

            }
            catch (Exception ex)
            {
                var Log = new Log()
                {
                    WhereFunction = "Organzie_ResourceUse_post",
                    Exception = ex.ToString(),
                    DateTime = DateTime.Now,
                };
                _MyDbContext.Logs.Add(Log);
                _MyDbContext.SaveChanges();
                Log = null;
                return Error("內部伺服器錯誤"); // 或其他適當的錯誤訊息
            }
        }

        public ActionResult RefrigerantHave(int id, string? search)
        {
            try
            {
                var RefrigerantHaves = _MyDbContext.RefrigerantHaves.Where(a => a.BasicId == id);//抓取該id的冷媒逸散(有)
                var a = _MyDbContext.Basicdatas.Find(id);//依盤查表名稱設定檔案存取路徑

                if (!string.IsNullOrEmpty(search))
                {

                }

                var pagedList = RefrigerantHaves.OrderByDescending(a => a.Id).ToPagedList();
                int count = 1;//jsgrid sort遞增
                //選擇前端jsgrid需顯示的資料
                var result = pagedList.Select((item, index) => new
                {
                    sort = count + index,
                    id = item.Id,
                    factoryname = item.FactoryName,
                    equipmentname = item.EquipmentName,
                    equipmenttype = item.EquipmentType,
                    refrigeranttype = item.RefrigerantType,
                    category = _MyDbContext.Selectdata.Where(a => a.Code == _MyDbContext.Coefficients.Where(a => a.Id.ToString() == item.RefrigerantType).Select(a => a.Category).FirstOrDefault()).Select(a => a.Name),
                    management = item.Management,
                    principal = item.Principal,
                    datasource = item.Datasource,
                    remark = item.Remark,
                    total = item.Total,
                    unit = _MyDbContext.Selectdata.Where(a => a.Code == item.Unit).Select(a => a.Name).FirstOrDefault(),
                    distributeratio = item.DistributeRatio,
                });
                ViewBag.DeatilsJson = JsonConvert.SerializeObject(result);//資料傳回前端
                ViewBag.Factory = JsonConvert.SerializeObject(_MyDbContext.BasicdataFactoryaddresses.Where(a => a.BasicId == id && (a.WherePlace == "CheckField" || a.WherePlace == "OutsideCheckField")).OrderByDescending(a => a.WherePlace == "CheckField").Select(a => new { a.Name, Id = a.Id.ToString() }).ToList());//工廠資料
                ViewBag.EquipmentType = JsonConvert.SerializeObject(_MyDbContext.Selectdata.Where(a => a.Type == "EquipmentType").Select(a => new { a.Code, a.Name, a.Sort }).OrderBy(a => a.Sort).ToList());//種類資料   
                ViewBag.Unit = JsonConvert.SerializeObject(_MyDbContext.Selectdata.Where(a => a.Code == "10").Select(a => new { a.Code, a.Name, a.Sort }).ToList());//單位資料(只有公斤)
                ViewBag.Refrigerant = JsonConvert.SerializeObject(_MyDbContext.Coefficients.Where(a => a.Type == "RefrigerantName").Select(a => new { Id = a.Id.ToString(), a.Name }).OrderBy(a => a.Id).ToList());//冷媒種類資料                
                result = null;
                pagedList = null;
                RefrigerantHaves = null;
                a = null;
                return View();
            }
            catch (Exception ex)
            {
                var Log = new Log()
                {
                    WhereFunction = "Organize_RefrigerantHaves_get",
                    Exception = ex.ToString(),
                    DateTime = DateTime.Now,
                };
                _MyDbContext.Logs.Add(Log);
                _MyDbContext.SaveChanges();
                Log = null;
                return Error("內部伺服器錯誤"); // 或其他適當的錯誤訊息
            }


        }

        //POST: ProductController/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult RefrigerantHave(int basicid, int id, string type, IFormCollection? collection)
        {
            try
            {
                return RedirectToAction("Fireequipment", "Organize", new { id = id });//有下一個頁面時，要改為導向下一個頁面

            }
            catch (Exception ex)
            {
                var Log = new Log()
                {
                    WhereFunction = "Organize_RefrigerantHave_post",
                    Exception = ex.ToString(),
                    DateTime = DateTime.Now,
                };
                _MyDbContext.Logs.Add(Log);
                _MyDbContext.SaveChanges();
                Log = null;
                return Error("內部伺服器錯誤"); // 或其他適當的錯誤訊息
            }
        }

        public ActionResult RefrigerantNone(int id, string? search)
        {
            try
            {
                var RefrigerantNones = _MyDbContext.RefrigerantNones.Where(a => a.BasicId == id);//抓取該id的冷媒逸散(有)
                var a = _MyDbContext.Basicdatas.Find(id);//依盤查表名稱設定檔案存取路徑

                if (!string.IsNullOrEmpty(search))
                {

                }

                var pagedList = RefrigerantNones.OrderByDescending(a => a.Id).ToPagedList();
                int count = 1;//jsgrid sort遞增
                //選擇前端jsgrid需顯示的資料
                var result = pagedList.Select((item, index) => new
                {
                    sort = count + index,
                    id = item.Id,
                    factoryname = item.FactoryName,
                    equipmentname = item.EquipmentName,
                    equipmenttype = item.EquipmentType,
                    equipmentlocation = item.EquipmentLocation,
                    refrigeranttype = item.RefrigerantType,
                    category = _MyDbContext.Selectdata.Where(a => a.Code == _MyDbContext.Coefficients.Where(a => a.Id.ToString() == item.RefrigerantType).Select(a => a.Category).FirstOrDefault()).Select(a => a.Name),
                    machinequantity = item.MachineQuantity,
                    manufacturers = item.Manufacturers,
                    management = item.Management,
                    principal = item.Principal,
                    remark = item.Remark,
                    refrigerantweight = item.RefrigerantWeight,
                    unit = _MyDbContext.Selectdata.Where(a => a.Code == item.Unit).Select(a => a.Name).FirstOrDefault(),
                    distributeratio = item.DistributeRatio,
                });
                ViewBag.DeatilsJson = JsonConvert.SerializeObject(result);//資料傳回前端
                ViewBag.Factory = JsonConvert.SerializeObject(_MyDbContext.BasicdataFactoryaddresses.Where(a => a.BasicId == id && (a.WherePlace == "CheckField" || a.WherePlace == "OutsideCheckField")).OrderByDescending(a => a.WherePlace == "CheckField").Select(a => new { a.Name, Id = a.Id.ToString() }).ToList());//工廠資料
                ViewBag.EquipmentType = JsonConvert.SerializeObject(_MyDbContext.Selectdata.Where(a => a.Type == "EquipmentType").Select(a => new { a.Code, a.Name, a.Sort }).OrderBy(a => a.Sort).ToList());//種類資料   
                ViewBag.Unit = JsonConvert.SerializeObject(_MyDbContext.Selectdata.Where(a => a.Code == "10").Select(a => new { a.Code, a.Name, a.Sort }).ToList());//單位資料(只有公斤)
                ViewBag.Refrigerant = JsonConvert.SerializeObject(_MyDbContext.Coefficients.Where(a => a.Type == "RefrigerantName").Select(a => new { Id = a.Id.ToString(), a.Name }).OrderBy(a => a.Id).ToList());//冷媒種類資料                

                result = null;
                pagedList = null;
                RefrigerantNones = null;
                a = null;
                return View();
            }
            catch (Exception ex)
            {
                var Log = new Log()
                {
                    WhereFunction = "Organize_RefrigerantNones_get",
                    Exception = ex.ToString(),
                    DateTime = DateTime.Now,
                };
                _MyDbContext.Logs.Add(Log);
                _MyDbContext.SaveChanges();
                Log = null;
                return Error("內部伺服器錯誤"); // 或其他適當的錯誤訊息
            }


        }

        //POST: ProductController/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult RefrigerantNone(int basicid, int id, string type, IFormCollection? collection)
        {
            try
            {
                return RedirectToAction("RefrigerantHave", "Organize", new { id = id });//有下一個頁面時，要改為導向下一個頁面

            }
            catch (Exception ex)
            {
                var Log = new Log()
                {
                    WhereFunction = "Organize_RefrigerantNones_post",
                    Exception = ex.ToString(),
                    DateTime = DateTime.Now,
                };
                _MyDbContext.Logs.Add(Log);
                _MyDbContext.SaveChanges();
                Log = null;
                return Error("內部伺服器錯誤"); // 或其他適當的錯誤訊息
            }
        }

        public ActionResult Fireequipment(int id, string? search)
        {
            try
            {
                var Fireequipment = _MyDbContext.Fireequipments.Where(a => a.BasicId == id);//抓取該id的消防
                var a = _MyDbContext.Basicdatas.Find(id);//依盤查表名稱設定檔案存取路徑

                if (!string.IsNullOrEmpty(search))
                {
                }

                var pagedList = Fireequipment.OrderByDescending(a => a.Id).ToPagedList();
                int count = 1;//jsgrid sort遞增
                //選擇前端jsgrid需顯示的資料
                var result = pagedList.Select((item, index) => new
                {
                    sort = count + index,
                    id = item.Id,
                    factoryname = item.FactoryName,
                    equipmentname = item.EquipmentName,
                    category = _MyDbContext.Selectdata.Where(a => a.Code == _MyDbContext.Coefficients.Where(a => a.Id.ToString() == item.EquipmentName).Select(a => a.Category).FirstOrDefault()).Select(a => a.Name),
                    ghgtype = item.Ghgtype,
                    remark = item.Remark,
                    beforetotal = item.BeforeTotal,
                    beforeunit = _MyDbContext.Selectdata.Where(a => a.Code == item.BeforeUnit).Select(a => a.Name).FirstOrDefault(),
                    convertnum = item.ConvertNum,
                    convertunit = _MyDbContext.Selectdata.Where(a => a.Code == item.AfertUnit).Select(a => a.Name).FirstOrDefault() + "/" + _MyDbContext.Selectdata.Where(a => a.Code == item.BeforeUnit).Select(a => a.Name).FirstOrDefault(),
                    aferttotal = item.AfertTotal,
                    afertUnit = item.AfertUnit,
                    distributeratio = item.DistributeRatio,
                });
                ViewBag.DeatilsJson = JsonConvert.SerializeObject(result);//資料傳回前端
                ViewBag.Factory = JsonConvert.SerializeObject(_MyDbContext.BasicdataFactoryaddresses.Where(a => a.BasicId == id && (a.WherePlace == "CheckField" || a.WherePlace == "OutsideCheckField")).OrderByDescending(a => a.WherePlace == "CheckField").Select(a => new { a.Name, Id = a.Id.ToString() }).ToList());//工廠資料
                ViewBag.GHGType = JsonConvert.SerializeObject(_MyDbContext.Selectdata.Where(a => a.Type == "GHGType").Select(a => new { a.Code, a.Name, a.Sort }).OrderBy(a => a.Sort).ToList());//溫室氣體種類
                ViewBag.Unit = JsonConvert.SerializeObject(_MyDbContext.Selectdata.Where(a => a.Code == "10").Select(a => new { a.Code, a.Name, a.Sort }).OrderBy(a => a.Sort).ToList());//單位資料
                ViewBag.FireEquipment = JsonConvert.SerializeObject(_MyDbContext.Coefficients.Where(a => a.Type == "FireEquipmentName").Select(a => new { Id = a.Id.ToString(), a.Name }).OrderBy(a => a.Id).ToList());//消防設備資料                


                int monthcount = (a.EndTime.Year - a.StartTime.Year) * 12 + a.EndTime.Month - a.StartTime.Month + 1;//相差多少月份
                int year = a.StartTime.Year;//系統從盤查區間自動帶出年
                int month = a.StartTime.Month;//系統自動帶出樂
                ViewBag.monthcount = monthcount;//月份級距
                ViewBag.year = year;
                ViewBag.month = month;

                result = null;
                pagedList = null;
                Fireequipment = null;
                a = null;
                return View();
            }
            catch (Exception ex)
            {
                var Log = new Log()
                {
                    WhereFunction = "Organize_Fireequipment_get",
                    Exception = ex.ToString(),
                    DateTime = DateTime.Now,
                };
                _MyDbContext.Logs.Add(Log);
                _MyDbContext.SaveChanges();
                Log = null;
                return Error("內部伺服器錯誤"); // 或其他適當的錯誤訊息
            }


        }

        //POST: ProductController/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Fireequipment(int basicid, int id, string type, IFormCollection? collection)
        {
            try
            {
                return RedirectToAction("Workinghour", "Organize", new { id = id });//有下一個頁面時，要改為導向下一個頁面

            }
            catch (Exception ex)
            {
                var Log = new Log()
                {
                    WhereFunction = "Organize_Fireequipment_post",
                    Exception = ex.ToString(),
                    DateTime = DateTime.Now,
                };
                _MyDbContext.Logs.Add(Log);
                _MyDbContext.SaveChanges();
                Log = null;
                return Error("內部伺服器錯誤"); // 或其他適當的錯誤訊息
            }
        }

        public ActionResult Workinghour(int id)
        {
            try
            {

                ViewBag.basicid = id;//給input存取
                ViewBag.Factory = _MyDbContext.BasicdataFactoryaddresses.Where(a => a.BasicId == id && (a.WherePlace == "CheckField" || a.WherePlace == "OutsideCheckField")).ToList();//廠址
                ViewBag.FactoryItem = JsonConvert.SerializeObject(_MyDbContext.BasicdataFactoryaddresses.Where(a => a.BasicId == id && (a.WherePlace == "CheckField" || a.WherePlace == "OutsideCheckField")).Select(a => new { a.Name, Id = a.Id.ToString() }).ToList());//工廠資料              


                //viewmodel資料設定
                var Workinghour = _MyDbContext.Workinghours.FirstOrDefault(a => a.BasicId == id);
                var Workinghours_Item_0 = _MyDbContext.Workinghours.Where(a => a.BasicId == id && a.Item == "0").ToList();
                var Workinghours_Item_1 = _MyDbContext.Workinghours.Where(a => a.BasicId == id && a.Item == "1").ToList();

                //把資料塞進viewmodel
                var viewModel = new WorkinghourVM
                {
                    Workinghour = Workinghour,
                    Workinghours_Item_0 = Workinghours_Item_0,
                    Workinghours_Item_1 = Workinghours_Item_1
                };

                return View(viewModel);
            }
            catch (Exception ex)
            {
                var Log = new Log()
                {
                    WhereFunction = "OrganzieDetail_Workinghour_get",
                    Exception = ex.ToString(),
                    DateTime = DateTime.Now,
                };
                _MyDbContext.Logs.Add(Log);
                _MyDbContext.SaveChanges();
                Log = null;
                return Error("內部伺服器錯誤"); // 或其他適當的錯誤訊息
            }
        }
        // POST: OrganzieController/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Workinghour(int id, string?[] ShiftMethod, decimal?[][] TotalWorkHour, string?[][] Management, string?[][] Principal, string?[][] Datasource, decimal?[][] DistributeRatio)
        {
            try
            {
                var Factory = _MyDbContext.BasicdataFactoryaddresses.Where(a => a.BasicId == id && (a.WherePlace == "CheckField" || a.WherePlace == "OutsideCheckField")).ToList();//廠址
                var Item = "";//判斷盤查表是否有工時資料
                var EditLog = _MyDbContext.Organizes.Where(a => a.BasicId == id && a.Account == AccountId()).First();

                Workinghour? Workinghour;
                int count = 0;
                foreach (var a in Factory)//更新工時資料
                {
                    for (int j = 0; j < 2; j++)//項目
                    {
                        var factoryitem = _MyDbContext.Workinghours.Where(b => b.FactoryId == a.Id && b.BasicId == id && b.Item == j.ToString()).FirstOrDefault();
                        if ((factoryitem.TotalWorkHour != TotalWorkHour[count][j]) || (factoryitem.Management != Management[count][j]) || (factoryitem.Principal != Principal[count][j]) || (factoryitem.Datasource != Datasource[count][j]) || (factoryitem.DistributeRatio != DistributeRatio[count][j]))
                        {
                            switch (j)
                            {
                                case 0: Item = "正式員工及約聘人員(具勞保)"; break;
                                case 1: Item = "廠商外派人員(不具勞保)"; break;
                            }
                            if (j < 3)
                            {
                                EditLog.EditLog += WorkinghourLog("編輯", a.Name, Item, TotalWorkHour[count][j]);
                            }
                            else
                            {
                                EditLog.EditLog += TotalWorkinghourLog("編輯", a.Name, Item, TotalWorkHour[count][j]);

                            }
                        }
                        factoryitem.TotalWorkHour = TotalWorkHour[count][j];
                        //factoryitem.WorkDay = WorkDay[count][j];
                        //factoryitem.WorkDayHour = WorkDayHour[count][j];
                        factoryitem.Management = Management[count][j];
                        factoryitem.Principal = Principal[count][j];
                        factoryitem.Datasource = Datasource[count][j];
                        factoryitem.DistributeRatio = DistributeRatio[count][j];
                        _MyDbContext.Workinghours.Update(factoryitem);
                    }
                    count++;
                };
                _MyDbContext.SaveChanges();

                return RedirectToAction("DumptreatmentOutsourcing", "Organize", new { id = id });//有下一個頁面時，要改為導向下一個頁面
                //return Sucess();//傳送表單後轉至頁面
            }
            //catch (IndexOutOfRangeException ex)
            //{
            //    return Error("請確認所有欄位的值輸入是否含科學符號'e' "); // 或其他適當的錯誤訊息
            //}
            catch (Exception ex)
            {

                var Log = new Log()
                {
                    WhereFunction = "Organzie_Workinghour_post",
                    Exception = ex.ToString(),
                    DateTime = DateTime.Now,
                };
                _MyDbContext.Logs.Add(Log);
                _MyDbContext.SaveChanges();
                Log = null;
                return Error("內部伺服器錯誤"); // 或其他適當的錯誤訊息
            }
        }

        public ActionResult DumptreatmentOutsourcing(int id, string? search)
        {
            try
            {
                var DumptreatmentOutsourcings = _MyDbContext.DumptreatmentOutsourcings.Where(a => a.BasicId == id);//抓取該id的空水廢排放
                var a = _MyDbContext.Basicdatas.Find(id);//依盤查表名稱設定檔案存取路徑

                if (!string.IsNullOrEmpty(search))
                {
                    var unit = DumptreatmentOutsourcings.Join(
                    _MyDbContext.Selectdata,
                    p => p.AfertUnit,
                    a => a.Code,
                    (p, a) => new { DumptreatmentOutsourcing = p, SelectData = a })
                    .Where(pa => pa.SelectData.Name.Contains(search))
                    .Select(pa => pa.DumptreatmentOutsourcing);

                    var factory = DumptreatmentOutsourcings.Join(
                    _MyDbContext.BasicdataFactoryaddresses,
                    p => p.FactoryName,
                    a => a.Sort.ToString(),
                    (p, a) => new { DumptreatmentOutsourcing = p, BasicdataFactoryaddress = a })
                    .Where(pa => pa.BasicdataFactoryaddress.Name.Contains(search) && pa.BasicdataFactoryaddress.BasicId == id)
                    .Select(pa => pa.DumptreatmentOutsourcing);

                    //前端jsgrid全部欄位模糊搜尋，除了工廠
                    DumptreatmentOutsourcings = DumptreatmentOutsourcings.Where(p => factory.Any(fe => fe.FactoryName.Contains(p.FactoryName)) || p.WasteName.Contains(search) || p.WasteMethod.Contains(search) || p.CleanerName.Contains(search)
                     || p.FinalAddress.Contains(search) || p.Remark.Contains(search) || p.BeforeTotal.ToString().Contains(search)
                       || p.ConvertNum.ToString().Contains(search) || p.AfertTotal.ToString().Contains(search)
                       || unit.Any(fe => fe.AfertUnit.Contains(p.AfertUnit)) || unit.Any(fe => fe.BeforeUnit.Contains(p.BeforeUnit)));
                }

                var pagedList = DumptreatmentOutsourcings.OrderByDescending(a => a.Id).ToPagedList();
                int count = 1;//jsgrid sort遞增
                //選擇前端jsgrid需顯示的資料
                var result = pagedList.Select((item, index) => new
                {
                    sort = count + index,
                    id = item.Id,
                    factoryname = item.FactoryName,
                    wastename = _MyDbContext.Coefficients.Find(item.WasteId).Name,
                    wastemethod = _MyDbContext.Coefficients.Find(item.WasteId).WasteMethod,
                    category = _MyDbContext.Selectdata.Where(a => a.Code == _MyDbContext.Coefficients.Where(a => a.Id == item.WasteId).Select(a => a.Category).FirstOrDefault()).Select(a => a.Name),
                    cleanername = item.CleanerName,
                    finaladdress = item.FinalAddress,
                    remark = item.Remark,
                    beforetotal = item.BeforeTotal,
                    beforeunit = _MyDbContext.Selectdata.Where(a => a.Code == item.BeforeUnit).Select(a => a.Name).FirstOrDefault(),
                    convertnum = item.ConvertNum,
                    convertunit = _MyDbContext.Selectdata.Where(a => a.Code == item.AfertUnit).Select(a => a.Name).FirstOrDefault() + "/" + _MyDbContext.Selectdata.Where(a => a.Code == item.BeforeUnit).Select(a => a.Name).FirstOrDefault(),
                    aferttotal = item.AfertTotal,
                    afertUnit = item.AfertUnit,
                    distributeratio = item.DistributeRatio,
                });
                ViewBag.DeatilsJson = JsonConvert.SerializeObject(result);//資料傳回前端
                ViewBag.Factory = JsonConvert.SerializeObject(_MyDbContext.BasicdataFactoryaddresses.Where(a => a.BasicId == id).Select(a => new { a.Name, Id = a.Id.ToString() }).ToList());//工廠資料                                
                ViewBag.WasteName = JsonConvert.SerializeObject(_MyDbContext.Coefficients.Where(a => a.Type == "dumptreatment_outsourcing").Select(a => new { a.Name }).Distinct().ToList());
                ViewBag.WasteMethod = JsonConvert.SerializeObject(_MyDbContext.Coefficients.Where(a => a.Type == "dumptreatment_outsourcing").Select(a => new { a.WasteMethod }).Distinct().ToList());
                ViewBag.Unit = JsonConvert.SerializeObject(_MyDbContext.Selectdata.Where(a => a.Code == "10" || a.Code == "11").Select(a => new { a.Code, a.Name, a.Sort }).OrderBy(a => a.Sort).ToList());//單位資料
                ViewBag.AfterUnits = JsonConvert.SerializeObject(_MyDbContext.Selectdata.Where(a => a.Type == "Unit" && a.Id >= 8 && a.Id <= 11).Select(a => new { a.Code, a.Name, a.Sort }).OrderBy(a => a.Sort).ToList());//單位資料(重量)
                ViewBag.Transportation = JsonConvert.SerializeObject(_MyDbContext.Selectdata.Where(a => a.Type == "Transportation").Select(a => new { a.Code, a.Name, a.Sort }).OrderBy(a => a.Sort).ToList());//運輸方式資料
                ViewBag.Car = JsonConvert.SerializeObject(_MyDbContext.Selectdata.Where(a => a.Type == "Car").Select(a => new { a.Code, a.Name, a.Sort }).OrderBy(a => a.Sort).ToList());//運輸方式資料


                int monthcount = (a.EndTime.Year - a.StartTime.Year) * 12 + a.EndTime.Month - a.StartTime.Month + 1;//相差多少月份
                int year = a.StartTime.Year;//系統從盤查區間自動帶出年
                int month = a.StartTime.Month;//系統自動帶出樂
                ViewBag.monthcount = monthcount;//月份級距
                ViewBag.year = year;
                ViewBag.month = month;

                result = null;
                pagedList = null;
                DumptreatmentOutsourcings = null;
                a = null;
                return View();
            }
            catch (Exception ex)
            {
                var Log = new Log()
                {
                    WhereFunction = "Organzie_DumptreatmentOutsourcing_get",
                    Exception = ex.ToString(),
                    DateTime = DateTime.Now,
                };
                _MyDbContext.Logs.Add(Log);
                _MyDbContext.SaveChanges();
                Log = null;
                return Error("內部伺服器錯誤"); // 或其他適當的錯誤訊息
            }


        }

        //POST: OrganzieController/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult DumptreatmentOutsourcing(int basicid, int id, string type, IFormCollection? collection)
        {
            try
            {
                return RedirectToAction("EvidenceFileManage", "Organize", new { id = id });//有下一個頁面時，要改為導向下一個頁面

            }
            catch (Exception ex)
            {
                var Log = new Log()
                {
                    WhereFunction = "Organzie_DumptreatmentOutsourcing_post",
                    Exception = ex.ToString(),
                    DateTime = DateTime.Now,
                };
                _MyDbContext.Logs.Add(Log);
                _MyDbContext.SaveChanges();
                Log = null;
                return Error("內部伺服器錯誤"); // 或其他適當的錯誤訊息
            }
        }

        public ActionResult EvidenceFileManage(int id)
        {
            try
            {
                var EnergyUse = _MyDbContext.Energyuses.Where(a => a.BasicId == id).ToList();
                var ResourceUse = _MyDbContext.Resourceuses.Where(a => a.BasicId == id).ToList();
                var RefrigerantHave = _MyDbContext.RefrigerantHaves.Where(a => a.BasicId == id).ToList();
                var RefrigerantNone = _MyDbContext.RefrigerantNones.Where(a => a.BasicId == id).ToList();
                var Coefficient = _MyDbContext.Coefficients.ToList();
                var Workinghours = _MyDbContext.Workinghours.Where(a => a.BasicId == id).ToList();
                var Workinghours_Item_0 = _MyDbContext.Workinghours.Where(a => a.BasicId == id && a.Item == "0").ToList();//正式員工(具勞保)
                var Workinghours_Item_1 = _MyDbContext.Workinghours.Where(a => a.BasicId == id && a.Item == "1").ToList();//_約聘人員(具勞保)
                var Workinghours_Item_2 = _MyDbContext.Workinghours.Where(a => a.BasicId == id && a.Item == "2").ToList();//廠商外派人員(不具勞保)
                var Workinghours_Item_3 = _MyDbContext.Workinghours.Where(a => a.BasicId == id && a.Item == "3").ToList();//總加班工時
                var Workinghours_Item_4 = _MyDbContext.Workinghours.Where(a => a.BasicId == id && a.Item == "4").ToList();//總請假工時
                var FactoryName = _MyDbContext.BasicdataFactoryaddresses.Where(a => a.BasicId == id && (a.WherePlace == "CheckField" || a.WherePlace == "OutsideCheckField")).ToList();//廠址
                var DumptreatmentOutsourcing = _MyDbContext.DumptreatmentOutsourcings.Where(a => a.BasicId == id).ToList();//廢棄物
                var BasicdataFactoryaddresses = _MyDbContext.BasicdataFactoryaddresses.Where(a => a.BasicId == id).ToList();


                var Fireequipment = _MyDbContext.Fireequipments.Where(a => a.BasicId == id).ToList();
                //var Goodsdistribution = _MyDbContext.Goodsdistributions.Where(a => a.BasicId == id).ToList();
                //var UseanddisposalUse = _MyDbContext.UseanddisposalUses.Where(a => a.BasicId == id).ToList();
                //var UseanddisposalDisposal = _MyDbContext.UseanddisposalDisposals.Where(a => a.BasicId == id).ToList();
                var viewModel = new ViewEvidenceFileManage
                {
                    Energyuse = EnergyUse,
                    Resourceuse = ResourceUse,
                    RefrigerantHave = RefrigerantHave,
                    RefrigerantNone = RefrigerantNone,
                    Coefficient = Coefficient,
                    Workinghours = Workinghours,
                    FactoryName = FactoryName,
                    Workinghours_Item_0 = Workinghours_Item_0,
                    Workinghours_Item_1 = Workinghours_Item_1,
                    Workinghours_Item_2 = Workinghours_Item_2,
                    Workinghours_Item_3 = Workinghours_Item_3,
                    Workinghours_Item_4 = Workinghours_Item_4,
                    Fireequipment = Fireequipment,
                    DumptreatmentOutsourcing = DumptreatmentOutsourcing,
                    BasicdataFactoryaddresses= BasicdataFactoryaddresses

                };
                return View(viewModel);

            }
            catch (Exception ex)
            {
                var Log = new Log()
                {
                    WhereFunction = "Organize_EvidenceFileManage_get",
                    Exception = ex.ToString(),
                    DateTime = DateTime.Now,
                };
                _MyDbContext.Logs.Add(Log);
                _MyDbContext.SaveChanges();
                Log = null;
                return Error("內部伺服器錯誤"); // 或其他適當的錯誤訊息
            }

        }

        // POST: ProductController/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult EvidenceFileManage(int id, IFormCollection collection)
        {

            try
            {
                return View();

            }
            catch (Exception ex)
            {
                var Log = new Log()
                {
                    WhereFunction = "Organize_EvidenceFileManage_post",
                    Exception = ex.ToString(),
                    DateTime = DateTime.Now,
                };
                _MyDbContext.Logs.Add(Log);
                _MyDbContext.SaveChanges();
                Log = null;
                return Error("內部伺服器錯誤"); // 或其他適當的錯誤訊息
            }
        }

        public ActionResult Delete(int id)
        {
            try
            {
                if (_MyDbContext.Basicdatas.Any(a => a.BasicId == id && a.Account != AccountId()))
                {
                    return Error("無權限刪除，只有建立此盤查表的人才可刪除盤查表");
                }
                var Organizes = _MyDbContext.Organizes.Where(a => a.BasicId == id).ToList();
                var Basicdatas = _MyDbContext.Basicdatas.Find(id);
                var factory = _MyDbContext.BasicdataFactoryaddresses.Where(a => a.BasicId == id).ToList();
                var Energyuses = _MyDbContext.Energyuses.Where(a => a.BasicId == id).ToList();
                foreach (var item in Energyuses)
                {
                    var DDatasources = _MyDbContext.DDatasources.Where(a => a.BindId == item.Id && a.BindWhere == "EnergyUse").ToList();
                    var DIntervalusetotals = _MyDbContext.DIntervalusetotals.Where(a => a.BindId == item.Id && a.BindWhere == "EnergyUse").ToList();
                    var DTransportations = _MyDbContext.DTransportations.Where(a => a.BindId == item.Id && a.BindWhere == "EnergyUse").ToList();
                    _MyDbContext.DDatasources.RemoveRange(DDatasources);
                    _MyDbContext.DIntervalusetotals.RemoveRange(DIntervalusetotals);
                    _MyDbContext.DTransportations.RemoveRange(DTransportations);
                }
                var Resourceuses = _MyDbContext.Resourceuses.Where(a => a.BasicId == id).ToList();
                foreach (var item in Resourceuses)
                {
                    var DDatasources = _MyDbContext.DDatasources.Where(a => a.BindId == item.Id && a.BindWhere == "ResourceUse").ToList();
                    var DIntervalusetotals = _MyDbContext.DIntervalusetotals.Where(a => a.BindId == item.Id && a.BindWhere == "ResourceUse").ToList();
                    var DTransportations = _MyDbContext.DTransportations.Where(a => a.BindId == item.Id && a.BindWhere == "ResourceUse").ToList();
                    _MyDbContext.DDatasources.RemoveRange(DDatasources);
                    _MyDbContext.DIntervalusetotals.RemoveRange(DIntervalusetotals);
                    _MyDbContext.DTransportations.RemoveRange(DTransportations);
                }
                var RefrigerantHaves = _MyDbContext.RefrigerantHaves.Where(a => a.BasicId == id).ToList();
                var RefrigerantNones = _MyDbContext.RefrigerantNones.Where(a => a.BasicId == id).ToList();
                var Fireequipments = _MyDbContext.Fireequipments.Where(a => a.BasicId == id).ToList();
                foreach (var item in Fireequipments)
                {
                    var DDatasources = _MyDbContext.DDatasources.Where(a => a.BindId == item.Id && a.BindWhere == "Fireequipment").ToList();
                    var DIntervalusetotals = _MyDbContext.DIntervalusetotals.Where(a => a.BindId == item.Id && a.BindWhere == "Fireequipment").ToList();
                    var DTransportations = _MyDbContext.DTransportations.Where(a => a.BindId == item.Id && a.BindWhere == "Fireequipment").ToList();
                    _MyDbContext.DDatasources.RemoveRange(DDatasources);
                    _MyDbContext.DIntervalusetotals.RemoveRange(DIntervalusetotals);
                    _MyDbContext.DTransportations.RemoveRange(DTransportations);
                }
                var Workinghours = _MyDbContext.Workinghours.Where(a => a.BasicId == id).ToList();
                var DumptreatmentOutsourcings = _MyDbContext.DumptreatmentOutsourcings.Where(a => a.BasicId == id).ToList();
                foreach (var item in DumptreatmentOutsourcings)
                {
                    var DDatasources = _MyDbContext.DDatasources.Where(a => a.BindId == item.Id && a.BindWhere == "DumptreatmentOutsourcing").ToList();
                    var DIntervalusetotals = _MyDbContext.DIntervalusetotals.Where(a => a.BindId == item.Id && a.BindWhere == "DumptreatmentOutsourcing").ToList();
                    var DTransportations = _MyDbContext.DTransportations.Where(a => a.BindId == item.Id && a.BindWhere == "DumptreatmentOutsourcing").ToList();
                    _MyDbContext.DDatasources.RemoveRange(DDatasources);
                    _MyDbContext.DIntervalusetotals.RemoveRange(DIntervalusetotals);
                    _MyDbContext.DTransportations.RemoveRange(DTransportations);
                }
                string directoryPath = FileSave.Path + id.ToString();
                if (Directory.Exists(directoryPath))
                {
                    Directory.Delete(directoryPath, true); // true 表示遞迴刪除子目錄和文件
                }
                _MyDbContext.Organizes.RemoveRange(Organizes);
                _MyDbContext.Basicdatas.Remove(Basicdatas);
                _MyDbContext.BasicdataFactoryaddresses.RemoveRange(factory);
                _MyDbContext.Energyuses.RemoveRange(Energyuses);
                _MyDbContext.Resourceuses.RemoveRange(Resourceuses);
                _MyDbContext.RefrigerantHaves.RemoveRange(RefrigerantHaves);
                _MyDbContext.RefrigerantNones.RemoveRange(RefrigerantNones);
                _MyDbContext.Fireequipments.RemoveRange(Fireequipments);
                _MyDbContext.Workinghours.RemoveRange(Workinghours);
                _MyDbContext.DumptreatmentOutsourcings.RemoveRange(DumptreatmentOutsourcings);


                _MyDbContext.SaveChanges();
                Organizes = null;
                Basicdatas = null;
                factory = null;


                return Sucess();
            }
            catch (Exception ex)
            {
                var Log = new Log()
                {
                    WhereFunction = "Organize_Delete_get",
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