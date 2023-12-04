using Project.Common;
using Project.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using NPOI.POIFS.FileSystem;
using Newtonsoft.Json;
using NPOI.POIFS.Crypt.Dsig;
using Org.BouncyCastle.Asn1.X509;
using Org.BouncyCastle.Utilities;
using Org.BouncyCastle.Utilities.Collections;
using Project.Common;
using System.Security.Cryptography;
using System.Security.Cryptography.Pkcs;
using System.Security.Policy;
using System.Text;
using System.Text.RegularExpressions;
using static NPOI.HSSF.Util.HSSFColor;
using Microsoft.AspNetCore.Diagnostics;
using Newtonsoft.Json.Serialization;

namespace Project.Controllers
{
    public class MemberController : CommonController
    {
        private MyDbContext _MyDbContext;
        private readonly IConfiguration _configuration;
        private readonly ILogger<MemberController> _logger;

        public MemberController(MyDbContext MyDbContext, IConfiguration configuration, ILogger<MemberController> logger)
        {
            _MyDbContext = MyDbContext;
            _configuration = configuration;
            _logger = logger;
        }
        // GET: MemberController/Create
        public ActionResult Register()
        {

            return View();
        }

        // POST: MemberController/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Register(IFormCollection collection, string rePasswd, string Account)
        {
            var account = collection["Account"].FirstOrDefault();
            var passwd = collection["Passwd"].FirstOrDefault();
            var Email = collection["Email"].FirstOrDefault();

            var repasswd = rePasswd;

            var a = _MyDbContext.Members.Any(a => a.Account == account);
            var Emails = _MyDbContext.Members.Any(a => a.Email == Email);

            if (a)//判斷帳號是否存在
            {
                return Error("帳號已存在");
            }
            if (Emails)
            {
                return Error("信箱已存在");
            }
            //密碼複雜度
            if (string.IsNullOrEmpty(passwd) == false)
            {

                //檢查複雜規則:至少須包含1符號+1大寫英+1小寫英+1數字，字長在6~18
                Regex regexFormat = new Regex(@"^(?=.*\d)(?=.*[A-Z])(?=.*[a-z])(?=.*\W).{6,50}$");
                if (regexFormat.IsMatch(passwd) == false)
                {
                    return Error("密碼組成須有英文大寫小寫、數字及特殊符號");
                }
            }
            if (passwd != repasswd)
            {
                return Error("請輸入正確密碼");
            }
            var Member = _MyDbContext.Members.Add(new Member()
            {
                Account = Account.ToLower(),
                Passwd = MD5Encrypt(passwd),
                Name = collection["Name"],
                Email = collection["Email"],
                Position = collection["Position"],
                Addr = collection["Addr"],
                Tel = collection["Tel"],
                CompanyName = collection["CompanyName"],
                UserType = collection["UserType"],
                Permissions = "0",
                LoginType = "Home"
            });
            _MyDbContext.Passwordhistories.Add(new Passwordhistory()
            {
                Account = Account.ToLower(),
                Password = MD5Encrypt(passwd),
                CreateTime = DateTime.Now,
            });

            _MyDbContext.SaveChanges();
            int MemberId = Member.Entity.MemberId;//該筆資料的Id

            var Suppliermanage1 = new Suppliermanage()
            {
                Account = MemberId.ToString(),
                SupplierName = "台灣電力公司",
                SupplierAddress = "臺北市中正區文盛里7鄰羅斯福路三段242號",

            };
            _MyDbContext.Suppliermanages.Add(Suppliermanage1);
            var Suppliermanage2 = new Suppliermanage()
            {
                Account = MemberId.ToString(),
                SupplierName = "台灣自來水公司",
                SupplierAddress = "臺中市北區雙十路二段2-1號",

            };
            _MyDbContext.Suppliermanages.Add(Suppliermanage2);
            _MyDbContext.SaveChanges();
            return Sucess();

        }

        public ActionResult Login()
        {
            return View();
        }

