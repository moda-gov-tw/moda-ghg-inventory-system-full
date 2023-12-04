using System;
using System.Collections.Generic;

namespace Project.Models;

public partial class Coefficient
{
    public int Id { get; set; }

    /// <summary>
    /// 排放源別
    /// </summary>
    public string? EmissionSource { get; set; }

    /// <summary>
    /// 名稱
    /// </summary>
    public string? Name { get; set; }

    /// <summary>
    /// 處理方法
    /// </summary>
    public string? WasteMethod { get; set; }

    public string? GreenhouseGases { get; set; }

    /// <summary>
    /// 單位
    /// </summary>
    public string? Unit { get; set; }

    /// <summary>
    /// 範疇別
    /// </summary>
    public string? Category { get; set; }

    public decimal? Co2coefficient { get; set; }

    public string? Co2unit { get; set; }

    public decimal? Ch4coefficient { get; set; }

    public string? Ch4unit { get; set; }

    public decimal? Ch4gwp { get; set; }

    public decimal? N2ocoefficient { get; set; }

    public string? N2ounit { get; set; }

    public decimal? N2ogwp { get; set; }

    public decimal? HfcsGwp { get; set; }

    public decimal? PfcsGwp { get; set; }

    public decimal? Sf6gwp { get; set; }

    public string? Type { get; set; }
}
