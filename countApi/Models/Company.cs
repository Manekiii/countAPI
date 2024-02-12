using System;
using System.Collections.Generic;

namespace countApi.Models;

public partial class Company
{
    public int Id { get; set; }

    public string? Code { get; set; }

    public string? Name { get; set; }

    public string? ContactNumber { get; set; }

    public byte Status { get; set; }

    public byte IsDelete { get; set; }

    public long? CreatedByUserId { get; set; }

    public DateTime CreatedDate { get; set; }

    public long? ModifiedByUserId { get; set; }

    public DateTime? ModifiedDate { get; set; }

    public virtual ICollection<Branch>? Branches { get; } = new List<Branch>();

    public virtual ICollection<UserDetail>? UserDetails { get; } = new List<UserDetail>();
}
