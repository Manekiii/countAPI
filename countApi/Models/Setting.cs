using System;
using System.Collections.Generic;

namespace countApi.Models;

public partial class Setting
{
    public int Id { get; set; }

    public string? Name { get; set; }

    public string? Detail { get; set; }

    public string? Default { get; set; }

    public byte Status { get; set; }

    public byte IsDelete { get; set; }

    public long? CreatedByUserId { get; set; }

    public DateTime CreatedDate { get; set; }

    public long? ModifiedByUserId { get; set; }

    public DateTime? ModifiedDate { get; set; }
}
