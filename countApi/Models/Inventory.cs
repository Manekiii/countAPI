using System;
using System.Collections.Generic;

namespace countApi.Models;

public partial class Inventory
{
    public long Id { get; set; }

    public string? ScanCode { get; set; }

    public string? ItemCode { get; set; }

    public string? LocationCode { get; set; }

    public decimal? Quantity { get; set; }

    public string? BranchCode { get; set; }

    public int? CreatedByUserId { get; set; }

    public DateTime CreatedDate { get; set; }

    public int? ModifiedByUserId { get; set; }

    public DateTime? ModifiedDate { get; set; }
}
