
# 開發環境
* 開發軟體 : Visual Studio 2022
* 開發框架 : NET 6
* 啓動環境 : IIS

# 專案說明
* DB    - 資料庫結構及基本資料
* Project   - GHG-數位產業自願性溫室氣體盤查系統

# 前置作業

* 匯入資料庫

* 設定各專案相關參數（appsettings.json）
```
{
  "ConnectionStrings": {
    "MySqlConnectionString": "Database=資料庫名稱;Data Source=位置;UserId=帳號;Password=密碼;charset=utf8mb4;port=port",
    "MySqlVersionString": "10.4.28-mariadb"
  },
  "EmailSet": {
    "IsOpen": 是否開啟寄信(true & false),
    "FromName": "信件title",
    "FromEmail": "信箱帳號",
    "FromPwd": "信箱密碼",
    "Smtp": "smtp",
    "Port": 發信port,
    "UseSsl": 是否開啟SSL(true & false)
  },
  "FileSave": {
    "Path": "檔案上傳路徑",
    "HtmlPath": "html下載路徑"
  },
  "Logging": {
    "LogLevel": {
      "Default": "Information",
      "Microsoft.AspNetCore": "Warning"
    }
  },
  "Urls": "https://localhost:7057",
  "AllowedHosts": "*"
}
```

# 專案建置

## 更新專案套件

* Visual Studio 工具

	- 工具 -> nuget 套件管理員 -> 套件管理員主控台

	- 指令 : Update-Package -reinstall

* 非 Visual Studio  工具

	- 可參考 https://learn.microsoft.com/en-us/nuget/reference/nuget-exe-cli-reference 做更新

## 專案相依性

可知目前專案對方案中各專案的相依性，以及確認目前專案是否都抓到套件，如有問題需做調整。

# 啟動架設站台 (IIS)

## IIS    

安裝  IIS

安裝  ASP.NET Core Hosting Bundle 

> https://dotnet.microsoft.com/en-us/download/dotnet/6.0

IIS 指定發布站台，並將應用程式集區，CLR 版本改為 "沒有受控(Managed)" 程式碼

# 管理系統測試登入資訊

帳號 : admin
密碼 : admin

---
# License

