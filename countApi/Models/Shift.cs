using System;
using System.Collections.Generic;

namespace countApi.Models;

public partial class Shift
{
    public int Id { get; set; }

    public string? Name { get; set; }

    public string? BranchCode { get; set; }

    public byte Status { get; set; }

    public byte IsDelete { get; set; }

    public long? CreatedByUserId { get; set; }

    public DateTime CreatedDate { get; set; }

    public long? ModifiedByUserId { get; set; }

    public DateTime? ModifiedDate { get; set; }

    public virtual Branch? BranchCodeNavigation { get; set; }

    public virtual ICollection<UserShift>? UserShifts { get; } = new List<UserShift>();
}
