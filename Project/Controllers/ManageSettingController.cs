using Project.Models;
using Microsoft.AspNetCore.Mvc;
using static Project.Controllers.CommonController;
using Newtonsoft.Json;
using X.PagedList;

namespace Project.Controllers
{
    public class ManageSettingController : CommonController
    {
        private MyDbContext _MyDbContext;
        public ManageSettingController(MyDbContext MyDbContext)
        {
            _MyDbContext = MyDbContext;
        }
        // GET: ProductController/Delete/5
        public ActionResult ManageSetting(int id, int ItemId, string? search)
        {
            
            try
            {
                var Suppliermanages = _MyDbContext.Suppliermanages.Where(a => a.Account ==AccountId() );//抓取該id的能源使用
                var pagedList = Suppliermanages.OrderByDescending(a => a.Id).ToPagedList();
                int count = 1;//jsgrid sort遞增
                //選擇前端jsgrid需顯示的資料
                var result = pagedList.Select((item, index) => new
                {
                    sort = count + index,
                    id = item.Id,
                    suppliername = item.SupplierName,
                    supplieraddress = item.SupplierAddress,

                });
                ViewBag.data = JsonConvert.SerializeObject(result);//資料傳回前端
                return View();

            }
            catch (Exception ex)
            {
                var Log = new Log()
                {
                    WhereFunction = "ManageSetting_ManageSetting_get",
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
        public ActionResult ManageSetting(int BasicId, int ItemId, string WhereForm, List<IFormFile> EvidenceFile)
        {
            try
            {

                return RedirectToAction("Details", "Organize");//有下一個頁面時，要改為導向下一個頁面

            }
            catch (Exception ex)
            {
                var Log = new Log()
                {
                    WhereFunction = "ManageSetting_ManageSetting_post",
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