<table class=MsoTableGrid border=1 cellspacing=0 cellpadding=0
 style='border-collapse:collapse;border:none'>
 <thead>
  <tr style='height:19.85pt'>
   <td width=146 style='width:109.4pt;border:solid windowtext 1.0pt;background:
   #D9D9D9;padding:0cm 5.4pt 0cm 5.4pt;height:19.85pt'>
   <p class=MsoNormal align=center style='text-align:center;line-height:normal'><b><span
   lang=ZH-CN style='font-family:"微軟正黑體",sans-serif;color:black'>元件名稱</span></b></p>
   </td>
   <td width=140 style='width:104.65pt;border:solid windowtext 1.0pt;
   border-left:none;background:#D9D9D9;padding:0cm 5.4pt 0cm 5.4pt;height:19.85pt'>
   <p class=MsoNormal align=center style='text-align:center;line-height:normal'><b><span
   style='font-family:"微軟正黑體",sans-serif;color:black'>版本號</span></b></p>
   </td>
   <td width=127 style='width:95.05pt;border:solid windowtext 1.0pt;border-left:
   none;background:#D9D9D9;padding:0cm 5.4pt 0cm 5.4pt;height:19.85pt'>
   <p class=MsoNormal align=center style='text-align:center;line-height:normal'><b><span
   style='font-family:"微軟正黑體",sans-serif;color:black'>授權條款</span></b></p>
   </td>
   <td width=127 style='width:95.05pt;border:solid windowtext 1.0pt;border-left:
   none;background:#D9D9D9;padding:0cm 5.4pt 0cm 5.4pt;height:19.85pt'>
   <p class=MsoNormal align=center style='text-align:center'><b><span
   style='font-family:DengXian;color:black'>授權條款鏈接</span></b></p>
   </td>
  </tr>
 </thead>
 <tr style='height:19.85pt'>
  <td width=146 style='width:109.4pt;border:solid windowtext 1.0pt;border-top:
  none;padding:0cm 5.4pt 0cm 5.4pt;height:19.85pt'>
  <p class=MsoNormal align=center style='text-align:center;line-height:normal'><span
  lang=EN-US style='font-family:"微軟正黑體",sans-serif'>jsgrid</span></p>
  </td>
  <td width=140 style='width:104.65pt;border-top:none;border-left:none;
  border-bottom:solid windowtext 1.0pt;border-right:solid windowtext 1.0pt;
  padding:0cm 5.4pt 0cm 5.4pt;height:19.85pt'>
  <p class=MsoNormal align=center style='text-align:center;line-height:normal'><span
  lang=EN-US style='font-family:"微軟正黑體",sans-serif'>v.1.5.3</span></p>
  </td>
  <td width=127 style='width:95.05pt;border-top:none;border-left:none;
  border-bottom:solid windowtext 1.0pt;border-right:solid windowtext 1.0pt;
  padding:0cm 5.4pt 0cm 5.4pt;height:19.85pt'>
  <p class=MsoNormal align=center style='text-align:center;line-height:normal'><span
  lang=EN-US style='font-family:"微軟正黑體",sans-serif'>MIT</span></p>
  </td>
  <td width=127 style='width:95.05pt;border-top:none;border-left:none;
  border-bottom:solid windowtext 1.0pt;border-right:solid windowtext 1.0pt;
  padding:0cm 5.4pt 0cm 5.4pt;height:19.85pt'>
  <p class=MsoNormal align=center style='text-align:center'><span lang=EN-US><a
  href="https://github.com/tabalinas/jsgrid/blob/master/LICENSE">jsgrid/LICENSE
  at master · tabalinas/jsgrid · GitHub</a></span></p>
  </td>
 </tr>
 <tr style='height:19.85pt'>
  <td width=146 style='width:109.4pt;border:solid windowtext 1.0pt;border-top:
  none;padding:0cm 5.4pt 0cm 5.4pt;height:19.85pt'>
  <p class=MsoNormal align=center style='text-align:center;line-height:normal'><span
  lang=EN-US style='font-family:"微軟正黑體",sans-serif'>jQuery</span></p>
  </td>
  <td width=140 style='width:104.65pt;border-top:none;border-left:none;
  border-bottom:solid windowtext 1.0pt;border-right:solid windowtext 1.0pt;
  padding:0cm 5.4pt 0cm 5.4pt;height:19.85pt'>
  <p class=MsoNormal align=center style='text-align:center;line-height:normal'><span
  lang=EN-US style='font-family:"微軟正黑體",sans-serif'>v.3.5.1</span></p>
  </td>
  <td width=127 style='width:95.05pt;border-top:none;border-left:none;
  border-bottom:solid windowtext 1.0pt;border-right:solid windowtext 1.0pt;
  padding:0cm 5.4pt 0cm 5.4pt;height:19.85pt'>
  <p class=MsoNormal align=center style='text-align:center;line-height:normal'><span
  lang=EN-US style='font-family:"微軟正黑體",sans-serif'>MIT</span></p>
  </td>
  <td width=127 style='width:95.05pt;border-top:none;border-left:none;
  border-bottom:solid windowtext 1.0pt;border-right:solid windowtext 1.0pt;
  padding:0cm 5.4pt 0cm 5.4pt;height:19.85pt'>
  <p class=MsoNormal align=center style='text-align:center'><span lang=EN-US><a
  href="https://github.com/jquery/jquery/blob/main/LICENSE.txt">jquery/LICENSE.txt
  at main · jquery/jquery · GitHub</a></span></p>
  </td>
 </tr>
 <tr style='height:19.85pt'>
  <td width=146 style='width:109.4pt;border:solid windowtext 1.0pt;border-top:
  none;padding:0cm 5.4pt 0cm 5.4pt;height:19.85pt'>
  <p class=MsoNormal align=center style='text-align:center;line-height:normal'><span
  lang=EN-US style='font-family:"微軟正黑體",sans-serif'>jQuery Validation</span></p>
  </td>
  <td width=140 style='width:104.65pt;border-top:none;border-left:none;
  border-bottom:solid windowtext 1.0pt;border-right:solid windowtext 1.0pt;
  padding:0cm 5.4pt 0cm 5.4pt;height:19.85pt'>
  <p class=MsoNormal align=center style='text-align:center;line-height:normal'><span
  lang=EN-US style='font-family:"微軟正黑體",sans-serif'>v.1.17.0</span></p>
  </td>
  <td width=127 style='width:95.05pt;border-top:none;border-left:none;
  border-bottom:solid windowtext 1.0pt;border-right:solid windowtext 1.0pt;
  padding:0cm 5.4pt 0cm 5.4pt;height:19.85pt'>
  <p class=MsoNormal align=center style='text-align:center;line-height:normal'><span
  lang=EN-US style='font-family:"微軟正黑體",sans-serif'>MIT</span></p>
  </td>
  <td width=127 style='width:95.05pt;border-top:none;border-left:none;
  border-bottom:solid windowtext 1.0pt;border-right:solid windowtext 1.0pt;
  padding:0cm 5.4pt 0cm 5.4pt;height:19.85pt'>
  <p class=MsoNormal align=center style='text-align:center'><span lang=EN-US><a
  href="https://github.com/jquery-validation/jquery-validation/blob/master/LICENSE.md">jquery-validation/LICENSE.md
  at master · jquery-validation/jquery-validation · GitHub</a></span></p>
  </td>
 </tr>
 <tr style='height:19.85pt'>
  <td width=146 style='width:109.4pt;border:solid windowtext 1.0pt;border-top:
  none;padding:0cm 5.4pt 0cm 5.4pt;height:19.85pt'>
  <p class=MsoNormal align=center style='text-align:center;line-height:normal'><span
  lang=EN-US style='font-family:"微軟正黑體",sans-serif'>bootstrap</span></p>
  </td>
  <td width=140 style='width:104.65pt;border-top:none;border-left:none;
  border-bottom:solid windowtext 1.0pt;border-right:solid windowtext 1.0pt;
  padding:0cm 5.4pt 0cm 5.4pt;height:19.85pt'>
  <p class=MsoNormal align=center style='text-align:center;line-height:normal'><span
  lang=EN-US style='font-family:"微軟正黑體",sans-serif'>v.5.1.0</span></p>
  </td>
  <td width=127 style='width:95.05pt;border-top:none;border-left:none;
  border-bottom:solid windowtext 1.0pt;border-right:solid windowtext 1.0pt;
  padding:0cm 5.4pt 0cm 5.4pt;height:19.85pt'>
  <p class=MsoNormal align=center style='text-align:center;line-height:normal'><span
  lang=EN-US style='font-family:"微軟正黑體",sans-serif'>MIT</span></p>
  </td>
  <td width=127 style='width:95.05pt;border-top:none;border-left:none;
  border-bottom:solid windowtext 1.0pt;border-right:solid windowtext 1.0pt;
  padding:0cm 5.4pt 0cm 5.4pt;height:19.85pt'>
  <p class=MsoNormal align=center style='text-align:center'><span lang=EN-US><a
  href="https://github.com/twbs/bootstrap/blob/main/LICENSE">bootstrap/LICENSE
  at main · twbs/bootstrap · GitHub</a></span></p>
  </td>
 </tr>
 <tr style='height:19.85pt'>
  <td width=146 style='width:109.4pt;border:solid windowtext 1.0pt;border-top:
  none;padding:0cm 5.4pt 0cm 5.4pt;height:19.85pt'>
  <p class=MsoNormal align=center style='text-align:center;line-height:normal'><span
  lang=EN-US style='font-family:"微軟正黑體",sans-serif'>echarts</span></p>
  </td>
  <td width=140 style='width:104.65pt;border-top:none;border-left:none;
  border-bottom:solid windowtext 1.0pt;border-right:solid windowtext 1.0pt;
  padding:0cm 5.4pt 0cm 5.4pt;height:19.85pt'>
  <p class=MsoNormal align=center style='text-align:center;line-height:normal'><span
  lang=EN-US style='font-family:"微軟正黑體",sans-serif'>v.5.4.2</span></p>
  </td>
  <td width=127 style='width:95.05pt;border-top:none;border-left:none;
  border-bottom:solid windowtext 1.0pt;border-right:solid windowtext 1.0pt;
  padding:0cm 5.4pt 0cm 5.4pt;height:19.85pt'>
  <p class=MsoNormal align=center style='text-align:center;line-height:normal'><span
  lang=EN-US style='font-family:"微軟正黑體",sans-serif'>APL</span></p>
  </td>
  <td width=127 style='width:95.05pt;border-top:none;border-left:none;
  border-bottom:solid windowtext 1.0pt;border-right:solid windowtext 1.0pt;
  padding:0cm 5.4pt 0cm 5.4pt;height:19.85pt'>
  <p class=MsoNormal align=center style='text-align:center'><span lang=EN-US><a
  href="https://github.com/apache/echarts/blob/master/LICENSE">echarts/LICENSE
  at master · apache/echarts · GitHub</a></span></p>
  </td>
 </tr>
 <tr style='height:19.85pt'>
  <td width=146 style='width:109.4pt;border:solid windowtext 1.0pt;border-top:
  none;padding:0cm 5.4pt 0cm 5.4pt;height:19.85pt'>
  <p class=MsoNormal align=center style='text-align:center;line-height:normal'><span
  lang=EN-US style='font-family:"微軟正黑體",sans-serif'>Magnific-popup</span></p>
  </td>
  <td width=140 style='width:104.65pt;border-top:none;border-left:none;
  border-bottom:solid windowtext 1.0pt;border-right:solid windowtext 1.0pt;
  padding:0cm 5.4pt 0cm 5.4pt;height:19.85pt'>
  <p class=MsoNormal align=center style='text-align:center;line-height:normal'><span
  lang=EN-US style='font-family:"微軟正黑體",sans-serif'>v.1.1.0</span></p>
  </td>
  <td width=127 style='width:95.05pt;border-top:none;border-left:none;
  border-bottom:solid windowtext 1.0pt;border-right:solid windowtext 1.0pt;
  padding:0cm 5.4pt 0cm 5.4pt;height:19.85pt'>
  <p class=MsoNormal align=center style='text-align:center;line-height:normal'><span
  lang=EN-US style='font-family:"微軟正黑體",sans-serif'>MIT</span></p>
  </td>
  <td width=127 style='width:95.05pt;border-top:none;border-left:none;
  border-bottom:solid windowtext 1.0pt;border-right:solid windowtext 1.0pt;
  padding:0cm 5.4pt 0cm 5.4pt;height:19.85pt'>
  <p class=MsoNormal align=center style='text-align:center'><span lang=EN-US><a
  href="https://github.com/dimsemenov/Magnific-Popup/blob/master/LICENSE">Magnific-Popup/LICENSE
  at master · dimsemenov/Magnific-Popup · GitHub</a></span></p>
  </td>
 </tr>
 <tr style='height:19.85pt'>
  <td width=146 style='width:109.4pt;border:solid windowtext 1.0pt;border-top:
  none;padding:0cm 5.4pt 0cm 5.4pt;height:19.85pt'>
  <p class=MsoNormal align=center style='text-align:center;line-height:normal'><span
  lang=EN-US style='font-family:"微軟正黑體",sans-serif'>exceljs</span></p>
  </td>
  <td width=140 style='width:104.65pt;border-top:none;border-left:none;
  border-bottom:solid windowtext 1.0pt;border-right:solid windowtext 1.0pt;
  padding:0cm 5.4pt 0cm 5.4pt;height:19.85pt'>
  <p class=MsoNormal align=center style='text-align:center;line-height:normal'><span
  lang=EN-US style='font-family:"微軟正黑體",sans-serif'>v.4.4.0</span></p>
  </td>
  <td width=127 style='width:95.05pt;border-top:none;border-left:none;
  border-bottom:solid windowtext 1.0pt;border-right:solid windowtext 1.0pt;
  padding:0cm 5.4pt 0cm 5.4pt;height:19.85pt'>
  <p class=MsoNormal align=center style='text-align:center;line-height:normal'><span
  lang=EN-US style='font-family:"微軟正黑體",sans-serif'>MIT</span></p>
  </td>
  <td width=127 style='width:95.05pt;border-top:none;border-left:none;
  border-bottom:solid windowtext 1.0pt;border-right:solid windowtext 1.0pt;
  padding:0cm 5.4pt 0cm 5.4pt;height:19.85pt'>
  <p class=MsoNormal align=center style='text-align:center'><span lang=EN-US><a
  href="https://github.com/exceljs/exceljs/blob/master/LICENSE">exceljs/LICENSE
  at master · exceljs/exceljs · GitHub</a></span></p>
  </td>
 </tr>
 <tr style='height:19.85pt'>
  <td width=146 style='width:109.4pt;border:solid windowtext 1.0pt;border-top:
  none;padding:0cm 5.4pt 0cm 5.4pt;height:19.85pt'>
  <p class=MsoNormal align=center style='text-align:center;line-height:normal'><span
  lang=EN-US style='font-family:"微軟正黑體",sans-serif;'>jquery-table2excel</span></p>
  </td>
  <td width=140 style='width:104.65pt;border-top:none;border-left:none;
  border-bottom:solid windowtext 1.0pt;border-right:solid windowtext 1.0pt;
  padding:0cm 5.4pt 0cm 5.4pt;height:19.85pt'>
  <p class=MsoNormal align=center style='text-align:center;line-height:normal'><span
  lang=EN-US style='font-family:"微軟正黑體",sans-serif'>v.1.1.1</span></p>
  </td>
  <td width=127 style='width:95.05pt;border-top:none;border-left:none;
  border-bottom:solid windowtext 1.0pt;border-right:solid windowtext 1.0pt;
  padding:0cm 5.4pt 0cm 5.4pt;height:19.85pt'>
  <p class=MsoNormal align=center style='text-align:center;line-height:normal'><span
  lang=EN-US style='font-family:"微軟正黑體",sans-serif;'>MIT</span></p>
  </td>
  <td width=127 style='width:95.05pt;border-top:none;border-left:none;
  border-bottom:solid windowtext 1.0pt;border-right:solid windowtext 1.0pt;
  padding:0cm 5.4pt 0cm 5.4pt;height:19.85pt'>
  <p class=MsoNormal align=center style='text-align:center'><span lang=EN-US><a
  href="https://zenorocha.mit-license.org/">MIT License (mit-license.org)</a></span></p>
  </td>
 </tr>
 <tr style='height:19.85pt'>
  <td width=146 style='width:109.4pt;border:solid windowtext 1.0pt;border-top:
  none;padding:0cm 5.4pt 0cm 5.4pt;height:19.85pt'>
  <p class=MsoNormal align=center style='text-align:center;line-height:normal'><span
  lang=EN-US style='font-family:"微軟正黑體",sans-serif;'>MariaDB
  Server</span></p>
  </td>
  <td width=140 style='width:104.65pt;border-top:none;border-left:none;
  border-bottom:solid windowtext 1.0pt;border-right:solid windowtext 1.0pt;
  padding:0cm 5.4pt 0cm 5.4pt;height:19.85pt'>
  <p class=MsoNormal align=center style='text-align:center;line-height:normal'><span
  lang=EN-US style='font-family:"微軟正黑體",sans-serif'>v.11.2.0</span></p>
  </td>
  <td width=127 style='width:95.05pt;border-top:none;border-left:none;
  border-bottom:solid windowtext 1.0pt;border-right:solid windowtext 1.0pt;
  padding:0cm 5.4pt 0cm 5.4pt;height:19.85pt'>
  <p class=MsoNormal align=center style='text-align:center;line-height:normal'><span
  lang=EN-US style='font-family:"微軟正黑體",sans-serif;'>GPLv2</span></p>
  </td>
  <td width=127 style='width:95.05pt;border-top:none;border-left:none;
  border-bottom:solid windowtext 1.0pt;border-right:solid windowtext 1.0pt;
  padding:0cm 5.4pt 0cm 5.4pt;height:19.85pt'>
  <p class=MsoNormal align=center style='text-align:center'><span lang=EN-US><a
  href="https://github.com/MariaDB/server/blob/11.4/COPYING">server/COPYING at
  11.4 · MariaDB/server · GitHub</a></span></p>
  </td>
 </tr>
 <tr style='height:19.85pt'>
  <td width=146 style='width:109.4pt;border:solid windowtext 1.0pt;border-top:
  none;padding:0cm 5.4pt 0cm 5.4pt;height:19.85pt'>
  <p class=MsoNormal align=center style='text-align:center;line-height:normal'><span
  lang=EN-US style='font-family:"微軟正黑體",sans-serif;'>ASP.NET
  Core</span></p>
  </td>
  <td width=140 style='width:104.65pt;border-top:none;border-left:none;
  border-bottom:solid windowtext 1.0pt;border-right:solid windowtext 1.0pt;
  padding:0cm 5.4pt 0cm 5.4pt;height:19.85pt'>
  <p class=MsoNormal align=center style='text-align:center;line-height:normal'><span
  lang=EN-US style='font-family:"微軟正黑體",sans-serif'>v.7.0.8</span></p>
  </td>
  <td width=127 style='width:95.05pt;border-top:none;border-left:none;
  border-bottom:solid windowtext 1.0pt;border-right:solid windowtext 1.0pt;
  padding:0cm 5.4pt 0cm 5.4pt;height:19.85pt'>
  <p class=MsoNormal align=center style='text-align:center;line-height:normal;
  page-break-after:avoid'><span lang=EN-US style='font-family:"微軟正黑體",sans-serif;
  '>MIT</span></p>
  </td>
  <td width=127 style='width:95.05pt;border-top:none;border-left:none;
  border-bottom:solid windowtext 1.0pt;border-right:solid windowtext 1.0pt;
  padding:0cm 5.4pt 0cm 5.4pt;height:19.85pt'>
  <p class=MsoNormal align=center style='text-align:center;page-break-after:
  avoid'><span lang=EN-US><a
  href="https://github.com/dotnet/aspnetcore/blob/main/LICENSE.txt">aspnetcore/LICENSE.txt
  at main · dotnet/aspnetcore · GitHub</a></span></p>
  </td>
 </tr>
</table>
