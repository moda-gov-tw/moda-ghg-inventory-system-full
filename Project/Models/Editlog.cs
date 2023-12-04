using System;
using System.Collections.Generic;

namespace Project.Models;

public partial class Editlog
{
    public int Id { get; set; }

    public int BasicId { get; set; }

    public string Account { get; set; } = null!;

    public string? Log { get; set; }
}
