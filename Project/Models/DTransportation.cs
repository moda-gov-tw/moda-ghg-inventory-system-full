using System;
using System.Collections.Generic;

namespace Project.Models;

public partial class DTransportation
{
    /// <summary>
    /// 主鍵
    /// </summary>
    public int Id { get; set; }

    /// <summary>
    /// 綁定表Id
    /// </summary>
    public int BindId { get; set; }

    /// <summary>
    /// 綁定哪個表
    /// </summary>
    public string BindWhere { get; set; } = null!;

    /// <summary>
    /// 運輸方式
    /// </summary>
    public string? Method { get; set; }

    /// <summary>
    /// 起點
    /// </summary>
    public string? StartLocation { get; set; }

    /// <summary>
    /// 終點
    /// </summary>
    public string? EndLocation { get; set; }

    /// <summary>
    /// 車種
    /// </summary>
    public string? Car { get; set; }

    /// <summary>
    /// 噸數
    /// </summary>
    public decimal? Tonnes { get; set; }

    /// <summary>
    /// 燃料
    /// </summary>
    public string? Fuel { get; set; }

    /// <summary>
    /// 陸運
    /// </summary>
    public decimal? Land { get; set; }

    /// <summary>
    /// 海運
    /// </summary>
    public decimal? Sea { get; set; }

    /// <summary>
    /// 空運
    /// </summary>
    public decimal? Air { get; set; }
}
