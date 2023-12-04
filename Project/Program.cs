using Project.Common;
using Project.Models;
using Microsoft.EntityFrameworkCore;
using System.Security.Cryptography;
using System.Security.Policy;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();
builder.Services.AddDbContext<MyDbContext>(options =>
{
    string DbconnectionString = builder.Configuration.GetConnectionString("MySqlConnectionString");
    options.UseMySql(DbconnectionString, new MySqlServerVersion(new Version(8, 10, 11)));
});

builder.Services.AddControllersWithViews();
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromHours(2);
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});

builder.Services.AddSingleton<IConfiguration>(builder.Configuration);

var app = builder.Build();


app.UseCookiePolicy(new CookiePolicyOptions
{
    HttpOnly = Microsoft.AspNetCore.CookiePolicy.HttpOnlyPolicy.Always,
    MinimumSameSitePolicy = SameSiteMode.Strict,
    Secure = CookieSecurePolicy.Always
});

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseStatusCodePagesWithReExecute("/Home/Error/{0}");
    app.UseHsts();
}

// 跳轉頁面的安全性標頭
app.UseMiddleware<RemoveServerHeaderMiddleware>();

app.UseSession();

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();
app.UseAuthorization();

app.Use(async (context, next) =>
{
    var hicos = AppSetting.GetValue("HiCOS:Switch");
    context.Session.SetString("HiCOS_TF", hicos=="1"?"true":"false");

    #region CSP標頭設置
    var nonce = Convert.ToBase64String(RandomNumberGenerator.GetBytes(32));
    context.Items["CSPNonce"] = nonce;
    #endregion

    await next();
});

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Member}/{action=Login}/{id?}");

app.Run();

#region 跨頁跳轉安全性規定
public class RemoveServerHeaderMiddleware
{
    private readonly RequestDelegate _next;

    public RemoveServerHeaderMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    public async Task Invoke(HttpContext context)
    {
        context.Response.StatusCode = StatusCodes.Status200OK;

        #region CSP標頭設置
        var nonce = Convert.ToBase64String(RandomNumberGenerator.GetBytes(32));
        context.Items["CSPNonce"] = nonce;
        #endregion

        await _next(context);
    }
}
#endregion

