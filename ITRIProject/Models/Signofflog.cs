using System;
using System.Collections.Generic;

namespace ITRIProject.Models;

public partial class Signofflog
{
    public int Id { get; set; }

    public int BasicId { get; set; }

    public string Sender { get; set; } = null!;

    public string? Reviewer { get; set; }

    public string? Status { get; set; }

    public string? Explanation { get; set; }

    public DateTime? ReviewTime { get; set; }
}