        // POST: MemberController/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Login(IFormCollection collection, string captchaCode, string Account)
        {
            string? account = string.IsNullOrEmpty(Account) ? "" : Account.ToLower();
            string? passwd = collection["Passwd"];
            try
            {
                //var a = _MyDbContext.Members.Where(x => x.Account == account && x.Passwd == MD5Encrypt(passwd));
                //if (a.Count() == 0)
                //{
                //    return Error("帳號或密碼錯誤");

                //}
                //驗證圖形驗證碼
                if (!string.Equals(captchaCode, HttpContext.Session.GetString("CaptchaCode"), StringComparison.OrdinalIgnoreCase))
                {
                    return Error("驗證碼錯誤");
                }

                //查詢登入失敗紀錄
                var lastLoginFailure = _MyDbContext.Logins.OrderByDescending(l => l.LoginDate).FirstOrDefault(l => l.Account == account);


                if (lastLoginFailure != null)//檢查是否有登入失敗紀錄
                {
                    //上次失敗時間超過15分鐘則重置時間及次數
                    double loginTime = DateTime.Now.Subtract(lastLoginFailure.LoginDate).TotalMinutes;
                    if (loginTime >= 15)
                    {
                        var loginEntry = _MyDbContext.Logins.FirstOrDefault(it => it.Account == account);

                        if (loginEntry != null)
                        {
                            loginEntry.LoginFailures = 0;
                            loginEntry.LoginDate = lastLoginFailure.LoginDate;

                            _MyDbContext.SaveChanges();
                        }

                    }
                    // 檢查是否需要等待，15分鐘內錯誤超過5次
                    if (lastLoginFailure.LoginFailures >= 5 && loginTime < 15)
                    {
                        return Error("登入次數過多，請15分鐘後再試。");
                    }

                }
                else if (lastLoginFailure == null)
                {
                    // 沒有先前的登入失敗紀錄，創建新的登入紀錄
                    var userLogin = new Login();
                    userLogin.Account = account;
                    userLogin.LoginDate = DateTime.Now;
                    userLogin.LoginFailures = 0;
                    userLogin.State = "N";
                    _MyDbContext.Logins.Add(userLogin);
                    _MyDbContext.SaveChanges();
                }

                lastLoginFailure = _MyDbContext.Logins.OrderByDescending(l => l.LoginDate).FirstOrDefault(l => l.Account == account);

                Member member = _MyDbContext.Members.FirstOrDefault(x => x.Account == account && x.Passwd == MD5Encrypt(passwd));

                if (member == null)
                {
                    // 更新最後一次登入失敗的紀錄
                    lastLoginFailure.Account = account;
                    lastLoginFailure.State = "N";
                    lastLoginFailure.LoginFailures++;
                    lastLoginFailure.LoginDate = DateTime.Now;

                    var loginEntry = _MyDbContext.Logins.FirstOrDefault(it => it.Account == lastLoginFailure.Account);

                    if (loginEntry != null)
                    {
                        loginEntry.LoginFailures = lastLoginFailure.LoginFailures;
                        loginEntry.LoginDate = lastLoginFailure.LoginDate;
                        loginEntry.State = lastLoginFailure.State;

                        _MyDbContext.SaveChanges();
                    }

                    return Error("帳號或密碼錯誤");
                }
                else
                {
                    // 插入成功的登入紀錄
                    lastLoginFailure.Account = account;
                    lastLoginFailure.LoginDate = DateTime.Now;
                    lastLoginFailure.LoginFailures = 0;
                    lastLoginFailure.State = "Y";

                    var loginEntry = _MyDbContext.Logins.FirstOrDefault(it => it.Account == lastLoginFailure.Account);

                    if (loginEntry != null)
                    {
                        loginEntry.LoginFailures = lastLoginFailure.LoginFailures;
                        loginEntry.LoginDate = lastLoginFailure.LoginDate;
                        loginEntry.State = lastLoginFailure.State;

                        _MyDbContext.SaveChanges();
                    }
                }

                var history = _MyDbContext.Passwordhistories.OrderByDescending(h=>h.CreateTime).FirstOrDefault(h => h.Account == account && h.Password == MD5Encrypt(passwd));
                string? name = member.Name;
                string Permissions = member.Permissions;
                string LoginType = member.LoginType;
                string email = member.Email;
                //有資料，判斷是否超過半年
                if (history != null)
                {
                    //檢查是否超過半年未修改密碼
                    double HistoryTime = DateTime.Now.Subtract((DateTime)history.CreateTime).TotalDays;
                    if (HistoryTime > 180)
                    {
                        HttpContext.Session.SetString("AccountId", member.MemberId.ToString());
                        HttpContext.Session.SetString("Account", member.Account.ToString());
                        HttpContext.Session.SetString("LoginType", LoginType);
                        HttpContext.Session.SetString("name", name);
                        return Json(new { Editpasswd = true, Editpasswds = "密碼超過半年未修改，請盡速修改" });
                        
                    }
                }
                else if (history == null)
                {
                    var a = _MyDbContext.Members.FirstOrDefault(a => a.Account == account && a.Passwd == MD5Encrypt(passwd));
                    if (a != null)//判斷帳號是否存在
                    {
                        _MyDbContext.Passwordhistories.Add(new Passwordhistory()
                        {
                            Account = Account.ToLower(),
                            Password = MD5Encrypt(passwd),
                            Email = a.Email,
                            CreateTime = DateTime.Now,
                        });
                        _MyDbContext.SaveChanges();
                    }

                }



                HttpContext.Session.SetString("AccountId", member.MemberId.ToString());
                HttpContext.Session.SetString("Account", member.Account.ToString());
                HttpContext.Session.SetString("LoginType", LoginType);
                HttpContext.Session.SetString("name", name);
                HttpContext.Session.SetString("Permissions", Permissions);
                HttpContext.Session.SetString("email", email);

                return Sucess(Permissions);
            }
            catch (Exception ex)
            {
                var Log = new Log()
                {
                    WhereFunction = "Member_Login_POST",
                    Exception = ex.ToString() + "\n登入錯誤",
                    DateTime = DateTime.Now,
                };
                _MyDbContext.Logs.Add(Log);
                _MyDbContext.SaveChanges();
                Log = null;
                return Error("內部伺服器錯誤"); // 或其他適當的錯誤訊息
            }

        }
        public ActionResult Logout()
        {
            HttpContext.Session.Clear();
            return RedirectToAction("Login", "Member");
        }
        public ActionResult Member()
        {
            var model = _MyDbContext.Members.FirstOrDefault(a => a.MemberId.ToString() == AccountId());
            return View(model);
        }

