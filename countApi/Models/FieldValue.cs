using System;
using System.Collections.Generic;

namespace countApi.Models;

public partial class FieldValue
{
    public long Id { get; set; }

    public int? ReferenceId { get; set; }

    public string? Group { get; set; }

    public int? FieldId { get; set; }

    public string? FieldName { get; set; }

    public string? Value { get; set; }

    public byte IsDelete { get; set; }

    public int? CreatedBy { get; set; }

    public DateTime CreatedDate { get; set; }

    public int? ModifiedBy { get; set; }

    public DateTime? ModifiedDate { get; set; }
}
