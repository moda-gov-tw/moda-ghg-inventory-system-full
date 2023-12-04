using System;
using System.Collections.Generic;

namespace Project.Models;

public partial class DDatasource
{
    public int Id { get; set; }

    public int BindId { get; set; }

    public string BindWhere { get; set; } = null!;

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
    /// 佐證資料
    /// </summary>
    public string? EvidenceFile { get; set; }
}
