using System;
using System.Collections.Generic;

namespace ITRIProject.Models;

public partial class Selectdatum
{
    public int Id { get; set; }

    /// <summary>
    /// 編號
    /// </summary>
    public string Code { get; set; } = null!;

    public string? Type { get; set; }

    public string? Name { get; set; }

    /// <summary>
    /// 排序
    /// </summary>
    public int Sort { get; set; }
}