        // POST: MemberController/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Member(IFormCollection collection, string rePasswd, string Account)
        {

            var model = _MyDbContext.Members.FirstOrDefault(a => a.MemberId.ToString() == AccountId());

            var emails = _MyDbContext.Members.Any(a=>a.MemberId.ToString() != AccountId() && a.Email == collection["Email"]);

            if (emails)
            {
                return Error("信箱已存在");
            }
            model.Name = collection["Name"];
            model.Email = collection["Email"];
            model.Position = collection["Position"];
            model.Addr = collection["Addr"];
            model.Tel = collection["Tel"];
            model.CompanyName = collection["CompanyName"];
            model.Department = collection["Department"];
            model.OfficeLocation = collection["OfficeLocation"];
            model.UserType = collection["UserType"];
            model.LoginType = "Home";
            _MyDbContext.Members.Update(model);
            _MyDbContext.SaveChanges();

            return Sucess();
        }
        [HttpPost]
        public IActionResult RegisterEmail(string email,string account)
        {
            try
            {
                var Emails = _MyDbContext.Members.Any(a => a.Email == email);
                var Accounts = _MyDbContext.Members.Any(a => a.Account == account);
                if (Accounts)
                {
                    return Error("帳號已存在");
                }
                if (Emails)
                {
                    return Error("信箱已存在");
                }
                VerifyCode model = new()
                {
                    Number = email,
                    VerifyCode1 = Guid.NewGuid().ToString("N").ToUpper().Substring(0, 6),
                    ExpireTime = DateTime.Now.AddMinutes(20),
                    IsVerify = 0
                };
                _MyDbContext.VerifyCodes.Add(model);
                _MyDbContext.SaveChanges();
                int newId = model.Id;

                //發Email
                MailHelper mailHelper = new();
                string text = $"您好\n您於 MODA平台（{AppSetting.GetValue("Urls")}）註冊帳號\n請輸入以下 {model.VerifyCode1} 驗證碼進行確認";
                string html = $"您好<br>您於<a href='{AppSetting.GetValue("Urls")}'>MODA平台</a>註冊帳號<br>請輸入以下 <b>{model.VerifyCode1} </b> 驗證碼進行確認";
                mailHelper.SendMail(email, "信箱驗證碼", html, "", html);

                return Sucess(newId);
            }
            catch (Exception ex)
            {
                var Log = new Log()
                {
                    WhereFunction = "RegisterEmail_Post",
                    Exception = ex.ToString(),
                    DateTime = DateTime.Now,
                };
                _MyDbContext.Logs.Add(Log);
                _MyDbContext.SaveChanges();
                Log = null;
                return Error("內部伺服器錯誤"); // 或其他適當的錯誤訊息
            }
        }
        
