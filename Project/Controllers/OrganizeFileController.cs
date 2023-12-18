using Project.Models.View;
using Project.Models;
using Microsoft.AspNetCore.Mvc;

namespace Project.Controllers
{
    public class OrganizeFileController : CommonController
    {
        private MyDbContext _MyDbContext;
        public OrganizeFileController(MyDbContext MyDbContext)
        {
            _MyDbContext = MyDbContext;
        }
        // GET: ProductController/Delete/5
        public ActionResult EvidenceFileDetail(int BasicId, int ItemId, string WhereForm)
        {
            try
            {
                ViewBag.BasicId = BasicId;//基本資料編號
                ViewBag.ItemId = ItemId;//該筆item的id
                ViewBag.WhereForm = WhereForm;//哪個表

                string Inventory = _MyDbContext.Basicdatas.Where(a => a.BasicId == BasicId).Select(a => a.BasicId.ToString()).FirstOrDefault();//抓取盤查表名稱
                ViewBag.Path = Inventory;//給予前端路徑
                //抓取Evidencefilemanages的值給予前端
                var EvidenceFileManage = _MyDbContext.Evidencefilemanages.Where(a => a.BasicId == BasicId && a.ItemId == ItemId && a.WhereForm == WhereForm).OrderByDescending(a => a.Id).ToList();
                var viewModel = new ViewEvidenceFileManage
                {
                    EvidenceFileManage = EvidenceFileManage,
                };

                var organize = _MyDbContext.Organizes.Where(a => a.BasicId == BasicId && a.Account == AccountId()).FirstOrDefault();
                if (organize != null)
                {
                    ViewBag.Status =organize.Status;
                }
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
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult EvidenceFileDetail(int BasicId, int ItemId, string WhereForm, List<IFormFile> EvidenceFile)
        {
            try
            {
                if (AccountId() == null)
                {
                    return Nologin();
                }
                var Basicdata = _MyDbContext.Basicdatas.Find(BasicId);//抓取基本資料資料
                string directoryPath = FileSave.Path  + Basicdata.BasicId.ToString() + "/" + WhereForm;//給予檔案路徑
                //查詢有無此黨名的資料夾，沒有則新建
                if (!Directory.Exists(directoryPath))
                {
                    Directory.CreateDirectory(directoryPath);
                }
                foreach (var file in EvidenceFile)
                {
                    if (file?.Length > 0)//如果有檔案時
                    {
                        if (file.FileName.Contains("+"))
                        {
                            return Error("檔名不可有+號");
                        }
                        var fileName = ItemId.ToString() + "_" + file.FileName;//檔名前面給予id，以防檔案重複名稱

                        // 檢查檔案是否已存在
                        var existingData = _MyDbContext.Evidencefilemanages
                            .FirstOrDefault(e =>
                                e.BasicId == BasicId &&
                                e.ItemId == ItemId &&
                                e.FileName == fileName &&
                                e.WhereForm == WhereForm);

                        if (existingData != null)//檔案已存在跳error
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
                            BasicId = BasicId,
                            ItemId = ItemId,
                            FileName = fileName,
                            WhereForm = WhereForm,
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
                    WhereFunction = "Organize_EvidenceFileDetail_post",
                    Exception = ex.ToString(),
                    DateTime = DateTime.Now,
                };
                _MyDbContext.Logs.Add(Log);
                _MyDbContext.SaveChanges();
                Log = null;
                return Error("內部伺服器錯誤"); // 或其他適當的錯誤訊息
            }
        }
        // GET: ProductController/Delete/5
        public ActionResult Delete(int BasicId, int ItemId, string WhereForm, string FileName)
        {
            try
            {
                var Basicdata = _MyDbContext.Basicdatas.Where(a => a.BasicId == BasicId).FirstOrDefault();//抓取基本資料
                string directoryPath = FileSave.Path + "/" + Basicdata.BasicId.ToString() + "/" + WhereForm + "/" + FileName;//檔案路徑
                System.IO.File.Delete(directoryPath);//刪除檔案
                //刪除該筆資料庫資料
                var EvidenceFile = _MyDbContext.Evidencefilemanages.Where(a => a.BasicId == BasicId && a.ItemId == ItemId && a.WhereForm == WhereForm && a.FileName == FileName).FirstOrDefault();
                _MyDbContext.Evidencefilemanages.RemoveRange(EvidenceFile);
                _MyDbContext.SaveChanges();
                EvidenceFile = null;
                Basicdata = null;
                return RedirectToAction("EvidenceFileDetail", "OrganizeFile", new { BasicId = BasicId, ItemId = ItemId, WhereForm = WhereForm, });

            }
            catch (Exception ex)
            {
                var Log = new Log()
                {
                    WhereFunction = "OrganizeFile_Delete_get",
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
