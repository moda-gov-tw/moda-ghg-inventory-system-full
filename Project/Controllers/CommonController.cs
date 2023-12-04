using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using System.Drawing;
using System.Security.Cryptography;
using System.Text.RegularExpressions;
using System.Text;
using System.Drawing.Imaging;
using Project.Common;
using System.Globalization;
using Project.Models;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;

namespace Project.Controllers
{
    public class CommonController : Controller
    {
        [HttpGet]
        /// <summary>
        /// Action執行前判斷
        /// </summary>
        /// <param name="filterContext"></param>
        /// 
        public static string MD5Encrypt(string inputString)
        {
            MD5 md5Hasher = MD5.Create();
            byte[] data = md5Hasher.ComputeHash(Encoding.UTF8.GetBytes(inputString));
            StringBuilder sBuilder = new StringBuilder();
            for (int i = 0; i < data.Length; i++)
            {
                sBuilder.Append(data[i].ToString("x2"));
            }
            return sBuilder.ToString();
        }
        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            var request = filterContext.HttpContext.Request;
            var url = request.Path.ToString(); // 获取请求的 URL
            var idMatch = Regex.Match(url, @"/(\d+)$"); // 使用正则表达式匹配 URL 中的数字部分
            if (idMatch.Success)
            {
                var id = idMatch.Groups[1].Value; // 提取匹配到的数字部分作为 ID
                ViewBag.Basicid = id;
            }
            base.OnActionExecuting(filterContext);
        }
        public IActionResult Captcha()
        {
            // 生成驗證碼
            var code = GenerateCaptchaCode();

            // 將驗證碼保存到 Session 中
            HttpContext.Session.SetString("CaptchaCode", code);

            // 生成驗證碼圖片並返回結果
            var image = GenerateCaptchaImage(code);
            return File(image.ToArray(), "image/jpeg");
        }

        // 生成隨機的四位數字驗證碼
        private string GenerateCaptchaCode()
        {
            var random = new Random();
            return random.Next(1000, 9999).ToString();
        }

