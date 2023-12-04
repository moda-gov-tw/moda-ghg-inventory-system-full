using System;
using System.Collections.Generic;

namespace Project.Models;

public partial class Signoff
{
    /// <summary>
    /// 主鍵
    /// </summary>
    public int Id { get; set; }

    public int BasicId { get; set; }

    public string Sender { get; set; } = null!;

    public string OrganName { get; set; } = null!;

    public string OrganNumber { get; set; } = null!;

    public string ContactPerson { get; set; } = null!;

    public string ContactPhone { get; set; } = null!;

    public string ContactEmail { get; set; } = null!;

    public DateTime ApplyTime { get; set; }

    public DateTime? ReviewTime { get; set; }

    public string Status { get; set; } = null!;

    /// <summary>
    /// 審核人
    /// </summary>
    public string? Reviewer { get; set; }
}
