using ITRIProject.Models;
using Microsoft.AspNetCore.Mvc;

namespace ITRIProject.Controllers
{
    public class CoefficientToolsController : CommonController
    {
        private MyDbContext _MyDbContext;
        public CoefficientToolsController(MyDbContext MyDbContext)
        {
            _MyDbContext = MyDbContext;
        }
        public async Task<ActionResult> EnergyUse(int basicid, int id, string type, IFormCollection? collection, decimal? co2coefficient, decimal? ch4coefficient, decimal? ch4gwp, decimal? n2ocoefficient, decimal? n2ogwp)
        {
            try
            {
                if (type == "add")
                {
                    //新增能源使用
                    var Coefficient = new Coefficient()
                    {
                        EmissionSource = collection["emissionsource"],
                        Name = collection["name"],
                        Unit = collection["unit"],
                        Category = collection["category"],
                        Co2coefficient = co2coefficient,
                        Ch4coefficient = ch4coefficient,
                        Ch4gwp= ch4gwp,
                        N2ocoefficient = n2ocoefficient,
                        N2ogwp=n2ogwp,
                        Type = "EnergyName",
                    };
                    _MyDbContext.Coefficients.Add(Coefficient);
                    _MyDbContext.SaveChanges();
                    Coefficient = null;
                    return Sucess();
                }
                if (type == "Edit")
                {
                    //更新能源使用
                    var Energycoefficient = _MyDbContext.Coefficients.Find(id);
                    Energycoefficient.EmissionSource = collection["emissionsource"];
                    Energycoefficient.Name = collection["name"];
                    Energycoefficient.Unit = collection["unit"];
                    Energycoefficient.Category = collection["category"];
                    Energycoefficient.Co2coefficient = co2coefficient;
                    Energycoefficient.Ch4coefficient = ch4coefficient;
                    Energycoefficient.Ch4gwp = ch4gwp;
                    Energycoefficient.N2ocoefficient = n2ocoefficient;
                    Energycoefficient.N2ogwp = n2ogwp;
                    _MyDbContext.Coefficients.Update(Energycoefficient);

                    _MyDbContext.SaveChanges();
                    Energycoefficient = null;

                    return Sucess();
                }
                if (type == "LoadData")
                {
                    if (AccountId() == null)
                    {
                        return Error("請先登入");

                    }
                    var Coefficients = _MyDbContext.Coefficients.Where(a=>a.Type=="EnergyName").ToList();//抓取該id的能源使用

                    var pagedList = Coefficients.OrderByDescending(a => a.Id).ToList();
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
                        ch4gwp = item.Ch4gwp,
                        n2ocoefficient = item.N2ocoefficient,
                        n2ounit = "KgN2O/" + _MyDbContext.Selectdata.Where(a => a.Code == item.Unit).Select(a => a.Name).FirstOrDefault(),
                        n2ogwp = item.N2ogwp,
                    });

                    return Json(result);
                }
                if (type == "delete")
                {

                    var Coefficients = _MyDbContext.Coefficients.Find(id);

                    //刪除能源使用，及詳細資料的3個table
                    _MyDbContext.Coefficients.Remove(Coefficients);

                    _MyDbContext.SaveChanges();
                    Coefficients = null;

                }
                return Sucess();
            }
            catch (Exception ex)
            {
                var Log = new Log()
                {
                    WhereFunction = "CoefficientTools_EnergyUse",
                    Exception = ex.ToString(),
                    DateTime = DateTime.Now,
                };
                _MyDbContext.Logs.Add(Log);
                _MyDbContext.SaveChanges();
                Log = null;
                return Error("內部伺服器錯誤"); // 或其他適當的錯誤訊息
            }
        }
        public async Task<ActionResult> ResourceUse(int basicid, int id, string type, IFormCollection? collection, decimal? co2coefficient, decimal? ch4coefficient, decimal? n2ocoefficient)
        {
            try
            {
                if (type == "add")
                {
                    //新增能源使用
                    var Coefficient = new Coefficient()
                    {
                        EmissionSource = collection["emissionsource"],
                        Name = collection["name"],
                        Unit = collection["unit"],
                        Category = collection["category"],
                        Co2coefficient = co2coefficient,
                        Type = "ResourceName",
                    };
                    _MyDbContext.Coefficients.Add(Coefficient);
                    _MyDbContext.SaveChanges();
                    Coefficient = null;
                    return Sucess();
                }
                if (type == "Edit")
                {
                    //更新能源使用
                    var ResourceUsecoefficient = _MyDbContext.Coefficients.Find(id);
                    ResourceUsecoefficient.EmissionSource = collection["emissionsource"];
                    ResourceUsecoefficient.Name = collection["name"];
                    ResourceUsecoefficient.Unit = collection["unit"];
                    ResourceUsecoefficient.Category = collection["category"];
                    ResourceUsecoefficient.Co2coefficient = co2coefficient;
                    _MyDbContext.Coefficients.Update(ResourceUsecoefficient);

                    _MyDbContext.SaveChanges();
                    ResourceUsecoefficient = null;

                    return Sucess();
                }
                if (type == "LoadData")
                {
                    if (AccountId() == null)
                    {
                        return Error("請先登入");

                    }
                    var Coefficients = _MyDbContext.Coefficients.Where(a=>a.Type== "ResourceName").ToList();//抓取該id的能源使用

                    var pagedList = Coefficients.OrderByDescending(a => a.Id).ToList();
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

                    return Json(result);
                }
                if (type == "delete")
                {

                    var Coefficients = _MyDbContext.Coefficients.Find(id);

                    //刪除能源使用，及詳細資料的3個table
                    _MyDbContext.Coefficients.Remove(Coefficients);

                    _MyDbContext.SaveChanges();
                    Coefficients = null;

                }
                return Sucess();
            }
            catch (Exception ex)
            {
                var Log = new Log()
                {
                    WhereFunction = "CoefficientTools_ResourceUse",
                    Exception = ex.ToString(),
                    DateTime = DateTime.Now,
                };
                _MyDbContext.Logs.Add(Log);
                _MyDbContext.SaveChanges();
                Log = null;
                return Error("內部伺服器錯誤"); // 或其他適當的錯誤訊息
            }
        }
        public async Task<ActionResult> Refrigerant(int basicid, int id, string type, IFormCollection? collection, decimal? hfcsgwp, decimal? pfcsgwp)
        {
            try
            {
                if (type == "add")
                {
                    //新增能源使用
                    var Coefficient = new Coefficient()
                    {
                        EmissionSource = collection["emissionsource"],
                        Name = collection["name"],
                        Unit = collection["unit"],
                        Category = collection["category"],
                        HfcsGwp = hfcsgwp,
                        PfcsGwp= pfcsgwp,
                        Type = "RefrigerantName",
                    };
                    _MyDbContext.Coefficients.Add(Coefficient);
                    _MyDbContext.SaveChanges();
                    Coefficient = null;
                    return Sucess();
                }
                if (type == "Edit")
                {
                    //更新能源使用
                    var Refrigerantcoefficient = _MyDbContext.Coefficients.Find(id);
                    Refrigerantcoefficient.EmissionSource = collection["emissionsource"];
                    Refrigerantcoefficient.Name = collection["name"];
                    Refrigerantcoefficient.Unit = collection["unit"];
                    Refrigerantcoefficient.Category = collection["category"];
                    Refrigerantcoefficient.HfcsGwp = hfcsgwp;
                    Refrigerantcoefficient.PfcsGwp = pfcsgwp;
                    _MyDbContext.Coefficients.Update(Refrigerantcoefficient);

                    _MyDbContext.SaveChanges();
                    Refrigerantcoefficient = null;

                    return Sucess();
                }
                if (type == "LoadData")
                {
                    if (AccountId() == null)
                    {
                        return Error("請先登入");

                    }
                    var Coefficients = _MyDbContext.Coefficients.Where(a => a.Type == "RefrigerantName").ToList();//抓取該id的能源使用

                    var pagedList = Coefficients.OrderByDescending(a => a.Id).ToList();
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

                    return Json(result);
                }
                if (type == "delete")
                {

                    var Coefficients = _MyDbContext.Coefficients.Find(id);

                    //刪除能源使用，及詳細資料的3個table
                    _MyDbContext.Coefficients.Remove(Coefficients);

                    _MyDbContext.SaveChanges();
                    Coefficients = null;

                }
                return Sucess();
            }
            catch (Exception ex)
            {
                var Log = new Log()
                {
                    WhereFunction = "CoefficientTools_Refrigerant",
                    Exception = ex.ToString(),
                    DateTime = DateTime.Now,
                };
                _MyDbContext.Logs.Add(Log);
                _MyDbContext.SaveChanges();
                Log = null;
                return Error("內部伺服器錯誤"); // 或其他適當的錯誤訊息
            }
        }
        public async Task<ActionResult> FireEquipment(int basicid, int id, string type, IFormCollection? collection,decimal? co2coefficient, decimal? hfcsgwp, decimal? sf6gwp)
        {
            try
            {
                if (type == "add")
                {
                    //新增能源使用
                    var Coefficient = new Coefficient()
                    {
                        EmissionSource = collection["emissionsource"],
                        Name = collection["name"],
                        Unit = collection["unit"],
                        Category = collection["category"],
                        Co2coefficient = co2coefficient,
                        HfcsGwp = hfcsgwp,
                        Sf6gwp = sf6gwp,
                        Type = "FireEquipmentName",
                    };
                    _MyDbContext.Coefficients.Add(Coefficient);
                    _MyDbContext.SaveChanges();
                    Coefficient = null;
                    return Sucess();
                }
                if (type == "Edit")
                {
                    //更新能源使用
                    var FireEquipmentcoefficient = _MyDbContext.Coefficients.Find(id);
                    FireEquipmentcoefficient.EmissionSource = collection["emissionsource"];
                    FireEquipmentcoefficient.Name = collection["name"];
                    FireEquipmentcoefficient.Unit = collection["unit"];
                    FireEquipmentcoefficient.Category = collection["category"];
                    FireEquipmentcoefficient.Co2coefficient= co2coefficient;
                    FireEquipmentcoefficient.HfcsGwp = hfcsgwp;
                    FireEquipmentcoefficient.Sf6gwp = sf6gwp;
                    _MyDbContext.Coefficients.Update(FireEquipmentcoefficient);

                    _MyDbContext.SaveChanges();
                    FireEquipmentcoefficient = null;

                    return Sucess();
                }
                if (type == "LoadData")
                {
                    if (AccountId() == null)
                    {
                        return Error("請先登入");

                    }
                    var Coefficients = _MyDbContext.Coefficients.Where(a => a.Type == "FireEquipmentName").ToList();//抓取該id的能源使用

                    var pagedList = Coefficients.OrderByDescending(a => a.Id).ToList();
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
                        sf6gwp = item.Sf6gwp,
                    });

                    return Json(result);
                }
                if (type == "delete")
                {

                    var Coefficients = _MyDbContext.Coefficients.Find(id);

                    //刪除能源使用，及詳細資料的3個table
                    _MyDbContext.Coefficients.Remove(Coefficients);

                    _MyDbContext.SaveChanges();
                    Coefficients = null;

                }
                return Sucess();
            }
            catch (Exception ex)
            {
                var Log = new Log()
                {
                    WhereFunction = "CoefficientTools_FireEquipment",
                    Exception = ex.ToString(),
                    DateTime = DateTime.Now,
                };
                _MyDbContext.Logs.Add(Log);
                _MyDbContext.SaveChanges();
                Log = null;
                return Error("內部伺服器錯誤"); // 或其他適當的錯誤訊息
            }
        }
        public async Task<ActionResult> dumptreatment_outsourcing(int basicid, int id, string type, IFormCollection? collection, decimal? co2coefficient, decimal? ch4coefficient, decimal? ch4gwp, decimal? n2ocoefficient, decimal? n2ogwp)
        {
            try
            {
                if (type == "add")
                {
                    //新增能源使用
                    var Coefficient = new Coefficient()
                    {
                        EmissionSource = collection["emissionsource"],
                        Name = collection["name"],
                        WasteMethod = collection["wastemethod"],
                        Unit = collection["unit"],
                        Category = collection["category"],
                        Co2coefficient = co2coefficient,
                        Type = "dumptreatment_outsourcing",
                    };
                    _MyDbContext.Coefficients.Add(Coefficient);
                    _MyDbContext.SaveChanges();
                    Coefficient = null;
                    return Sucess();
                }
                if (type == "Edit")
                {
                    //更新能源使用
                    var Dumptreatmentcoefficient = _MyDbContext.Coefficients.Find(id);
                    Dumptreatmentcoefficient.EmissionSource = collection["emissionsource"];
                    Dumptreatmentcoefficient.Name = collection["name"];
                    Dumptreatmentcoefficient.WasteMethod = collection["wastemethod"];
                    Dumptreatmentcoefficient.Unit = collection["unit"];
                    Dumptreatmentcoefficient.Category = collection["category"];
                    Dumptreatmentcoefficient.Co2coefficient = co2coefficient;

                    _MyDbContext.Coefficients.Update(Dumptreatmentcoefficient);

                    _MyDbContext.SaveChanges();
                    Dumptreatmentcoefficient = null;

                    return Sucess();
                }
                if (type == "LoadData")
                {
                    if (AccountId() == null)
                    {
                        return Error("請先登入");

                    }
                    var Coefficients = _MyDbContext.Coefficients.Where(a => a.Type == "dumptreatment_outsourcing").ToList();//抓取該id的能源使用

                    var pagedList = Coefficients.OrderByDescending(a => a.Id).ToList();
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

                    return Json(result);
                }
                if (type == "delete")
                {

                    var Coefficients = _MyDbContext.Coefficients.Find(id);

                    //刪除能源使用，及詳細資料的3個table
                    _MyDbContext.Coefficients.Remove(Coefficients);



                    _MyDbContext.SaveChanges();
                    Coefficients = null;

                }
                return Sucess();
            }
            catch (Exception ex)
            {
                var Log = new Log()
                {
                    WhereFunction = "CoefficientTools_dumptreatment_outsourcing",
                    Exception = ex.ToString(),
                    DateTime = DateTime.Now,
                };
                _MyDbContext.Logs.Add(Log);
                _MyDbContext.SaveChanges();
                Log = null;
                return Error("內部伺服器錯誤"); // 或其他適當的錯誤訊息
            }
        }
        public async Task<ActionResult> Transportation(int basicid, int id, string type, IFormCollection? collection, decimal? co2coefficient, decimal? ch4coefficient, decimal? n2ocoefficient)
        {
            try
            {
                if (type == "add")
                {
                    //新增能源使用
                    var Coefficient = new Coefficient()
                    {
                        EmissionSource = collection["emissionsource"],
                        Name = collection["name"],
                        Unit = collection["unit"],
                        Category = collection["category"],
                        Co2coefficient = co2coefficient,
                        Type = "Transportation",
                    };
                    _MyDbContext.Coefficients.Add(Coefficient);
                    _MyDbContext.SaveChanges();
                    Coefficient = null;
                    return Sucess();
                }
                if (type == "Edit")
                {
                    //更新能源使用
                    var ResourceUsecoefficient = _MyDbContext.Coefficients.Find(id);
                    ResourceUsecoefficient.EmissionSource = collection["emissionsource"];
                    ResourceUsecoefficient.Name = collection["name"];
                    ResourceUsecoefficient.Unit = collection["unit"];
                    ResourceUsecoefficient.Category = collection["category"];
                    ResourceUsecoefficient.Co2coefficient = co2coefficient;
                    _MyDbContext.Coefficients.Update(ResourceUsecoefficient);

                    _MyDbContext.SaveChanges();
                    ResourceUsecoefficient = null;

                    return Sucess();
                }
                if (type == "LoadData")
                {
                    if (AccountId() == null)
                    {
                        return Error("請先登入");

                    }
                    var Coefficients = _MyDbContext.Coefficients.Where(a => a.Type == "Transportation").ToList();//抓取該id的能源使用

                    var pagedList = Coefficients.OrderByDescending(a => a.Id).ToList();
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

                    return Json(result);
                }
                if (type == "delete")
                {

                    var Coefficients = _MyDbContext.Coefficients.Find(id);

                    //刪除能源使用，及詳細資料的3個table
                    _MyDbContext.Coefficients.Remove(Coefficients);

                    _MyDbContext.SaveChanges();
                    Coefficients = null;

                }
                return Sucess();
            }
            catch (Exception ex)
            {
                var Log = new Log()
                {
                    WhereFunction = "CoefficientTools_ResourceUse",
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
