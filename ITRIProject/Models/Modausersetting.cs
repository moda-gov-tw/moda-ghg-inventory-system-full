using System;
using System.Collections.Generic;

namespace ITRIProject.Models;

public partial class Modausersetting
{
    public int Id { get; set; }

    /// <summary>
    /// 名字
    /// </summary>
    public string? Name { get; set; }

    /// <summary>
    /// 郵件信箱
    /// </summary>
    public string? Email { get; set; }

    /// <summary>
    /// 公司名稱
    /// </summary>
    public string? CompanyName { get; set; }

    /// <summary>
    /// 部門
    /// </summary>
    public string? Department { get; set; }

    /// <summary>
    /// 科別
    /// </summary>
    public string? OfficeLocation { get; set; }

    /// <summary>
    /// 職位
    /// </summary>
    public string? Position { get; set; }
}
