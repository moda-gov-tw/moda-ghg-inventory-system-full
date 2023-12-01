using System;
using System.Collections.Generic;

namespace ITRIProject.Models;

public partial class Passwordhistory
{
    /// <summary>
    /// 主鍵
    /// </summary>
    public int Id { get; set; }

    /// <summary>
    /// 帳號
    /// </summary>
    public string Account { get; set; } = null!;

    /// <summary>
    /// 密碼
    /// </summary>
    public string Password { get; set; } = null!;

    /// <summary>
    /// 創建時間
    /// </summary>
    public DateTime CreateTime { get; set; }

    public string? Email { get; set; }
}
