using System;
using System.Collections.Generic;

namespace Project.Models;

public partial class Evidencefilemanage
{
    /// <summary>
    /// 主鍵
    /// </summary>
    public int Id { get; set; }

    /// <summary>
    /// 基本資料編號
    /// </summary>
    public int BasicId { get; set; }

    /// <summary>
    /// 哪筆資料
    /// </summary>
    public int ItemId { get; set; }

    /// <summary>
    /// 檔案名稱
    /// </summary>
    public string FileName { get; set; } = null!;

    /// <summary>
    /// 哪個表的檔案
    /// </summary>
    public string WhereForm { get; set; } = null!;

    /// <summary>
    /// 時間
    /// </summary>
    public DateTime Time { get; set; }
}