        public IActionResult EditPassword()
        {
            return View();
        }

        [HttpPost]
        
        //[ValidateAntiForgeryToken]
        public IActionResult EditPassword(IFormCollection collection)
        {
            try
            {
                string? Passwd = collection["Passwd"];
                string? newpassword = collection["newpassword"];
                Passwd = MD5Encrypt(Passwd);

                var model = _MyDbContext.Members.FirstOrDefault(u => u.MemberId.ToString() == AccountId() && u.Passwd == Passwd);
                if (model == null)
                {
                    return Error("舊密碼錯誤，請檢查");
                }
                if (string.IsNullOrEmpty(newpassword) == false && newpassword.Length >= 6)
                {

                    //檢查複雜規則:至少須包含1符號+1大寫英+1小寫英+1數字，字長在6~18
                    Regex regexFormat = new Regex(@"^(?=.*\d)(?=.*[A-Z])(?=.*[a-z])(?=.*\W).{6,18}$");
                    if (regexFormat.IsMatch(newpassword) == false)
                    {
                        return Error("密碼組成須有英文大寫小寫、數字及特殊符號");
                    }
                }

                newpassword = MD5Encrypt(newpassword);
                //確認是否與前三次相同

                var sphList = _MyDbContext.Passwordhistories.AsQueryable()
                .Where(sph => sph.Account == Account())
                .OrderByDescending(sph => sph.CreateTime)
                .Take(3)
                .ToList();

                foreach (var sph in sphList)
                {
                    if (sph.Password == newpassword)
                    {
                        return Error("密碼不能和上次相同");
                    }
                }

                model.Passwd = newpassword;

                Passwordhistory history = new Passwordhistory();
                history.Account = model.Account;
                history.Password = model.Passwd;
                history.Email = model.Email;
                history.CreateTime = DateTime.Now;
                _MyDbContext.Passwordhistories.Add(history);
                _MyDbContext.SaveChanges();

                string Permissions = model.Permissions;
                HttpContext.Session.SetString("Permissions", Permissions);
				return Sucess();
            }
            catch (Exception ex)
            {
                var Log = new Log()
                {
                    WhereFunction = "EditPassword_Post",
                    Exception = ex.ToString(),
                    DateTime = DateTime.Now,
                };
                _MyDbContext.Logs.Add(Log);
                _MyDbContext.SaveChanges();
                Log = null;
                return Error("內部伺服器錯誤"); // 或其他適當的錯誤訊息
            }


        }

