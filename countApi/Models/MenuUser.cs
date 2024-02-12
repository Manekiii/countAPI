using System;
using System.Collections.Generic;

namespace countApi.Models;

public partial class MenuUser
{
    public int MenuId { get; set; }

    public int UserId { get; set; }

    public byte Add { get; set; }

    public byte Edit { get; set; }

    public byte Delete { get; set; }

    public virtual Menu Menu { get; set; } = null!;

    public virtual User User { get; set; } = null!;
}