        // 生成圖形驗證碼圖片
        private MemoryStream GenerateCaptchaImage(string code)
        {
            var image = new Bitmap(120, 40);
            var graphics = Graphics.FromImage(image);
            graphics.Clear(Color.White);

            using (var font = new Font(FontFamily.GenericMonospace, 24, FontStyle.Bold))
            using (var brush = new SolidBrush(Color.Black))
            {
                graphics.DrawString(code, font, brush, 10, 5);
            }

            var memoryStream = new MemoryStream();
            image.Save(memoryStream, ImageFormat.Jpeg);
            memoryStream.Position = 0;

            return memoryStream;
        }
        public string AccountId()
        {
            return HttpContext.Session.GetString("AccountId");
        }
        public string Account()
        {
            return HttpContext.Session.GetString("Account");
        }
        public string Email()
        {
            return HttpContext.Session.GetString("email");
        }
        public string Name()
        {
            return HttpContext.Session.GetString("name");
        }
        public int Basicid(int id)
        {

            return id;

        }
        public ActionResult Error(string error)
        {
            return Json(new { success = false, error = error });
        }
        public ActionResult Sucess(string data)
        {
            // 在此處處理不帶值的情況
            return Json(new { success = true, sucess = data });
        }
        public ActionResult Sucess(object data = null)
        {
            // 在此處處理不帶值的情況
            return Json(new { success = true, sucess = data });
        }
        public ActionResult Sucess(int? id)
        {
            return Json(new { success = true, id = id });
        }
        public ActionResult Sucess()
        {
            // 在此處處理不帶值的情況
            return Json(new { success = true });
        }
        public ActionResult Nologin()
        {
            // 在此處處理不帶值的情況
            return Json(new { nologin = true });
        }
        public static class ConnectionStrings
        {
            public static string MySqlConnectionString = AppSetting.GetValue("ConnectionStrings:MySqlConnectionString");
            public static string MySqlVersionString = AppSetting.GetValue("ConnectionStrings:MySqlVersionString");
        }
        public static class FileSave
        {
            public static string Path = AppSetting.GetValue("FileSave:Path");
        }
        public static class HtmlFileSave
        {
            public static string HtmlPath = AppSetting.GetValue("FileSave:HtmlPath");
        }
        async public Task<string> Company(string BusineseNumber)
        {
            var client = new HttpClient();
            var request = new HttpRequestMessage(HttpMethod.Get, $"https://data.gcis.nat.gov.tw/od/data/api/5F64D864-61CB-4D0D-8AD9-492047CC1EA6?$format=json&$filter=Business_Accounting_NO%20eq%20{BusineseNumber}");
            var response = await client.SendAsync(request);
            response.EnsureSuccessStatusCode();
            var result = await response.Content.ReadAsStringAsync();
            JArray jsonArray = JArray.Parse(result);
            // 将 JArray 转换为 JSON 字符串
            string jsonString = JsonConvert.SerializeObject(jsonArray);

            return jsonString;
        }
        public decimal? Total(int BasicDataStartyear, int BasicDataStartmonth, int BasicDataEndyear, int BasicDataEndmonth, string StartTime, string EndTime, decimal? Num, int j, int monthcount,int SelMonth)
        {
            decimal? total = 0;


            DateTime StartparsedDate = DateTime.ParseExact(StartTime, "yyy/M/d", CultureInfo.InvariantCulture);
            int StartYear = StartparsedDate.Year; // 加上 1911 轉換為西元年份
            int StartMonth = StartparsedDate.Month;
            int StartDay = StartparsedDate.Day;

            DateTime EndparsedDate = DateTime.ParseExact(EndTime, "yyy/M/d", CultureInfo.InvariantCulture);
            int EndYear = EndparsedDate.Year; // 加上 1911 轉換為西元年份
            int EndMonth = EndparsedDate.Month;
            int EndDay = EndparsedDate.Day;

            int SmonthCount = 0;//相差月份
            int EmonthCount = 0;//相差月份

            int SDayTotal = 0;
            int EDayTotal = 0;


            if (j == 0)
            {
                 SmonthCount = (BasicDataStartyear - StartYear) * 12 + (BasicDataStartmonth - StartMonth);//相差月份
                 EmonthCount = (EndYear - BasicDataStartyear) * 12 + (EndMonth - BasicDataStartmonth);//相差月份
                if (SmonthCount > 0)
                {//有值再跑
                    for (int k = 0; k < SmonthCount; k++)
                    {//for迴圈跑相差多少月份

                        SDayTotal += DateTime.DaysInMonth(StartYear + 1911, StartMonth);//開始天數累加，getDaysInMonth抓取該月天數
                        StartMonth++;//開始月份累加

                        if (StartMonth > 12)
                        {//月份超過12個月時，月份從1開始，年分加1
                            StartMonth = 1;
                            StartYear++;
                        }
                    }
                    SDayTotal -= (StartDay - 1);//開始天數，減去實際開始天數，5/13，SDayTotal-= (13-1)
                }

                if (EmonthCount >= 0)
                {//有值再跑
                    for (int k = 0; k < EmonthCount; k++)
                    {//for迴圈跑相差多少月份，排除填寫的月份
                        EndMonth--;//結束時間7/13，則從6月開始累加
                        if (EndMonth < 1)
                        {//月份超過12個月時，月份從1開始，年分加1
                            EndMonth = 12;
                            EndYear--;
                        }
                        EDayTotal += DateTime.DaysInMonth(EndYear + 1911, EndMonth);
                    }
                    EDayTotal += EndDay;//加上排除月份的天數
                }
                total += Math.Round(Num * EDayTotal / (EDayTotal + SDayTotal) ??0,2);//數值*EDayTotal佔的比例，小數點2位
            }
            //最後一筆做運算
            else if (j == monthcount)
            {
                SmonthCount = (BasicDataEndyear - StartYear) * 12 + (BasicDataEndmonth - StartMonth);//相差月份
                EmonthCount = (EndYear - BasicDataEndyear) * 12 + (EndMonth - BasicDataEndmonth);//相差月份
                //開始時間總天數
                if ((SmonthCount + 1) > 0)
                {//for迴圈跑相差多少月份
                    for (int k = 0; k < (SmonthCount + 1); k++)
                    {
                        SDayTotal += DateTime.DaysInMonth(StartYear + 1911, StartMonth);//開始天數累加，getDaysInMonth抓取該月天數
                        StartMonth -= 1;
                        if (StartMonth > 12)
                        {//月份超過12個月時，月份從1開始，年分加1
                            StartMonth = 1;
                            StartYear++;
                        }
                    }
                    SDayTotal -= StartDay - 1;
                }
                //結束時間總天數

                if (EmonthCount > 0)
                {//有值再跑
                    for (int k = 0; k < (EmonthCount - 1); k++)
                    {//for迴圈跑相差多少月份，排除填寫的月份
                        EndMonth--;//結束時間7/13，則從6月開始累加
                        if (EndMonth < 1)
                        {//月份超過12個月時，月份從1開始，年分加1
                            EndMonth = 12;
                            EndYear--;
                        }
                        EDayTotal += DateTime.DaysInMonth(EndYear + 1911, EndMonth);

                    }
                    EDayTotal += EndDay;//加上排除月份的天數
                }
                total += Math.Round(Num * SDayTotal / (EDayTotal + SDayTotal)??0,2);//數值*SDayTotal佔的比例，小數點2位

            }
            else
            {
                total += Num;
            }

            return total;
        }

