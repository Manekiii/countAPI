using System;
using System.Collections.Generic;

namespace countApi.Models;

public partial class Transaction
{
    public long Id { get; set; }

    public string? ScanCode { get; set; }

    public string? ItemCode { get; set; }

    public string? LocationCode { get; set; }

    public decimal Quantity { get; set; }

    public string? BranchCode { get; set; }

    public byte IsDelete { get; set; }

    public byte IsUploaded { get; set; }

    public DateTime? ScanDate { get; set; }

    public int? UserShiftId { get; set; }

    public long? CreatedByUserId { get; set; }

    public DateTime CreatedDate { get; set; }

    public long? ModifiedByUserId { get; set; }

    public DateTime? ModifiedDate { get; set; }

    public virtual Branch? BranchCodeNavigation { get; set; }

    public virtual User? CreatedByUser { get; set; }

    public virtual User? ModifiedByUser { get; set; }
}
