using System;
using System.Collections.Generic;

namespace ITRIProject.Models;

public partial class Member
{
    /// <summary>
    /// 主鍵
    /// </summary>
    public int MemberId { get; set; }

    /// <summary>
    /// 帳號
    /// </summary>
    public string Account { get; set; } = null!;

    /// <summary>
    /// 密碼
    /// </summary>
    public string? Passwd { get; set; }

    /// <summary>
    /// 公司名稱
    /// </summary>
    public string? CompanyName { get; set; }

    /// <summary>
    /// 部門
    /// </summary>
    public string? Department { get; set; }

    /// <summary>
    /// 科別
    /// </summary>
    public string? OfficeLocation { get; set; }

    /// <summary>
    /// 職稱
    /// </summary>
    public string? Position { get; set; }

    /// <summary>
    /// 名稱
    /// </summary>
    public string? Name { get; set; }

    /// <summary>
    /// 權限
    /// </summary>
    public string? Permissions { get; set; }

    /// <summary>
    /// 聯絡地址
    /// </summary>
    public string? Addr { get; set; }

    /// <summary>
    /// 聯絡電話
    /// </summary>
    public string? Tel { get; set; }

    /// <summary>
    /// 電子信箱
    /// </summary>
    public string? Email { get; set; }

    /// <summary>
    /// 用戶類型
    /// </summary>
    public string? UserType { get; set; }

    /// <summary>
    /// 登入類型
    /// </summary>
    public string LoginType { get; set; } = null!;
}
