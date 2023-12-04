using System;
using System.Collections.Generic;

namespace Project.Models;

public partial class Energyuse
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

    public int? SupplierId { get; set; }

    /// <summary>
    /// 能源名稱
    /// </summary>
    public string? EnergyName { get; set; }

    /// <summary>
    /// 設備名稱
    /// </summary>
    public string? EquipmentName { get; set; }

    /// <summary>
    /// 設備位置
    /// </summary>
    public string? EquipmentLocation { get; set; }

    /// <summary>
    /// 供應商名稱
    /// </summary>
    public string? SupplierName { get; set; }

    /// <summary>
    /// 供應商地址
    /// </summary>
    public string? SupplierAddress { get; set; }

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

    public string? Account { get; set; }

    public DateTime? EditTime { get; set; }
}
