using Project.Models;
using Microsoft.AspNetCore.Mvc;
using NPOI.Util;
using Project.Common;

namespace Project.Controllers
{
    public class ToolsController : CommonController
    {
        private MyDbContext _MyDbContext;
        public ToolsController(MyDbContext MyDbContext)
        {
            _MyDbContext = MyDbContext;
        }
        public async Task<ActionResult> ManageSetting(int id, string type, string account, IFormCollection? collection)
        {
            try
            {
                if (type == "add")
                {
                    //新增
                    var Suppliermanage = new Suppliermanage()
                    {
                        Account = AccountId(),
                        SupplierName = collection["suppliername"],
                        SupplierAddress = collection["supplieraddress"],

                    };
                    _MyDbContext.Suppliermanages.Add(Suppliermanage);
                    _MyDbContext.SaveChanges();
                    Suppliermanage = null;

                    return Sucess();
                }
                if (type == "Edit")
                {
                    //修改
                    var Suppliermanages = _MyDbContext.Suppliermanages.Find(id);
                    Suppliermanages.Account = AccountId();
                    Suppliermanages.SupplierName = collection["suppliername"];
                    Suppliermanages.SupplierAddress = collection["supplieraddress"];
                    _MyDbContext.Suppliermanages.Update(Suppliermanages);

                    _MyDbContext.SaveChanges();
                    Suppliermanages = null;


                    return Sucess();
                }
                if (type == "LoadData")
                {
                    if (AccountId() == null)
                    {
                        return Error("請先登入");

                    }
                    var Suppliermanages = _MyDbContext.Suppliermanages.Where(a => a.Account == AccountId()).OrderByDescending(a => a.Id).ToList();
                    int count = 1;
                    //選取前端jsgrid要顯示的欄位
                    var result = Suppliermanages.Select((item, index) => new
                    {
                        sort = count + index,
                        id = item.Id,
                        suppliername = item.SupplierName,
                        supplieraddress = item.SupplierAddress,
                    });
                    var retresult = Json(result);
                    return retresult;
                }
                if (type == "delete")
                {

                    var Suppliermanages = _MyDbContext.Suppliermanages.Find(id);

                    _MyDbContext.Suppliermanages.Remove(Suppliermanages);
                    _MyDbContext.SaveChanges();


                }
                return Sucess();
            }
            catch (Exception ex)
            {
                var Log = new Log()
                {
                    WhereFunction = "Tools_UseanddisposalUse",
                    Exception = ex.ToString(),
                    DateTime = DateTime.Now,
                };
                _MyDbContext.Logs.Add(Log);
                _MyDbContext.SaveChanges();
                Log = null;
                return Error("內部伺服器錯誤"); // 或其他適當的錯誤訊息
            }

        }
        //type(新增，修改，載入，刪除)
        public async Task<ActionResult> EnergyUse(int basicid, int id, string type, IFormCollection? collection, decimal? aferttotal, decimal? convertnum, decimal? beforetotal, decimal? distributeratio)
        {
            try
            {
                var a = _MyDbContext.Basicdatas.Find(basicid);//依盤查表名稱設定檔案存取路徑
                var EditLog = _MyDbContext.Organizes.Where(a => a.BasicId == basicid && a.Account == AccountId()).First();

                string energyname = "";
                string AfertUnit = "";
                string Unit = "";

                if (type != "LoadData")
                {
                    energyname = _MyDbContext.Coefficients.Where(a => a.Id.ToString() == collection["energyname"].ToString() && a.Type == "EnergyName").Select(a => a.Name).FirstOrDefault();
                    AfertUnit = _MyDbContext.Selectdata.Where(a => a.Code == collection["afertUnit"].ToString()).Select(a => a.Name).FirstOrDefault();
                }
                if(type == "Edit" || type== "delete")
                {
                    var Energyuses = _MyDbContext.Energyuses.Find(id);
                    if (Energyuses.BeforeUnit != null)
                    {
                        Unit = _MyDbContext.Selectdata.Where(a => a.Code == Energyuses.BeforeUnit.ToString()).Select(a => a.Name).FirstOrDefault();
                    }
                }
                if (type == "add")
                {
                    //新增能源使用
                    var Energyuse = new Energyuse()
                    {
                        BasicId = basicid,
                        FactoryName = collection["factoryname"],
                        EnergyName = collection["energyname"],
                        EquipmentName = collection["equipmentname"],
                        EquipmentLocation = collection["equipmentlocation"],
                        SupplierId = !String.IsNullOrEmpty(collection["suppliername"]) ? _MyDbContext.Suppliermanages.Where(a => a.SupplierName == collection["suppliername"].ToString() && a.Account == AccountId()).Select(a => a.Id).FirstOrDefault() : 0,
                        SupplierName = collection["suppliername"],
                        SupplierAddress = !String.IsNullOrEmpty(collection["suppliername"]) ? _MyDbContext.Suppliermanages.Where(a => a.SupplierName == collection["suppliername"].ToString() && a.Account == AccountId()).Select(a => a.SupplierAddress).FirstOrDefault() : "",
                        Remark = collection["remark"],
                        ConvertNum = convertnum,
                        AfertUnit = collection["afertUnit"],
                        DistributeRatio = distributeratio,
                        Account = AccountId(),
                        EditTime = DateTime.Now,
                    };
                    _MyDbContext.Energyuses.Add(Energyuse);
                    EditLog.EditLog += EnergyUseLog("新增", energyname, collection["equipmentname"], collection["equipmentlocation"], 0, "");
                    _MyDbContext.SaveChanges();
                    Energyuse = null;
                    return Sucess();
                }
                if (type == "Edit")
                {
                    //更新能源使用
                    var Energyuses = _MyDbContext.Energyuses.Find(id);
                    Energyuses.FactoryName = collection["factoryname"];
                    Energyuses.EnergyName = collection["energyname"];
                    Energyuses.EquipmentName = collection["equipmentname"];
                    Energyuses.EquipmentLocation = collection["equipmentlocation"];
                    Energyuses.SupplierId = _MyDbContext.Suppliermanages.Where(a => a.SupplierName == collection["suppliername"].ToString() && a.Account == AccountId()).Select(a => a.Id).FirstOrDefault();
                    Energyuses.SupplierName = _MyDbContext.Suppliermanages.Where(a => a.Id == Energyuses.SupplierId).Select(a => a.SupplierName).FirstOrDefault();
                    Energyuses.SupplierAddress = _MyDbContext.Suppliermanages.Where(a => a.Id == Energyuses.SupplierId).Select(a => a.SupplierAddress).FirstOrDefault();
                    Energyuses.Remark = collection["remark"];
                    Energyuses.ConvertNum = convertnum;
                    Energyuses.Account = AccountId();
                    Energyuses.EditTime = DateTime.Now;
                    //判斷詳細資料的總量是否為空，不為空則以總量做運算
                    if (Energyuses.BeforeTotal != null)
                    {
                        if (convertnum != null)
                        {
                            Energyuses.AfertTotal = Energyuses.BeforeTotal * convertnum;
                        }
                        else
                        {
                            Energyuses.AfertTotal = Energyuses.BeforeTotal;
                        }
                    }
                    Energyuses.AfertUnit = collection["afertUnit"];
                    Energyuses.DistributeRatio = distributeratio;
                    if (_MyDbContext.Energyuses.Any(a => a.Id == id && Energyuses.EnergyName == "1" && (Energyuses.BeforeUnit != "25" && Energyuses.BeforeUnit != "117"))) //電力
                    {
                        if(_MyDbContext.DIntervalusetotals.Any(a=>a.BindId==id && a.Type== "Unit" && a.BindWhere== "EnergyUse" && a.Num == "117"))
                        {
                            Energyuses.BeforeUnit = "117";
                        }
                        else
                        {
                            Energyuses.BeforeUnit = "25"; //度,千度
                        }
                    }
                    else if (_MyDbContext.Energyuses.Any(a => a.Id == id && Energyuses.EnergyName == "2" && Energyuses.BeforeUnit != "22" ))//天然氣
                    {
                        Energyuses.BeforeUnit = "22"; //立方公尺
                    }
                    else if (_MyDbContext.Energyuses.Any(a => a.Id == id && (Energyuses.EnergyName == "3" || Energyuses.EnergyName == "4" || Energyuses.EnergyName == "5" || Energyuses.EnergyName == "6" || Energyuses.EnergyName == "7")  && (Energyuses.BeforeUnit != "14" && Energyuses.BeforeUnit != "15"))) //車用柴油,車用汽油,柴油,汽油,液化石油氣
                    {
                        if (_MyDbContext.DIntervalusetotals.Any(a => a.BindId == id && a.Type == "Unit" && a.BindWhere == "EnergyUse" && a.Num == "15"))
                        {
                            Energyuses.BeforeUnit = "15";
                        }
                        else
                        {
                            Energyuses.BeforeUnit = "14"; //公升,公秉
                        }
                    }
                    _MyDbContext.Energyuses.Update(Energyuses);

                    EditLog.EditLog += EnergyUseLog("編輯", energyname, collection["equipmentname"], collection["equipmentlocation"], Energyuses.BeforeTotal, Unit);

                    _MyDbContext.SaveChanges();
                    Energyuses = null;

                    return Sucess();
                }
                if (type == "LoadData")
                {
                    if (AccountId() == null)
                    {
                        return Error("請先登入");

                    }
                    var Energyuses = _MyDbContext.Energyuses.Where(a => a.BasicId == basicid).OrderByDescending(a => a.Id).ToList();
                    int count = 1;
                    //選取前端jsgrid要顯示的欄位
                    var result = Energyuses.Select((item, index) => new
                    {
                        sort = count + index,
                        id = item.Id,
                        factoryname = item.FactoryName,
                        energyname = item.EnergyName,
                        category = _MyDbContext.Selectdata.Where(a => a.Code == _MyDbContext.Coefficients.Where(a => a.Id.ToString() == item.EnergyName).Select(a => a.Category).FirstOrDefault()).Select(a => a.Name),
                        equipmentname = item.EquipmentName,
                        equipmentlocation = item.EquipmentLocation,
                        supplieid = item.SupplierId,
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

                    return Json(result);
                }
                if (type == "delete")
                {

                    var Energyuses = _MyDbContext.Energyuses.Find(id);

                    var DTransportations = _MyDbContext.DTransportations.Where(a => a.BindId == id && a.BindWhere == "EnergyUse");//依id和屬於哪個表作塞選
                    var DIntervalusetotals = _MyDbContext.DIntervalusetotals.Where(a => a.BindId == id && a.BindWhere == "EnergyUse");
                    var DDatasources = _MyDbContext.DDatasources.Where(a => a.BindId == id && a.BindWhere == "EnergyUse");
                    var Evidencefilemanages = _MyDbContext.Evidencefilemanages.Where(a => a.ItemId == id && a.WhereForm == "能源使用");
                    string directoryPath = FileSave.Path + AccountId() + "/" + a.BasicId + "/能源使用";//檔案路徑

                    foreach (var item in Evidencefilemanages)
                    {
                        if (System.IO.File.Exists(directoryPath + "/" + item.FileName))
                        {
                            System.IO.File.Delete(directoryPath + "/" + item.FileName);
                        }
                    }

                    //刪除能源使用，及詳細資料的3個table
                    _MyDbContext.Energyuses.Remove(Energyuses);
                    _MyDbContext.DTransportations.RemoveRange(DTransportations);
                    _MyDbContext.DIntervalusetotals.RemoveRange(DIntervalusetotals);
                    _MyDbContext.DDatasources.RemoveRange(DDatasources);
                    _MyDbContext.Evidencefilemanages.RemoveRange(Evidencefilemanages);
                    EditLog.EditLog += EnergyUseLog("刪除", energyname, collection["equipmentname"], collection["equipmentlocation"], Energyuses.BeforeTotal, Unit);

                    _MyDbContext.SaveChanges();
                    Energyuses = null;
                    DTransportations = null;
                    DIntervalusetotals = null;
                    DDatasources = null;

                }
                return Sucess();
            }
            catch (Exception ex)
            {
                var Log = new Log()
                {
                    WhereFunction = "Tools_EnergyUse",
                    Exception = ex.ToString(),
                    DateTime = DateTime.Now,
                };
                _MyDbContext.Logs.Add(Log);
                _MyDbContext.SaveChanges();
                Log = null;
                return Error("內部伺服器錯誤"); // 或其他適當的錯誤訊息
            }
        }

        public async Task<ActionResult> ResourceUse(int basicid, int id, string type, IFormCollection? collection, decimal? aferttotal, decimal? convertnum, decimal? beforetotal, decimal? distributeratio)
        {
            try
            {
                var a = _MyDbContext.Basicdatas.Find(basicid);//依盤查表名稱設定檔案存取路徑
                var EditLog = _MyDbContext.Organizes.Where(a => a.BasicId == basicid && a.Account == AccountId()).First();

                string energyname = "";
                string AfertUnit = "";
                string Unit = "";

                if (type != "LoadData")
                {
                    energyname = _MyDbContext.Coefficients.Where(a => a.Id.ToString() == collection["energyname"].ToString() && a.Type == "ResourceName").Select(a => a.Name).FirstOrDefault();
                    AfertUnit = _MyDbContext.Selectdata.Where(a => a.Code == collection["afertUnit"].ToString()).Select(a => a.Name).FirstOrDefault();
                }
                if (type == "Edit" || type == "delete")
                {
                    var Resourceuses = _MyDbContext.Resourceuses.Find(id);
                    if (Resourceuses.BeforeUnit != null)
                    {
                        Unit = _MyDbContext.Selectdata.Where(a => a.Code == Resourceuses.BeforeUnit.ToString()).Select(a => a.Name).FirstOrDefault();
                    }
                }
                if (type == "add")
                {
                    //新增能源使用
                    var Resourceuse = new Resourceuse()
                    {
                        BasicId = basicid,
                        FactoryName = collection["factoryname"],
                        EnergyName = collection["energyname"],
                        EquipmentName = collection["equipmentname"],
                        EquipmentLocation = collection["equipmentlocation"],
                        SupplierId = !String.IsNullOrEmpty(collection["suppliername"]) ? _MyDbContext.Suppliermanages.Where(a => a.SupplierName == collection["suppliername"].ToString() && a.Account == AccountId()).Select(a => a.Id).FirstOrDefault() : 0,
                        SupplierName = collection["suppliername"],
                        SupplierAddress = !String.IsNullOrEmpty(collection["suppliername"]) ? _MyDbContext.Suppliermanages.Where(a => a.SupplierName == collection["suppliername"].ToString() && a.Account == AccountId()).Select(a => a.SupplierAddress).FirstOrDefault() : "",
                        Remark = collection["remark"],
                        ConvertNum = convertnum,
                        AfertUnit = collection["afertUnit"],
                        DistributeRatio = distributeratio,
                    };
                    _MyDbContext.Resourceuses.Add(Resourceuse);
                    EditLog.EditLog += ResourceUseLog("新增", energyname, collection["equipmentname"], collection["equipmentlocation"], 0, "");
                    _MyDbContext.SaveChanges();
                    Resourceuse = null;
                    a = null;
                    return Sucess();
                }
                if (type == "Edit")
                {
                    //更新能源使用
                    var Resourceuses = _MyDbContext.Resourceuses.Find(id);
                    Resourceuses.FactoryName = collection["factoryname"];
                    Resourceuses.EnergyName = collection["energyname"];
                    Resourceuses.EquipmentName = collection["equipmentname"];
                    Resourceuses.EquipmentLocation = collection["equipmentlocation"];
                    Resourceuses.SupplierId = _MyDbContext.Suppliermanages.Where(a => a.SupplierName == collection["suppliername"].ToString() && a.Account == AccountId()).Select(a => a.Id).FirstOrDefault();
                    Resourceuses.SupplierName = _MyDbContext.Suppliermanages.Where(a => a.Id == Resourceuses.SupplierId).Select(a => a.SupplierName).FirstOrDefault();
                    Resourceuses.SupplierAddress = _MyDbContext.Suppliermanages.Where(a => a.Id == Resourceuses.SupplierId).Select(a => a.SupplierAddress).FirstOrDefault();
                    Resourceuses.Remark = collection["remark"];
                    Resourceuses.ConvertNum = convertnum;
                    //判斷詳細資料的總量是否為空，不為空則以總量做運算
                    if (Resourceuses.BeforeTotal != null)
                    {
                        if (convertnum != null)
                        {
                            Resourceuses.AfertTotal = Resourceuses.BeforeTotal * convertnum;
                        }
                        else
                        {
                            Resourceuses.AfertTotal = Resourceuses.BeforeTotal;
                        }
                    }
                    Resourceuses.AfertUnit = collection["afertUnit"];
                    Resourceuses.DistributeRatio = distributeratio;
                    _MyDbContext.Resourceuses.Update(Resourceuses);
                    EditLog.EditLog += ResourceUseLog("編輯", energyname, collection["equipmentname"], collection["equipmentlocation"], Resourceuses.BeforeTotal, Unit);
                    _MyDbContext.SaveChanges();
                    Resourceuses = null;
                    a = null;

                    return Sucess();
                }
                if (type == "LoadData")
                {
                    if (AccountId() == null)
                    {
                        return Error("請先登入");

                    }
                    var Resourceuses = _MyDbContext.Resourceuses.Where(a => a.BasicId == basicid).OrderByDescending(a => a.Id).ToList();
                    int count = 1;
                    //選取前端jsgrid要顯示的欄位
                    var result = Resourceuses.Select((item, index) => new
                    {
                        sort = count + index,
                        id = item.Id,
                        factoryname = item.FactoryName,
                        energyname = item.EnergyName,
                        category = _MyDbContext.Selectdata.Where(a => a.Code == _MyDbContext.Coefficients.Where(a => a.Id.ToString() == item.EnergyName).Select(a => a.Category).FirstOrDefault()).Select(a => a.Name),
                        equipmentname = item.EquipmentName,
                        equipmentlocation = item.EquipmentLocation,
                        supplieid = item.SupplierId,
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

                    return Json(result);
                }
                if (type == "delete")
                {
                    string directoryPath = FileSave.Path + AccountId() + "/" + a.BasicId;//檔案路徑

                    var Resourceuses = _MyDbContext.Resourceuses.Find(id);
                    var DTransportations = _MyDbContext.DTransportations.Where(a => a.BindId == id && a.BindWhere == "ResourceUse");//依id和屬於哪個表作塞選
                    var DIntervalusetotals = _MyDbContext.DIntervalusetotals.Where(a => a.BindId == id && a.BindWhere == "ResourceUse");
                    var DDatasources = _MyDbContext.DDatasources.Where(a => a.BindId == id && a.BindWhere == "ResourceUse");
                    //刪除能源使用，及詳細資料的3個table
                    _MyDbContext.Resourceuses.Remove(Resourceuses);
                    _MyDbContext.DTransportations.RemoveRange(DTransportations);
                    _MyDbContext.DIntervalusetotals.RemoveRange(DIntervalusetotals);
                    _MyDbContext.DDatasources.RemoveRange(DDatasources);
                    EditLog.EditLog += ResourceUseLog("刪除", energyname, collection["equipmentname"], collection["equipmentlocation"], Resourceuses.BeforeTotal, Unit);

                    _MyDbContext.SaveChanges();
                    Resourceuses = null;
                    DTransportations = null;
                    DIntervalusetotals = null;
                    DDatasources = null;
                    a = null;

                }
                return Sucess();
            }
            catch (Exception ex)
            {
                var Log = new Log()
                {
                    WhereFunction = "Tools_ResourceUse",
                    Exception = ex.ToString(),
                    DateTime = DateTime.Now,
                };
                _MyDbContext.Logs.Add(Log);
                _MyDbContext.SaveChanges();
                Log = null;
                return Error("內部伺服器錯誤"); // 或其他適當的錯誤訊息
            }

        }

        public async Task<ActionResult> RefrigerantHave(int basicid, int id, string type, IFormCollection? collection, decimal? convertnum, decimal? total, string unit, decimal? distributeratio)
        {
            try
            {
                var a = _MyDbContext.Basicdatas.Find(basicid);//依盤查表名稱設定檔案存取路徑
                var EditLog = _MyDbContext.Organizes.Where(a => a.BasicId == basicid && a.Account == AccountId()).First();

                string refrigeranttype = "";
                string equipmenttype = "";
                if (type != "LoadData")
                {
                    refrigeranttype = _MyDbContext.Coefficients.Where(a => a.Id.ToString() == collection["refrigeranttype"].ToString() && a.Type == "RefrigerantName").Select(a => a.Name).FirstOrDefault();
                    equipmenttype = _MyDbContext.Selectdata.Where(a => a.Code == collection["equipmenttype"].ToString() && a.Type == "EquipmentType").Select(a => a.Name).FirstOrDefault();
                }
                if (type == "add")
                {
                    //新增冷媒逸散&填充(有)
                    var RefrigerantHave = new RefrigerantHave()
                    {
                        BasicId = basicid,
                        FactoryName = collection["factoryname"],
                        EquipmentName = collection["equipmentname"],
                        EquipmentType = collection["equipmenttype"],
                        RefrigerantType = collection["refrigeranttype"],
                        Management = collection["management"],
                        Principal = collection["principal"],
                        Datasource = collection["datasource"],
                        Remark = collection["remark"],
                        Total = total,
                        Unit = "10", //table_selectdata_公斤
                        DistributeRatio = distributeratio,
                    };
                    _MyDbContext.RefrigerantHaves.Add(RefrigerantHave);
                    EditLog.EditLog += RefrigerantHaveLog("新增", collection["equipmentname"], refrigeranttype, total);
                    _MyDbContext.SaveChanges();
                    RefrigerantHave = null;
                    a = null;
                    return Sucess();
                }
                if (type == "Edit")
                {
                    //更新冷媒逸散&填充(有)
                    var RefrigerantHaves = _MyDbContext.RefrigerantHaves.Find(id);
                    RefrigerantHaves.FactoryName = collection["factoryname"];
                    RefrigerantHaves.EquipmentName = collection["equipmentname"];
                    RefrigerantHaves.EquipmentType = collection["equipmenttype"];
                    RefrigerantHaves.RefrigerantType = collection["refrigeranttype"];
                    RefrigerantHaves.Management = collection["management"];
                    RefrigerantHaves.Principal = collection["principal"];
                    RefrigerantHaves.Datasource = collection["datasource"];
                    RefrigerantHaves.Remark = collection["remark"];
                    RefrigerantHaves.Total = total;
                    RefrigerantHaves.DistributeRatio = distributeratio;
                    _MyDbContext.RefrigerantHaves.Update(RefrigerantHaves);
                    EditLog.EditLog += RefrigerantHaveLog("編輯", collection["equipmentname"], refrigeranttype, total);

                    _MyDbContext.SaveChanges();
                    RefrigerantHaves = null;
                    a = null;
                }
                if (type == "LoadData")
                {
                    if (AccountId() == null)
                    {
                        return Error("請先登入");

                    }
                    var RefrigerantHaves = _MyDbContext.RefrigerantHaves.Where(a => a.BasicId == basicid).OrderByDescending(a => a.Id).ToList();
                    int count = 1;
                    //選取前端jsgrid要顯示的欄位
                    var result = RefrigerantHaves.Select((item, index) => new
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

                    return Json(result);
                }
                if (type == "delete")
                {
                    var RefrigerantHaves = _MyDbContext.RefrigerantHaves.Find(id);
                    var Evidencefilemanages = _MyDbContext.Evidencefilemanages.Where(a => a.ItemId == id && a.WhereForm == "冷媒逸散/有進行冷媒填充");
                    string directoryPath = FileSave.Path + AccountId() + "/" + a.BasicId + "/冷媒逸散/有進行冷媒填充";//檔案路徑
                    foreach (var item in Evidencefilemanages)
                    {
                        if (System.IO.File.Exists(directoryPath + "/" + item.FileName))
                        {
                            System.IO.File.Delete(directoryPath + "/" + item.FileName);
                        }
                    }
                    //刪除2個table
                    _MyDbContext.RefrigerantHaves.Remove(RefrigerantHaves);
                    _MyDbContext.Evidencefilemanages.RemoveRange(Evidencefilemanages);
                    EditLog.EditLog += RefrigerantHaveLog("刪除", collection["equipmentname"], refrigeranttype, total);

                    _MyDbContext.SaveChanges();
                    RefrigerantHaves = null;
                    a = null;

                }
                return Sucess();
            }
            catch (Exception ex)
            {
                var Log = new Log()
                {
                    WhereFunction = "Tools_RefrigerantHave",
                    Exception = ex.ToString(),
                    DateTime = DateTime.Now,
                };
                _MyDbContext.Logs.Add(Log);
                _MyDbContext.SaveChanges();
                Log = null;
                return Error("內部伺服器錯誤"); // 或其他適當的錯誤訊息
            }

        }
        public async Task<ActionResult> RefrigerantNone(int basicid, int id, string type, IFormCollection? collection, decimal? refrigerantweight, string unit, int machinequantity, decimal? distributeratio)
        {
            try
            {
                var a = _MyDbContext.Basicdatas.Find(basicid);//依盤查表名稱設定檔案存取路徑
                var EditLog = _MyDbContext.Organizes.Where(a => a.BasicId == basicid && a.Account == AccountId()).First();

                string refrigeranttype = "";
                string equipmenttype = "";
                if (type != "LoadData")
                {
                    refrigeranttype = _MyDbContext.Coefficients.Where(a => a.Id.ToString() == collection["refrigeranttype"].ToString() && a.Type == "RefrigerantName").Select(a => a.Name).FirstOrDefault();
                    equipmenttype = _MyDbContext.Selectdata.Where(a => a.Code == collection["equipmenttype"].ToString() && a.Type== "EquipmentType").Select(a => a.Name).FirstOrDefault();

                }
                if (type == "add")
                {
                    //新增冷媒逸散&填充(無)
                    var RefrigerantNone = new RefrigerantNone()
                    {
                        BasicId = basicid,
                        FactoryName = collection["factoryname"],
                        EquipmentName = collection["equipmentname"],
                        EquipmentType = collection["equipmenttype"],
                        EquipmentLocation = collection["equipmentlocation"],
                        RefrigerantType = collection["refrigeranttype"],
                        MachineQuantity = machinequantity,
                        Manufacturers = collection["manufacturers"],
                        Management = collection["management"],
                        Principal = collection["principal"],
                        Remark = collection["remark"],
                        RefrigerantWeight = refrigerantweight,
                        Unit = "10", //table_selectdata_公斤
                        DistributeRatio = distributeratio,
                    };
                    _MyDbContext.RefrigerantNones.Add(RefrigerantNone);
                    EditLog.EditLog += RefrigerantNoneLog("新增", collection["equipmentname"], equipmenttype, collection["equipmentlocation"], refrigeranttype, refrigerantweight);

                    _MyDbContext.SaveChanges();
                    RefrigerantNone = null;
                    a = null;
                    return Sucess();
                }
                if (type == "Edit")
                {
                    //更新冷媒逸散&填充(無)
                    var RefrigerantNones = _MyDbContext.RefrigerantNones.Find(id);
                    RefrigerantNones.FactoryName = collection["factoryname"];
                    RefrigerantNones.EquipmentName = collection["equipmentname"];
                    RefrigerantNones.EquipmentType = collection["equipmenttype"];
                    RefrigerantNones.EquipmentLocation = collection["equipmentlocation"];
                    RefrigerantNones.RefrigerantType = collection["refrigeranttype"];
                    RefrigerantNones.MachineQuantity = machinequantity;
                    RefrigerantNones.Manufacturers = collection["manufacturers"];
                    RefrigerantNones.Management = collection["management"];
                    RefrigerantNones.Principal = collection["principal"];
                    RefrigerantNones.Remark = collection["remark"];
                    RefrigerantNones.RefrigerantWeight = refrigerantweight;
                    RefrigerantNones.DistributeRatio = distributeratio;
                    _MyDbContext.RefrigerantNones.Update(RefrigerantNones);
                    EditLog.EditLog += RefrigerantNoneLog("編輯", collection["equipmentname"], equipmenttype, collection["equipmentlocation"], refrigeranttype, refrigerantweight);
                    _MyDbContext.SaveChanges();
                    RefrigerantNones = null;
                    a = null;

                    return Sucess();
                }
                if (type == "LoadData")
                {
                    if (AccountId() == null)
                    {
                        return Error("請先登入");

                    }
                    var RefrigerantNones = _MyDbContext.RefrigerantNones.Where(a => a.BasicId == basicid).OrderByDescending(a => a.Id).ToList();
                    int count = 1;
                    //選取前端jsgrid要顯示的欄位
                    var result = RefrigerantNones.Select((item, index) => new
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

                    return Json(result);
                }
                if (type == "delete")
                {
                    var RefrigerantNones = _MyDbContext.RefrigerantNones.Find(id);
                    var Evidencefilemanages = _MyDbContext.Evidencefilemanages.Where(a => a.ItemId == id && a.WhereForm == "冷媒逸散/無進行冷媒填充");
                    string directoryPath = FileSave.Path + AccountId() + "/" + a.BasicId + "/冷媒逸散/無進行冷媒填充";//檔案路徑
                    foreach (var item in Evidencefilemanages)
                    {
                        if (System.IO.File.Exists(directoryPath + "/" + item.FileName))
                        {
                            System.IO.File.Delete(directoryPath + "/" + item.FileName);
                        }
                    }
                    //刪除2個table
                    _MyDbContext.RefrigerantNones.Remove(RefrigerantNones);
                    _MyDbContext.Evidencefilemanages.RemoveRange(Evidencefilemanages);
                    EditLog.EditLog += RefrigerantNoneLog("刪除", collection["equipmentname"], equipmenttype, collection["equipmentlocation"], refrigeranttype, refrigerantweight);
                    _MyDbContext.SaveChanges();
                    RefrigerantNones = null;
                    a = null;

                }
                return Sucess();
            }
            catch (Exception ex)
            {
                var Log = new Log()
                {
                    WhereFunction = "Tools_RefrigerantNones",
                    Exception = ex.ToString(),
                    DateTime = DateTime.Now,
                };
                _MyDbContext.Logs.Add(Log);
                _MyDbContext.SaveChanges();
                Log = null;
                return Error("內部伺服器錯誤"); // 或其他適當的錯誤訊息
            }
        }

        public async Task<ActionResult> Fireequipment(int basicid, int id, string type, IFormCollection? collection, decimal? convertnum, decimal? beforetotal, decimal? distributeratio,decimal? aferttotal)
        {
            try
            {
                var a = _MyDbContext.Basicdatas.Find(basicid);//依盤查表名稱設定檔案存取路徑
                var EditLog = _MyDbContext.Organizes.Where(a => a.BasicId == basicid && a.Account == AccountId()).First();

                string EquipmentName = "";
                string AfertUnit = "";
                string Ghgtype = "";
                string Unit = "";
                if (type != "LoadData")
                {
                    EquipmentName = _MyDbContext.Coefficients.Where(a => a.Id.ToString() == collection["equipmentname"].ToString() && a.Type == "FireEquipmentName").Select(a => a.Name).FirstOrDefault();
                    AfertUnit = _MyDbContext.Selectdata.Where(a => a.Code == collection["afertUnit"].ToString() && a.Type=="Unit").Select(a => a.Name).FirstOrDefault();
                    Ghgtype = _MyDbContext.Selectdata.Where(a => a.Code == collection["ghgtype"].ToString() && a.Type == "GHGType").Select(a => a.Name).FirstOrDefault();
                }
                if (type == "Edit" || type == "delete")
                {
                    var Fireequipments = _MyDbContext.Fireequipments.Find(id);
                    if (Fireequipments.BeforeUnit != null)
                    {
                        Unit = _MyDbContext.Selectdata.Where(a => a.Code == Fireequipments.BeforeUnit.ToString()).Select(a => a.Name).FirstOrDefault();
                    }
                }
                if (type == "add")
                {
                    //新增
                    var Fireequipment = new Fireequipment()
                    {
                        BasicId = basicid,
                        FactoryName = collection["factoryname"],
                        EquipmentName = collection["equipmentname"],
                        Ghgtype = collection["ghgtype"],
                        Remark = collection["remark"],
                        ConvertNum = convertnum,
                        AfertUnit = collection["afertUnit"],
                        DistributeRatio = distributeratio,
                    };
                    _MyDbContext.Fireequipments.Add(Fireequipment);
                    EditLog.EditLog += FireequipmentLog("新增", EquipmentName, Ghgtype, 0, "");
                    _MyDbContext.SaveChanges();
                    Fireequipment = null;
                    a = null;
                    return Sucess();
                }
                if (type == "Edit")
                {
                    //更新消防
                    var Fireequipments = _MyDbContext.Fireequipments.Find(id);
                    Fireequipments.FactoryName = collection["factoryname"];
                    Fireequipments.EquipmentName = collection["equipmentName"];
                    Fireequipments.Ghgtype = collection["ghgtype"];
                    Fireequipments.Remark = collection["remark"];
                    Fireequipments.ConvertNum = convertnum;
                    //判斷詳細資料的總量是否為空，不為空則以總量做運算
                    if (Fireequipments.BeforeTotal != null)
                    {
                        if (convertnum != null)
                        {
                            Fireequipments.AfertTotal = Fireequipments.BeforeTotal * convertnum;
                        }
                        else
                        {
                            Fireequipments.AfertTotal = Fireequipments.BeforeTotal;
                        }
                    }
                    Fireequipments.AfertUnit = collection["afertUnit"];
                    Fireequipments.DistributeRatio = distributeratio;
                    _MyDbContext.Fireequipments.Update(Fireequipments);
                    EditLog.EditLog += FireequipmentLog("編輯", EquipmentName, Ghgtype, Fireequipments.BeforeTotal, Unit);
                    _MyDbContext.SaveChanges();
                    Fireequipments = null;
                    a = null;

                    return Sucess();
                }
                if (type == "LoadData")
                {
                    if (AccountId() == null)
                    {
                        return Error("請先登入");

                    }
                    var Fireequipments = _MyDbContext.Fireequipments.Where(a => a.BasicId == basicid).OrderByDescending(a => a.Id).ToList();
                    int count = 1;
                    //選取前端jsgrid要顯示的欄位
                    var result = Fireequipments.Select((item, index) => new
                    {
                        sort = count + index,
                        id = item.Id,
                        factoryname = item.FactoryName,
                        equipmentname = item.EquipmentName,
                        category = _MyDbContext.Selectdata.Where(a => a.Code == _MyDbContext.Coefficients.Where(a => a.Id.ToString() == item.EquipmentName).Select(a => a.Category).FirstOrDefault()).Select(a => a.Name),
                        Ghgtype = item.Ghgtype,
                        remark = item.Remark,
                        beforetotal = item.BeforeTotal,
                        beforeunit = _MyDbContext.Selectdata.Where(a => a.Code == item.BeforeUnit).Select(a => a.Name).FirstOrDefault(),
                        convertnum = item.ConvertNum,
                        convertunit = _MyDbContext.Selectdata.Where(a => a.Code == item.AfertUnit).Select(a => a.Name).FirstOrDefault() + "/" + _MyDbContext.Selectdata.Where(a => a.Code == item.BeforeUnit).Select(a => a.Name).FirstOrDefault(),
                        aferttotal = item.AfertTotal,
                        afertUnit = item.AfertUnit,
                        distributeratio = item.DistributeRatio,
                    });

                    return Json(result);
                }
                if (type == "delete")
                {

                    var Fireequipments = _MyDbContext.Fireequipments.Find(id);
                    var DIntervalusetotals = _MyDbContext.DIntervalusetotals.Where(a => a.BindId == id && a.BindWhere == "Fireequipment");
                    var DDatasources = _MyDbContext.DDatasources.Where(a => a.BindId == id && a.BindWhere == "Fireequipment");
                    var Evidencefilemanages = _MyDbContext.Evidencefilemanages.Where(a => a.ItemId == id && a.WhereForm == "其他逸散設備");
                    string directoryPath = FileSave.Path + AccountId() + "/" + a.BasicId + "/其他逸散設備";//檔案路徑

                    foreach (var item in Evidencefilemanages)
                    {
                        if (System.IO.File.Exists(directoryPath + "/" + item.FileName))
                        {
                            System.IO.File.Delete(directoryPath + "/" + item.FileName);
                        }
                    }
                    _MyDbContext.Fireequipments.Remove(Fireequipments);

                    _MyDbContext.DIntervalusetotals.RemoveRange(DIntervalusetotals);
                    _MyDbContext.DDatasources.RemoveRange(DDatasources);
                    _MyDbContext.Evidencefilemanages.RemoveRange(Evidencefilemanages);
                    EditLog.EditLog += FireequipmentLog("刪除", EquipmentName, Ghgtype, Fireequipments.BeforeTotal, Unit);

                    _MyDbContext.SaveChanges();
                    Fireequipments = null;
                    DIntervalusetotals = null;
                    DDatasources = null;
                    a = null;

                }
                return Sucess();
            }
            catch (Exception ex)
            {
                var Log = new Log()
                {
                    WhereFunction = "Tools_Fireequipment",
                    Exception = ex.ToString(),
                    DateTime = DateTime.Now,
                };
                _MyDbContext.Logs.Add(Log);
                _MyDbContext.SaveChanges();
                Log = null;
                return Error("內部伺服器錯誤"); // 或其他適當的錯誤訊息
            }

        }

        public async Task<ActionResult> DumptreatmentOutsourcing(int basicid, int id, string type, IFormCollection? collection, decimal? convertnum, decimal? beforetotal, decimal? distributeratio,decimal? aferttotal)
        {
            try
            {
                var a = _MyDbContext.Basicdatas.Find(basicid);//依盤查表名稱設定檔案存取路徑
                string WasteName = collection["wastename"];
                string WasteMethod = collection["wastemethod"];
                var EditLog = _MyDbContext.Organizes.Where(a => a.BasicId == basicid && a.Account == AccountId()).First();

                string AfertUnit = "";
                string Unit = "";

                if (type != "LoadData")
                {
                    AfertUnit = _MyDbContext.Selectdata.Where(a => a.Code == collection["afertUnit"].ToString() && a.Type == "Unit").Select(a => a.Name).FirstOrDefault();
                }
                if (type == "Edit" || type == "delete")
                {
                    var DumptreatmentOutsourcings = _MyDbContext.DumptreatmentOutsourcings.Find(id);
                    if (DumptreatmentOutsourcings.BeforeUnit != null)
                    {
                        Unit = _MyDbContext.Selectdata.Where(a => a.Code == DumptreatmentOutsourcings.BeforeUnit.ToString()).Select(a => a.Name).FirstOrDefault();
                    }
                }
                if (type == "add")
                {
                    var Coefficients = _MyDbContext.Coefficients.FirstOrDefault(a => a.Name == WasteName && a.WasteMethod == WasteMethod && a.Type == "dumptreatment_outsourcing");
                    if (Coefficients == null)
                    {
                        List<string> Method = _MyDbContext.Coefficients.Where(a => a.Name == WasteName && a.Type == "dumptreatment_outsourcing").Select(a => a.WasteMethod).ToList();
                        string result = string.Join(", ", Method);
                        return Error($"{WasteName}，請選擇{result}");
                    }
                    //新增空水委外處理
                    var DumptreatmentOutsourcing = new DumptreatmentOutsourcing()
                    {
                        BasicId = basicid,
                        FactoryName = collection["factoryname"],
                        WasteId = Coefficients.Id,
                        //WasteName = collection["wastename"],
                        //WasteMethod = collection["wastemethod"],
                        CleanerName = collection["cleanername"],
                        FinalAddress = collection["finaladdress"],
                        Remark = collection["remark"],
                        ConvertNum = convertnum,
                        AfertUnit = collection["afertUnit"],
                        DistributeRatio = distributeratio,
                    };
                    _MyDbContext.DumptreatmentOutsourcings.Add(DumptreatmentOutsourcing);
                    EditLog.EditLog += DumptreatmentOutsourcingLog("新增", WasteName, WasteMethod, collection["cleanername"], 0, "");
                    _MyDbContext.SaveChanges();
                    DumptreatmentOutsourcing = null;
                    a = null;
                    return Sucess();
                }
                if (type == "Edit")
                {
                    var Coefficients = _MyDbContext.Coefficients.FirstOrDefault(a => a.Name == WasteName && a.WasteMethod == WasteMethod && a.Type == "dumptreatment_outsourcing");
                    if (Coefficients == null)
                    {
                        List<string> Method = _MyDbContext.Coefficients.Where(a => a.Name == WasteName && a.Type == "dumptreatment_outsourcing").Select(a => a.WasteMethod).ToList();
                        string result = string.Join(", ", Method);
                        return Error($"{WasteName}，請選擇{result}");
                    }
                    //更新空水廢排放處理
                    var DumptreatmentOutsourcings = _MyDbContext.DumptreatmentOutsourcings.Find(id);
                    DumptreatmentOutsourcings.FactoryName = collection["factoryname"];
                    DumptreatmentOutsourcings.WasteId = Coefficients.Id;
                    //DumptreatmentOutsourcings.WasteName = collection["wastename"];
                    //DumptreatmentOutsourcings.WasteMethod = collection["wastemethod"];
                    DumptreatmentOutsourcings.CleanerName = collection["cleanername"];
                    DumptreatmentOutsourcings.FinalAddress = collection["finaladdress"];
                    DumptreatmentOutsourcings.Remark = collection["remark"];
                    DumptreatmentOutsourcings.ConvertNum = convertnum;
                    //判斷詳細資料的總量是否為空，不為空則以總量做運算
                    if (DumptreatmentOutsourcings.BeforeTotal != null)
                    {
                        if (convertnum != null)
                        {
                            DumptreatmentOutsourcings.AfertTotal = DumptreatmentOutsourcings.BeforeTotal * convertnum;
                        }
                        else
                        {
                            DumptreatmentOutsourcings.AfertTotal = DumptreatmentOutsourcings.BeforeTotal;
                        }
                    }
                    DumptreatmentOutsourcings.AfertUnit = collection["afertUnit"];
                    DumptreatmentOutsourcings.DistributeRatio = distributeratio;
                    _MyDbContext.DumptreatmentOutsourcings.Update(DumptreatmentOutsourcings);
                    EditLog.EditLog += DumptreatmentOutsourcingLog("編輯", WasteName, WasteMethod, collection["cleanername"], DumptreatmentOutsourcings.BeforeTotal, Unit);

                    _MyDbContext.SaveChanges();
                    DumptreatmentOutsourcings = null;
                    a = null;

                    return Sucess();
                }
                if (type == "LoadData")
                {
                    if (AccountId() == null)
                    {
                        return Nologin();

                    }
                    var DumptreatmentOutsourcings = _MyDbContext.DumptreatmentOutsourcings.Where(a => a.BasicId == basicid).OrderByDescending(a => a.Id).ToList();
                    int count = 1;
                    //選取前端jsgrid要顯示的欄位
                    var result = DumptreatmentOutsourcings.Select((item, index) => new
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

                    return Json(result);
                }
                if (type == "delete")
                {
                    var DumptreatmentOutsourcings = _MyDbContext.DumptreatmentOutsourcings.Find(id);
                    var DTransportations = _MyDbContext.DTransportations.Where(a => a.BindId == id && a.BindWhere == "DumptreatmentOutsourcing");//依id和屬於哪個表作塞選
                    var DIntervalusetotals = _MyDbContext.DIntervalusetotals.Where(a => a.BindId == id && a.BindWhere == "DumptreatmentOutsourcing");
                    var DDatasources = _MyDbContext.DDatasources.Where(a => a.BindId == id && a.BindWhere == "DumptreatmentOutsourcing");
                    var Evidencefilemanages = _MyDbContext.Evidencefilemanages.Where(a => a.ItemId == id && a.WhereForm == "廢棄物處理");
                    string directoryPath = FileSave.Path + AccountId() + "/" + a.BasicId + "/廢棄物處理";//檔案路徑
                    foreach (var item in Evidencefilemanages)
                    {
                        if (System.IO.File.Exists(directoryPath + "/" + item.FileName))
                        {
                            System.IO.File.Delete(directoryPath + "/" + item.FileName);
                        }
                    }
                    //刪除能源使用，及詳細資料的3個table
                    _MyDbContext.DumptreatmentOutsourcings.Remove(DumptreatmentOutsourcings);
                    _MyDbContext.DTransportations.RemoveRange(DTransportations);
                    _MyDbContext.DIntervalusetotals.RemoveRange(DIntervalusetotals);
                    _MyDbContext.DDatasources.RemoveRange(DDatasources);
                    _MyDbContext.Evidencefilemanages.RemoveRange(Evidencefilemanages);
                    EditLog.EditLog += DumptreatmentOutsourcingLog("刪除", WasteName, WasteMethod, collection["cleanername"], DumptreatmentOutsourcings.BeforeTotal, Unit);
                    _MyDbContext.SaveChanges();
                    DumptreatmentOutsourcings = null;
                    DTransportations = null;
                    DIntervalusetotals = null;
                    DDatasources = null;
                    a = null;

                }
                if (type == "Check")
                {
                    var Coefficients = _MyDbContext.Coefficients.FirstOrDefault(a => a.Name == WasteName && a.WasteMethod == WasteMethod && a.Type == "dumptreatment_outsourcing");
                    if (Coefficients == null)
                    {
                        List<string> Method = _MyDbContext.Coefficients.Where(a => a.Name == WasteName && a.Type == "dumptreatment_outsourcing").Select(a => a.WasteMethod).ToList();
                        string result = string.Join(", ", Method);
                        return Error($"{WasteName}，請選擇{result}");
                    }

                    return Sucess();
                }
                return Sucess();
            }
            catch (Exception ex)
            {
                var Log = new Log()
                {
                    WhereFunction = "Tools_DumptreatmentOutsourcing",
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
