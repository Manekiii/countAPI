using System;
using System.Collections.Generic;

namespace countApi.Models;

public partial class VInventory
{
    public long Id { get; set; }

    public string? ScanCode { get; set; }

    public string? ItemCode { get; set; }

    public string? LocationCode { get; set; }

    public decimal? Quantity { get; set; }

    public string? BranchAlias { get; set; }

    public string? BranchCode { get; set; }

    public string? BranchName { get; set; }

    public int? CreatedByUserId { get; set; }

    public string? CreatedBy { get; set; }

    public DateTime CreatedDate { get; set; }

    public int? ModifiedByUserId { get; set; }

    public string? ModifiedBy { get; set; }

    public DateTime? ModifiedDate { get; set; }
}
