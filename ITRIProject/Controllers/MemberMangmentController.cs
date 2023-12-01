using ITRIProject.Models;
using ITRIProject.Models.View;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using NPOI.OpenXmlFormats.Dml.Diagram;
using NPOI.SS.Formula.Functions;
using Org.BouncyCastle.Utilities;
using Project.Common;
using System.Collections.Generic;
using System.Data;
using X.PagedList;
using static NPOI.HSSF.Util.HSSFColor;

namespace ITRIProject.Controllers
{
    public class MemberMangmentController : CommonController
    {
       
        private MyDbContext _MyDbContext;
        public MemberMangmentController(MyDbContext MyDbContext)
        {
            _MyDbContext = MyDbContext;
        }
        // GET: ProductController/Delete/5

        public IActionResult Index(int page = 1, int size = 10 ,int months = 0 )
        {
            //查詢會員
            var predicate = _MyDbContext.Members
              .GroupJoin(
                  _MyDbContext.Logins,
                  member => member.Account,
                  login => login.Account,
                  (member, logins) => new MemberManagement
                  {
                      MemberId = member.MemberId,
                      Account = member.Account,
                      UserType = member.UserType,
                      CompanyName = member.CompanyName,
                      Email = member.Email,
                      Name = member.Name,
                      Position = member.Position,
                      Tel = member.Tel,
                      Addr = member.Addr,
                      LoginDate = logins.OrderByDescending(login => login.LoginDate)
                                       .Select(login => (DateTime?)login.LoginDate)
                                       .FirstOrDefault() // Get the latest login date or null
                  })
              .ToList();
            
            //全部會員
            if (months==0)
            {
                var list = predicate.OrderByDescending( member => member.LoginDate )
                            .ToList()
                            .ToPagedList(pageNumber: page, pageSize: size);
                return View(list);
            }
            else
            {
                DateTime startDate = DateTime.Now.AddMonths(-months);//現在時間-選擇時間
                var endDate = DateTime.Now;
                var list = predicate.Where(member => member.LoginDate < startDate) // 添加日期范围过滤条件
                    .OrderByDescending(member => member.LoginDate)
                    .ToList()
                    .ToPagedList(pageNumber: page, pageSize: size);
                return View(list);
            };

        }
        public ActionResult Delete(int id)
        {
            try
            {
                var Members = _MyDbContext.Members.Find(id);

                _MyDbContext.Members.Remove(Members);


                _MyDbContext.SaveChanges();
                Members = null;



                return Sucess();
            }
            catch (Exception ex)
            {
                var Log = new Models.Log()
                {
                    WhereFunction = "MemberMangment_Delete_get",
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
