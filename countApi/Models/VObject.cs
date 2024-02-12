using System;
using System.Collections.Generic;

namespace countApi.Models;

public partial class VObject
{
    public int Id { get; set; }

    public string? Form { get; set; }

    public string Alias { get; set; } = null!;

    public string Name { get; set; } = null!;

    public string? Placeholder { get; set; }

    public string InputType { get; set; } = null!;

    public byte ScanCapable { get; set; }

    public string? BranchCode { get; set; }

    public string? BranchName { get; set; }

    public byte Status { get; set; }

    public string StatusName { get; set; } = null!;

    public byte IsDelete { get; set; }

    public long? CreatedByUserId { get; set; }

    public string? CreatedBy { get; set; }

    public DateTime CreatedDate { get; set; }

    public long? ModifiedByUserId { get; set; }

    public string? ModifiedBy { get; set; }

    public DateTime? ModifiedDate { get; set; }
}
