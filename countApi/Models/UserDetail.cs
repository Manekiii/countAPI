using System;
using System.Collections.Generic;

namespace countApi.Models;

public partial class UserDetail
{
    public int Id { get; set; }

    public long UserId { get; set; }

    public int? ReferenceId { get; set; }

    public byte IsExternal { get; set; }

    public int? CompanyId { get; set; }

    public string? Address { get; set; }

    public long? CreatedByUserId { get; set; }

    public DateTime CreatedDate { get; set; }

    public long? ModifiedByUserId { get; set; }

    public DateTime? ModifiedDate { get; set; }

    public virtual Company? Company { get; set; }

    public virtual User? User { get; set; } = null!;
}
