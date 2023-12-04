using System;
using System.Collections.Generic;

namespace Project.Models;

public partial class Organize
{
    /// <summary>
    /// 主鍵
    /// </summary>
    public int Id { get; set; }

    public int BasicId { get; set; }

    /// <summary>
    /// 盤查表
    /// </summary>
    public string Inventory { get; set; } = null!;

    /// <summary>
    /// 盤查開始時間
    /// </summary>
    public DateTime? StartTime { get; set; }

    /// <summary>
    /// 盤查結束時間
    /// </summary>
    public DateTime? EndTime { get; set; }

    /// <summary>
    /// 聯絡窗口
    /// </summary>
    public string? ContactName { get; set; }

    /// <summary>
    /// 更新時間
    /// </summary>
    public DateTime? UpdateTime { get; set; }

    /// <summary>
    /// 狀態
    /// </summary>
    public string? Status { get; set; }

    /// <summary>
    /// 帳號綁定
    /// </summary>
    public string? Account { get; set; }

    public string? EditLog { get; set; }
}