        public string EnergyUseLog(string type ,string? EnergyName, string? EquipmentName,string? EquipmentLocation,decimal? Total,string? Unit)
        {
            return $"<div>{type}能源使用：能源名稱：{EnergyName}，設備名稱：{EquipmentName}，設備位置：{EquipmentLocation}，總量：{Total}，單位：{Unit}，時間：{DateTime.Now}</div>";
        }
        public string ResourceUseLog(string type, string? ResourceName, string? EquipmentName, string? EquipmentLocation, decimal? Total, string? Unit)
        {
            return $"<div>{type}資源使用：能源名稱：{ResourceName}，設備名稱：{EquipmentName}，設備位置：{EquipmentLocation}，總量：{Total}，單位：{Unit}，時間：{DateTime.Now}</div>";
        }
        public string RefrigerantHaveLog(string type,  string? EquipmentName, string? RefrigerantType,decimal? Total)
        {
            return $"<div>{type}有冷媒填充：設備名稱：{EquipmentName}，冷媒種類：{RefrigerantType}，填充量：{Total}，單位：公斤(kg)，時間：{DateTime.Now}</div>";
        }
        public string RefrigerantNoneLog(string type, string? EquipmentName,string EquipmentType,string? EquipmentLocation, string? RefrigerantType, decimal? RefrigerantWeight)
        {
            return $"<div>{type}無冷媒填充：設備名稱：{EquipmentName}，設備類型：{EquipmentType}，設備位置：{EquipmentLocation}，冷媒種類：{RefrigerantType}，冷媒容量or重量：{RefrigerantWeight}，單位：公斤(kg)，時間：{DateTime.Now}</div>";
        }
        public string FireequipmentLog(string type, string? EquipmentName, string? Ghgtype, decimal? Total, string? Unit)
        {
            return $"<div>{type}消防設備：設備名稱：{EquipmentName}，設備類型：{Ghgtype}，總量：{Total}，單位：{Unit}，時間：{DateTime.Now}</div>";
        }
        public string DumptreatmentOutsourcingLog(string type, string? WasteName, string? WasteMethod, string? CleanerName, decimal? Total, string? Unit)
        {
            return $"<div>{type}廢棄物處理：空水廢名稱：{WasteName}，空水廢處理方式：{WasteMethod}，清運商名稱：{CleanerName}，總量：{Total}，單位：{Unit}，時間：{DateTime.Now}</div>";
        }
        public string WorkinghourLog(string type, string Factory, string? item, decimal? TotalWorkHour )
        {
            return $"<div>{type}工時資料(廠址：{Factory})：項目：{item}，總工時：{TotalWorkHour}，時間：{DateTime.Now}</div>";
        }
        public string TotalWorkinghourLog(string type, string Factory, string? item, decimal? TotalWorkHour)
        {
            return $"<div>{type}工時資料(廠址：{Factory})：項目：{item}，總工時：{TotalWorkHour}，時間：{DateTime.Now}</div>";
        }
    }
}
