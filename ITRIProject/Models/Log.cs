using System;
using System.Collections.Generic;

namespace ITRIProject.Models;

public partial class Log
{
    public int Id { get; set; }

    /// <summary>
    /// 哪個方法發生錯誤
    /// </summary>
    public string? WhereFunction { get; set; }

    /// <summary>
    /// 錯誤內容
    /// </summary>
    public string? Exception { get; set; }

    /// <summary>
    /// 時間
    /// </summary>
    public DateTime? DateTime { get; set; }
}
