using System;
using System.Collections.Generic;

namespace ITRIProject.Models;

/// <summary>
/// 驗證表
/// </summary>
public partial class VerifyCode
{
    /// <summary>
    /// 主鍵
    /// </summary>
    public int Id { get; set; }

    /// <summary>
    /// 驗證號碼
    /// </summary>
    public string Number { get; set; } = null!;

    /// <summary>
    /// 驗證碼
    /// </summary>
    public string VerifyCode1 { get; set; } = null!;

    /// <summary>
    /// 過期時間
    /// </summary>
    public DateTime? ExpireTime { get; set; }

    /// <summary>
    /// 是否已驗證
    /// </summary>
    public ulong IsVerify { get; set; }

    /// <summary>
    /// 驗證時間
    /// </summary>
    public DateTime? VerifyTime { get; set; }

    /// <summary>
    /// 創建時間
    /// </summary>
    public DateTime? CreateTime { get; set; }
}
