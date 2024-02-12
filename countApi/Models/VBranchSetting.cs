using System;
using System.Collections.Generic;

namespace countApi.Models;

public partial class VBranchSetting
{
    public int Id { get; set; }

    public string BranchCode { get; set; } = null!;

    public string? BranchName { get; set; }

    public int? SettingId { get; set; }

    public string? SettingName { get; set; }

    public string? Value { get; set; }

    public byte Status { get; set; }

    public string StatusName { get; set; } = null!;

    public int? CreatedByUserId { get; set; }

    public string? CreatedBy { get; set; }

    public DateTime? CreatedDate { get; set; }

    public int? ModifiedByUserId { get; set; }

    public string? ModifiedBy { get; set; }

    public DateTime? ModifiedDate { get; set; }
}
