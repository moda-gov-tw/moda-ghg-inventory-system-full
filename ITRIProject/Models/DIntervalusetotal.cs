using System;
using System.Collections.Generic;

namespace ITRIProject.Models;

public partial class DIntervalusetotal
{
    /// <summary>
    /// 主鍵
    /// </summary>
    public int Id { get; set; }

    /// <summary>
    /// 綁定表id
    /// </summary>
    public int BindId { get; set; }

    /// <summary>
    /// 數值
    /// </summary>
    public string? Num { get; set; }

    /// <summary>
    /// 綁定哪個表
    /// </summary>
    public string? BindWhere { get; set; }

    /// <summary>
    /// 類型
    /// </summary>
    public string Type { get; set; } = null!;

    /// <summary>
    /// 陣列取值用
    /// </summary>
    public int ArraySort { get; set; }
}
