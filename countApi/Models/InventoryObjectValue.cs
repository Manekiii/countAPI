using System;
using System.Collections.Generic;

namespace countApi.Models;

public partial class InventoryObjectValue
{
    public long Id { get; set; }

    public long InventoryId { get; set; }

    public int ObjectId { get; set; }

    public string? Value { get; set; }

    public int? CreatedByUserId { get; set; }

    public DateTime CreatedDate { get; set; }

    public int? ModifiedByUserId { get; set; }

    public DateTime? ModifiedDate { get; set; }
}
