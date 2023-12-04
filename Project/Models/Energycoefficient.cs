using System;
using System.Collections.Generic;

namespace Project.Models;

public partial class Energycoefficient
{
    public int Id { get; set; }

    /// <summary>
    /// 排放源別
    /// </summary>
    public string? EmissionSource { get; set; }

    /// <summary>
    /// 能源類別
    /// </summary>
    public string? EnergyCategory { get; set; }

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

    public decimal? N2ocoefficient { get; set; }

    public string? N2ounit { get; set; }
}
