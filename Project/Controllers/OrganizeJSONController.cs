using Project.Common;
using Project.Models;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json.Linq;
using NPOI.Util;
using Project.Common;
using static Project.Controllers.CommonController;

namespace Project.Controllers
{
    public class OrganizeJSONController : CommonController
    {
        private MyDbContext _MyDbContext;
        public OrganizeJSONController(MyDbContext MyDbContext)
        {
            _MyDbContext = MyDbContext;
        }
        // GET: ProductController/Delete/5
        public ActionResult EnergyUseImport(int BasicId)
        {
            try
            {
                ViewBag.BasicId = BasicId;
                return View();

            }
            catch (Exception ex)
            {
                var Log = new Log()
                {
                    WhereFunction = "OrganizeJSON_EnergyUseImportJSON_get",
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
        public ActionResult EnergyUseImport(IFormFile file, int BasicId, int SelMonth)

        {
            var fileName = DateTime.Now.ToString("yyyyMMddHHmmss") + file.FileName;//給予檔案不重複名稱
            string fileDir = FileSave.Path + "/JSON";//檔案路徑

            try
            {
                var a = _MyDbContext.Basicdatas.Where(a => a.BasicId == BasicId).FirstOrDefault();//基本資料
                List<string> result = new List<string>();//顯示資料

                string filePath = FileSave.Path + "/JSON" + fileName;//檔案路徑
                if (!Directory.Exists(fileDir))//沒資料夾則新建
                {
                    Directory.CreateDirectory(fileDir);
                }
                using (var stream = new FileStream(fileDir + fileName, FileMode.Create))
                {
                    file.CopyTo(stream);//新增檔案
                }

                result.Add($"{DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")} 檔案已上傳成功");
                result.Add($"{DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")} ---------開始匯入資料---------");
                result.Add($"{DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")} 資料成功讀入");
                result.Add($"{DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")} ---------開始匯入資料開始---------");
                result.Add($"{DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")} {file.FileName} 資料匯入開始");

                JArray EnergyUseJsonData;
                try
                {
                    EnergyUseJsonData = JSONImport.JSONImports(filePath);
                }
                catch (Exception)
                {
                    return Error("Json格式錯誤"); // 或其他適當的錯誤訊息
                }

