using System;
using System.Collections.Generic;

namespace Project.Models;

public partial class Suppliermanage
{
    public int Id { get; set; }

    /// <summary>
    /// 帳號
    /// </summary>
    public string? Account { get; set; }

    /// <summary>
    /// 供應商名稱
    /// </summary>
    public string? SupplierName { get; set; }

    /// <summary>
    /// 供應商地址
    /// </summary>
    public string? SupplierAddress { get; set; }
}
