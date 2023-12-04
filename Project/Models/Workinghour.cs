using System;
using System.Collections.Generic;

namespace Project.Models;

public partial class Workinghour
{
    public int Id { get; set; }

    /// <summary>
    /// 基本資料編號
    /// </summary>
    public int BasicId { get; set; }

    /// <summary>
    /// 工廠id
    /// </summary>
    public int FactoryId { get; set; }

    /// <summary>
    /// 項目
    /// </summary>
    public string? Item { get; set; }

    /// <summary>
    /// 總工時
    /// </summary>
    public decimal? TotalWorkHour { get; set; }

    /// <summary>
    /// 管理部門
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
    /// 分配方式
    /// </summary>
    public decimal? DistributeRatio { get; set; }
}