                List<string> errorList = new List<string>();
                if (EnergyUseJsonData == null)
                {
                    errorList.Add($"{DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")} {file.FileName} 資料為空");
                }
                else
                {
                    int index = 0;
                    //判斷是否有錯誤
                    foreach (var data in EnergyUseJsonData)
                    {
                        var FactoryName = data["場域"]?.ParseToString();
                        var EnergyName = data["能源名稱"]?.ParseToString();
                        var EquipmentName = data["錶號或使用設備名稱"]?.ParseToString();
                        var EquipmentLocation = data["錶或設備位置"]?.ParseToString();
                        var SupplierName = data["供應商名稱"]?.ParseToString();


                        var Method = data["運輸方式"]?.ParseToString();
                        var StartLocation = data["起點"]?.ParseToString();
                        var EndLocation = data["迄點"]?.ParseToString();
                        var Car = data["車種"]?.ParseToString();
                        var Tonnes = data["噸數"]?.ParseToDecimal();
                        var Fuel = data["燃料"]?.ParseToString();
                        var Land = data["陸運(km)"]?.ParseToDecimal(-1);
                        var LandCondition = data["陸運(km)"]?.ParseToDecimal();//陸運條件
                        var Sea = data["海運(nm)"]?.ParseToDecimal(-1);
                        var SeaCondition = data["海運(nm)"]?.ParseToDecimal();//海運條件
                        var Air = data["空運(km)"]?.ParseToDecimal(-1);
                        var AirCondition = data["空運(km)"]?.ParseToDecimal();//空運條件

                        var Management = data["管理單位"]?.ParseToString();
                        var Principal = data["負責人員"]?.ParseToString();
                        var Datasource = data["數據來源"]?.ParseToString();

                        var Remark = data["備註"]?.ParseToString();
                        var Unit = data["單位"]?.ParseToString();
                        var DistributeRatio = data["分配比率"]?.ParseToDecimal(-1);
                        var DistributeRatioCondition = data["分配比率"]?.ParseToDecimal(-1); //分配比率條件

                        if (!_MyDbContext.BasicdataFactoryaddresses.Any(a => a.BasicId == BasicId && a.Name == FactoryName))
                        {
                            errorList.Add($"{DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")} {file.FileName} 第{index + 1}行失敗，原因(場域不存在)");
                        }
                        if (!_MyDbContext.Coefficients.Any(a => a.Name == EnergyName && a.Type == "EnergyName"))
                        {
                            errorList.Add($"{DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")} {file.FileName} 第{index + 1}行失敗，原因(能源名稱不存在)");
                        }
                        if (!_MyDbContext.Suppliermanages.Any(a => a.SupplierName == SupplierName && a.Account == AccountId()))
                        {
                            errorList.Add($"{DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")} {file.FileName} 第{index + 1}行失敗，原因(供應商名稱不存在)");
                        }
                        if (!_MyDbContext.Selectdata.Any(a => a.Name == Method) && Method != "")
                        {
                            errorList.Add($"{DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")} {file.FileName} 第{index + 1}行失敗，原因(運輸方式不存在)");
                        }
                        if (EnergyName == "電力" && Method != "管線") //電力只能為管線
                        {
                            errorList.Add($"{DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")} {file.FileName} 第{index + 1}行失敗，原因(能源名稱為：電力，運輸方式應為：管線)");
                        }
                        if (!_MyDbContext.Selectdata.Any(a => a.Name == Car) && Car != "")
                        {
                            errorList.Add($"{DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")} {file.FileName} 第{index + 1}行失敗，原因(車種不存在)");
                        }
                        if (EnergyName == "電力")
                        {
                            if (!_MyDbContext.Selectdata.Any(a => a.Name == Unit && (a.Code == "25" || a.Code == "117")))
                            {
                                errorList.Add($"{DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")} {file.FileName} 第{index + 1}行失敗，原因(電力單位須為:度(kwh),千度(mwh))");
                            }
                        }
                        if (EnergyName == "車用柴油" || EnergyName == "車用汽油" || EnergyName == "柴油" || EnergyName == "汽油" || EnergyName == "液化石油氣")
                        {
                            if (EnergyName == "車用柴油" && !_MyDbContext.Selectdata.Any(a => a.Name == Unit && (a.Code == "14" || a.Code == "15")))
                            {
                                errorList.Add($"{DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")} {file.FileName} 第{index + 1}行失敗，原因(車用柴油須為:公升(L)),公秉(kl))");
                            }
                            else if (EnergyName == "車用汽油" && !_MyDbContext.Selectdata.Any(a => a.Name == Unit && (a.Code == "14" || a.Code == "15")))
                            {
                                errorList.Add($"{DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")} {file.FileName} 第{index + 1}行失敗，原因(車用汽油須為:公升(L)),公秉(kl))");
                            }
                            else if (EnergyName == "柴油" && !_MyDbContext.Selectdata.Any(a => a.Name == Unit && (a.Code == "14" || a.Code == "15")))
                            {
                                errorList.Add($"{DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")} {file.FileName} 第{index + 1}行失敗，原因(柴油須為:公升(L)),公秉(kl))");
                            }
                            else if (EnergyName == "汽油" && !_MyDbContext.Selectdata.Any(a => a.Name == Unit && (a.Code == "14" || a.Code == "15")))
                            {
                                errorList.Add($"{DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")} {file.FileName} 第{index + 1}行失敗，原因(汽油須為:公升(L)),公秉(kl))");
                            }
                            else if (EnergyName == "液化石油氣" && !_MyDbContext.Selectdata.Any(a => a.Name == Unit && (a.Code == "14" || a.Code == "15")))
                            {
                                errorList.Add($"{DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")} {file.FileName} 第{index + 1}行失敗，原因(液化石油氣須為:公升(L)),公秉(kl))");
                            }
                        }
                        if (EnergyName == "天然氣")
                        {
                            if (!_MyDbContext.Selectdata.Any(a => a.Name == Unit && a.Code == "22"))
                            {
                                errorList.Add($"{DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")} {file.FileName} 第{index + 1}行失敗，原因(天然氣單位須為:立方公尺(m3))");
                            }
                        }
                        if (Land == -1)
                        {
                            errorList.Add($"{DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")} {file.FileName} 第{index + 1}行失敗，原因(陸運填寫不正確)");
                        }
                        if (LandCondition < 0)
                        {
                            errorList.Add($"{DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")} {file.FileName} 第{index + 1}行失敗，原因(陸運不可小於0)");
                        }
                        if (Sea == -1)
                        {
                            errorList.Add($"{DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")} {file.FileName} 第{index + 1}行失敗，原因(海運填寫不正確)");
                        }
                        if (SeaCondition < 0)
                        {
                            errorList.Add($"{DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")} {file.FileName} 第{index + 1}行失敗，原因(海運不可小於0)");
                        }
                        if (Air == -1)
                        {
                            errorList.Add($"{DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")} {file.FileName} 第{index + 1}行失敗，原因(空運填寫不正確)");
                        }
                        if (AirCondition < 0)
                        {
                            errorList.Add($"{DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")} {file.FileName} 第{index + 1}行失敗，原因(空運不可小於0)");
                        }                        
                        if (DistributeRatio == -1 || DistributeRatio == null || DistributeRatioCondition < 0 || DistributeRatioCondition > 100)
                        {
                            errorList.Add($"{DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")} {file.FileName} 第{index + 1}行失敗，原因(分配比率應介於0-100之間)");
                        }
                        if (Management == "")
                        {
                            errorList.Add($"{DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")} {file.FileName} 第{index + 1}行失敗，原因(管理單位為必填項目)");
                        }
                        if (Principal == "")
                        {
                            errorList.Add($"{DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")} {file.FileName} 第{index + 1}行失敗，原因(負責人員為必填項目)");
                        }
                        if (Datasource == "")
                        {
                            errorList.Add($"{DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")} {file.FileName} 第{index + 1}行失敗，原因(數據來源為必填項目)");
                        }

                        int monthcount = (a.EndTime.Year - a.StartTime.Year) * 12 + a.EndTime.Month - a.StartTime.Month + 1;//相差多少月份
                        int year, month;
                        if (a.StartTime.Month - SelMonth <= 0)
                        {
                            year = a.StartTime.Year - 1 - 1911;//系統從盤查區間自動帶出年
                            month = Math.Abs(a.StartTime.Month - SelMonth + 12);//系統自動帶出樂
                        }
                        else
                        {
                            year = a.StartTime.Year - 1911;//系統從盤查區間自動帶出年
                            month = a.StartTime.Month - SelMonth;//系統自動帶出樂
                        }

                        for (int j = 0; j < monthcount + 1; j += SelMonth)
                        {

                            var StartTime = data[year + "/" + month + "起始日期"].ParseToTWDate("1");
                            var EndTime = data[year + "/" + month + "結束日期"].ParseToTWDate("1");
                            var Num = data[year + "/" + month + "數值"].ParseToDecimal(-1);
                            var NumCondition = data[year + "/" + month + "數值"].ParseToDecimal();//數值條件

                            if (StartTime == "1")
                            {
                                errorList.Add($"{DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")} {file.FileName} 第{index + 1}行失敗，原因(" + year + "/" + month + "起始日期" + "填寫不正確)");
                            }
                            if (EndTime == "1")
                            {
                                errorList.Add($"{DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")} {file.FileName} 第{index + 1}行失敗，原因(" + year + "/" + month + "結束日期" + "填寫不正確)");
                            }
                            if (Num == -1)
                            {
                                errorList.Add($"{DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")} {file.FileName} 第{index + 1}行失敗，原因(" + year + "/" + month + "數值" + "填寫不正確)");
                            }
                            if (NumCondition < 0)
                            {
                                errorList.Add($"{DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")} {file.FileName} 第{index + 1}行失敗，原因(" + year + "/" + month + "數值" + "不可為負數)");
                            }
                            if (Num != null && (StartTime == "" || EndTime == ""))
                            {
                                errorList.Add($"{DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")} {file.FileName} 第{index + 1}行失敗，原因(" + year + "/" + month + "缺少起始或結束時間)");
                            }

                            month += SelMonth;
                            if (month > 12)
                            {
                                month -= 12;
                                year++;
                            }
                        }
                        index++;
                    }
                }
                
                //沒錯誤則新增資料
                if (errorList.Count == 0)
                {
                    foreach (var data in EnergyUseJsonData!)
                    {
                        var FactoryName = data["場域"]?.ParseToString();
                        var EnergyName = data["能源名稱"]?.ParseToString();
                        var EquipmentName = data["錶號或使用設備名稱"]?.ParseToString();
                        var EquipmentLocation = data["錶或設備位置"]?.ParseToString();
                        var SupplierName = data["供應商名稱"]?.ParseToString();


                        var Method = data["運輸方式"]?.ParseToString();
                        var StartLocation = data["起點"]?.ParseToString();
                        var EndLocation = data["迄點"]?.ParseToString();
                        var Car = data["車種"]?.ParseToString();
                        var Tonnes = data["噸數"]?.ParseToDecimal();
                        var Fuel = data["燃料"]?.ParseToString();
                        var Land = data["陸運(km)"]?.ParseToDecimal(-1);
                        var Sea = data["海運(nm)"]?.ParseToDecimal(-1);
                        var Air = data["空運(km)"]?.ParseToDecimal(-1);

                        var Management = data["管理單位"]?.ParseToString();
                        var Principal = data["負責人員"]?.ParseToString();
                        var Datasource = data["數據來源"]?.ParseToString();

                        var Remark = data["備註"]?.ParseToString();
                        var Unit = data["單位"]?.ParseToString();
                        var DistributeRatio = data["分配比率"]?.ParseToDecimal(-1);

                        var Energyuse = new Energyuse()
                        {
                            BasicId = BasicId,
                            FactoryName = _MyDbContext.BasicdataFactoryaddresses.FirstOrDefault(a => a.BasicId == BasicId && a.Name == FactoryName) != null ? _MyDbContext.BasicdataFactoryaddresses.FirstOrDefault(a => a.BasicId == BasicId && a.Name == FactoryName).Id.ToString() : null,
                            EnergyName = _MyDbContext.Coefficients.FirstOrDefault(a => a.Type == "EnergyName" && a.Name == EnergyName) != null ? _MyDbContext.Coefficients.FirstOrDefault(a => a.Type == "EnergyName" && a.Name == EnergyName).Id.ToString() : null,
                            EquipmentName = EquipmentName,
                            EquipmentLocation = EquipmentLocation,
                            SupplierId = _MyDbContext.Suppliermanages.FirstOrDefault(a => a.SupplierName == SupplierName && a.Account == AccountId()) != null ? _MyDbContext.Suppliermanages.FirstOrDefault(a => a.SupplierName == SupplierName && a.Account == AccountId()).Id : null,
                            Remark = Remark,
                            BeforeUnit = _MyDbContext.Selectdata.FirstOrDefault(a => a.Type == "Unit" && a.Name == Unit) != null ? _MyDbContext.Selectdata.FirstOrDefault(a => a.Type == "Unit" && a.Name == Unit).Code.ToString() : null,
                            DistributeRatio = DistributeRatio,

                        };
                        var Energy = _MyDbContext.Energyuses.Add(Energyuse);
                        _MyDbContext.SaveChanges(); // 儲存變更至資料庫
                        int EnergyUseId = Energy.Entity.Id;//該筆資料的Id

                        var DTransportation = new DTransportation()
                        {
                            BindId = EnergyUseId,
                            Method = _MyDbContext.Selectdata.FirstOrDefault(a => a.Type == "Transportation" && a.Name == Method) != null ? _MyDbContext.Selectdata.FirstOrDefault(a => a.Type == "Transportation" && a.Name == Method).Code.ToString() : null,
                            StartLocation = StartLocation,
                            EndLocation = EndLocation,
                            Car = _MyDbContext.Selectdata.FirstOrDefault(a => a.Type == "Car" && a.Name == Car) != null ? _MyDbContext.Selectdata.FirstOrDefault(a => a.Type == "Car" && a.Name == Car).Code.ToString() : null,
                            Tonnes = Tonnes,
                            Fuel = Fuel,
                            Land = Land,
                            Sea = Sea,
                            Air = Air,
                            BindWhere = "EnergyUse",

                        };
                        _MyDbContext.DTransportations.Add(DTransportation);
                        var DDatasource = new DDatasource()
                        {
                            BindId = EnergyUseId,
                            Management = Management,
                            Principal = Principal,
                            Datasource = Datasource,
                            BindWhere = "EnergyUse",

                        };
                        _MyDbContext.DDatasources.Add(DDatasource);
                        DIntervalusetotal DIntervalusetotal;
                        DIntervalusetotal = new DIntervalusetotal()
                        {
                            BindId = EnergyUseId,
                            Num = _MyDbContext.Selectdata.FirstOrDefault(a => a.Type == "Unit" && a.Name == Unit) != null ? _MyDbContext.Selectdata.FirstOrDefault(a => a.Type == "Unit" && a.Name == Unit).Code.ToString() : null,
                            Type = "Unit",
                            BindWhere = "EnergyUse",
                            ArraySort = 0,

                        };
                        _MyDbContext.DIntervalusetotals.Add(DIntervalusetotal);
                        string SelMonthNum = "";
                        if (SelMonth == 1)
                        {
                            SelMonthNum = "OneMonth";
                        }
                        else if (SelMonth == 2)
                        {
                            SelMonthNum = "TwoMonth";

                        }
                        DIntervalusetotal = new DIntervalusetotal()
                        {
                            BindId = EnergyUseId,
                            BindWhere = "EnergyUse",
                            Num = SelMonthNum,
                            Type = "SelMonth",
                        };
                        //新增單位
                        _MyDbContext.DIntervalusetotals.Add(DIntervalusetotal);
                        int monthcount = (a.EndTime.Year - a.StartTime.Year) * 12 + a.EndTime.Month - a.StartTime.Month + 1;//相差多少月份
                        int year, month;
                        if (a.StartTime.Month - SelMonth <= 0)
                        {
                            year = a.StartTime.Year - 1 - 1911;//系統從盤查區間自動帶出年
                            month = Math.Abs(a.StartTime.Month - SelMonth + 12);//系統自動帶出樂
                        }
                        else
                        {
                            year = a.StartTime.Year - 1911;//系統從盤查區間自動帶出年
                            month = a.StartTime.Month - SelMonth;//系統自動帶出樂
                        }
                        decimal? total = 0;

                        for (int j = 0; j < monthcount + 1; j += SelMonth)
                        {

                            var DateTimes = year + " / " + month;
                            var StartTime = data[year + "/" + month + "起始日期"].ParseToTWDate("1");
                            var EndTime = data[year + "/" + month + "結束日期"].ParseToTWDate("1");
                            var Num = data[year + "/" + month + "數值"].ParseToDecimal(-1);
                            DIntervalusetotal = new DIntervalusetotal()
                            {
                                BindId = EnergyUseId,
                                Num = DateTimes,
                                Type = "DateTime",
                                BindWhere = "EnergyUse",
                                ArraySort = j,
                            };
                            _MyDbContext.DIntervalusetotals.Add(DIntervalusetotal);
                            DIntervalusetotal = new DIntervalusetotal()
                            {
                                BindId = EnergyUseId,
                                Num = StartTime,
                                Type = "StartTime",
                                BindWhere = "EnergyUse",
                                ArraySort = j,

                            };
                            _MyDbContext.DIntervalusetotals.Add(DIntervalusetotal);
                            DIntervalusetotal = new DIntervalusetotal()
                            {
                                BindId = EnergyUseId,
                                Num = EndTime,
                                Type = "EndTime",
                                BindWhere = "EnergyUse",
                                ArraySort = j,

                            };
                            _MyDbContext.DIntervalusetotals.Add(DIntervalusetotal);
                            DIntervalusetotal = new DIntervalusetotal()
                            {
                                BindId = EnergyUseId,
                                Num = Num.ToString(),
                                Type = "Num",
                                BindWhere = "EnergyUse",
                                ArraySort = j,

                            };
                            _MyDbContext.DIntervalusetotals.Add(DIntervalusetotal);

                            if (j == 0 || j == monthcount)
                            {
                                if (StartTime != "" && EndTime != "" && Num.ToString() != "")
                                {
                                    total += Total(a.StartTime.Year - 1911, a.StartTime.Month, a.EndTime.Year - 1911, a.EndTime.Month, StartTime, EndTime, Num, j, monthcount, SelMonth);//年份計算j和monthcount判斷第一筆及最後一筆需用到
                                }
                            }
                            else
                            {
                                if (Num.ToString() != "")
                                {
                                    total += Num;
                                }



                            }
                            month += SelMonth;
                            if (month > 12)
                            {
                                month -= 12;
                                year++;
                            }
                        }

                        var Energyuses = _MyDbContext.Energyuses.Find(EnergyUseId);
                        Energyuses.BeforeTotal = Math.Round(Convert.ToDecimal(total), 2);

                        if (Energyuses.ConvertNum != null)
                        {
                            Energyuses.AfertTotal = total * Energyuses.ConvertNum;
                        }
                        else
                        {
                            Energyuses.AfertTotal = Math.Round(Convert.ToDecimal(total), 2);
                        }
                        var EditLog = _MyDbContext.Organizes.Where(a => a.BasicId == BasicId && a.Account == AccountId()).First();
                        EditLog.EditLog += EnergyUseLog("新增", EnergyName, EquipmentName, EquipmentLocation, Energyuses.AfertTotal, Unit);
                        _MyDbContext.SaveChanges(); // 儲存變更至資料庫
                    }

                }

                foreach (var error in errorList)
                {
                    result.Add($"<span style='color:#f00'>{error}</span>");
                }
                result.Add($"{DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")} {file.FileName} 資料匯入結束");
                result.Add($"{DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")} ---------所有匯入資料結束---------");
                if (errorList.Count > 0)
                {
                    result.Add($"{DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")} 資料有錯誤 請修正過資料再重新匯入");
                }
                else
                {
                    result.Add($"{DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")} 資料正確 匯入成功 共匯入{EnergyUseJsonData!.Count}筆");
                }


                System.IO.File.Delete(fileDir + fileName);

                return Sucess(result);
            }
            catch (Exception ex)
            {
                System.IO.File.Delete(fileDir + fileName);

                var Log = new Log()
                {
                    WhereFunction = "OrganizeJSON_EnergyUseImportJSON_post",
                    Exception = ex.ToString(),
                    DateTime = DateTime.Now,
                };
                _MyDbContext.Logs.Add(Log);
                _MyDbContext.SaveChanges();
                Log = null;
                if (ex.Message.Contains("does not belong to table"))
                {
                    return Error("請重新下載範本");
                }
                if (ex.Message.Contains("逗號數量不同"))
                {
                    return Error(ex.Message);
                }
                return Error("內部伺服器錯誤"); // 或其他適當的錯誤訊息
            }
        }

        public ActionResult ResourceUseImport(int BasicId)
        {
            try
            {
                ViewBag.BasicId = BasicId;
                return View();

            }
            catch (Exception ex)
            {
                var Log = new Log()
                {
                    WhereFunction = "OrganizeJSON_ResourceUseImportJSON_get",
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
        public ActionResult ResourceUseImport(IFormFile file, int BasicId, int SelMonth)

        {
            var fileName = DateTime.Now.ToString("yyyyMMddHHmmss") + file.FileName;//給予檔案不重複名稱
            string fileDir = FileSave.Path + "/JSON";//檔案路徑

            try
            {
                var a = _MyDbContext.Basicdatas.Where(a => a.BasicId == BasicId).FirstOrDefault();//基本資料
                List<string> result = new List<string>();//顯示資料

                string filePath = FileSave.Path + "/JSON" + fileName;//檔案路徑
                if (!Directory.Exists(fileDir))//沒資料夾則新建
                {
                    Directory.CreateDirectory(fileDir);
                }
                using (var stream = new FileStream(fileDir + fileName, FileMode.Create))
                {
                    file.CopyTo(stream);//新增檔案
                }

                result.Add($"{DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")} 檔案已上傳成功");
                result.Add($"{DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")} ---------開始匯入資料---------");
                result.Add($"{DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")} 資料成功讀入");
                result.Add($"{DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")} ---------開始匯入資料開始---------");
                result.Add($"{DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")} {file.FileName} 資料匯入開始");

                JArray ResourceUseJsonData;
                try
                {
                    ResourceUseJsonData = JSONImport.JSONImports(filePath);
                }
                catch (Exception)
                {
                    return Error("Json格式錯誤"); // 或其他適當的錯誤訊息
                }
                List<string> errorList = new List<string>();
                if (ResourceUseJsonData == null)
                {
                    errorList.Add($"{DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")} {file.FileName} 資料為空");
                }
                else
                {
                    int index = 0;
                    //判斷是否有錯誤
                    foreach (var data in ResourceUseJsonData)
                    {
                        var FactoryName = data["場域"]?.ParseToString();
                        var ResourceName = data["資源名稱"]?.ParseToString();
                        var EquipmentName = data["錶號或使用設備名稱"]?.ParseToString();
                        var EquipmentLocation = data["錶或設備位置"]?.ParseToString();
                        var SupplierName = data["供應商名稱"]?.ParseToString();


                        var Method = data["運輸方式"]?.ParseToString();
                        var StartLocation = data["起點"]?.ParseToString();
                        var EndLocation = data["迄點"]?.ParseToString();
                        var Car = data["車種"]?.ParseToString();
                        var Tonnes = data["噸數"]?.ParseToDecimal();
                        var Fuel = data["燃料"]?.ParseToString();
                        var Land = data["陸運(km)"]?.ParseToDecimal(-1);
                        var LandCondition = data["陸運(km)"]?.ParseToDecimal();//陸運條件
                        var Sea = data["海運(nm)"]?.ParseToDecimal(-1);
                        var SeaCondition = data["海運(nm)"]?.ParseToDecimal();//海運條件
                        var Air = data["空運(km)"]?.ParseToDecimal(-1);
                        var AirCondition = data["空運(km)"]?.ParseToDecimal();//空運條件

                        var Management = data["管理單位"]?.ParseToString();
                        var Principal = data["負責人員"]?.ParseToString();
                        var Datasource = data["數據來源"]?.ParseToString();

                        var Remark = data["備註"]?.ParseToString();
                        var Unit = data["單位"]?.ParseToString();
                        var DistributeRatio = data["分配比率"]?.ParseToDecimal(-1);
                        var DistributeRatioCondition = data["分配比率"]?.ParseToDecimal(-1); //分配比率條件


                        if (!_MyDbContext.BasicdataFactoryaddresses.Any(a => a.BasicId == BasicId && a.Name == FactoryName))
                        {
                            errorList.Add($"{DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")} {file.FileName} 第{index + 1}行失敗，原因(場域不存在)");
                        }
                        if (!_MyDbContext.Coefficients.Any(a => a.Name == ResourceName && a.Type == "ResourceName"))
                        {
                            errorList.Add($"{DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")} {file.FileName} 第{index + 1}行失敗，原因(資源名稱不存在)");
                        }
                        if (!_MyDbContext.Suppliermanages.Any(a => a.SupplierName == SupplierName && a.Account == AccountId()))
                        {
                            errorList.Add($"{DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")} {file.FileName} 第{index + 1}行失敗，原因(供應商名稱不存在)");
                        }
                        if (!_MyDbContext.Selectdata.Any(a => a.Name == Method) && Method != "")
                        {
                            errorList.Add($"{DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")} {file.FileName} 第{index + 1}行失敗，原因(運輸方式不存在)");
                        }
                        if (!_MyDbContext.Selectdata.Any(a => a.Name == Car) && Car != "")
                        {
                            errorList.Add($"{DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")} {file.FileName} 第{index + 1}行失敗，原因(車種不存在)");
                        }
                        var UnitCompare = _MyDbContext.Coefficients
                      .Where(a => a.Type == "ResourceName")
                      .Join(
                          _MyDbContext.Selectdata,
                          c => c.Unit,
                          s => s.Code,
                          (c, s) => new { Name = s.Name, Code = s.Code, Sort = s.Sort }
                      )
                      .OrderBy(r => r.Sort) // 根據 Sort 欄位進行升序排序
                      .Select(item => item.Name) //只要Name做比對
                      .ToList();
                        if (!UnitCompare.Contains(Unit))
                        { 
                            errorList.Add($"{DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")} {file.FileName} 第{index + 1}行失敗，原因(單位須為：{UnitCompare[0]})");
                        }
                        if (Land == -1)
                        {
                            errorList.Add($"{DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")} {file.FileName} 第{index + 1}行失敗，原因(陸運填寫不正確)");
                        }
                        if (LandCondition < 0)
                        {
                            errorList.Add($"{DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")} {file.FileName} 第{index + 1}行失敗，原因(陸運不可小於0)");
                        }
                        if (Sea == -1)
                        {
                            errorList.Add($"{DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")} {file.FileName} 第{index + 1}行失敗，原因(海運填寫不正確)");
                        }
                        if (SeaCondition < 0)
                        {
                            errorList.Add($"{DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")} {file.FileName} 第{index + 1}行失敗，原因(海運不可小於0)");
                        }
                        if (Air == -1)
                        {
                            errorList.Add($"{DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")} {file.FileName} 第{index + 1}行失敗，原因(空運填寫不正確)");
                        }
                        if (AirCondition < 0)
                        {
                            errorList.Add($"{DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")} {file.FileName} 第{index + 1}行失敗，原因(空運不可小於0)");
                        }
                        if (DistributeRatio == -1 || DistributeRatio == null || DistributeRatioCondition < 0 || DistributeRatioCondition > 100)
                        {
                            errorList.Add($"{DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")} {file.FileName} 第{index + 1}行失敗，原因(分配比率應介於0-100之間)");
                        }
                        if (Management == "")
                        {
                            errorList.Add($"{DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")} {file.FileName} 第{index + 1}行失敗，原因(管理單位為必填項目)");
                        }
                        if (Principal == "")
                        {
                            errorList.Add($"{DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")} {file.FileName} 第{index + 1}行失敗，原因(負責人員為必填項目)");
                        }
                        if (Datasource == "")
                        {
                            errorList.Add($"{DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")} {file.FileName} 第{index + 1}行失敗，原因(數據來源為必填項目)");
                        }

                        int monthcount = (a.EndTime.Year - a.StartTime.Year) * 12 + a.EndTime.Month - a.StartTime.Month + 1;//相差多少月份
                        int year, month;
                        if (a.StartTime.Month - SelMonth <= 0)
                        {
                            year = a.StartTime.Year - 1 - 1911;//系統從盤查區間自動帶出年
                            month = Math.Abs(a.StartTime.Month - SelMonth + 12);//系統自動帶出樂
                        }
                        else
                        {
                            year = a.StartTime.Year - 1911;//系統從盤查區間自動帶出年
                            month = a.StartTime.Month - SelMonth;//系統自動帶出樂
                        }

                        for (int j = 0; j < monthcount + 1; j += SelMonth)
                        {

                            var StartTime = data[year + "/" + month + "起始日期"].ParseToTWDate("1");
                            var EndTime = data[year + "/" + month + "結束日期"].ParseToTWDate("1");
                            var Num = data[year + "/" + month + "數值"].ParseToDecimal(-1);
                            var NumCondition = data[year + "/" + month + "數值"].ParseToDecimal();//數值條件

                            if (StartTime == "1")
                            {
                                errorList.Add($"{DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")} {file.FileName} 第{index + 1}行失敗，原因(" + year + "/" + month + "起始日期" + "填寫不正確)");
                            }
                            if (EndTime == "1")
                            {
                                errorList.Add($"{DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")} {file.FileName} 第{index + 1}行失敗，原因(" + year + "/" + month + "結束日期" + "填寫不正確)");
                            }
                            if (Num == -1)
                            {
                                errorList.Add($"{DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")} {file.FileName} 第{index + 1}行失敗，原因(" + year + "/" + month + "數值" + "填寫不正確)");
                            }
                            if (NumCondition < 0)
                            {
                                errorList.Add($"{DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")} {file.FileName} 第{index + 1}行失敗，原因(" + year + "/" + month + "數值" + "不可為負數)");
                            }
                            if (Num != null && (StartTime == "" || EndTime == ""))
                            {
                                errorList.Add($"{DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")} {file.FileName} 第{index + 1}行失敗，原因(" + year + "/" + month + "缺少起始或結束時間)");
                            }

                            month += SelMonth;
                            if (month > 12)
                            {
                                month -= 12;
                                year++;
                            }
                        }
                        index++;
                    }
                }

                //沒錯誤則新增資料
                if (errorList.Count == 0)
                {
                    foreach (var data in ResourceUseJsonData!)
                    {
                        var FactoryName = data["場域"]?.ParseToString();
                        var ResourceName = data["資源名稱"]?.ParseToString();
                        var EquipmentName = data["錶號或使用設備名稱"]?.ParseToString();
                        var EquipmentLocation = data["錶或設備位置"]?.ParseToString();
                        var SupplierName = data["供應商名稱"]?.ParseToString();


                        var Method = data["運輸方式"]?.ParseToString();
                        var StartLocation = data["起點"]?.ParseToString();
                        var EndLocation = data["迄點"]?.ParseToString();
                        var Car = data["車種"]?.ParseToString();
                        var Tonnes = data["噸數"]?.ParseToDecimal();
                        var Fuel = data["燃料"]?.ParseToString();
                        var Land = data["陸運(km)"]?.ParseToDecimal(-1);
                        var Sea = data["海運(nm)"]?.ParseToDecimal(-1);
                        var Air = data["空運(km)"]?.ParseToDecimal(-1);

                        var Management = data["管理單位"]?.ParseToString();
                        var Principal = data["負責人員"]?.ParseToString();
                        var Datasource = data["數據來源"]?.ParseToString();

                        var Remark = data["備註"]?.ParseToString();
                        var Unit = data["單位"]?.ParseToString();
                        var DistributeRatio = data["分配比率"]?.ParseToDecimal(-1);
                        var Resourceuse = new Resourceuse()
                        {
                            BasicId = BasicId,
                            FactoryName = _MyDbContext.BasicdataFactoryaddresses.FirstOrDefault(a => a.BasicId == BasicId && a.Name == FactoryName) != null ? _MyDbContext.BasicdataFactoryaddresses.FirstOrDefault(a => a.BasicId == BasicId && a.Name == FactoryName).Id.ToString() : null,
                            EnergyName = _MyDbContext.Coefficients.FirstOrDefault(a => a.Type == "ResourceName" && a.Name == ResourceName) != null ? _MyDbContext.Coefficients.FirstOrDefault(a => a.Type == "ResourceName" && a.Name == ResourceName).Id.ToString() : null,
                            EquipmentName = EquipmentName,
                            EquipmentLocation = EquipmentLocation,
                            SupplierId = _MyDbContext.Suppliermanages.FirstOrDefault(a => a.SupplierName == SupplierName && a.Account == AccountId()) != null ? _MyDbContext.Suppliermanages.FirstOrDefault(a => a.SupplierName == SupplierName && a.Account == AccountId()).Id : null,
                            Remark = Remark,
                            BeforeUnit = _MyDbContext.Selectdata.FirstOrDefault(a => a.Type == "Unit" && a.Name == Unit) != null ? _MyDbContext.Selectdata.FirstOrDefault(a => a.Type == "Unit" && a.Name == Unit).Code.ToString() : null,
                            DistributeRatio = DistributeRatio,

                        };
                        var Resource = _MyDbContext.Resourceuses.Add(Resourceuse);

                        _MyDbContext.SaveChanges(); // 儲存變更至資料庫
                        int ResourceUseId = Resource.Entity.Id;//該筆資料的Id

                        var DTransportation = new DTransportation()
                        {
                            BindId = ResourceUseId,
                            Method = _MyDbContext.Selectdata.FirstOrDefault(a => a.Type == "Transportation" && a.Name == Method) != null ? _MyDbContext.Selectdata.FirstOrDefault(a => a.Type == "Transportation" && a.Name == Method).Code.ToString() : null,
                            StartLocation = StartLocation,
                            EndLocation = EndLocation,
                            Car = _MyDbContext.Selectdata.FirstOrDefault(a => a.Type == "Car" && a.Name == Car) != null ? _MyDbContext.Selectdata.FirstOrDefault(a => a.Type == "Car" && a.Name == Car).Code.ToString() : null,
                            Tonnes = Tonnes,
                            Fuel = Fuel,
                            Land = Land,
                            Sea = Sea,
                            Air = Air,
                            BindWhere = "ResourceUse",

                        };
                        _MyDbContext.DTransportations.Add(DTransportation);
                        var DDatasource = new DDatasource()
                        {
                            BindId = ResourceUseId,
                            Management = Management,
                            Principal = Principal,
                            Datasource = Datasource,
                            BindWhere = "ResourceUse",

                        };
                        _MyDbContext.DDatasources.Add(DDatasource);
                        DIntervalusetotal DIntervalusetotal;
                        DIntervalusetotal = new DIntervalusetotal()
                        {
                            BindId = ResourceUseId,
                            Num = _MyDbContext.Selectdata.FirstOrDefault(a => a.Type == "Unit" && a.Name == Unit) != null ? _MyDbContext.Selectdata.FirstOrDefault(a => a.Type == "Unit" && a.Name == Unit).Code.ToString() : null,
                            Type = "Unit",
                            BindWhere = "ResourceUse",
                            ArraySort = 0,

                        };
                        _MyDbContext.DIntervalusetotals.Add(DIntervalusetotal);
                        string SelMonthNum = "";
                        if (SelMonth == 1)
                        {
                            SelMonthNum = "OneMonth";
                        }
                        else if (SelMonth == 2)
                        {
                            SelMonthNum = "TwoMonth";

                        }
                        DIntervalusetotal = new DIntervalusetotal()
                        {
                            BindId = ResourceUseId,
                            BindWhere = "ResourceUse",
                            Num = SelMonthNum,
                            Type = "SelMonth",
                        };
                        //新增單位
                        _MyDbContext.DIntervalusetotals.Add(DIntervalusetotal);
                        int monthcount = (a.EndTime.Year - a.StartTime.Year) * 12 + a.EndTime.Month - a.StartTime.Month + 1;//相差多少月份
                        int year, month;
                        if (a.StartTime.Month - SelMonth <= 0)
                        {
                            year = a.StartTime.Year - 1 - 1911;//系統從盤查區間自動帶出年
                            month = Math.Abs(a.StartTime.Month - SelMonth + 12);//系統自動帶出樂
                        }
                        else
                        {
                            year = a.StartTime.Year - 1911;//系統從盤查區間自動帶出年
                            month = a.StartTime.Month - SelMonth;//系統自動帶出樂
                        }
                        decimal? total = 0;

                        for (int j = 0; j < monthcount + 1; j += SelMonth)
                        {

                            var DateTimes = year + " / " + month;
                            var StartTime = data[year + "/" + month + "起始日期"].ParseToTWDate("1");
                            var EndTime = data[year + "/" + month + "結束日期"].ParseToTWDate("1");
                            var Num = data[year + "/" + month + "數值"].ParseToDecimal(-1);
                            DIntervalusetotal = new DIntervalusetotal()
                            {
                                BindId = ResourceUseId,
                                Num = DateTimes,
                                Type = "DateTime",
                                BindWhere = "ResourceUse",
                                ArraySort = j,
                            };
                            _MyDbContext.DIntervalusetotals.Add(DIntervalusetotal);
                            DIntervalusetotal = new DIntervalusetotal()
                            {
                                BindId = ResourceUseId,
                                Num = StartTime,
                                Type = "StartTime",
                                BindWhere = "ResourceUse",
                                ArraySort = j,

                            };
                            _MyDbContext.DIntervalusetotals.Add(DIntervalusetotal);
                            DIntervalusetotal = new DIntervalusetotal()
                            {
                                BindId = ResourceUseId,
                                Num = EndTime,
                                Type = "EndTime",
                                BindWhere = "ResourceUse",
                                ArraySort = j,

                            };
                            _MyDbContext.DIntervalusetotals.Add(DIntervalusetotal);
                            DIntervalusetotal = new DIntervalusetotal()
                            {
                                BindId = ResourceUseId,
                                Num = Num.ToString(),
                                Type = "Num",
                                BindWhere = "ResourceUse",
                                ArraySort = j,

                            };
                            _MyDbContext.DIntervalusetotals.Add(DIntervalusetotal);

                            if (j == 0 || j == monthcount)
                            {
                                if (StartTime != "" && EndTime != "" && Num.ToString() != "")
                                {
                                    total += Total(a.StartTime.Year - 1911, a.StartTime.Month, a.EndTime.Year - 1911, a.EndTime.Month, StartTime, EndTime, Num, j, monthcount, SelMonth);//年份計算j和monthcount判斷第一筆及最後一筆需用到
                                }
                            }
                            else
                            {
                                if (Num.ToString() != "")
                                {
                                    total += Num;
                                }
                            }
                            month += SelMonth;
                            if (month > 12)
                            {
                                month -= 12;
                                year++;
                            }
                        }

                        var Resourceuses = _MyDbContext.Resourceuses.Find(ResourceUseId);
                        Resourceuses.BeforeTotal = Math.Round(Convert.ToDecimal(total), 2);

                        if (Resourceuses.ConvertNum != null)
                        {
                            Resourceuses.AfertTotal = total * Resourceuses.ConvertNum;
                        }
                        else
                        {
                            Resourceuses.AfertTotal = Math.Round(Convert.ToDecimal(total), 2);
                        }
                        var EditLog = _MyDbContext.Organizes.Where(a => a.BasicId == BasicId && a.Account == AccountId()).First();
                        EditLog.EditLog += ResourceUseLog("新增", ResourceName, EquipmentName, EquipmentLocation, Resourceuses.AfertTotal, Unit);
                        _MyDbContext.SaveChanges(); // 儲存變更至資料庫
                    }

                }

                foreach (var error in errorList)
                {
                    result.Add($"<span style='color:#f00'>{error}</span>");
                }
                result.Add($"{DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")} {file.FileName} 資料匯入結束");
                result.Add($"{DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")} ---------所有匯入資料結束---------");
                if (errorList.Count > 0)
                {
                    result.Add($"{DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")} 資料有錯誤 請修正過資料再重新匯入");
                }
                else
                {
                    result.Add($"{DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")} 資料正確 匯入成功 共匯入{ResourceUseJsonData!.Count}筆");
                }


                System.IO.File.Delete(fileDir + fileName);

                return Sucess(result);
            }
            catch (Exception ex)
            {
                System.IO.File.Delete(fileDir + fileName);

                var Log = new Log()
                {
                    WhereFunction = "OrganizeJSON_ResourceUseImportJSON_post",
                    Exception = ex.ToString(),
                    DateTime = DateTime.Now,
                };
                _MyDbContext.Logs.Add(Log);
                _MyDbContext.SaveChanges();
                Log = null;
                if (ex.Message.Contains("does not belong to table"))
                {
                    return Error("請重新下載範本");
                }
                if (ex.Message.Contains("逗號數量不同"))
                {
                    return Error(ex.Message);
                }
                return Error("內部伺服器錯誤"); // 或其他適當的錯誤訊息
            }
        }

        public ActionResult RefrigerantNoneImport(int BasicId)
        {
            try
            {
                ViewBag.BasicId = BasicId;
                return View();

            }
            catch (Exception ex)
            {
                var Log = new Log()
                {
                    WhereFunction = "OrganizeJSON_RefrigerantNoneImportJSON_get",
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
        public ActionResult RefrigerantNoneImport(IFormFile file, int BasicId)

        {
            var fileName = DateTime.Now.ToString("yyyyMMddHHmmss") + file.FileName;//給予檔案不重複名稱
            string fileDir = FileSave.Path + "/JSON";//檔案路徑

            try
            {
                var a = _MyDbContext.Basicdatas.Where(a => a.BasicId == BasicId).FirstOrDefault();//基本資料
                List<string> result = new List<string>();//顯示資料

                string filePath = FileSave.Path + "/JSON" + fileName;//檔案路徑
                if (!Directory.Exists(fileDir))//沒資料夾則新建
                {
                    Directory.CreateDirectory(fileDir);
                }
                using (var stream = new FileStream(fileDir + fileName, FileMode.Create))
                {
                    file.CopyTo(stream);//新增檔案
                }

                result.Add($"{DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")} 檔案已上傳成功");
                result.Add($"{DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")} ---------開始匯入資料---------");
                result.Add($"{DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")} 資料成功讀入");
                result.Add($"{DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")} ---------開始匯入資料開始---------");
                result.Add($"{DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")} {file.FileName} 資料匯入開始");


                JArray RefrigerantNoneJsonData;
                try
                {
                    RefrigerantNoneJsonData = JSONImport.JSONImports(filePath);
                }
                catch (Exception)
                {
                    return Error("Json格式錯誤"); // 或其他適當的錯誤訊息
                }

                List<string> errorList = new List<string>();
                if (RefrigerantNoneJsonData == null)
                {
                    errorList.Add($"{DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")} {file.FileName} 資料為空");
                }
                else
                {
                    int index = 0;
                    //判斷是否有錯誤
                    foreach (var data in RefrigerantNoneJsonData)
                    {
                        var FactoryName = data["場域"]?.ParseToString();
                        var EquipmentName = data["設備名稱"]?.ParseToString();
                        var EquipmentType = data["設備類型"]?.ParseToString();
                        var EquipmentLocation = data["設備位置"]?.ParseToString();
                        var RefrigerantType = data["冷媒種類"]?.ParseToString();
                        var MachineQuantity = data["設備數量"]?.ParseToInt(-1);
                        var MachineQuantityCondition = data["設備數量"]?.ParseToInt();//設備數量條件
                        var Manufacturers = data["廠牌"]?.ParseToString();
                        var Management = data["管理單位"]?.ParseToString();
                        var Principal = data["負責人員"]?.ParseToString();
                        var Remark = data["備註"]?.ParseToString();
                        var RefrigerantWeight = data["冷媒重量"]?.ParseToDecimal(-1);
                        var RefrigerantWeightCondition = data["冷媒重量"]?.ParseToDecimal();//冷媒重量條件
                        var Unit = data["單位"]?.ParseToString();
                        var DistributeRatio = data["分配比率"]?.ParseToDecimal(-1);
                        var DistributeRatioCondition = data["分配比率"]?.ParseToDecimal();//分配比率條件


                        if (!_MyDbContext.BasicdataFactoryaddresses.Any(a => a.BasicId == BasicId && a.Name == FactoryName))
                        {
                            errorList.Add($"{DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")} {file.FileName} 第{index + 1}行失敗，原因(場域不存在)");
                        }
                        if (EquipmentName == "")
                        {
                            errorList.Add($"{DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")} {file.FileName} 第{index + 1}行失敗，原因(設備名稱為必填項目)");
                        }
                        if (!_MyDbContext.Selectdata.Any(a => a.Name == EquipmentType))
                        {
                            errorList.Add($"{DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")} {file.FileName} 第{index + 1}行失敗，原因(設備類型不存在)");
                        }
                        if (!_MyDbContext.Coefficients.Any(a => a.Name == RefrigerantType && a.Type == "RefrigerantName"))
                        {
                            errorList.Add($"{DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")} {file.FileName} 第{index + 1}行失敗，原因(冷媒種類不存在)");
                        }
                        if (MachineQuantity == -1 || MachineQuantity == null || MachineQuantityCondition < 0)
                        {
                            errorList.Add($"{DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")} {file.FileName} 第{index + 1}行失敗，原因(設備數量須為正整數)");
                        }
                        if (Management == "")
                        {
                            errorList.Add($"{DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")} {file.FileName} 第{index + 1}行失敗，原因(管理單位為必填項目)");
                        }
                        if (Principal == "")
                        {
                            errorList.Add($"{DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")} {file.FileName} 第{index + 1}行失敗，原因(負責人員為必填項目)");
                        }
                        if (RefrigerantWeight == -1  || RefrigerantWeight ==null || RefrigerantWeightCondition < 0)
                        {
                            errorList.Add($"{DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")} {file.FileName} 第{index + 1}行失敗，原因(冷媒重量須為大於0的數值)");
                        }
                        if (Unit != "公斤(kg)")
                        {
                            errorList.Add($"{DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")} {file.FileName} 第{index + 1}行失敗，原因(單位須為公斤(kg))");
                        }
                        if (DistributeRatio == -1 || DistributeRatio == null || DistributeRatioCondition < 0 || DistributeRatioCondition > 100)
                        {
                            errorList.Add($"{DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")} {file.FileName} 第{index + 1}行失敗，原因(分配比率應介於0-100之間)");
                        }

                    }
                }
                //沒錯誤則新增資料
                if (errorList.Count == 0)
                {
                    foreach (var data in RefrigerantNoneJsonData!)
                    {
                            var FactoryName = data["場域"]?.ParseToString();
                            var EquipmentName = data["設備名稱"]?.ParseToString();
                            var EquipmentType = data["設備類型"]?.ParseToString();
                            var EquipmentLocation = data["設備位置"]?.ParseToString();
                            var RefrigerantType = data["冷媒種類"]?.ParseToString();
                            var MachineQuantity = data["設備數量"]?.ParseToInt(-1);
                            var Manufacturers = data["廠牌"]?.ParseToString();
                            var Management = data["管理單位"]?.ParseToString();
                            var Principal = data["負責人員"]?.ParseToString();
                            var Remark = data["備註"]?.ParseToString();
                            var RefrigerantWeight = data["冷媒重量"]?.ParseToDecimal(-1);
                            var Unit = data["單位"]?.ParseToString();
                            var DistributeRatio = data["分配比率"]?.ParseToDecimal(-1);

                            var RefrigerantNoneAdd = new RefrigerantNone()
                        {
                                BasicId = BasicId,
                                FactoryName = _MyDbContext.BasicdataFactoryaddresses.FirstOrDefault(a => a.BasicId == BasicId && a.Name == FactoryName) != null ? _MyDbContext.BasicdataFactoryaddresses.FirstOrDefault(a => a.BasicId == BasicId && a.Name == FactoryName).Id.ToString() : null,
                                EquipmentName = EquipmentName,
                                EquipmentType = _MyDbContext.Selectdata.FirstOrDefault(a => a.Type == "EquipmentType" && a.Name == EquipmentType) != null ? _MyDbContext.Selectdata.FirstOrDefault(a => a.Type == "EquipmentType" && a.Name == EquipmentType).Code.ToString() : null,
                                EquipmentLocation = EquipmentLocation,
                                RefrigerantType = _MyDbContext.Coefficients.FirstOrDefault(a => a.Type == "RefrigerantName" && a.Name == RefrigerantType) != null ? _MyDbContext.Coefficients.FirstOrDefault(a => a.Type == "RefrigerantName" && a.Name == RefrigerantType).Id.ToString() : null,
                                MachineQuantity = MachineQuantity,
                                Manufacturers = Manufacturers,
                                Management = Management,
                                Principal = Principal,
                                Remark = Remark,
                                RefrigerantWeight = RefrigerantWeight,
                                Unit = "10",//selectdata_公斤
                                DistributeRatio = DistributeRatio

                            };
                        var RefrigerantNone = _MyDbContext.RefrigerantNones.Add(RefrigerantNoneAdd);
                        _MyDbContext.SaveChanges(); // 儲存變更至資料庫
                    }

                }

                foreach (var error in errorList)
                {
                    result.Add($"<span style='color:#f00'>{error}</span>");
                }
                result.Add($"{DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")} {file.FileName} 資料匯入結束");
                result.Add($"{DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")} ---------所有匯入資料結束---------");
                if (errorList.Count > 0)
                {
                    result.Add($"{DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")} 資料有錯誤 請修正過資料再重新匯入");
                }
                else
                {
                    result.Add($"{DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")} 資料正確 匯入成功 共匯入{RefrigerantNoneJsonData!.Count}筆");
                }

                System.IO.File.Delete(fileDir + fileName);

                return Sucess(result);
            }
            catch (Exception ex)
            {
                System.IO.File.Delete(fileDir + fileName);

                var Log = new Log()
                {
                    WhereFunction = "OrganizeJSON_RefrigerantNoneImportJSON_post",
                    Exception = ex.ToString(),
                    DateTime = DateTime.Now,
                };
                _MyDbContext.Logs.Add(Log);
                _MyDbContext.SaveChanges();
                Log = null;
                if (ex.Message.Contains("does not belong to table"))
                {
                    return Error("請重新下載範本");
                }
                if (ex.Message.Contains("逗號數量不同"))
                {
                    return Error(ex.Message);
                }
                return Error("內部伺服器錯誤"); // 或其他適當的錯誤訊息
            }
        }

        public ActionResult RefrigerantHaveImport(int BasicId)
        {
            try
            {
                ViewBag.BasicId = BasicId;
                return View();

            }
            catch (Exception ex)
            {
                var Log = new Log()
                {
                    WhereFunction = "OrganizeJSON_RefrigerantHaveImportJSON_get",
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
        public ActionResult RefrigerantHaveImport(IFormFile file, int BasicId)

        {
            var fileName = DateTime.Now.ToString("yyyyMMddHHmmss") + file.FileName;//給予檔案不重複名稱
            string fileDir = FileSave.Path + "/JSON";//檔案路徑

            try
            {
                var a = _MyDbContext.Basicdatas.Where(a => a.BasicId == BasicId).FirstOrDefault();//基本資料
                List<string> result = new List<string>();//顯示資料

                string filePath = FileSave.Path + "/JSON" + fileName;//檔案路徑
                if (!Directory.Exists(fileDir))//沒資料夾則新建
                {
                    Directory.CreateDirectory(fileDir);
                }
                using (var stream = new FileStream(fileDir + fileName, FileMode.Create))
                {
                    file.CopyTo(stream);//新增檔案
                }

                result.Add($"{DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")} 檔案已上傳成功");
                result.Add($"{DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")} ---------開始匯入資料---------");
                result.Add($"{DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")} 資料成功讀入");
                result.Add($"{DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")} ---------開始匯入資料開始---------");
                result.Add($"{DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")} {file.FileName} 資料匯入開始");

                JArray RefrigerantHaveJsonData;
                try
                {
                    RefrigerantHaveJsonData = JSONImport.JSONImports(filePath);
                }
                catch (Exception)
                {
                    return Error("Json格式錯誤"); // 或其他適當的錯誤訊息
                }


                List<string> errorList = new List<string>();
                if (RefrigerantHaveJsonData == null)
                {
                    errorList.Add($"{DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")} {file.FileName} 資料為空");
                }
                else
                {
                    int index = 0;
                    //判斷是否有錯誤
                    foreach (var data in RefrigerantHaveJsonData)
                    {
                        var FactoryName = data["場域"]?.ParseToString();
                        var EquipmentName = data["設備名稱"]?.ParseToString();
                        var EquipmentType = data["設備類型"]?.ParseToString();
                        var RefrigerantType = data["冷媒種類"]?.ParseToString();
                        var Management = data["管理單位"]?.ParseToString();
                        var Principal = data["負責人員"]?.ParseToString();
                        var Datasource = data["數據來源"]?.ParseToString();
                        var Remark = data["備註"]?.ParseToString();
                        var Total = data["填充量"]?.ParseToDecimal(-1);
                        var TotalCondition = data["填充量"]?.ParseToDecimal();//填充量條件
                        var Unit = data["單位"]?.ParseToString();
                        var DistributeRatio = data["分配比率"]?.ParseToDecimal(-1);
                        var DistributeRatioCondition = data["分配比率"]?.ParseToDecimal();//分配比率條件


                        if (!_MyDbContext.BasicdataFactoryaddresses.Any(a => a.BasicId == BasicId && a.Name == FactoryName))
                        {
                            errorList.Add($"{DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")} {file.FileName} 第{index + 1}行失敗，原因(場域不存在)");
                        }
                        if (!_MyDbContext.Selectdata.Any(a => a.Name == EquipmentType))
                        {
                            errorList.Add($"{DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")} {file.FileName} 第{index + 1}行失敗，原因(設備類型不存在)");
                        }
                        if (!_MyDbContext.Coefficients.Any(a => a.Name == RefrigerantType && a.Type == "RefrigerantName"))
                        {
                            errorList.Add($"{DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")} {file.FileName} 第{index + 1}行失敗，原因(冷媒種類不存在)");
                        }
                        if (Total == -1 || TotalCondition < 0)
                        {
                            errorList.Add($"{DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")} {file.FileName} 第{index + 1}行失敗，原因(填充量須為大於0的數)");
                        }
                        if (DistributeRatio == -1)
                        {
                            errorList.Add($"{DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")} {file.FileName} 第{index + 1}行失敗，原因(分配比率填寫不正確)");
                        }
                        if (Management == "")
                        {
                            errorList.Add($"{DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")} {file.FileName} 第{index + 1}行失敗，原因(管理單位為必填項目)");
                        }
                        if (Principal == "")
                        {
                            errorList.Add($"{DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")} {file.FileName} 第{index + 1}行失敗，原因(負責人員為必填項目)");
                        }
                        if (Datasource == "")
                        {
                            errorList.Add($"{DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")} {file.FileName} 第{index + 1}行失敗，原因(數據來源為必填項目)");
                        }                        
                        if (Unit != "公斤(kg)")
                        {
                            errorList.Add($"{DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")} {file.FileName} 第{index + 1}行失敗，原因(單位須為公斤(kg))");
                        }
                        if (DistributeRatio == -1 || DistributeRatio == null || DistributeRatioCondition < 0 || DistributeRatioCondition > 100)
                        {
                            errorList.Add($"{DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")} {file.FileName} 第{index + 1}行失敗，原因(分配比率應介於0-100之間)");
                        }
                    }
                }
                //沒錯誤則新增資料
                if (errorList.Count == 0)
                {
                    foreach (var data in RefrigerantHaveJsonData!)
                    {
                        var FactoryName = data["場域"]?.ParseToString();
                        var EquipmentName = data["設備名稱"]?.ParseToString();
                        var EquipmentType = data["設備類型"]?.ParseToString();
                        var RefrigerantType = data["冷媒種類"]?.ParseToString();
                        var Management = data["管理單位"]?.ParseToString();
                        var Principal = data["負責人員"]?.ParseToString();
                        var Datasource = data["數據來源"]?.ParseToString();
                        var Remark = data["備註"]?.ParseToString();
                        var Total = data["填充量"]?.ParseToDecimal(-1);
                        var Unit = data["單位"]?.ParseToString();
                        var DistributeRatio = data["分配比率"]?.ParseToDecimal(-1);

                        var RefrigerantHaveAdd = new RefrigerantHave()
                        {
                            BasicId = BasicId,
                            FactoryName = _MyDbContext.BasicdataFactoryaddresses.FirstOrDefault(a => a.BasicId == BasicId && a.Name == FactoryName) != null ? _MyDbContext.BasicdataFactoryaddresses.FirstOrDefault(a => a.BasicId == BasicId && a.Name == FactoryName).Id.ToString() : null,
                            EquipmentName = EquipmentName,
                            EquipmentType = _MyDbContext.Selectdata.FirstOrDefault(a => a.Type == "EquipmentType" && a.Name == EquipmentType) != null ? _MyDbContext.Selectdata.FirstOrDefault(a => a.Type == "EquipmentType" && a.Name == EquipmentType).Code.ToString() : null,
                            RefrigerantType = _MyDbContext.Coefficients.FirstOrDefault(a => a.Type == "RefrigerantName" && a.Name == RefrigerantType) != null ? _MyDbContext.Coefficients.FirstOrDefault(a => a.Type == "RefrigerantName" && a.Name == RefrigerantType).Id.ToString() : null,
                            Management = Management,
                            Principal = Principal,
                            Datasource = Datasource,
                            Remark = Remark,
                            Total = Total,
                            Unit = "10",//selectdata_公斤
                            DistributeRatio = DistributeRatio

                        };
                        var RefrigerantHave = _MyDbContext.RefrigerantHaves.Add(RefrigerantHaveAdd);
                        _MyDbContext.SaveChanges(); // 儲存變更至資料庫
                    }

                }

                foreach (var error in errorList)
                {
                    result.Add($"<span style='color:#f00'>{error}</span>");
                }
                result.Add($"{DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")} {file.FileName} 資料匯入結束");
                result.Add($"{DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")} ---------所有匯入資料結束---------");
                if (errorList.Count > 0)
                {
                    result.Add($"{DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")} 資料有錯誤 請修正過資料再重新匯入");
                }
                else
                {
                    result.Add($"{DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")} 資料正確 匯入成功 共匯入{RefrigerantHaveJsonData!.Count}筆");
                }

                System.IO.File.Delete(fileDir + fileName);

                return Sucess(result);
            }
            catch (Exception ex)
            {
                System.IO.File.Delete(fileDir + fileName);

                var Log = new Log()
                {
                    WhereFunction = "OrganizeJSON_RefrigerantHaveImportJSON_post",
                    Exception = ex.ToString(),
                    DateTime = DateTime.Now,
                };
                _MyDbContext.Logs.Add(Log);
                _MyDbContext.SaveChanges();
                Log = null;
                if (ex.Message.Contains("does not belong to table"))
                {
                    return Error("請重新下載範本");
                }
                if (ex.Message.Contains("逗號數量不同"))
                {
                    return Error(ex.Message);
                }
                return Error("內部伺服器錯誤"); // 或其他適當的錯誤訊息
            }
        }

        public ActionResult FireequipmentImport(int BasicId)
        {
            try
            {
                ViewBag.BasicId = BasicId;
                return View();

            }
            catch (Exception ex)
            {
                var Log = new Log()
                {
                    WhereFunction = "OrganizeJSON_FireequipmentImportJSON_get",
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
        public ActionResult FireequipmentImport(IFormFile file, int BasicId)

        {
            var fileName = DateTime.Now.ToString("yyyyMMddHHmmss") + file.FileName;//給予檔案不重複名稱
            string fileDir = FileSave.Path + "/JSON";//檔案路徑

            try
            {
                var a = _MyDbContext.Basicdatas.Where(a => a.BasicId == BasicId).FirstOrDefault();//基本資料
                List<string> result = new List<string>();//顯示資料

                string filePath = FileSave.Path + "/JSON" + fileName;//檔案路徑
                if (!Directory.Exists(fileDir))//沒資料夾則新建
                {
                    Directory.CreateDirectory(fileDir);
                }
                using (var stream = new FileStream(fileDir + fileName, FileMode.Create))
                {
                    file.CopyTo(stream);//新增檔案
                }

                result.Add($"{DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")} 檔案已上傳成功");
                result.Add($"{DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")} ---------開始匯入資料---------");
                result.Add($"{DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")} 資料成功讀入");
                result.Add($"{DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")} ---------開始匯入資料開始---------");
                result.Add($"{DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")} {file.FileName} 資料匯入開始");

                JArray FireequipmentJsonData;
                try
                {
                    FireequipmentJsonData = JSONImport.JSONImports(filePath);
                }
                catch (Exception)
                {
                    return Error("Json格式錯誤"); // 或其他適當的錯誤訊息
                }

                List<string> errorList = new List<string>();
                if (FireequipmentJsonData == null)
                {
                    errorList.Add($"{DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")} {file.FileName} 資料為空");
                }
                else
                {
                    int index = 0;
                    //判斷是否有錯誤
                    foreach (var data in FireequipmentJsonData)
                    {
                        var FactoryName = data["場域"]?.ParseToString();
                        var EquipmentName = data["設備名稱"]?.ParseToString();
                        var Ghgtype = data["溫室氣體種類"]?.ParseToString();

                        var Remark = data["備註"]?.ParseToString();

                        var Management = data["管理單位"]?.ParseToString();
                        var Principal = data["負責人員"]?.ParseToString();
                        var Datasource = data["數據來源"]?.ParseToString();

                        var Unit = data["單位"]?.ParseToString();
                        var DistributeRatio = data["分配比率"]?.ParseToDecimal(-1);
                        var DistributeRatioCondition = data["分配比率"]?.ParseToDecimal(); //分配比率條件


                        if (!_MyDbContext.BasicdataFactoryaddresses.Any(a => a.BasicId == BasicId && a.Name == FactoryName))
                        {
                            errorList.Add($"{DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")} {file.FileName} 第{index + 1}行失敗，原因(場域不存在)");
                        }
                        if (!_MyDbContext.Coefficients.Any(a => a.Name == EquipmentName && a.Type == "FireEquipmentName"))
                        {
                            errorList.Add($"{DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")} {file.FileName} 第{index + 1}行失敗，原因(設備名稱不存在)");
                        }
                        if (!_MyDbContext.Selectdata.Any(a => a.Name == Ghgtype && a.Type == "GHGtype"))
                        {
                            errorList.Add($"{DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")} {file.FileName} 第{index + 1}行失敗，原因(溫室氣體種類不存在)");
                        }
                        if (!_MyDbContext.Selectdata.Where(a => a.Code == "10").Select(a => a.Name).Contains(Unit))
                        {
                            errorList.Add($"{DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")} {file.FileName} 第{index + 1}行失敗，原因(單位須為:公斤(kg)");
                        }
                        if (DistributeRatio == -1 || DistributeRatio == null || DistributeRatioCondition < 0 || DistributeRatioCondition > 100)
                        {
                            errorList.Add($"{DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")} {file.FileName} 第{index + 1}行失敗，原因(分配比率應介於0-100之間)");
                        }
                        if (Management == "")
                        {
                            errorList.Add($"{DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")} {file.FileName} 第{index + 1}行失敗，原因(管理單位為必填項目)");
                        }
                        if (Principal == "")
                        {
                            errorList.Add($"{DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")} {file.FileName} 第{index + 1}行失敗，原因(負責人員為必填項目)");
                        }
                        if (Datasource == "")
                        {
                            errorList.Add($"{DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")} {file.FileName} 第{index + 1}行失敗，原因(數據來源為必填項目)");
                        }

                        int monthcount = (a.EndTime.Year - a.StartTime.Year) * 12 + a.EndTime.Month - a.StartTime.Month + 1;//相差多少月份
                        int year, month;
                        year = a.StartTime.Year - 1911;//系統從盤查區間自動帶出年
                        month = a.StartTime.Month;//系統自動帶出樂

                        for (int j = 0; j < monthcount; j++)
                        {
                            var DateTimes = year + " / " + month;
                            var Num = data[year + "/" + month + "數值"]?.ParseToDecimal(-1);
                            var NumCondition = data[year + "/" + month + "數值"].ParseToDecimal();//數值條件

                            if (Num == -1)
                            {
                                errorList.Add($"{DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")} {file.FileName} 第{index + 1}行失敗，原因(" + year + "/" + month + "數值" + "填寫不正確)");
                            }
                            if (NumCondition < 0)
                            {
                                errorList.Add($"{DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")} {file.FileName} 第{index + 1}行失敗，原因(" + year + "/" + month + "數值" + "不可為負數)");
                            }

                            month++;
                            if (month == 13)
                            {
                                month = 1;
                                year++;
                            }
                        }
                        index++;
                    }
                }

                //沒錯誤則新增資料
                if (errorList.Count == 0)
                {
                    foreach (var data in FireequipmentJsonData!)
                    {
                        var FactoryName = data["場域"]?.ParseToString();
                        var EquipmentName = data["設備名稱"]?.ParseToString();
                        var Ghgtype = data["溫室氣體種類"]?.ParseToString();

                        var Remark = data["備註"]?.ParseToString();

                        var Management = data["管理單位"]?.ParseToString();
                        var Principal = data["負責人員"]?.ParseToString();
                        var Datasource = data["數據來源"]?.ParseToString();

                        var Unit = data["單位"]?.ParseToString();
                        var DistributeRatio = data["分配比率"]?.ParseToDecimal(-1);

                        var Fireequipment = new Fireequipment()
                        {
                            BasicId = BasicId,
                            FactoryName = _MyDbContext.BasicdataFactoryaddresses.FirstOrDefault(a => a.BasicId == BasicId && a.Name == FactoryName) != null ? _MyDbContext.BasicdataFactoryaddresses.FirstOrDefault(a => a.BasicId == BasicId && a.Name == FactoryName).Id.ToString() : null,
                            EquipmentName = _MyDbContext.Coefficients.FirstOrDefault(a => a.Type == "FireEquipmentName" && a.Name == EquipmentName) != null ? _MyDbContext.Coefficients.FirstOrDefault(a => a.Type == "FireEquipmentName" && a.Name == EquipmentName).Id.ToString() : null,
                            Ghgtype = _MyDbContext.Selectdata.FirstOrDefault(a => a.Type == "GHGType" && a.Name == Ghgtype) != null ? _MyDbContext.Selectdata.FirstOrDefault(a => a.Type == "GHGType" && a.Name == Ghgtype).Code.ToString() : null,
                            Remark = Remark,
                            BeforeUnit = _MyDbContext.Selectdata.FirstOrDefault(a => a.Type == "Unit" && a.Name == Unit) != null ? _MyDbContext.Selectdata.FirstOrDefault(a => a.Type == "Unit" && a.Name == Unit).Code.ToString() : null,
                            DistributeRatio = DistributeRatio

                        };
                        var Energy = _MyDbContext.Fireequipments.Add(Fireequipment);

                        _MyDbContext.SaveChanges(); // 儲存變更至資料庫
                        int FireequipmentId = Energy.Entity.Id;//該筆資料的Id
                                               
                        var DDatasource = new DDatasource()
                        {
                            BindId = FireequipmentId,
                            Management = Management,
                            Principal = Principal,
                            Datasource = Datasource,
                            BindWhere = "Fireequipment",

                        };
                        _MyDbContext.DDatasources.Add(DDatasource);
                        DIntervalusetotal DIntervalusetotal;
                        DIntervalusetotal = new DIntervalusetotal()
                        {
                            BindId = FireequipmentId,
                            Num = _MyDbContext.Selectdata.FirstOrDefault(a => a.Type == "Unit" && a.Name == Unit) != null ? _MyDbContext.Selectdata.FirstOrDefault(a => a.Type == "Unit" && a.Name == Unit).Code.ToString() : null,
                            Type = "Unit",
                            BindWhere = "Fireequipment",
                            ArraySort = 0,

                        };
                        _MyDbContext.DIntervalusetotals.Add(DIntervalusetotal);
                        int monthcount = (a.EndTime.Year - a.StartTime.Year) * 12 + a.EndTime.Month - a.StartTime.Month + 1;//相差多少月份
                        int year, month;
                        year = a.StartTime.Year - 1911;//系統從盤查區間自動帶出年
                        month = a.StartTime.Month;//系統自動帶出樂
                        decimal? total = 0;

                        for (int j = 0; j < monthcount; j++)
                        {

                            var DateTimes = year + " / " + month;
                            var Num = data[year + "/" + month + "數值"]?.ParseToDecimal(-1);
                            DIntervalusetotal = new DIntervalusetotal()
                            {
                                BindId = FireequipmentId,
                                Num = DateTimes,
                                Type = "DateTime",
                                BindWhere = "Fireequipment",
                                ArraySort = j,
                            };
                            _MyDbContext.DIntervalusetotals.Add(DIntervalusetotal);

                            DIntervalusetotal = new DIntervalusetotal()
                            {
                                BindId = FireequipmentId,
                                Num = Num.ToString(),
                                Type = "Num",
                                BindWhere = "Fireequipment",
                                ArraySort = j,

                            };
                            _MyDbContext.DIntervalusetotals.Add(DIntervalusetotal);

                            if (Num.ToString() != "")
                            {
                                total += Num;
                            }

                            month++;
                            if (month == 13)
                            {
                                month = 1;
                                year++;
                            }
                        }

                        var Fireequipments = _MyDbContext.Fireequipments.Find(FireequipmentId);
                        Fireequipments.BeforeTotal = Math.Round(Convert.ToDecimal(total), 2);

                        if (Fireequipments.ConvertNum != null)
                        {
                            Fireequipments.AfertTotal = total * Fireequipments.ConvertNum;
                        }
                        else
                        {
                            Fireequipments.AfertTotal = Math.Round(Convert.ToDecimal(total), 2);
                        }

                        _MyDbContext.SaveChanges(); // 儲存變更至資料庫
                    }

                }

                foreach (var error in errorList)
                {
                    result.Add($"<span style='color:#f00'>{error}</span>");
                }
                result.Add($"{DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")} {file.FileName} 資料匯入結束");
                result.Add($"{DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")} ---------所有匯入資料結束---------");
                if (errorList.Count > 0)
                {
                    result.Add($"{DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")} 資料有錯誤 請修正過資料再重新匯入");
                }
                else
                {
                    result.Add($"{DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")} 資料正確 匯入成功 共匯入{FireequipmentJsonData!.Count}筆");
                }


                System.IO.File.Delete(fileDir + fileName);

                return Sucess(result);
            }
            catch (Exception ex)
            {
                System.IO.File.Delete(fileDir + fileName);

                var Log = new Log()
                {
                    WhereFunction = "OrganizeJSON_FireequipmentImportJSON_post",
                    Exception = ex.ToString(),
                    DateTime = DateTime.Now,
                };
                _MyDbContext.Logs.Add(Log);
                _MyDbContext.SaveChanges();
                Log = null;
                if (ex.Message.Contains("does not belong to table"))
                {
                    return Error("請重新下載範本");
                }
                if (ex.Message.Contains("逗號數量不同"))
                {
                    return Error(ex.Message);
                }
                return Error("內部伺服器錯誤"); // 或其他適當的錯誤訊息
            }
        }

        public ActionResult WorkinghourImport(int BasicId)
        {
            try
            {
                ViewBag.BasicId = BasicId;
                return View();

            }
            catch (Exception ex)
            {
                var Log = new Log()
                {
                    WhereFunction = "OrganizeJSON_WorkinghourImportJSON_get",
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
        public ActionResult WorkinghourImport(IFormFile file, int BasicId)

        {
            var fileName = DateTime.Now.ToString("yyyyMMddHHmmss") + file.FileName;//給予檔案不重複名稱
            string fileDir = FileSave.Path + "/JSON";//檔案路徑

            try
            {
                var a = _MyDbContext.Basicdatas.Where(a => a.BasicId == BasicId).FirstOrDefault();//基本資料
                List<string> result = new List<string>();//顯示資料

                string filePath = FileSave.Path + "/JSON" + fileName;//檔案路徑
                if (!Directory.Exists(fileDir))//沒資料夾則新建
                {
                    Directory.CreateDirectory(fileDir);
                }
                using (var stream = new FileStream(fileDir + fileName, FileMode.Create))
                {
                    file.CopyTo(stream);//新增檔案
                }

                result.Add($"{DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")} 檔案已上傳成功");
                result.Add($"{DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")} ---------開始匯入資料---------");
                result.Add($"{DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")} 資料成功讀入");
                result.Add($"{DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")} ---------開始匯入資料開始---------");
                result.Add($"{DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")} {file.FileName} 資料匯入開始");

                JArray WorkinghourJsonData ;
                try
                {
                    WorkinghourJsonData = JSONImport.JSONImports(filePath);
                }
                catch (Exception)
                {
                    return Error("Json格式錯誤"); // 或其他適當的錯誤訊息
                }
                List<string> errorList = new List<string>();
                if (WorkinghourJsonData == null)
                {
                    errorList.Add($"{DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")} {file.FileName} 資料為空");
                }
                else
                {
                    int index = 0;
                    //判斷是否有錯誤
                    foreach (var data in WorkinghourJsonData)
                    {
                        var FactoryName = data["場域"]?.ParseToString();
                        var Item = data["項目"]?.ParseToString();
                        var TotalWorkHour = data["總工時"]?.ParseToDecimal(-1);
                        var TotalWorkHourCondition = data["總工時"]?.ParseToDecimal();// >=0
                        var Management = data["管理部門"]?.ParseToString();
                        var Principal = data["負責人員"]?.ParseToString();
                        var Datasource = data["數據來源"]?.ParseToString();
                        var DistributeRatio = data["分配比率"]?.ParseToDecimal(-1);

                        if (Item != "正式員工及約聘人員(具勞保)" && Item != "廠商外派人員(不具勞保)")
                        {
                            Item = "0";
                        }


                        if (!_MyDbContext.BasicdataFactoryaddresses.Any(a => a.BasicId == BasicId && a.Name == FactoryName))
                        {
                            errorList.Add($"{DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")} {file.FileName} 第{index + 1}行失敗，原因(場域不存在)");
                        }
                        if (Item == "")
                        {
                            errorList.Add($"{DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")} {file.FileName} 第{index + 1}行失敗，原因(項目為必填)");
                        }
                        if (TotalWorkHour == null && Item == "正式員工及約聘人員(具勞保)")
                        {
                            errorList.Add($"{DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")} {file.FileName} 第{index + 1}行失敗，原因(總工時為必填)");
                        }
                        if (Management == "" && Item == "正式員工及約聘人員(具勞保)")
                        {
                            errorList.Add($"{DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")} {file.FileName} 第{index + 1}行失敗，原因(管理部門為必填)");
                        }
                        if (Principal == "" && Item == "正式員工及約聘人員(具勞保)")
                        {
                            errorList.Add($"{DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")} {file.FileName} 第{index + 1}行失敗，原因(負責人員為必填)");
                        }
                        if (Datasource == "" && Item == "正式員工及約聘人員(具勞保)")
                        {
                            errorList.Add($"{DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")} {file.FileName} 第{index + 1}行失敗，原因(數據來源為必填)");
                        }
                        if (DistributeRatio == null && Item == "正式員工及約聘人員(具勞保)")
                        {
                            errorList.Add($"{DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")} {file.FileName} 第{index + 1}行失敗，原因(分配比率(%)為必填)");
                        }
                        if (Item == "0")
                        {
                            errorList.Add($"{DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")} {file.FileName} 第{index + 1}行失敗，原因(項目不存在)");
                        }
                        if (TotalWorkHour == -1)
                        {
                            errorList.Add($"{DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")} {file.FileName} 第{index + 1}行失敗，原因(總工時填寫須為小數點 2 位以內的數字)");
                        }
                        if ((TotalWorkHourCondition < 0 ) && (Item == "正式員工及約聘人員(具勞保)"  || Item == "廠商外派人員(不具勞保)")) // 總工時要大於0
                        {
                            errorList.Add($"{DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")} {file.FileName} 第{index + 1}行失敗，原因(每日工作時數(小時)/總時數(小時)填寫須在0-12小時內)");
                        }
                        if (DistributeRatio == -1)
                        {
                            errorList.Add($"{DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")} {file.FileName} 第{index + 1}行失敗，原因(分配比例填寫須為小數點 4 位以內的數字)");
                        }
                    }
                }
                //沒錯誤則新增資料
                if (errorList.Count == 0)
                {
                    foreach (var data in WorkinghourJsonData!)
                    {
                        var ItemNum = "";  //驗證項目
                        var FactoryName = data["場域"]?.ParseToString();
                        var Item = data["項目"]?.ParseToString();
                        var TotalWorkHour = data["總工時"]?.ParseToDecimal(-1);
                        var Management = data["管理部門"]?.ParseToString();
                        var Principal = data["負責人員"]?.ParseToString();
                        var Datasource = data["數據來源"]?.ParseToString();
                        var DistributeRatio = data["分配比率"]?.ParseToDecimal(-1);

                        //判斷Item
                        if (Item == "正式員工及約聘人員(具勞保)")
                        {
                            ItemNum = "0";
                        }
                        else if (Item == "廠商外派人員(不具勞保)")
                        {
                            ItemNum = "1";
                        }

                        //更新工時資料
                        var FactoryNameId = _MyDbContext.BasicdataFactoryaddresses.FirstOrDefault(a => a.BasicId == BasicId && a.Name == FactoryName).Id;
                        var WorkinghourUpdate = _MyDbContext.Workinghours.Where(a => a.BasicId == BasicId && a.FactoryId == FactoryNameId && a.Item == ItemNum).FirstOrDefault();
                        WorkinghourUpdate.TotalWorkHour = TotalWorkHour;
                        WorkinghourUpdate.Management = Management;
                        WorkinghourUpdate.Principal = Principal;
                        WorkinghourUpdate.Datasource = Datasource;
                        WorkinghourUpdate.DistributeRatio = DistributeRatio;
                        _MyDbContext.Workinghours.Update(WorkinghourUpdate);
                        _MyDbContext.SaveChanges(); // 儲存變更至資料庫
                    }

                }

                foreach (var error in errorList)
                {
                    result.Add($"<span style='color:#f00'>{error}</span>");
                }
                result.Add($"{DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")} {file.FileName} 資料匯入結束");
                result.Add($"{DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")} ---------所有匯入資料結束---------");
                if (errorList.Count > 0)
                {
                    result.Add($"{DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")} 資料有錯誤 請修正過資料再重新匯入");
                }
                else
                {
                    result.Add($"{DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")} 資料正確 匯入成功 共匯入{WorkinghourJsonData!.Count}筆");
                }

                System.IO.File.Delete(fileDir + fileName);

                return Sucess(result);
            }
            catch (Exception ex)
            {
                System.IO.File.Delete(fileDir + fileName);

                var Log = new Log()
                {
                    WhereFunction = "OrganizeJSON_WorkinghourImportJSON_post",
                    Exception = ex.ToString(),
                    DateTime = DateTime.Now,
                };
                _MyDbContext.Logs.Add(Log);
                _MyDbContext.SaveChanges();
                Log = null;
                if (ex.Message.Contains("does not belong to table"))
                {
                    return Error("請重新下載範本");
                }
                if (ex.Message.Contains("逗號數量不同"))
                {
                    return Error(ex.Message);
                }
                return Error("內部伺服器錯誤"); // 或其他適當的錯誤訊息
            }
        }

        public ActionResult DumptreatmentOutsourcingImport(int BasicId)
        {
            try
            {
                ViewBag.BasicId = BasicId;
                return View();

            }
            catch (Exception ex)
            {
                var Log = new Log()
                {
                    WhereFunction = "OrganizeJSON_DumptreatmentOutsourcingImportJSON_get",
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
        public ActionResult DumptreatmentOutsourcingImport(IFormFile file, int BasicId, int SelMonth)

        {
            var fileName = DateTime.Now.ToString("yyyyMMddHHmmss") + file.FileName;//給予檔案不重複名稱
            string fileDir = FileSave.Path + "/JSON";//檔案路徑

            try
            {
                var a = _MyDbContext.Basicdatas.Where(a => a.BasicId == BasicId).FirstOrDefault();//基本資料
                List<string> result = new List<string>();//顯示資料

                string filePath = FileSave.Path + "/JSON" + fileName;//檔案路徑
                if (!Directory.Exists(fileDir))//沒資料夾則新建
                {
                    Directory.CreateDirectory(fileDir);
                }
                using (var stream = new FileStream(fileDir + fileName, FileMode.Create))
                {
                    file.CopyTo(stream);//新增檔案
                }

                result.Add($"{DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")} 檔案已上傳成功");
                result.Add($"{DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")} ---------開始匯入資料---------");
                result.Add($"{DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")} 資料成功讀入");
                result.Add($"{DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")} ---------開始匯入資料開始---------");
                result.Add($"{DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")} {file.FileName} 資料匯入開始");

                var DumptreatmentOutsourcingJsonData = JSONImport.JSONImports(filePath);
                List<string> errorList = new List<string>();
                if (DumptreatmentOutsourcingJsonData == null)
                {
                    errorList.Add($"{DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")} {file.FileName} 資料為空");
                }
                else
                {
                    int index = 0;
                    //判斷是否有錯誤
                    foreach (var data in DumptreatmentOutsourcingJsonData)
                    {
                        var FactoryName = data["場域"]?.ParseToString();
                        var WasteName = data["廢棄物名稱"]?.ParseToString();
                        var WasteMethod = data["廢棄物處理方式"]?.ParseToString();
                        var CleanerName = data["清運商名稱"]?.ParseToString();
                        var FinalAddress = data["最終處理地址"]?.ParseToString();


                        var Method = data["運輸方式"]?.ParseToString();
                        var StartLocation = data["起點"]?.ParseToString();
                        var EndLocation = data["迄點"]?.ParseToString();
                        var Car = data["車種"]?.ParseToString();
                        var Tonnes = data["噸數"]?.ParseToDecimal();
                        var Fuel = data["燃料"]?.ParseToString();
                        var Land = data["陸運(km)"]?.ParseToDecimal(-1);
                        var LandCondition = data["陸運(km)"]?.ParseToDecimal();//陸運條件
                        var Sea = data["海運(nm)"]?.ParseToDecimal(-1);
                        var SeaCondition = data["海運(nm)"]?.ParseToDecimal();//海運條件
                        var Air = data["空運(km)"]?.ParseToDecimal(-1);
                        var AirCondition = data["空運(km)"]?.ParseToDecimal();//空運條件

                        var Management = data["管理單位"]?.ParseToString();
                        var Principal = data["負責人員"]?.ParseToString();
                        var Datasource = data["數據來源"]?.ParseToString();

                        var Remark = data["備註"]?.ParseToString();
                        var Unit = data["單位"]?.ParseToString();
                        var DistributeRatio = data["分配比率"]?.ParseToDecimal(-1);
                        var DistributeRatioCondition = data["分配比率"]?.ParseToDecimal(-1); //分配比率條件


                        if (!_MyDbContext.BasicdataFactoryaddresses.Any(a => a.BasicId == BasicId && a.Name == FactoryName))
                        {
                            errorList.Add($"{DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")} {file.FileName} 第{index + 1}行失敗，原因(場域不存在)");
                        }
                        if (WasteName == "" || WasteMethod == "")
                        {
                            errorList.Add($"{DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")} {file.FileName} 第{index + 1}行失敗，原因(廢棄物名稱或廢棄物處理方式未選取)");
                        }
                        else if (_MyDbContext.Coefficients.FirstOrDefault(a => a.Name == WasteName && a.WasteMethod == WasteMethod && a.Type == "dumptreatment_outsourcing") == null)
                        {
                            List<string> Methods = _MyDbContext.Coefficients.Where(a => a.Name == WasteName && a.Type == "dumptreatment_outsourcing").Select(a => a.WasteMethod).ToList();
                            string results = string.Join(", ", Methods);
                            errorList.Add($"{DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")} {file.FileName} 第{index + 1}行失敗，原因({WasteName}，請選擇{results})");
                        }
                        if (!_MyDbContext.Selectdata.Any(a => a.Name == Method) && Method != "")
                        {
                            errorList.Add($"{DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")} {file.FileName} 第{index + 1}行失敗，原因(運輸方式不存在)");
                        }
                        if (!_MyDbContext.Selectdata.Any(a => a.Name == Car) && Car != "")
                        {
                            errorList.Add($"{DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")} {file.FileName} 第{index + 1}行失敗，原因(車種不存在)");
                        }
                        if (!_MyDbContext.Selectdata.Where(a => a.Code == "10" || a.Code == "11").Select(a => a.Name).Contains(Unit))
                        {
                            errorList.Add($"{DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")} {file.FileName} 第{index + 1}行失敗，原因(轉換前單位不存在)");
                        }
                        if (Land == -1)
                        {
                            errorList.Add($"{DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")} {file.FileName} 第{index + 1}行失敗，原因(陸運填寫不正確)");
                        }
                        if (LandCondition < 0)
                        {
                            errorList.Add($"{DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")} {file.FileName} 第{index + 1}行失敗，原因(陸運不可小於0)");
                        }
                        if (Sea == -1)
                        {
                            errorList.Add($"{DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")} {file.FileName} 第{index + 1}行失敗，原因(海運填寫不正確)");
                        }
                        if (SeaCondition < 0)
                        {
                            errorList.Add($"{DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")} {file.FileName} 第{index + 1}行失敗，原因(海運不可小於0)");
                        }
                        if (Air == -1)
                        {
                            errorList.Add($"{DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")} {file.FileName} 第{index + 1}行失敗，原因(空運填寫不正確)");
                        }
                        if (AirCondition < 0)
                        {
                            errorList.Add($"{DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")} {file.FileName} 第{index + 1}行失敗，原因(空運不可小於0)");
                        }
                        if (DistributeRatio == -1 || DistributeRatio == null || DistributeRatioCondition < 0 || DistributeRatioCondition > 100)
                        {
                            errorList.Add($"{DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")} {file.FileName} 第{index + 1}行失敗，原因(分配比率應介於0-100之間)");
                        }
                        if (Management == "")
                        {
                            errorList.Add($"{DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")} {file.FileName} 第{index + 1}行失敗，原因(管理單位為必填項目)");
                        }
                        if (Principal == "")
                        {
                            errorList.Add($"{DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")} {file.FileName} 第{index + 1}行失敗，原因(負責人員為必填項目)");
                        }
                        if (Datasource == "")
                        {
                            errorList.Add($"{DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")} {file.FileName} 第{index + 1}行失敗，原因(數據來源為必填項目)");
                        }

                        int monthcount = (a.EndTime.Year - a.StartTime.Year) * 12 + a.EndTime.Month - a.StartTime.Month + 1;//相差多少月份
                        int year, month;
                        if (a.StartTime.Month - SelMonth <= 0)
                        {
                            year = a.StartTime.Year - 1 - 1911;//系統從盤查區間自動帶出年
                            month = Math.Abs(a.StartTime.Month - SelMonth + 12);//系統自動帶出樂
                        }
                        else
                        {
                            year = a.StartTime.Year - 1911;//系統從盤查區間自動帶出年
                            month = a.StartTime.Month - SelMonth;//系統自動帶出樂
                        }

                        for (int j = 0; j < monthcount + 1; j += SelMonth)
                        {

                            var StartTime = data[year + "/" + month + "起始日期"].ParseToTWDate("1");
                            var EndTime = data[year + "/" + month + "結束日期"].ParseToTWDate("1");
                            var Num = data[year + "/" + month + "數值"].ParseToDecimal(-1);
                            var NumCondition = data[year + "/" + month + "數值"].ParseToDecimal();//數值條件

                            if (StartTime == "1")
                            {
                                errorList.Add($"{DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")} {file.FileName} 第{index + 1}行失敗，原因(" + year + "/" + month + "起始日期" + "填寫不正確)");
                            }
                            if (EndTime == "1")
                            {
                                errorList.Add($"{DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")} {file.FileName} 第{index + 1}行失敗，原因(" + year + "/" + month + "結束日期" + "填寫不正確)");
                            }
                            if (Num == -1)
                            {
                                errorList.Add($"{DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")} {file.FileName} 第{index + 1}行失敗，原因(" + year + "/" + month + "數值" + "填寫不正確)");
                            }
                            if (NumCondition < 0)
                            {
                                errorList.Add($"{DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")} {file.FileName} 第{index + 1}行失敗，原因(" + year + "/" + month + "數值" + "不可為負數)");
                            }
                            if (Num != null && (StartTime == "" || EndTime == ""))
                            {
                                errorList.Add($"{DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")} {file.FileName} 第{index + 1}行失敗，原因(" + year + "/" + month + "缺少起始或結束時間)");
                            }

                            month += SelMonth;
                            if (month > 12)
                            {
                                month -= 12;
                                year++;
                            }
                        }
                        index++;
                    }
                }

                //沒錯誤則新增資料
                if (errorList.Count == 0)
                {
                    foreach (var data in DumptreatmentOutsourcingJsonData!)
                    {
                        var FactoryName = data["場域"]?.ParseToString();
                        var WasteName = data["廢棄物名稱"]?.ParseToString();
                        var WasteMethod = data["廢棄物處理方式"]?.ParseToString();
                        var CleanerName = data["清運商名稱"]?.ParseToString();
                        var FinalAddress = data["最終處理地址"]?.ParseToString();


                        var Method = data["運輸方式"]?.ParseToString();
                        var StartLocation = data["起點"]?.ParseToString();
                        var EndLocation = data["迄點"]?.ParseToString();
                        var Car = data["車種"]?.ParseToString();
                        var Tonnes = data["噸數"]?.ParseToDecimal();
                        var Fuel = data["燃料"]?.ParseToString();
                        var Land = data["陸運(km)"]?.ParseToDecimal(-1);
                        var Sea = data["海運(nm)"]?.ParseToDecimal(-1);
                        var Air = data["空運(km)"]?.ParseToDecimal(-1);

                        var Management = data["管理單位"]?.ParseToString();
                        var Principal = data["負責人員"]?.ParseToString();
                        var Datasource = data["數據來源"]?.ParseToString();

                        var Remark = data["備註"]?.ParseToString();
                        var Unit = data["單位"]?.ParseToString();
                        var DistributeRatio = data["分配比率"]?.ParseToDecimal(-1);

                        var DumptreatmentOutsourcing = new DumptreatmentOutsourcing()
                        {
                            BasicId = BasicId,
                            FactoryName = _MyDbContext.BasicdataFactoryaddresses.FirstOrDefault(a => a.BasicId == BasicId && a.Name == FactoryName) != null ? _MyDbContext.BasicdataFactoryaddresses.FirstOrDefault(a => a.BasicId == BasicId && a.Name == FactoryName).Id.ToString() : null,
                            WasteId = _MyDbContext.Coefficients.FirstOrDefault(a => a.Name == WasteName && a.WasteMethod == WasteMethod && a.Type == "dumptreatment_outsourcing") != null ? _MyDbContext.Coefficients.FirstOrDefault(a => a.Name == WasteName && a.WasteMethod == WasteMethod && a.Type == "dumptreatment_outsourcing").Id : null,
                            CleanerName = CleanerName,
                            FinalAddress = FinalAddress,
                            Remark = Remark,
                            BeforeUnit = _MyDbContext.Selectdata.FirstOrDefault(a => a.Type == "Unit" && a.Name == Unit) != null ? _MyDbContext.Selectdata.FirstOrDefault(a => a.Type == "Unit" && a.Name == Unit).Code.ToString() : null,
                            DistributeRatio = DistributeRatio

                        };
                        var Outsourcing = _MyDbContext.DumptreatmentOutsourcings.Add(DumptreatmentOutsourcing);

                        _MyDbContext.SaveChanges(); // 儲存變更至資料庫
                        int DumptreatmentOutsourcingId = Outsourcing.Entity.Id;//該筆資料的Id

                        var DTransportation = new DTransportation()
                        {
                            BindId = DumptreatmentOutsourcingId,
                            Method = _MyDbContext.Selectdata.FirstOrDefault(a => a.Type == "Transportation" && a.Name == Method) != null ? _MyDbContext.Selectdata.FirstOrDefault(a => a.Type == "Transportation" && a.Name == Method).Code.ToString() : null,
                            StartLocation = StartLocation,
                            EndLocation = EndLocation,
                            Car = _MyDbContext.Selectdata.FirstOrDefault(a => a.Type == "Car" && a.Name == Car) != null ? _MyDbContext.Selectdata.FirstOrDefault(a => a.Type == "Car" && a.Name == Car).Code.ToString() : null,
                            Tonnes = Tonnes,
                            Fuel = Fuel,
                            Land = Land,
                            Sea = Sea,
                            Air = Air,
                            BindWhere = "DumptreatmentOutsourcing",

                        };
                        _MyDbContext.DTransportations.Add(DTransportation);
                        var DDatasource = new DDatasource()
                        {
                            BindId = DumptreatmentOutsourcingId,
                            Management = Management,
                            Principal = Principal,
                            Datasource = Datasource,
                            BindWhere = "DumptreatmentOutsourcing",

                        };
                        _MyDbContext.DDatasources.Add(DDatasource);
                        DIntervalusetotal DIntervalusetotal;
                        DIntervalusetotal = new DIntervalusetotal()
                        {
                            BindId = DumptreatmentOutsourcingId,
                            Num = _MyDbContext.Selectdata.FirstOrDefault(a => a.Type == "Unit" && a.Name == Unit) != null ? _MyDbContext.Selectdata.FirstOrDefault(a => a.Type == "Unit" && a.Name == Unit).Code.ToString() : null,
                            Type = "Unit",
                            BindWhere = "DumptreatmentOutsourcing",
                            ArraySort = 0,

                        };
                        _MyDbContext.DIntervalusetotals.Add(DIntervalusetotal);
                        string SelMonthNum = "";
                        if (SelMonth == 1)
                        {
                            SelMonthNum = "OneMonth";
                        }
                        else if (SelMonth == 2)
                        {
                            SelMonthNum = "TwoMonth";

                        }
                        DIntervalusetotal = new DIntervalusetotal()
                        {
                            BindId = DumptreatmentOutsourcingId,
                            BindWhere = "DumptreatmentOutsourcing",
                            Num = SelMonthNum,
                            Type = "SelMonth",
                        };
                        //新增單位
                        _MyDbContext.DIntervalusetotals.Add(DIntervalusetotal);
                        int monthcount = (a.EndTime.Year - a.StartTime.Year) * 12 + a.EndTime.Month - a.StartTime.Month + 1;//相差多少月份
                        int year, month;
                        if (a.StartTime.Month - SelMonth <= 0)
                        {
                            year = a.StartTime.Year - 1 - 1911;//系統從盤查區間自動帶出年
                            month = Math.Abs(a.StartTime.Month - SelMonth + 12);//系統自動帶出樂
                        }
                        else
                        {
                            year = a.StartTime.Year - 1911;//系統從盤查區間自動帶出年
                            month = a.StartTime.Month - SelMonth;//系統自動帶出樂
                        }
                        decimal? total = 0;

                        for (int j = 0; j < monthcount + 1; j += SelMonth)
                        {

                            var DateTimes = year + " / " + month;
                            var StartTime = data[year + "/" + month + "起始日期"]?.ParseToTWDate("1");
                            var EndTime = data[year + "/" + month + "結束日期"]?.ParseToTWDate("1");
                            var Num = data[year + "/" + month + "數值"]?.ParseToDecimal(-1);

                            DIntervalusetotal = new DIntervalusetotal()
                            {
                                BindId = DumptreatmentOutsourcingId,
                                Num = DateTimes,
                                Type = "DateTime",
                                BindWhere = "DumptreatmentOutsourcing",
                                ArraySort = j,
                            };
                            _MyDbContext.DIntervalusetotals.Add(DIntervalusetotal);
                            DIntervalusetotal = new DIntervalusetotal()
                            {
                                BindId = DumptreatmentOutsourcingId,
                                Num = StartTime,
                                Type = "StartTime",
                                BindWhere = "DumptreatmentOutsourcing",
                                ArraySort = j,

                            };
                            _MyDbContext.DIntervalusetotals.Add(DIntervalusetotal);
                            DIntervalusetotal = new DIntervalusetotal()
                            {
                                BindId = DumptreatmentOutsourcingId,
                                Num = EndTime,
                                Type = "EndTime",
                                BindWhere = "DumptreatmentOutsourcing",
                                ArraySort = j,

                            };
                            _MyDbContext.DIntervalusetotals.Add(DIntervalusetotal);
                            DIntervalusetotal = new DIntervalusetotal()
                            {
                                BindId = DumptreatmentOutsourcingId,
                                Num = Num.ToString(),
                                Type = "Num",
                                BindWhere = "DumptreatmentOutsourcing",
                                ArraySort = j,

                            };
                            _MyDbContext.DIntervalusetotals.Add(DIntervalusetotal);

                            if (j == 0 || j == monthcount)
                            {
                                if (StartTime != "" && EndTime != "" && Num.ToString() != "")
                                {
                                    total += Total(a.StartTime.Year - 1911, a.StartTime.Month, a.EndTime.Year - 1911, a.EndTime.Month, StartTime, EndTime, Num, j, monthcount, SelMonth);//年份計算j和monthcount判斷第一筆及最後一筆需用到
                                }
                            }
                            else
                            {
                                if (Num.ToString() != "")
                                {
                                    total += Num;
                                }



                            }
                            month += SelMonth;
                            if (month > 12)
                            {
                                month -= 12;
                                year++;
                            }
                        }

                        var DumptreatmentOutsourcings = _MyDbContext.DumptreatmentOutsourcings.Find(DumptreatmentOutsourcingId);
                        DumptreatmentOutsourcings.BeforeTotal = Math.Round(Convert.ToDecimal(total), 2);

                        if (DumptreatmentOutsourcings.ConvertNum != null)
                        {
                            DumptreatmentOutsourcings.AfertTotal = total * DumptreatmentOutsourcings.ConvertNum;
                        }
                        else
                        {
                            DumptreatmentOutsourcings.AfertTotal = Math.Round(Convert.ToDecimal(total), 2);
                        }

                        _MyDbContext.SaveChanges(); // 儲存變更至資料庫
                    }

                }

                foreach (var error in errorList)
                {
                    result.Add($"<span style='color:#f00'>{error}</span>");
                }
                result.Add($"{DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")} {file.FileName} 資料匯入結束");
                result.Add($"{DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")} ---------所有匯入資料結束---------");
                if (errorList.Count > 0)
                {
                    result.Add($"{DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")} 資料有錯誤 請修正過資料再重新匯入");
                }
                else
                {
                    result.Add($"{DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")} 資料正確 匯入成功 共匯入{DumptreatmentOutsourcingJsonData!.Count}筆");
                }


                System.IO.File.Delete(fileDir + fileName);

                return Sucess(result);
            }
            catch (Exception ex)
            {
                System.IO.File.Delete(fileDir + fileName);

                var Log = new Log()
                {
                    WhereFunction = "OrganizeJSON_DumptreatmentOutsourcingImportJSON_post",
                    Exception = ex.ToString(),
                    DateTime = DateTime.Now,
                };
                _MyDbContext.Logs.Add(Log);
                _MyDbContext.SaveChanges();
                Log = null;
                if (ex.Message.Contains("does not belong to table"))
                {
                    return Error("請重新下載範本");
                }
                if (ex.Message.Contains("逗號數量不同"))
                {
                    return Error(ex.Message);
                }
                return Error("內部伺服器錯誤"); // 或其他適當的錯誤訊息
            }
        }
    }
}
