using System;
using System.Collections.Generic;

namespace Project.Models;

public partial class DumptreatmentOutsourcing
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

    public int? WasteId { get; set; }

    /// <summary>
    /// 空水廢名稱
    /// </summary>
    public string? WasteName { get; set; }

    /// <summary>
    /// 空水廢處理方法
    /// </summary>
    public string? WasteMethod { get; set; }

    /// <summary>
    /// 清運商名稱
    /// </summary>
    public string? CleanerName { get; set; }

    /// <summary>
    /// 最終處理地址
    /// </summary>
    public string? FinalAddress { get; set; }

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
    /// 單位轉換輛
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
