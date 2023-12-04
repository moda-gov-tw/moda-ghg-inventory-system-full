using System;
using System.Collections.Generic;

namespace Project.Models;

public partial class RefrigerantHave
{
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
    /// 設備類型
    /// </summary>
    public string? EquipmentType { get; set; }

    /// <summary>
    /// 冷媒種類
    /// </summary>
    public string? RefrigerantType { get; set; }

    /// <summary>
    /// 管理單位
    /// </summary>
    public string? Management { get; set; }

    /// <summary>
    /// 負責人員
    /// </summary>
    public string? Principal { get; set; }

    /// <summary>
    /// 數據來源
    /// </summary>
    public string? Datasource { get; set; }

    /// <summary>
    /// 備註
    /// </summary>
    public string? Remark { get; set; }

    /// <summary>
    /// 總量
    /// </summary>
    public decimal? Total { get; set; }

    /// <summary>
    /// 單位
    /// </summary>
    public string? Unit { get; set; }

    /// <summary>
    /// 分配比率
    /// </summary>
    public decimal? DistributeRatio { get; set; }
}
