using System;
using System.Collections.Generic;

namespace Project.Models;

public partial class Basicdata
{
    public int BasicId { get; set; }

    /// <summary>
    /// 機關名稱
    /// </summary>
    public string? OrganName { get; set; }

    /// <summary>
    /// 機關代號
    /// </summary>
    public string? OrganNumber { get; set; }

    /// <summary>
    /// 聯絡人名稱
    /// </summary>
    public string? ContactPerson { get; set; }

    /// <summary>
    /// 聯絡人電話
    /// </summary>
    public string? ContactPhone { get; set; }

    /// <summary>
    /// 連絡人信箱
    /// </summary>
    public string? ContactEmail { get; set; }

    /// <summary>
    /// 開始時間
    /// </summary>
    public DateTime StartTime { get; set; }

    /// <summary>
    /// 結束時間
    /// </summary>
    public DateTime EndTime { get; set; }

    public string? Account { get; set; }
}
