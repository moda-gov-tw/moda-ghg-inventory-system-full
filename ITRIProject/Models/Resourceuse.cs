using System;
using System.Collections.Generic;

namespace ITRIProject.Models;

public partial class Resourceuse
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

    public int? SupplierId { get; set; }

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
    /// 總量	
    /// </summary>
    public decimal? BeforeTotal { get; set; }

    /// <summary>
    /// 單位	
    /// </summary>
    public string? BeforeUnit { get; set; }

    /// <summary>
    /// 單位轉換量	
    /// </summary>
    public decimal? ConvertNum { get; set; }

    /// <summary>
    /// 總量	
    /// </summary>
    public decimal? AfertTotal { get; set; }

    /// <summary>
    /// 單位	
    /// </summary>
    public string? AfertUnit { get; set; }

    /// <summary>
    /// 分配比率
    /// </summary>
    public decimal? DistributeRatio { get; set; }
}
