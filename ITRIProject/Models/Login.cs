using System;
using System.Collections.Generic;

namespace ITRIProject.Models;

public partial class Login
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
    /// 狀態
    /// </summary>
    public string State { get; set; } = null!;

    /// <summary>
    /// 時間
    /// </summary>
    public DateTime LoginDate { get; set; }

    /// <summary>
    /// 登入失敗次數
    /// </summary>
    public int LoginFailures { get; set; }
}
