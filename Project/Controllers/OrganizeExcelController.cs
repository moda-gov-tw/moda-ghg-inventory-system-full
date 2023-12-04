using Project.Models.View;
using Project.Models;
using Microsoft.AspNetCore.Mvc;
using static Project.Controllers.CommonController;
using Newtonsoft.Json;
using Microsoft.AspNetCore.Http;
using Project.Common;
using NPOI.HPSF;
using Microsoft.EntityFrameworkCore;
using System.Globalization;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Diagnostics.Metrics;
using System.Linq;

namespace Project.Controllers
{
    public class OrganizeExcelController : CommonController
    {
        private MyDbContext _MyDbContext;
        public OrganizeExcelController(MyDbContext MyDbContext)
        {
            _MyDbContext = MyDbContext;
        }
        // GET: OrganizeController/Delete/5
        public ActionResult EnergyUseImportExcel(int BasicId)
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
                    WhereFunction = "OrganizeExcel_EnergyUseImportExcel_get",
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
        public ActionResult EnergyUseImportExcel(IFormFile file, int BasicId,int SelMonth)
        {
            var fileName = DateTime.Now.ToString("yyyyMMddHHmmss") + file.FileName;//給予檔案不重複名稱
            string fileDir = FileSave.Path + "/excel";//檔案路徑

            try
            {
                var a = _MyDbContext.Basicdatas.Where(a => a.BasicId == BasicId).FirstOrDefault();//基本資料
                List<string> result = new List<string>();//顯示資料

                string filePath = FileSave.Path + "/excel" + fileName;//檔案路徑

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

                var dt = NPOIHelper.ImportExcel_Row2And3_Data4(filePath);
                List<string> errorList = new List<string>();
                if (dt.Rows.Count == 0)
                {
                    errorList.Add($"{DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")} {file.FileName} 資料為空");
                }
                //判斷是否有錯誤
                for (int i = 0; i < dt.Rows.Count; i++)
                {
                    var FactoryName = dt.Rows[i]["場域"].ParseToString();
                    var EnergyName = dt.Rows[i]["能源名稱"].ParseToString();
                    var EquipmentName = dt.Rows[i]["錶號或使用設備名稱"].ParseToString();
                    var EquipmentLocation = dt.Rows[i]["錶或設備位置"].ParseToString();
                    var SupplierName = dt.Rows[i]["供應商名稱"].ParseToString();


                    var Method = dt.Rows[i]["運輸方式"].ParseToString();
                    var StartLocation = dt.Rows[i]["起點"].ParseToString();
                    var EndLocation = dt.Rows[i]["迄點"].ParseToString();
                    var Car = dt.Rows[i]["車種"].ParseToString();
                    var Tonnes = dt.Rows[i]["噸數"].ParseToDecimal();
                    var Fuel = dt.Rows[i]["燃料"].ParseToString();
                    var Land = dt.Rows[i]["陸運(km)"].ParseToDecimal(-1);
                    var LandCondition = dt.Rows[i]["陸運(km)"].ParseToDecimal();//陸運條件
                    var Sea = dt.Rows[i]["海運(nm)"].ParseToDecimal(-1);
                    var SeaCondition = dt.Rows[i]["海運(nm)"].ParseToDecimal();//海運條件
                    var Air = dt.Rows[i]["空運(km)"].ParseToDecimal(-1);
                    var AirCondition = dt.Rows[i]["空運(km)"].ParseToDecimal();//空運條件

                    var Management = dt.Rows[i]["管理單位"].ParseToString();
                    var Principal = dt.Rows[i]["負責人員"].ParseToString();
                    var Datasource = dt.Rows[i]["數據來源"].ParseToString();

                    var Remark = dt.Rows[i]["備註"].ParseToString();
                    var Unit = dt.Rows[i]["單位"].ParseToString();
                    var DistributeRatio = dt.Rows[i]["分配比率"].ParseToDecimal(-1);
                    var DistributeRatioCondition = dt.Rows[i]["分配比率"].ParseToDecimal(); //分配比率條件


                    if (!_MyDbContext.BasicdataFactoryaddresses.Any(a => a.BasicId == BasicId && a.Name == FactoryName))
                    {
                        errorList.Add($"{DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")} {file.FileName} 第{i + 4}行失敗，原因(場域不存在)");
                    }
                    if (!_MyDbContext.Coefficients.Any(a => a.Name == EnergyName && a.Type == "EnergyName"))
                    {
                        errorList.Add($"{DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")} {file.FileName} 第{i + 4}行失敗，原因(能源種類不存在)");
                    }
                    if (!_MyDbContext.Suppliermanages.Any(a => a.SupplierName == SupplierName && a.Account == AccountId()))
                    {
                        errorList.Add($"{DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")} {file.FileName} 第{i + 4}行失敗，原因(供應商名稱不存在)");
                    }
                    if (!_MyDbContext.Selectdata.Any(a => a.Name == Method) && Method != "")
                    {
                        errorList.Add($"{DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")} {file.FileName} 第{i + 4}行失敗，原因(運輸方式不存在)");
                    }
                    if (EnergyName == "電力" && Method != "管線") //電力只能為管線
                    {
                        errorList.Add($"{DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")} {file.FileName} 第{i + 4}行失敗，原因(能源名稱為：電力，運輸方式應為：管線)");
                    }
                    if (!_MyDbContext.Selectdata.Any(a => a.Name == Car) && Car != "")
                    {
                        errorList.Add($"{DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")} {file.FileName} 第{i + 4}行失敗，原因(車種不存在)");
                    }
                    if (EnergyName == "電力")
                    {
                        if (! _MyDbContext.Selectdata.Any(a => a.Name == Unit && (a.Code =="25"|| a.Code == "117")))
                        { 
                        errorList.Add($"{DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")} {file.FileName} 第{i + 4}行失敗，原因(電力單位須為:度(kwh),千度(mwh))");
                        }
                    }
                    if (EnergyName == "車用柴油" || EnergyName == "車用汽油" || EnergyName == "柴油" || EnergyName == "汽油" || EnergyName == "液化石油氣")
                    {
                        if (EnergyName == "車用柴油" && !_MyDbContext.Selectdata.Any(a => a.Name == Unit && (a.Code == "14" || a.Code == "15")))
                        {
                            errorList.Add($"{DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")} {file.FileName} 第{i + 4}行失敗，原因(車用柴油須為:公升(L)),公秉(kl))");
                        }
                        else if (EnergyName == "車用汽油" && !_MyDbContext.Selectdata.Any(a => a.Name == Unit && (a.Code == "14" || a.Code == "15")))
                        {
                            errorList.Add($"{DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")} {file.FileName} 第{i + 4}行失敗，原因(車用汽油須為:公升(L)),公秉(kl))");
                        }
                        else if (EnergyName == "柴油" && !_MyDbContext.Selectdata.Any(a => a.Name == Unit && (a.Code == "14" || a.Code == "15")))
                        {
                            errorList.Add($"{DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")} {file.FileName} 第{i + 4}行失敗，原因(柴油須為:公升(L)),公秉(kl))");
                        }
                        else if (EnergyName == "汽油" && !_MyDbContext.Selectdata.Any(a => a.Name == Unit && (a.Code == "14" || a.Code == "15")))
                        {
                            errorList.Add($"{DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")} {file.FileName} 第{i + 4}行失敗，原因(汽油須為:公升(L)),公秉(kl))");
                        }
                        else if (EnergyName == "液化石油氣" && !_MyDbContext.Selectdata.Any(a => a.Name == Unit && (a.Code == "14" || a.Code == "15")))
                        {
                            errorList.Add($"{DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")} {file.FileName} 第{i + 4}行失敗，原因(液化石油氣須為:公升(L)),公秉(kl))");
                        }
                    }
                    if (EnergyName == "天然氣")
                    {
                        if (!_MyDbContext.Selectdata.Any(a => a.Name == Unit && a.Code == "22" ))
                        {
                            errorList.Add($"{DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")} {file.FileName} 第{i + 4}行失敗，原因(天然氣單位須為:立方公尺(m3))");
                        }
                    }
                    if (Land == -1)
                    {
                        errorList.Add($"{DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")} {file.FileName} 第{i + 4}行失敗，原因(陸運填寫不正確)");
                    }
                    if (LandCondition < 0)
                    {
                        errorList.Add($"{DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")} {file.FileName} 第{i + 4}行失敗，原因(陸運不可小於0)");
                    }
                    if (Sea == -1)
                    {
                        errorList.Add($"{DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")} {file.FileName} 第{i + 4}行失敗，原因(海運填寫不正確)");
                    }
                    if (SeaCondition < 0)
                    {
                        errorList.Add($"{DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")} {file.FileName} 第{i + 4}行失敗，原因(海運不可小於0)");
                    }
                    if (Air == -1)
                    {
                        errorList.Add($"{DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")} {file.FileName} 第{i + 4}行失敗，原因(空運填寫不正確)");
                    }
                    if (AirCondition < 0)
                    {
                        errorList.Add($"{DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")} {file.FileName} 第{i + 4}行失敗，原因(空運不可小於0)");
                    }
                    if (DistributeRatio == -1 || DistributeRatio == null || DistributeRatioCondition < 0 || DistributeRatioCondition > 100)
                    {
                        errorList.Add($"{DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")} {file.FileName} 第{i + 4}行失敗，原因(分配比率應介於0-100之間)");
                    }
                    if (Management == "")
                    {
                        errorList.Add($"{DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")} {file.FileName} 第{i + 4}行失敗，原因(管理單位為必填項目)");
                    }
                    if (Principal == "")
                    {
                        errorList.Add($"{DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")} {file.FileName} 第{i + 4}行失敗，原因(負責人員為必填項目)");
                    }
                    if (Datasource == "")
                    {
                        errorList.Add($"{DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")} {file.FileName} 第{i + 4}行失敗，原因(數據來源為必填項目)");
                    }

                    int monthcount = (a.EndTime.Year - a.StartTime.Year) * 12 + a.EndTime.Month - a.StartTime.Month + 1;//相差多少月份
                    int year, month;
                    if (a.StartTime.Month - SelMonth <= 0)
                    {
                        year = a.StartTime.Year - 1 - 1911;//系統從盤查區間自動帶出年
                        month = Math.Abs(a.StartTime.Month - SelMonth +12);//系統自動帶出樂
                    }
                    else
                    {
                        year = a.StartTime.Year - 1911;//系統從盤查區間自動帶出年
                        month = a.StartTime.Month - SelMonth;//系統自動帶出樂
                    }

                    for (int j = 0; j < monthcount + 1; j+= SelMonth)
                    {

                        var StartTime = dt.Rows[i][year + "/" + month + "起始日期"].ParseToTWDate("1");
                        var EndTime = dt.Rows[i][year + "/" + month + "結束日期"].ParseToTWDate("1");
                        var Num = dt.Rows[i][year + "/" + month + "數值"].ParseToDecimal(-1);
                        var NumCondition = dt.Rows[i][year + "/" + month + "數值"].ParseToDecimal();//數值條件

                        if (StartTime == "1")
                        {
                            errorList.Add($"{DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")} {file.FileName} 第{i + 4}行失敗，原因(" + year + "/" + month + "起始日期" + "填寫不正確)");
                        }
                        if (EndTime == "1")
                        {
                            errorList.Add($"{DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")} {file.FileName} 第{i + 4}行失敗，原因(" + year + "/" + month + "結束日期" + "填寫不正確)");
                        }
                        if (Num == -1)
                        {
                            errorList.Add($"{DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")} {file.FileName} 第{i + 4}行失敗，原因(" + year + "/" + month + "數值" + "填寫不正確)");
                        }
                        if (NumCondition < 0)
                        {
                            errorList.Add($"{DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")} {file.FileName} 第{i + 4}行失敗，原因(" + year + "/" + month + "數值" + "不可為負數)");
                        }
                        if (Num != null && (StartTime == "" || EndTime == ""))
                        {
                            errorList.Add($"{DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")} {file.FileName} 第{i + 4}行失敗，原因(" + year + "/" + month + "缺少起始或結束時間)");
                        }

                        month += SelMonth;
                        if (month > 12)
                        {
                            month -= 12;
                            year++;
                        }
                    }
                }
                //沒錯誤則新增資料
                if (errorList.Count == 0)
                {
                    for (int i = 0; i < dt.Rows.Count; i++)
                    {
                        var FactoryName = dt.Rows[i]["場域"].ParseToString();
                        var EnergyName = dt.Rows[i]["能源名稱"].ParseToString();
                        var EquipmentName = dt.Rows[i]["錶號或使用設備名稱"].ParseToString();
                        var EquipmentLocation = dt.Rows[i]["錶或設備位置"].ParseToString();
                        var SupplierName = dt.Rows[i]["供應商名稱"].ParseToString();


                        var Method = dt.Rows[i]["運輸方式"].ParseToString();
                        var StartLocation = dt.Rows[i]["起點"].ParseToString();
                        var EndLocation = dt.Rows[i]["迄點"].ParseToString();
                        var Car = dt.Rows[i]["車種"].ParseToString();
                        var Tonnes = dt.Rows[i]["噸數"].ParseToDecimal();
                        var Fuel = dt.Rows[i]["燃料"].ParseToString();
                        var Land = dt.Rows[i]["陸運(km)"].ParseToDecimal(-1);
                        var Sea = dt.Rows[i]["海運(nm)"].ParseToDecimal(-1);
                        var Air = dt.Rows[i]["空運(km)"].ParseToDecimal(-1);

                        var Management = dt.Rows[i]["管理單位"].ParseToString();
                        var Principal = dt.Rows[i]["負責人員"].ParseToString();
                        var Datasource = dt.Rows[i]["數據來源"].ParseToString();

                        var Remark = dt.Rows[i]["備註"].ParseToString();
                        var Unit = dt.Rows[i]["單位"].ParseToString();
                        var DistributeRatio = dt.Rows[i]["分配比率"].ParseToDecimal(-1);

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
                            DistributeRatio = DistributeRatio

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
                        else if(SelMonth == 2)
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

                        for (int j = 0; j < monthcount + 1; j+= SelMonth)
                        {

                            var DateTimes = year + " / " + month;
                            var StartTime = dt.Rows[i][year + "/" + month + "起始日期"].ParseToTWDate("1");
                            var EndTime = dt.Rows[i][year + "/" + month + "結束日期"].ParseToTWDate("1");
                            var Num = dt.Rows[i][year + "/" + month + "數值"].ParseToDecimal(-1);
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
                        EditLog.EditLog += EnergyUseLog("新增", EnergyName, EquipmentName, EquipmentLocation, Energyuses.AfertTotal,Unit);
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
                    result.Add($"{DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")} 資料正確 匯入成功 共匯入{dt.Rows.Count}筆");
                }


                System.IO.File.Delete(fileDir + fileName);

                return Sucess(result);


            }
            catch (Exception ex)
            {
                System.IO.File.Delete(fileDir + fileName);

                var Log = new Log()
                {
                    WhereFunction = "OrganizeExcel_EnergyUseImportExcel_post",
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
                return Error("內部伺服器錯誤"); // 或其他適當的錯誤訊息
            }
        }

        public ActionResult ResourceUseImportExcel(int BasicId)
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
                    WhereFunction = "OrganizeExcel_ResourceUseImportExcel_get",
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
        public ActionResult ResourceUseImportExcel(IFormFile file, int BasicId, int SelMonth)
        {
            var fileName = DateTime.Now.ToString("yyyyMMddHHmmss") + file.FileName;//給予檔案不重複名稱
            string fileDir = FileSave.Path + "/excel";//檔案路徑

            try
            {
                var a = _MyDbContext.Basicdatas.Where(a => a.BasicId == BasicId).FirstOrDefault();//基本資料
                List<string> result = new List<string>();//顯示資料

                string filePath = FileSave.Path + "/excel" + fileName;//檔案路徑
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

                var ResourceUsedt = NPOIHelper.ImportExcel_Row2And3_Data4(filePath);
                List<string> errorList = new List<string>();
                if (ResourceUsedt.Rows.Count == 0)
                {
                    errorList.Add($"{DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")} {file.FileName} 資料為空");
                }
                //判斷是否有錯誤
                for (int i = 0; i < ResourceUsedt.Rows.Count; i++)
                {
                    var FactoryName = ResourceUsedt.Rows[i]["場域"].ParseToString();
                    var ResourceName = ResourceUsedt.Rows[i]["資源名稱"].ParseToString();
                    var EquipmentName = ResourceUsedt.Rows[i]["錶號或使用設備名稱"].ParseToString();
                    var EquipmentLocation = ResourceUsedt.Rows[i]["錶或設備位置"].ParseToString();
                    var SupplierName = ResourceUsedt.Rows[i]["供應商名稱"].ParseToString();


                    var Method = ResourceUsedt.Rows[i]["運輸方式"].ParseToString();
                    var StartLocation = ResourceUsedt.Rows[i]["起點"].ParseToString();
                    var EndLocation = ResourceUsedt.Rows[i]["迄點"].ParseToString();
                    var Car = ResourceUsedt.Rows[i]["車種"].ParseToString();
                    var Tonnes = ResourceUsedt.Rows[i]["噸數"].ParseToDecimal();
                    var Fuel = ResourceUsedt.Rows[i]["燃料"].ParseToString();
                    var Land = ResourceUsedt.Rows[i]["陸運(km)"].ParseToDecimal(-1);//陸運條件
                    var LandCondition = ResourceUsedt.Rows[i]["陸運(km)"].ParseToDecimal();//陸運條件
                    var Sea = ResourceUsedt.Rows[i]["海運(nm)"].ParseToDecimal(-1);
                    var SeaCondition = ResourceUsedt.Rows[i]["海運(nm)"].ParseToDecimal();//海運條件
                    var Air = ResourceUsedt.Rows[i]["空運(km)"].ParseToDecimal(-1);
                    var AirCondition = ResourceUsedt.Rows[i]["空運(km)"].ParseToDecimal();//空運條件

                    var Management = ResourceUsedt.Rows[i]["管理單位"].ParseToString();
                    var Principal = ResourceUsedt.Rows[i]["負責人員"].ParseToString();
                    var Datasource = ResourceUsedt.Rows[i]["數據來源"].ParseToString();

                    var Remark = ResourceUsedt.Rows[i]["備註"].ParseToString();
                    var Unit = ResourceUsedt.Rows[i]["單位"].ParseToString();
                    var DistributeRatio = ResourceUsedt.Rows[i]["分配比率"].ParseToDecimal(-1);
                    var DistributeRatioCondition = ResourceUsedt.Rows[i]["分配比率"].ParseToDecimal(); //分配比率條件


                    if (!_MyDbContext.BasicdataFactoryaddresses.Any(a => a.BasicId == BasicId && a.Name == FactoryName))
                    {
                        errorList.Add($"{DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")} {file.FileName} 第{i + 4}行失敗，原因(場域不存在)");
                    }
                    if (!_MyDbContext.Coefficients.Any(a => a.Name == ResourceName && a.Type == "ResourceName"))
                    {
                        errorList.Add($"{DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")} {file.FileName} 第{i + 4}行失敗，原因(資源名稱不存在)");
                    }
                    if (!_MyDbContext.Suppliermanages.Any(a => a.SupplierName == SupplierName && a.Account == AccountId()))
                    {
                        errorList.Add($"{DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")} {file.FileName} 第{i + 4}行失敗，原因(供應商名稱不存在)");
                    }
                    if (!_MyDbContext.Selectdata.Any(a => a.Name == Method) && Method != "")
                    {
                        errorList.Add($"{DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")} {file.FileName} 第{i + 4}行失敗，原因(運輸方式不存在)");
                    }
                    if (!_MyDbContext.Selectdata.Any(a => a.Name == Car) && Car != "")
                    {
                        errorList.Add($"{DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")} {file.FileName} 第{i + 4}行失敗，原因(車種不存在)");
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
                        errorList.Add($"{DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")} {file.FileName} 第{i + 4}行失敗，原因(單位須為:{UnitCompare[0]})");
                    }
                    if (Land == -1)
                    {
                        errorList.Add($"{DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")} {file.FileName} 第{i + 4}行失敗，原因(陸運填寫不正確)");
                    }
                    if (LandCondition < 0)
                    {
                        errorList.Add($"{DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")} {file.FileName} 第{i + 4}行失敗，原因(陸運不可小於0)");
                    }
                    if (Sea == -1)
                    {
                        errorList.Add($"{DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")} {file.FileName} 第{i + 4}行失敗，原因(海運填寫不正確)");
                    }
                    if (SeaCondition < 0)
                    {
                        errorList.Add($"{DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")} {file.FileName} 第{i + 4}行失敗，原因(海運不可小於0)");
                    }
                    if (Air == -1)
                    {
                        errorList.Add($"{DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")} {file.FileName} 第{i + 4}行失敗，原因(空運填寫不正確)");
                    }
                    if (AirCondition < 0)
                    {
                        errorList.Add($"{DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")} {file.FileName} 第{i + 4}行失敗，原因(空運不可小於0)");
                    }                    
                    if (DistributeRatio == -1 || DistributeRatio == null || DistributeRatioCondition < 0 || DistributeRatioCondition > 100)
                    {
                        errorList.Add($"{DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")} {file.FileName} 第{i + 4}行失敗，原因(分配比率應介於0-100之間)");
                    }
                    if (Management == "")
                    {
                        errorList.Add($"{DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")} {file.FileName} 第{i + 4}行失敗，原因(管理單位為必填項目)");
                    }
                    if (Principal == "")
                    {
                        errorList.Add($"{DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")} {file.FileName} 第{i + 4}行失敗，原因(負責人員為必填項目)");
                    }
                    if (Datasource == "")
                    {
                        errorList.Add($"{DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")} {file.FileName} 第{i + 4}行失敗，原因(數據來源為必填項目)");
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

                        var StartTime = ResourceUsedt.Rows[i][year + "/" + month + "起始日期"].ParseToTWDate("1");
                        var EndTime = ResourceUsedt.Rows[i][year + "/" + month + "結束日期"].ParseToTWDate("1");
                        var Num = ResourceUsedt.Rows[i][year + "/" + month + "數值"].ParseToDecimal(-1);
                        var NumCondition = ResourceUsedt.Rows[i][year + "/" + month + "數值"].ParseToDecimal();//數值條件

                        if (StartTime == "1")
                        {
                            errorList.Add($"{DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")} {file.FileName} 第{i + 4}行失敗，原因(" + year + "/" + month + "起始日期" + "填寫不正確)");
                        }
                        if (EndTime == "1")
                        {
                            errorList.Add($"{DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")} {file.FileName} 第{i + 4}行失敗，原因(" + year + "/" + month + "結束日期" + "填寫不正確)");
                        }
                        if (Num == -1)
                        {
                            errorList.Add($"{DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")} {file.FileName} 第{i + 4}行失敗，原因(" + year + "/" + month + "數值" + "填寫不正確)");
                        }
                        if (NumCondition < 0)
                        {
                            errorList.Add($"{DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")} {file.FileName} 第{i + 4}行失敗，原因(" + year + "/" + month + "數值" + "不可為負數)");
                        }
                        if (Num != null && (StartTime == "" || EndTime == ""))
                        {
                            errorList.Add($"{DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")} {file.FileName} 第{i + 4}行失敗，原因(" + year + "/" + month + "缺少起始或結束時間)");
                        }

                        month += SelMonth;
                        if (month > 12)
                        {
                            month -= 12;
                            year++;
                        }
                    }

                }
                //沒錯誤則新增資料
                if (errorList.Count == 0)
                {
                    for (int i = 0; i < ResourceUsedt.Rows.Count; i++)
                    {
                        var FactoryName = ResourceUsedt.Rows[i]["場域"].ParseToString();
                        var ResourceName = ResourceUsedt.Rows[i]["資源名稱"].ParseToString();
                        var EquipmentName = ResourceUsedt.Rows[i]["錶號或使用設備名稱"].ParseToString();
                        var EquipmentLocation = ResourceUsedt.Rows[i]["錶或設備位置"].ParseToString();
                        var SupplierName = ResourceUsedt.Rows[i]["供應商名稱"].ParseToString();


                        var Method = ResourceUsedt.Rows[i]["運輸方式"].ParseToString();
                        var StartLocation = ResourceUsedt.Rows[i]["起點"].ParseToString();
                        var EndLocation = ResourceUsedt.Rows[i]["迄點"].ParseToString();
                        var Car = ResourceUsedt.Rows[i]["車種"].ParseToString();
                        var Tonnes = ResourceUsedt.Rows[i]["噸數"].ParseToDecimal();
                        var Fuel = ResourceUsedt.Rows[i]["燃料"].ParseToString();
                        var Land = ResourceUsedt.Rows[i]["陸運(km)"].ParseToDecimal(-1);
                        var Sea = ResourceUsedt.Rows[i]["海運(nm)"].ParseToDecimal(-1);
                        var Air = ResourceUsedt.Rows[i]["空運(km)"].ParseToDecimal(-1);

                        var Management = ResourceUsedt.Rows[i]["管理單位"].ParseToString();
                        var Principal = ResourceUsedt.Rows[i]["負責人員"].ParseToString();
                        var Datasource = ResourceUsedt.Rows[i]["數據來源"].ParseToString();

                        var Remark = ResourceUsedt.Rows[i]["備註"].ParseToString();
                        var Unit = ResourceUsedt.Rows[i]["單位"].ParseToString();
                        var DistributeRatio = ResourceUsedt.Rows[i]["分配比率"].ParseToDecimal(-1);

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
                            DistributeRatio = DistributeRatio

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
                            var StartTime = ResourceUsedt.Rows[i][year + "/" + month + "起始日期"].ParseToTWDate("1");
                            var EndTime = ResourceUsedt.Rows[i][year + "/" + month + "結束日期"].ParseToTWDate("1");
                            var Num = ResourceUsedt.Rows[i][year + "/" + month + "數值"].ParseToDecimal(-1);
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
                    result.Add($"{DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")} 資料正確 匯入成功 共匯入{ResourceUsedt.Rows.Count}筆");
                }


                System.IO.File.Delete(fileDir + fileName);

                return Sucess(result);


            }
            catch (Exception ex)
            {
                System.IO.File.Delete(fileDir + fileName);

                var Log = new Log()
                {
                    WhereFunction = "OrganizeExcel_ResourceUseImportExcel_post",
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
                return Error("內部伺服器錯誤"); // 或其他適當的錯誤訊息
            }
        }

        public ActionResult FireequipmentImportExcel(int BasicId)
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
                    WhereFunction = "OrganizeExcel_FireequipmentImportExcel_get",
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
        public ActionResult FireequipmentImportExcel(IFormFile file, int BasicId)
        {
            var fileName = DateTime.Now.ToString("yyyyMMddHHmmss") + file.FileName;//給予檔案不重複名稱
            string fileDir = FileSave.Path + "/excel";//檔案路徑

            try
            {
                var a = _MyDbContext.Basicdatas.Where(a => a.BasicId == BasicId).FirstOrDefault();//基本資料
                List<string> result = new List<string>();//顯示資料

                string filePath = FileSave.Path + "/excel" + fileName;//檔案路徑
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

                var Fireequipmentdt = NPOIHelper.ImportExcel_Row2And3_Data4(filePath);
                List<string> errorList = new List<string>();
                if (Fireequipmentdt.Rows.Count == 0)
                {
                    errorList.Add($"{DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")} {file.FileName} 資料為空");
                }
                //判斷是否有錯誤
                for (int i = 0; i < Fireequipmentdt.Rows.Count; i++)
                {
                    var FactoryName = Fireequipmentdt.Rows[i]["場域"].ParseToString();
                    var EquipmentName = Fireequipmentdt.Rows[i]["設備名稱"].ParseToString();
                    var Ghgtype = Fireequipmentdt.Rows[i]["溫室氣體種類"].ParseToString();
                    var Remark = Fireequipmentdt.Rows[i]["備註"].ParseToString();
                    var Unit = Fireequipmentdt.Rows[i]["單位"].ParseToString();
                    var DistributeRatio = Fireequipmentdt.Rows[i]["分配比率"].ParseToDecimal(-1);
                    var DistributeRatioCondition = Fireequipmentdt.Rows[i]["分配比率"].ParseToDecimal(); //分配比率條件

                    var Management = Fireequipmentdt.Rows[i]["管理單位"].ParseToString();
                    var Principal = Fireequipmentdt.Rows[i]["負責人員"].ParseToString();
                    var Datasource = Fireequipmentdt.Rows[i]["數據來源"].ParseToString();

                    if (!_MyDbContext.BasicdataFactoryaddresses.Any(a => a.BasicId == BasicId && a.Name == FactoryName))
                    {
                        errorList.Add($"{DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")} {file.FileName} 第{i + 4}行失敗，原因(場域不存在)");
                    }
                    if (!_MyDbContext.Coefficients.Any(a => a.Name == EquipmentName && a.Type == "FireEquipmentName"))
                    {
                        errorList.Add($"{DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")} {file.FileName} 第{i + 4}行失敗，原因(設備名稱不存在)");
                    }
                    if (!_MyDbContext.Selectdata.Any(a => a.Name == Ghgtype && a.Type == "GHGtype"))
                    {
                        errorList.Add($"{DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")} {file.FileName} 第{i + 4}行失敗，原因(溫室氣體種類不存在)");
                    }
                    if (!_MyDbContext.Selectdata.Where(a => a.Code == "10").Select(a => a.Name).Contains(Unit))
                    {
                        errorList.Add($"{DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")} {file.FileName} 第{i + 4}行失敗，原因(單位須為:公斤(kg)");
                    }
                    if (DistributeRatio == -1 || DistributeRatio == null || DistributeRatioCondition < 0 || DistributeRatioCondition > 100)
                    {
                        errorList.Add($"{DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")} {file.FileName} 第{i + 4}行失敗，原因(分配比率應介於0-100之間)");
                    }
                    if (Management == "")
                    {
                        errorList.Add($"{DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")} {file.FileName} 第{i + 4}行失敗，原因(管理單位為必填項目)");
                    }
                    if (Principal == "")
                    {
                        errorList.Add($"{DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")} {file.FileName} 第{i + 4}行失敗，原因(負責人員為必填項目)");
                    }
                    if (Datasource == "")
                    {
                        errorList.Add($"{DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")} {file.FileName} 第{i + 4}行失敗，原因(數據來源為必填項目)");
                    }

                    int monthcount = (a.EndTime.Year - a.StartTime.Year) * 12 + a.EndTime.Month - a.StartTime.Month + 1;//相差多少月份
                    int year, month;
                    year = a.StartTime.Year - 1911;//系統從盤查區間自動帶出年
                    month = a.StartTime.Month;//系統自動帶出樂
                    

                    for (int j = 0; j < monthcount; j++)
                    {

                        var Num = Fireequipmentdt.Rows[i][year + "/" + month + "數值"].ParseToDecimal(-1);
                        var NumCondition = Fireequipmentdt.Rows[i][year + "/" + month + "數值"].ParseToDecimal();//數值條件

                        if (Num == -1)
                        {
                            errorList.Add($"{DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")} {file.FileName} 第{i + 4}行失敗，原因(" + year + "/" + month + "數值" + "填寫不正確)");
                        }
                        if (NumCondition < 0)
                        {
                            errorList.Add($"{DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")} {file.FileName} 第{i + 4}行失敗，原因(" + year + "/" + month + "數值" + "不可為負數)");
                        }

                        month++;
                        if (month == 13)
                        {
                            month = 1;
                            year++;
                        }
                    }

                }
                //沒錯誤則新增資料
                if (errorList.Count == 0)
                {
                    for (int i = 0; i < Fireequipmentdt.Rows.Count; i++)
                    {
                        var FactoryName = Fireequipmentdt.Rows[i]["場域"].ParseToString();

                        var EquipmentName = Fireequipmentdt.Rows[i]["設備名稱"].ParseToString();
                        var Ghgtype = Fireequipmentdt.Rows[i]["溫室氣體種類"].ParseToString();

                        var Remark = Fireequipmentdt.Rows[i]["備註"].ParseToString();
                        var Unit = Fireequipmentdt.Rows[i]["單位"].ParseToString();
                        var DistributeRatio = Fireequipmentdt.Rows[i]["分配比率"].ParseToDecimal(-1);
                        var Management = Fireequipmentdt.Rows[i]["管理單位"].ParseToString();
                        var Principal = Fireequipmentdt.Rows[i]["負責人員"].ParseToString();
                        var Datasource = Fireequipmentdt.Rows[i]["數據來源"].ParseToString();
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

                            var Num = Fireequipmentdt.Rows[i][year + "/" + month + "數值"].ParseToDecimal(-1);
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
                    result.Add($"{DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")} 資料正確 匯入成功 共匯入{Fireequipmentdt.Rows.Count}筆");
                }


                System.IO.File.Delete(fileDir + fileName);

                return Sucess(result);


            }
            catch (Exception ex)
            {
                System.IO.File.Delete(fileDir + fileName);

                var Log = new Log()
                {
                    WhereFunction = "OrganizeExcel_FireequipmentImportExcel_post",
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
                return Error("內部伺服器錯誤"); // 或其他適當的錯誤訊息
            }
        }


        public ActionResult DumptreatmentOutsourcingImportExcel(int BasicId)
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
                    WhereFunction = "OrganizeExcel_DumptreatmentOutsourcingImportExcel_get",
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
        public ActionResult DumptreatmentOutsourcingImportExcel(IFormFile file, int BasicId, int SelMonth)
        {
            var fileName = DateTime.Now.ToString("yyyyMMddHHmmss") + file.FileName;//給予檔案不重複名稱
            string fileDir = FileSave.Path + "/excel";//檔案路徑

            try
            {
                var a = _MyDbContext.Basicdatas.Where(a => a.BasicId == BasicId).FirstOrDefault();//基本資料
                List<string> result = new List<string>();//顯示資料

                string filePath = FileSave.Path + "/excel" + fileName;//檔案路徑
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

                var DumptreatmentOutsourcingdt = NPOIHelper.ImportExcel_Row2And3_Data4(filePath);
                List<string> errorList = new List<string>();
                if (DumptreatmentOutsourcingdt.Rows.Count == 0)
                {
                    errorList.Add($"{DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")} {file.FileName} 資料為空");
                }
                //判斷是否有錯誤
                for (int i = 0; i < DumptreatmentOutsourcingdt.Rows.Count; i++)
                {
                    var FactoryName = DumptreatmentOutsourcingdt.Rows[i]["場域"].ParseToString();
                    var WasteName = DumptreatmentOutsourcingdt.Rows[i]["廢棄物名稱"].ParseToString();
                    var WasteMethod = DumptreatmentOutsourcingdt.Rows[i]["廢棄物處理方式"].ParseToString();
                    var CleanerName = DumptreatmentOutsourcingdt.Rows[i]["清運商名稱"].ParseToString();
                    var FinalAddress = DumptreatmentOutsourcingdt.Rows[i]["最終處理地址"].ParseToString();

                    var Method = DumptreatmentOutsourcingdt.Rows[i]["運輸方式"].ParseToString();
                    var StartLocation = DumptreatmentOutsourcingdt.Rows[i]["起點"].ParseToString();
                    var EndLocation = DumptreatmentOutsourcingdt.Rows[i]["迄點"].ParseToString();
                    var Car = DumptreatmentOutsourcingdt.Rows[i]["車種"].ParseToString();
                    var Tonnes = DumptreatmentOutsourcingdt.Rows[i]["噸數"].ParseToDecimal();
                    var Fuel = DumptreatmentOutsourcingdt.Rows[i]["燃料"].ParseToString();
                    var Land = DumptreatmentOutsourcingdt.Rows[i]["陸運(km)"].ParseToDecimal(-1);//陸運條件
                    var LandCondition = DumptreatmentOutsourcingdt.Rows[i]["陸運(km)"].ParseToDecimal();//陸運條件
                    var Sea = DumptreatmentOutsourcingdt.Rows[i]["海運(nm)"].ParseToDecimal(-1);
                    var SeaCondition = DumptreatmentOutsourcingdt.Rows[i]["海運(nm)"].ParseToDecimal();//海運條件
                    var Air = DumptreatmentOutsourcingdt.Rows[i]["空運(km)"].ParseToDecimal(-1);
                    var AirCondition = DumptreatmentOutsourcingdt.Rows[i]["空運(km)"].ParseToDecimal();//空運條件

                    var Management = DumptreatmentOutsourcingdt.Rows[i]["管理單位"].ParseToString();
                    var Principal = DumptreatmentOutsourcingdt.Rows[i]["負責人員"].ParseToString();
                    var Datasource = DumptreatmentOutsourcingdt.Rows[i]["數據來源"].ParseToString();

                    var Remark = DumptreatmentOutsourcingdt.Rows[i]["備註"].ParseToString();
                    var Unit = DumptreatmentOutsourcingdt.Rows[i]["單位"].ParseToString();
                    var DistributeRatio = DumptreatmentOutsourcingdt.Rows[i]["分配比率"].ParseToDecimal(-1);
                    var DistributeRatioCondition = DumptreatmentOutsourcingdt.Rows[i]["分配比率"].ParseToDecimal(); //分配比率條件


                    if (!_MyDbContext.BasicdataFactoryaddresses.Any(a => a.BasicId == BasicId && a.Name == FactoryName))
                    {
                        errorList.Add($"{DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")} {file.FileName} 第{i + 4}行失敗，原因(場域不存在)");
                    }                    
                    if (WasteName == "" || WasteMethod == "")
                    {
                        errorList.Add($"{DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")} {file.FileName} 第{i + 4}行失敗，原因(廢棄物名稱或廢棄物處理方式未選取)");
                    }
                    else if (_MyDbContext.Coefficients.FirstOrDefault(a => a.Name == WasteName && a.WasteMethod == WasteMethod && a.Type == "dumptreatment_outsourcing") == null)
                    {
                        List<string> Methods = _MyDbContext.Coefficients.Where(a => a.Name == WasteName && a.Type == "dumptreatment_outsourcing").Select(a => a.WasteMethod).ToList();
                        string results = string.Join(", ", Methods);
                        errorList.Add($"{DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")} {file.FileName} 第{i + 4}行失敗，原因({WasteName}，請選擇{results})");
                    }
                    if (!_MyDbContext.Selectdata.Any(a => a.Name == Method) && Method != "")
                    {
                        errorList.Add($"{DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")} {file.FileName} 第{i + 4}行失敗，原因(運輸方式不存在)");
                    }
                    if (!_MyDbContext.Selectdata.Any(a => a.Name == Car) && Car != "")
                    {
                        errorList.Add($"{DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")} {file.FileName} 第{i + 4}行失敗，原因(車種不存在)");
                    }
                    if (!_MyDbContext.Selectdata.Where(a => a.Code == "10" || a.Code == "11").Select(a => a.Name).Contains(Unit))
                    {
                        errorList.Add($"{DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")} {file.FileName} 第{i + 4}行失敗，原因(單位不存在)");
                    }
                    if (Land == -1)
                    {
                        errorList.Add($"{DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")} {file.FileName} 第{i + 4}行失敗，原因(陸運填寫不正確)");
                    }
                    if (LandCondition < 0)
                    {
                        errorList.Add($"{DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")} {file.FileName} 第{i + 4}行失敗，原因(陸運不可小於0)");
                    }
                    if (Sea == -1)
                    {
                        errorList.Add($"{DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")} {file.FileName} 第{i + 4}行失敗，原因(海運填寫不正確)");
                    }
                    if (SeaCondition < 0)
                    {
                        errorList.Add($"{DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")} {file.FileName} 第{i + 4}行失敗，原因(海運不可小於0)");
                    }
                    if (Air == -1)
                    {
                        errorList.Add($"{DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")} {file.FileName} 第{i + 4}行失敗，原因(空運填寫不正確)");
                    }
                    if (AirCondition < 0)
                    {
                        errorList.Add($"{DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")} {file.FileName} 第{i + 4}行失敗，原因(空運不可小於0)");
                    }
                    if (DistributeRatio == -1 || DistributeRatio == null || DistributeRatioCondition < 0 || DistributeRatioCondition > 100)
                    {
                        errorList.Add($"{DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")} {file.FileName} 第{i + 4}行失敗，原因(分配比率應介於0-100之間)");
                    }
                    if (Management == "")
                    {
                        errorList.Add($"{DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")} {file.FileName} 第{i + 4}行失敗，原因(管理單位為必填項目)");
                    }
                    if (Principal == "")
                    {
                        errorList.Add($"{DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")} {file.FileName} 第{i + 4}行失敗，原因(負責人員為必填項目)");
                    }
                    if (Datasource == "")
                    {
                        errorList.Add($"{DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")} {file.FileName} 第{i + 4}行失敗，原因(數據來源為必填項目)");
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

                        var StartTime = DumptreatmentOutsourcingdt.Rows[i][year + "/" + month + "起始日期"].ParseToTWDate("1");
                        var EndTime = DumptreatmentOutsourcingdt.Rows[i][year + "/" + month + "結束日期"].ParseToTWDate("1");
                        var Num = DumptreatmentOutsourcingdt.Rows[i][year + "/" + month + "數值"].ParseToDecimal(-1);
                        var NumCondition = DumptreatmentOutsourcingdt.Rows[i][year + "/" + month + "數值"].ParseToDecimal();//數值條件

                        if (StartTime == "1")
                        {
                            errorList.Add($"{DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")} {file.FileName} 第{i + 4}行失敗，原因(" + year + "/" + month + "起始日期" + "填寫不正確)");
                        }
                        if (EndTime == "1")
                        {
                            errorList.Add($"{DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")} {file.FileName} 第{i + 4}行失敗，原因(" + year + "/" + month + "結束日期" + "填寫不正確)");
                        }
                        if (Num == -1)
                        {
                            errorList.Add($"{DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")} {file.FileName} 第{i + 4}行失敗，原因(" + year + "/" + month + "數值" + "填寫不正確)");
                        }
                        if (NumCondition < 0)
                        {
                            errorList.Add($"{DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")} {file.FileName} 第{i + 4}行失敗，原因(" + year + "/" + month + "數值" + "不可為負數)");
                        }
                        if (Num != null && (StartTime == "" || EndTime == ""))
                        {
                            errorList.Add($"{DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")} {file.FileName} 第{i + 4}行失敗，原因(" + year + "/" + month + "缺少起始或結束時間)");
                        }

                        month += SelMonth;
                        if (month > 12)
                        {
                            month -= 12;
                            year++;
                        }
                    }
                }
                //沒錯誤則新增資料
                if (errorList.Count == 0)
                {
                    for (int i = 0; i < DumptreatmentOutsourcingdt.Rows.Count; i++)
                    {
                        var FactoryName = DumptreatmentOutsourcingdt.Rows[i]["場域"].ParseToString();
                        var WasteName = DumptreatmentOutsourcingdt.Rows[i]["廢棄物名稱"].ParseToString();
                        var WasteMethod = DumptreatmentOutsourcingdt.Rows[i]["廢棄物處理方式"].ParseToString();
                        var CleanerName = DumptreatmentOutsourcingdt.Rows[i]["清運商名稱"].ParseToString();
                        var FinalAddress = DumptreatmentOutsourcingdt.Rows[i]["最終處理地址"].ParseToString();

                        var Method = DumptreatmentOutsourcingdt.Rows[i]["運輸方式"].ParseToString();
                        var StartLocation = DumptreatmentOutsourcingdt.Rows[i]["起點"].ParseToString();
                        var EndLocation = DumptreatmentOutsourcingdt.Rows[i]["迄點"].ParseToString();
                        var Car = DumptreatmentOutsourcingdt.Rows[i]["車種"].ParseToString();
                        var Tonnes = DumptreatmentOutsourcingdt.Rows[i]["噸數"].ParseToDecimal();
                        var Fuel = DumptreatmentOutsourcingdt.Rows[i]["燃料"].ParseToString();
                        var Land = DumptreatmentOutsourcingdt.Rows[i]["陸運(km)"].ParseToDecimal(-1);
                        var Sea = DumptreatmentOutsourcingdt.Rows[i]["海運(nm)"].ParseToDecimal(-1);
                        var Air = DumptreatmentOutsourcingdt.Rows[i]["空運(km)"].ParseToDecimal(-1);

                        var Management = DumptreatmentOutsourcingdt.Rows[i]["管理單位"].ParseToString();
                        var Principal = DumptreatmentOutsourcingdt.Rows[i]["負責人員"].ParseToString();
                        var Datasource = DumptreatmentOutsourcingdt.Rows[i]["數據來源"].ParseToString();

                        var Remark = DumptreatmentOutsourcingdt.Rows[i]["備註"].ParseToString();
                        var Unit = DumptreatmentOutsourcingdt.Rows[i]["單位"].ParseToString();
                        var DistributeRatio = DumptreatmentOutsourcingdt.Rows[i]["分配比率"].ParseToDecimal(-1);

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
                            var StartTime = DumptreatmentOutsourcingdt.Rows[i][year + "/" + month + "起始日期"].ParseToTWDate("1");
                            var EndTime = DumptreatmentOutsourcingdt.Rows[i][year + "/" + month + "結束日期"].ParseToTWDate("1");
                            var Num = DumptreatmentOutsourcingdt.Rows[i][year + "/" + month + "數值"].ParseToDecimal(-1);
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
                    result.Add($"{DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")} 資料正確 匯入成功 共匯入{DumptreatmentOutsourcingdt.Rows.Count}筆");
                }


                System.IO.File.Delete(fileDir + fileName);

                return Sucess(result);


            }
            catch (Exception ex)
            {
                System.IO.File.Delete(fileDir + fileName);

                var Log = new Log()
                {
                    WhereFunction = "OrganizeExcel_DumptreatmentOutsourcingImportExcel_post",
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
                return Error("內部伺服器錯誤"); // 或其他適當的錯誤訊息
            }
        }



        public ActionResult RefrigerantNoneImportExcel(int BasicId)
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
                    WhereFunction = "OrganizeExcel_RefrigerantNoneImportExcel_get",
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
        public ActionResult RefrigerantNoneImportExcel(IFormFile file, int BasicId)
        {
            var fileName = DateTime.Now.ToString("yyyyMMddHHmmss") + file.FileName;//給予檔案不重複名稱
            string fileDir = FileSave.Path + "/excel";//檔案路徑

            try
            {
                var a = _MyDbContext.Basicdatas.Where(a => a.BasicId == BasicId).FirstOrDefault();//基本資料
                List<string> result = new List<string>();//顯示資料

                string filePath = FileSave.Path + "/excel" + fileName;//檔案路徑
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

                var RefrigerantNonedt = NPOIHelper.ImportExcel_Row1And3_Data4(filePath);
                List<string> errorList = new List<string>();
                if (RefrigerantNonedt.Rows.Count == 0)
                {
                    errorList.Add($"{DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")} {file.FileName} 資料為空");
                }
                //判斷是否有錯誤
                for (int i = 0; i < RefrigerantNonedt.Rows.Count; i++)
                {
                    var FactoryName = RefrigerantNonedt.Rows[i]["場域"].ParseToString();
                    var EquipmentName = RefrigerantNonedt.Rows[i]["設備名稱"].ParseToString();
                    var EquipmentType = RefrigerantNonedt.Rows[i]["設備類型"].ParseToString();
                    var EquipmentLocation = RefrigerantNonedt.Rows[i]["設備位置"].ParseToString();
                    var RefrigerantType = RefrigerantNonedt.Rows[i]["冷媒種類"].ParseToString();
                    var MachineQuantity = RefrigerantNonedt.Rows[i]["設備數量"].ParseToInt(-1);
                    var MachineQuantityCondition = RefrigerantNonedt.Rows[i]["設備數量"].ParseToInt();//設備數量條件
                    var Manufacturers = RefrigerantNonedt.Rows[i]["廠牌"].ParseToString();

                    var Management = RefrigerantNonedt.Rows[i]["管理單位"].ParseToString();
                    var Principal = RefrigerantNonedt.Rows[i]["負責人員"].ParseToString();
                    var Remark = RefrigerantNonedt.Rows[i]["備註"].ParseToString();
                    var RefrigerantWeight = RefrigerantNonedt.Rows[i]["冷媒重量"].ParseToDecimal(-1);
                    var RefrigerantWeightCondition = RefrigerantNonedt.Rows[i]["冷媒重量"].ParseToDecimal();//冷媒重量條件
                    var Unit = RefrigerantNonedt.Rows[i]["單位"].ParseToString();
                    var DistributeRatio = RefrigerantNonedt.Rows[i]["分配比率"].ParseToDecimal(-1);
                    var DistributeRatioCondition = RefrigerantNonedt.Rows[i]["分配比率"].ParseToDecimal();//分配比率條件


                    if (!_MyDbContext.BasicdataFactoryaddresses.Any(a => a.BasicId == BasicId && a.Name == FactoryName))
                    {
                        errorList.Add($"{DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")} {file.FileName} 第{i + 4}行失敗，原因(場域不存在)");
                    }
                    if (EquipmentName == "")
                    {
                        errorList.Add($"{DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")} {file.FileName} 第{i + 4}行失敗，原因(設備名稱為必填項目)");
                    }
                    if (!_MyDbContext.Selectdata.Any(a => a.Name == EquipmentType))
                    {
                        errorList.Add($"{DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")} {file.FileName} 第{i + 4}行失敗，原因(設備類型不存在)");
                    }
                    if (!_MyDbContext.Coefficients.Any(a => a.Name == RefrigerantType && a.Type == "RefrigerantName"))
                    {
                        errorList.Add($"{DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")} {file.FileName} 第{i + 4}行失敗，原因(冷媒種類不存在)");
                    }
                    if (MachineQuantity == -1 || MachineQuantity == null || MachineQuantityCondition < 0)
                    {
                        errorList.Add($"{DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")} {file.FileName} 第{i + 4}行失敗，原因(設備數量須為正整數)");
                    }
                    if (Management == "")
                    {
                        errorList.Add($"{DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")} {file.FileName} 第{i + 4}行失敗，原因(管理單位為必填項目)");
                    }
                    if (Principal == "")
                    {
                        errorList.Add($"{DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")} {file.FileName} 第{i + 4}行失敗，原因(負責人員為必填項目)");
                    }
                    if (RefrigerantWeight == -1 || RefrigerantWeight == null || RefrigerantWeightCondition < 0)
                    {
                        errorList.Add($"{DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")} {file.FileName} 第{i + 4}行失敗，原因(冷媒重量須為大於0的數值)");
                    }
                    if (Unit != "公斤(kg)")
                    {
                        errorList.Add($"{DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")} {file.FileName} 第{i + 4}行失敗，原因(單位須為公斤(kg))");
                    }
                    if (DistributeRatio == -1 || DistributeRatio == null || DistributeRatioCondition < 0 || DistributeRatioCondition > 100)
                    {
                        errorList.Add($"{DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")} {file.FileName} 第{i + 4}行失敗，原因(分配比率應介於0-100之間)");
                    }

                }
                //沒錯誤則新增資料
                if (errorList.Count == 0)
                {
                    for (int i = 0; i < RefrigerantNonedt.Rows.Count; i++)
                    {
                        var FactoryName = RefrigerantNonedt.Rows[i]["場域"].ParseToString();
                        var EquipmentName = RefrigerantNonedt.Rows[i]["設備名稱"].ParseToString();
                        var EquipmentType = RefrigerantNonedt.Rows[i]["設備類型"].ParseToString();
                        var EquipmentLocation = RefrigerantNonedt.Rows[i]["設備位置"].ParseToString();
                        var RefrigerantType = RefrigerantNonedt.Rows[i]["冷媒種類"].ParseToString();
                        var MachineQuantity = RefrigerantNonedt.Rows[i]["設備數量"].ParseToInt(-1);
                        var Manufacturers = RefrigerantNonedt.Rows[i]["廠牌"].ParseToString();

                        var Management = RefrigerantNonedt.Rows[i]["管理單位"].ParseToString();
                        var Principal = RefrigerantNonedt.Rows[i]["負責人員"].ParseToString();
                        var Remark = RefrigerantNonedt.Rows[i]["備註"].ParseToString();
                        var RefrigerantWeight = RefrigerantNonedt.Rows[i]["冷媒重量"].ParseToDecimal(-1);
                        var Unit = RefrigerantNonedt.Rows[i]["單位"].ParseToString();
                        var DistributeRatio = RefrigerantNonedt.Rows[i]["分配比率"].ParseToDecimal(-1);
                        

                        var RefrigerantNone = new RefrigerantNone()
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
                            Unit = _MyDbContext.Selectdata.FirstOrDefault(a => a.Code == "10") != null ? _MyDbContext.Selectdata.FirstOrDefault(a => a.Type == "Unit" && a.Code == "10").Code.ToString() : null,
                            DistributeRatio = DistributeRatio

                        };
                        var RefrigerantNoneItem = _MyDbContext.RefrigerantNones.Add(RefrigerantNone);

                        _MyDbContext.SaveChanges(); // 儲存變更至資料庫
                        int RefrigerantNoneId = RefrigerantNoneItem.Entity.Id;//該筆資料的Id



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
                    result.Add($"{DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")} 資料正確 匯入成功 共匯入{RefrigerantNonedt.Rows.Count}筆");
                }


                System.IO.File.Delete(fileDir + fileName);

                return Sucess(result);


            }
            catch (Exception ex)
            {
                System.IO.File.Delete(fileDir + fileName);

                var Log = new Log()
                {
                    WhereFunction = "OrganizeExcel_RefrigerantNoneImportExcel_post",
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
                return Error("內部伺服器錯誤"); // 或其他適當的錯誤訊息
            }
        }
        public ActionResult RefrigerantHaveImportExcel(int BasicId)
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
                    WhereFunction = "OrganizeExcel_RefrigerantHaveImportExcel_get",
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
        public ActionResult RefrigerantHaveImportExcel(IFormFile file, int BasicId)
        {
            var fileName = DateTime.Now.ToString("yyyyMMddHHmmss") + file.FileName;//給予檔案不重複名稱
            string fileDir = FileSave.Path + "/excel";//檔案路徑

            try
            {
                var a = _MyDbContext.Basicdatas.Where(a => a.BasicId == BasicId).FirstOrDefault();//基本資料
                List<string> result = new List<string>();//顯示資料

                string filePath = FileSave.Path + "/excel" + fileName;//檔案路徑
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

                var RefrigerantHavedt = NPOIHelper.ImportExcel_Row3And4_Data5(filePath);
                List<string> errorList = new List<string>();
                if (RefrigerantHavedt.Rows.Count == 0)
                {
                    errorList.Add($"{DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")} {file.FileName} 資料為空");
                }
                //判斷是否有錯誤
                for (int i = 0; i < RefrigerantHavedt.Rows.Count; i++)
                {
                    var FactoryName = RefrigerantHavedt.Rows[i]["場域"].ParseToString();
                    var EquipmentName = RefrigerantHavedt.Rows[i]["設備名稱"].ParseToString();
                    var EquipmentType = RefrigerantHavedt.Rows[i]["設備類型"].ParseToString();
                    var RefrigerantType = RefrigerantHavedt.Rows[i]["冷媒種類"].ParseToString();

                    var Management = RefrigerantHavedt.Rows[i]["管理單位"].ParseToString();
                    var Principal = RefrigerantHavedt.Rows[i]["負責人員"].ParseToString();
                    var Datasource = RefrigerantHavedt.Rows[i]["負責人員"].ParseToString();

                    var Remark = RefrigerantHavedt.Rows[i]["備註"].ParseToString();
                    var Total = RefrigerantHavedt.Rows[i]["填充量"].ParseToDecimal(-1);
                    var TotalCondition = RefrigerantHavedt.Rows[i]["填充量"].ParseToDecimal();//填充量條件
                    var Unit = RefrigerantHavedt.Rows[i]["單位"].ParseToString();
                    var DistributeRatio = RefrigerantHavedt.Rows[i]["分配比率"].ParseToDecimal(-1);
                    var DistributeRatioCondition = RefrigerantHavedt.Rows[i]["分配比率"].ParseToDecimal();//分配比率條件


                    if (!_MyDbContext.BasicdataFactoryaddresses.Any(a => a.BasicId == BasicId && a.Name == FactoryName))
                    {
                        errorList.Add($"{DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")} {file.FileName} 第{i + 5}行失敗，原因(場域不存在)");
                    }
                    if (EquipmentName == "")
                    {
                        errorList.Add($"{DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")} {file.FileName} 第{i + 5}行失敗，原因(設備名稱為必填項目)");
                    }
                    if (!_MyDbContext.Selectdata.Any(a => a.Name == EquipmentType))
                    {
                        errorList.Add($"{DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")} {file.FileName} 第{i + 5}行失敗，原因(設備類型不存在)");
                    }
                    if (!_MyDbContext.Coefficients.Any(a => a.Name == RefrigerantType && a.Type == "RefrigerantName"))
                    {
                        errorList.Add($"{DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")} {file.FileName} 第{i + 5}行失敗，原因(冷媒種類不存在)");
                    }
                    if (Management == "")
                    {
                        errorList.Add($"{DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")} {file.FileName} 第{i + 5}行失敗，原因(管理單位為必填項目)");
                    }
                    if (Principal == "")
                    {
                        errorList.Add($"{DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")} {file.FileName} 第{i + 5}行失敗，原因(負責人員為必填項目)");
                    }
                    if (Datasource == "")
                    {
                        errorList.Add($"{DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")} {file.FileName} 第{i + 5}行失敗，原因(數據來源為必填項目)");
                    }
                    if (Total == -1 || Total == null|| TotalCondition < 0)
                    {
                        errorList.Add($"{DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")} {file.FileName} 第{i + 5}行失敗，原因(填充量須為大於0的數)");
                    }
                    if (Unit != "公斤(kg)")
                    {
                        errorList.Add($"{DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")} {file.FileName} 第{i + 5}行失敗，原因(單位須為公斤(kg))");
                    }
                    if (DistributeRatio == -1 || DistributeRatio == null || DistributeRatioCondition < 0 || DistributeRatioCondition > 100)
                    {
                        errorList.Add($"{DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")} {file.FileName} 第{i + 5}行失敗，原因(分配比率應介於0-100之間)");
                    }
                }
                //沒錯誤則新增資料
                if (errorList.Count == 0)
                {
                    for (int i = 0; i < RefrigerantHavedt.Rows.Count; i++)
                    {
                        var FactoryName = RefrigerantHavedt.Rows[i]["場域"].ParseToString();
                        var EquipmentName = RefrigerantHavedt.Rows[i]["設備名稱"].ParseToString();
                        var EquipmentType = RefrigerantHavedt.Rows[i]["設備類型"].ParseToString();
                        var RefrigerantType = RefrigerantHavedt.Rows[i]["冷媒種類"].ParseToString();

                        var Management = RefrigerantHavedt.Rows[i]["管理單位"].ParseToString();
                        var Principal = RefrigerantHavedt.Rows[i]["負責人員"].ParseToString();
                        var Datasource = RefrigerantHavedt.Rows[i]["負責人員"].ParseToString();

                        var Remark = RefrigerantHavedt.Rows[i]["備註"].ParseToString();
                        var Total = RefrigerantHavedt.Rows[i]["填充量"].ParseToDecimal(-1);
                        var Unit = RefrigerantHavedt.Rows[i]["單位"].ParseToString();
                        var DistributeRatio = RefrigerantHavedt.Rows[i]["分配比率"].ParseToDecimal(-1);

                        var RefrigerantHave = new RefrigerantHave()
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
                            Unit = _MyDbContext.Selectdata.FirstOrDefault(a => a.Code == "10") != null ? _MyDbContext.Selectdata.FirstOrDefault(a => a.Type == "Unit" && a.Code == "10").Code.ToString() : null,
                            DistributeRatio = DistributeRatio

                        };
                        var RefrigerantHaveItem = _MyDbContext.RefrigerantHaves.Add(RefrigerantHave);
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
                    result.Add($"{DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")} 資料正確 匯入成功 共匯入{RefrigerantHavedt.Rows.Count}筆");
                }


                System.IO.File.Delete(fileDir + fileName);

                return Sucess(result);


            }
            catch (Exception ex)
            {
                System.IO.File.Delete(fileDir + fileName);

                var Log = new Log()
                {
                    WhereFunction = "OrganizeExcel_RefrigerantHaveImportExcel_post",
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
                return Error("內部伺服器錯誤"); // 或其他適當的錯誤訊息
            }
        }
        public ActionResult WorkinghourImportExcel(int BasicId)
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
                    WhereFunction = "OrganizeExcel_WorkinghourImportExcel_get",
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
        public ActionResult WorkinghourImportExcel(IFormFile file, int BasicId, IFormCollection? collection)
        {
            var fileName = DateTime.Now.ToString("yyyyMMddHHmmss") + file.FileName;//給予檔案不重複名稱
            string fileDir = FileSave.Path + "/excel";//檔案路徑

            try
            {
                var a = _MyDbContext.Basicdatas.Where(a => a.BasicId == BasicId).FirstOrDefault();//基本資料
                List<string> result = new List<string>();//顯示資料

                string filePath = FileSave.Path + "/excel" + fileName;//檔案路徑
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

                var Workinghourdt = NPOIHelper.ImportExcel_WorkingHours(filePath);
                List<string> errorList = new List<string>();
                List<string> correctList = new List<string>();
                if (Workinghourdt.Rows.Count == 0)
                {
                    errorList.Add($"{DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")} {file.FileName} 資料為空");
                }
                //判斷是否有錯誤
                for (int i = 0; i < Workinghourdt.Rows.Count; i++)
                {
                    var FactoryName = Workinghourdt.Rows[i]["場域"].ParseToString();
                    var Item = Workinghourdt.Rows[i]["項目"].ParseToString();
                    var TotalWorkHour = Workinghourdt.Rows[i]["總工時"].ParseToDecimal(-1);
                    var TotalWorkHourCondition = Workinghourdt.Rows[i]["總工時"].ParseToDecimal(); // 總工時要大於0
                    var Management = Workinghourdt.Rows[i]["管理部門"].ParseToString();
                    var Principal = Workinghourdt.Rows[i]["負責人員"].ParseToString();
                    var Datasource = Workinghourdt.Rows[i]["數據來源"].ParseToString();
                    var DistributeRatio = Workinghourdt.Rows[i]["分配比率"].ParseToDecimal(-1);

                    if (Item != "正式員工及約聘人員(具勞保)" && Item != "廠商外派人員(不具勞保)")
                    {
                        Item = "0";
                    }

                    if (!_MyDbContext.BasicdataFactoryaddresses.Any(a => a.BasicId == BasicId && a.Name == FactoryName) && i%2==0)
                    {
                        errorList.Add($"{DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")} {file.FileName} 第{i + 4}行失敗，原因(場域不存在)");
                    }
                    if (Item == "")
                    {
                        errorList.Add($"{DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")} {file.FileName} 第{i + 4}行失敗，原因(項目為必填)");
                    }
                    if (TotalWorkHour == null && Item == "正式員工及約聘人員(具勞保)")
                    {
                        errorList.Add($"{DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")} {file.FileName} 第{i + 4}行失敗，原因(總工時為必填)");
                    }
                    if (Management == "" && Item == "正式員工及約聘人員(具勞保)")
                    {
                        errorList.Add($"{DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")} {file.FileName} 第{i + 4}行失敗，原因(管理部門為必填)");
                    }
                    if (Principal == "" && Item == "正式員工及約聘人員(具勞保)")
                    {
                        errorList.Add($"{DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")} {file.FileName} 第{i + 4}行失敗，原因(負責人員為必填)");
                    }
                    if (Datasource == "" && Item == "正式員工及約聘人員(具勞保)")
                    {
                        errorList.Add($"{DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")} {file.FileName} 第{i + 4}行失敗，原因(數據來源為必填)");
                    }
                    if (DistributeRatio == null && Item == "正式員工及約聘人員(具勞保)")
                    {
                        errorList.Add($"{DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")} {file.FileName} 第{i + 4}行失敗，原因(分配比率為必填)");
                    }
                    if (Item == "0")
                    {
                        errorList.Add($"{DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")} {file.FileName} 第{i + 4}行失敗，原因(項目不存在)");
                    }
                    if (TotalWorkHour == -1)
                    {
                        errorList.Add($"{DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")} {file.FileName} 第{i + 4}行失敗，原因(每日工作時數(小時)/總時數(小時)填寫須為小數點 4 位以內的數字)");
                    }
                    if ((TotalWorkHourCondition < 0 ) && (Item == "正式員工及約聘人員(具勞保)" || Item == "廠商外派人員(不具勞保)")) // 總工時要大於0
                    {
                        errorList.Add($"{DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")} {file.FileName} 第{i + 4}行失敗，原因(每日工作時數(小時)/總時數(小時)填寫須在0-12小時內)");
                    }
                    if (DistributeRatio == -1)
                    {
                        errorList.Add($"{DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")} {file.FileName} 第{i + 4}行失敗，原因(分配比例填寫須為小數點 4 位以內的數字)");
                    }
                }


                if (errorList.Count == 0)
                {
                    int counter = 0;
                    for (int i = 0; i < Workinghourdt.Rows.Count; i++)
                    {
                        int ItemCount = i % 2;//五個項目
                        var FactoryName = Workinghourdt.Rows[counter]["場域"].ParseToString();                        
                        
                        var TotalWorkHour = Workinghourdt.Rows[i]["總工時"].ParseToDecimal(-1);
                        var Management = Workinghourdt.Rows[i]["管理部門"].ParseToString();
                        var Principal = Workinghourdt.Rows[i]["負責人員"].ParseToString();
                        var Datasource = Workinghourdt.Rows[i]["數據來源"].ParseToString();
                        var DistributeRatio = Workinghourdt.Rows[i]["分配比率"].ParseToDecimal(-1);


                        //更新工時資料
                        var FactoryNameId = _MyDbContext.BasicdataFactoryaddresses.FirstOrDefault(a => a.BasicId == BasicId && a.Name == FactoryName).Id;
                        var WorkinghourUpdate = _MyDbContext.Workinghours.Where(a => a.BasicId == BasicId && a.FactoryId == FactoryNameId && a.Item == ItemCount.ToString()).FirstOrDefault();
                        WorkinghourUpdate.TotalWorkHour = TotalWorkHour;
                        WorkinghourUpdate.Management = Management;
                        WorkinghourUpdate.Principal = Principal;
                        WorkinghourUpdate.Datasource = Datasource;
                        WorkinghourUpdate.DistributeRatio = DistributeRatio;
                        _MyDbContext.Workinghours.Update(WorkinghourUpdate);
                        _MyDbContext.SaveChanges(); // 儲存變更至資料庫

                        if ((i + 1) % 2 == 0)//場域每五列讀一次
                        {
                            counter += 2;
                        }
                    }

                }

                //顯示匯入資料的狀況
                foreach (var correct in correctList)
                {
                    result.Add($"<span style='color:#28B463'>{correct}</span>");
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
                    result.Add($"{DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")} 資料正確 匯入成功 共匯入{Workinghourdt.Rows.Count}筆");
                }


                System.IO.File.Delete(fileDir + fileName);

                return Sucess(result);


            }
            catch (Exception ex)
            {
                System.IO.File.Delete(fileDir + fileName);

                var Log = new Log()
                {
                    WhereFunction = "OrganizeExcel_WorkinghourImportExcel_post",
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
                return Error("內部伺服器錯誤"); // 或其他適當的錯誤訊息
            }
        }

    }
}
