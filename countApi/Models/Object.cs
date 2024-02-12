using System;
using System.Collections.Generic;

namespace countApi.Models;

public partial class Object
{
    public int Id { get; set; }

    public string? Form { get; set; }

    public string Alias { get; set; } = null!;

    public string Name { get; set; } = null!;

    public string? Placeholder { get; set; }

    /// <summary>
    /// Text, Number, Date
    /// </summary>
    public string InputType { get; set; } = null!;

    public byte ScanCapable { get; set; }

    public byte IsRequired { get; set; }

    public string? BranchCode { get; set; }

    public int? Sort { get; set; }

    public byte Status { get; set; }

    public byte IsDelete { get; set; }

    public long? CreatedByUserId { get; set; }

    public DateTime CreatedDate { get; set; }

    public long? ModifiedByUserId { get; set; }

    public DateTime? ModifiedDate { get; set; }
}
