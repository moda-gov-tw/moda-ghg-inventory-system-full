using System;
using System.Collections.Generic;

namespace Project.Models;

public partial class Fireequipment
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
    /// 廠址	
    /// </summary>
    public string? FactoryName { get; set; }

    /// <summary>
    /// 設備名稱
    /// </summary>
    public string? EquipmentName { get; set; }

    /// <summary>
    /// 溫室氣體種類
    /// </summary>
    public string? Ghgtype { get; set; }

    /// <summary>
    /// 備註
    /// </summary>
    public string? Remark { get; set; }

    /// <summary>
    /// 轉換前總量
    /// </summary>
    public decimal? BeforeTotal { get; set; }

    /// <summary>
    /// 轉換前單位
    /// </summary>
    public string? BeforeUnit { get; set; }

    /// <summary>
    /// 單位轉換之值
    /// </summary>
    public decimal? ConvertNum { get; set; }

    /// <summary>
    /// 轉換後總量
    /// </summary>
    public decimal? AfertTotal { get; set; }

    /// <summary>
    /// 轉換後單位
    /// </summary>
    public string? AfertUnit { get; set; }

    /// <summary>
    /// 分配比率
    /// </summary>
    public decimal? DistributeRatio { get; set; }
}
