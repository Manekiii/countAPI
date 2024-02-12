using System;
using System.Collections.Generic;

namespace countApi.Models;

public partial class BranchSetting
{
    public int Id { get; set; }

    public string BranchCode { get; set; } = null!;

    public int? SettingId { get; set; }

    public string? Value { get; set; }

    public byte Status { get; set; }

    public int? CreatedByUserId { get; set; }

    public DateTime? CreatedDate { get; set; }

    public int? ModifiedByUserId { get; set; }

    public DateTime? ModifiedDate { get; set; }

    public virtual Branch BranchCodeNavigation { get; set; } = null!;
}
