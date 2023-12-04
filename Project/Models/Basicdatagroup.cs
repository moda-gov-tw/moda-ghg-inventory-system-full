using System;
using System.Collections.Generic;

namespace Project.Models;

public partial class Basicdatagroup
{
    public int Id { get; set; }

    public string Account { get; set; } = null!;

    public string? Name { get; set; }

    public int BasicId { get; set; }
}
