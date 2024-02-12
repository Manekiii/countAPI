using System;
using System.Collections.Generic;

namespace countApi.Models;

public partial class Field
{
    public int Id { get; set; }

    public string? Group { get; set; }

    public string? Name { get; set; }

    public string Status { get; set; } = null!;

    public byte IsDelete { get; set; }

    public int? CreatedBy { get; set; }

    public DateTime CreatedDate { get; set; }

    public int? ModifiedBy { get; set; }

    public DateTime? ModifiedDate { get; set; }
}
