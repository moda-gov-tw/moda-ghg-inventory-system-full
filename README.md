
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
