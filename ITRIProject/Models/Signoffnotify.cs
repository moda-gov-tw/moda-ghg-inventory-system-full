using System;
using System.Collections.Generic;

namespace ITRIProject.Models;

public partial class Signoffnotify
{
    public int Id { get; set; }

    public int BasicId { get; set; }

    /// <summary>
    /// 發送人
    /// </summary>
    public string Sender { get; set; } = null!;

    /// <summary>
    /// 組織名稱
    /// </summary>
    public string OrganName { get; set; } = null!;

    /// <summary>
    /// 發送時間
    /// </summary>
    public DateTime ApplyTime { get; set; }

    /// <summary>
    /// 簽和狀態
    /// </summary>
    public string Status { get; set; } = null!;

    /// <summary>
    /// 收件人
    /// </summary>
    public string Recipient { get; set; } = null!;

    /// <summary>
    /// 是否讀取
    /// </summary>
    public ulong IsRead { get; set; }
}
