using System;
using System.Collections.Generic;

namespace countApi.Models;

public partial class Branch
{
    public int Id { get; set; }

    public string Code { get; set; } = null!;

    public string? Alias { get; set; }

    public string? Name { get; set; }

    public string? Address { get; set; }

    public string? ContactId { get; set; }

    public string? ContactName { get; set; }

    public string? ContactNumber { get; set; }

    public string? ContactEmail { get; set; }

    public string? Area { get; set; }

    public string? Region { get; set; }

    public string? MapReference { get; set; }

    public string? Latitude { get; set; }

    public string? Longitude { get; set; }

    public int? CompanyId { get; set; }

    public int? GroupId { get; set; }

    public byte Status { get; set; }

    public byte IsDelete { get; set; }

    public int? CreatedByUserId { get; set; }

    public DateTime CreatedDate { get; set; }

    public int? ModifiedByUserId { get; set; }

    public DateTime? ModifiedDate { get; set; }

    public virtual ICollection<BranchSetting>? BranchSettings { get; } = new List<BranchSetting>();

    public virtual Company? Company { get; set; }

    public virtual Group? Group { get; set; }

    public virtual ICollection<Shift>? Shifts { get; } = new List<Shift>();

    public virtual ICollection<Transaction>? Transactions { get; } = new List<Transaction>();

    public virtual ICollection<UserBranchAccess>? UserBranchAccesses { get; } = new List<UserBranchAccess>();
    public virtual UserBranchAccess? UserBranchAccesses1 { get; set; }
}
