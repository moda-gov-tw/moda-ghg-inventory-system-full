using ITRIProject.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json.Serialization;
using Newtonsoft.Json;
using System.Diagnostics;

namespace ITRIProject.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;

        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
        }

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }

        //[ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        //public IActionResult Error()
        //{
        //    return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        //}
        public IActionResult Test()
        {
            return View();
        }
        /// <summary>
        /// 全局異常處理
        /// </summary>
        /// <param name="id">接收狀態碼(404:請求Url不存在(包括請求地址及靜態資源文件);)</param>
        /// <returns></returns>
        [AllowAnonymous]
        public IActionResult Error(int id)
        {
            int statusCode = id;
            string requestType = HttpContext.Request.Headers["X-Requested-With"];
            var exceptionDetails = HttpContext.Features.Get<IExceptionHandlerFeature>();
            var statusCodeResult = HttpContext.Features.Get<IStatusCodeReExecuteFeature>();
            if (statusCode != 403 && statusCode != 404) statusCode = HttpContext.Response.StatusCode;

            string message = $"代碼:{statusCode},地址:{statusCodeResult?.OriginalPath},{exceptionDetails?.Error}";

            if (!string.IsNullOrEmpty(requestType) && requestType.Equals("XMLHttpRequest", StringComparison.CurrentCultureIgnoreCase))
            {
                string msg = $"請求錯誤，代碼{statusCode}";
                _logger.LogError("Ajax請求-{message}", message);
                HttpContext.Response.ContentType = "application/json;charset=utf-8";
                HttpContext.Response.StatusCode = StatusCodes.Status200OK;
                return JsonResult(statusCode, msg);
            }

            _logger.LogError("{message}", message);
            ViewBag.statusCode = statusCode;
            ViewBag.message = $"請求錯誤，代碼{statusCode}";
            return View(ViewBag);
        }

        /// <summary>
        /// 返回JSON
        /// </summary>
        /// <returns></returns>
        [AllowAnonymous]
        protected IActionResult JsonResult(int code, string msg, object data = null, int? count = null)
        {
            string jsonStr = SerializeObject(new ResultStatus(code, msg, data, count));
            return Content(jsonStr, "application/json");
        }

        public class ResultStatus
        {
            private int? count1;

            public ResultStatus(int code, string msg, object data, int? count1)
            {
                this.code = code;
                this.msg = msg;
                this.data = data;
                this.count1 = count1;
            }

            public int code { get; set; }
            public string msg { get; set; }
            public object data { get; set; }
            public int count { get; set; }
        }

        /// <summary>
        /// 序列化
        /// </summary>
        /// <param name="value">對象</param>
        /// <param name="isCamelCase">是否啟用駝峰</param>
        /// <returns></returns>
        protected string SerializeObject(object value, bool isCamelCase = false)
        {
            var serializerSettings = new JsonSerializerSettings();
            if (isCamelCase)
            {
                serializerSettings.ContractResolver = new CamelCasePropertyNamesContractResolver();
            }
            serializerSettings.DateFormatString = "yyyy-MM-dd HH:mm:ss";

            return JsonConvert.SerializeObject(value, Formatting.Indented, serializerSettings);
        }
    }
}