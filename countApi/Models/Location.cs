using System;
using System.Collections.Generic;

namespace countApi.Models;

public partial class Location
{
    public long Id { get; set; }

    public string? BranchCode { get; set; }

    public string? Code { get; set; }

    public string? Description { get; set; }

    public byte? Status { get; set; }

    public byte IsDelete { get; set; }

    public byte IsUpload { get; set; }

    public long? CreatedByUserId { get; set; }

    public DateTime CreatedDate { get; set; }

    public long? ModifiedByUserId { get; set; }

    public DateTime? ModifiedDate { get; set; }
}
