using System;
using System.Collections.Generic;

namespace Project.Models;

public partial class BasicdataFactoryaddress
{
    public int Id { get; set; }

    /// <summary>
    /// 基本資料編號
    /// </summary>
    public int BasicId { get; set; }

    /// <summary>
    /// 名稱
    /// </summary>
    public string? Name { get; set; }

    /// <summary>
    /// 地址
    /// </summary>
    public string? Address { get; set; }

    /// <summary>
    /// 盤查哪裡
    /// </summary>
    public string? WherePlace { get; set; }

    /// <summary>
    /// 輪班方式
    /// </summary>
    public string? ShiftMethod { get; set; }

    public int? Sort { get; set; }
}