        public ActionResult ForgetPwd()
        {

            return View();
        }
        [HttpPost]
		[ValidateAntiForgeryToken]
		public IActionResult ForgetPwd(string email, string vercode,string Register)
        {
            try
            {
                email = email == null ? "" : email.Trim();
                vercode = vercode == null ? "" : vercode.Trim();
                if (string.IsNullOrEmpty(email))
                {
                    return Error("email不能為空");
                }
                if (string.IsNullOrEmpty(vercode))
                {
                    return Error("驗證碼不能為空");
                }
                //驗證碼            
                var verId = Request.Form["verId"];
                if (string.IsNullOrEmpty(verId))
                {
                    return Error("郵箱驗證碼不存在");
                }
                var code = _MyDbContext.VerifyCodes.AsQueryable().First(a => a.Id == verId.ToString().ParseToInt());
                if (code == null)
                {
                    return Error("郵箱驗證碼不存在");
                }
                if (code.Number != email)
                {
                    return Error("郵箱驗證碼與郵箱不匹配");
                }
                if (code.VerifyCode1 != vercode)
                {
                    return Error("郵箱驗證碼不正確");
                }
                if (code.IsVerify == 0)
                {
                    if (code.ExpireTime != null && code.ExpireTime < DateTime.Now)
                    {
                        return Error("郵箱驗證碼已過期，請重新獲取");
                    }
                    code.IsVerify = 1;
                    code.VerifyTime = DateTime.Now;
                    _MyDbContext.VerifyCodes.Update(code);
                    _MyDbContext.SaveChanges();
                }

                var user = _MyDbContext.Members.FirstOrDefault(u => u.Email == email);
                if (user == null && Register !="0")
                {
                    return Error("用戶不存在");
                }
                HttpContext.Session.SetString("email", email);

                return Sucess();
            }
            catch (Exception ex)
            {
                var Log = new Log()
                {
                    WhereFunction = "ForgetPwd_Post",
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
        public IActionResult ForgetPwdEmail(string email)
        {
            try
            {
                email = email == null ? "" : email.Trim();
                if (string.IsNullOrEmpty(email))
                {
                    return Error("email不能為空");
                }

                var user = _MyDbContext.Members.FirstOrDefault(u => u.Email == email);
                if (user == null)
                {
                    return Error("查無此用戶");
                }

                VerifyCode model = new()
                {
                    Number = email,
                    VerifyCode1 = Guid.NewGuid().ToString("N").ToUpper().Substring(0, 6),
                    ExpireTime = DateTime.Now.AddMinutes(20),
                    IsVerify = 0
                };
                _MyDbContext.VerifyCodes.Add(model);
                _MyDbContext.SaveChanges();
                int newId = model.Id;

                //發Email
                MailHelper mailHelper = new();
                string text = $"您好\n您於 MODA平台（{AppSetting.GetValue("Urls")}）找回密碼\n請輸入以下 {model.VerifyCode1} 驗證碼進行確認";
                string html = $"您好<br>您於<a href='{AppSetting.GetValue("Urls")}'>MODA平台</a>找回密碼<br>請輸入以下 <b>{model.VerifyCode1} </b> 驗證碼進行確認";
                mailHelper.SendMail(email, "MODA平台找回密碼", html, "", html);

                return Sucess(newId);
            }
            catch (Exception ex)
            {
                var Log = new Log()
                {
                    WhereFunction = "ForgetPwdEmail_Post",
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
        public IActionResult ForgetPwd2(string newpassword)
        {
            try
            {
                var vermail = HttpContext.Session.GetString("email");
                var model = _MyDbContext.Members.FirstOrDefault(u => u.Email == vermail);
                if (model == null)
                {
                    return Error("無此帳號");
                }
                if (string.IsNullOrEmpty(newpassword) == false && newpassword.Length >= 6)
                {

                    //檢查複雜規則:至少須包含1符號+1大寫英+1小寫英+1數字，字長在6~18
                    Regex regexFormat = new Regex(@"^(?=.*\d)(?=.*[A-Z])(?=.*[a-z])(?=.*\W).{6,18}$");
                    if (regexFormat.IsMatch(newpassword) == false)
                    {
                        return Error("密碼組成須有英文大寫小寫、數字及特殊符號");
                    }
                }

                newpassword = MD5Encrypt(newpassword);
                //確認是否與前三次相同

                var sphList = _MyDbContext.Passwordhistories.AsQueryable()
                .Where(sph => sph.Account == vermail)
                .OrderByDescending(sph => sph.CreateTime)
                .Take(3)
                .ToList();

                foreach (var sph in sphList)
                {
                    if (sph.Password == newpassword)
                    {
                        return Error("密碼不能和上次相同");
                    }
                }

                model.Passwd = newpassword;

                Passwordhistory history = new Passwordhistory();
                history.Account = model.Account;
                //history.Email = model.Email;
                history.Password = model.Passwd;
                history.CreateTime = DateTime.Now;
                _MyDbContext.Passwordhistories.Add(history);
                _MyDbContext.SaveChanges();

                return Sucess();
            }
            catch (Exception ex)
            {
                var Log = new Log()
                {
                    WhereFunction = "ForgetPwd2_Post",
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
