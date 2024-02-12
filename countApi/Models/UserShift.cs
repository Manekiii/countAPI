using System;
using System.Collections.Generic;

namespace countApi.Models;

public partial class UserShift
{
    public int Id { get; set; }

    public long UserId { get; set; }

    public int ShiftId { get; set; }

    public byte Status { get; set; }

    public long? CreatedByUserId { get; set; }

    public DateTime CreatedDate { get; set; }

    public long? ModifiedByUserId { get; set; }

    public DateTime? ModifiedDate { get; set; }

    public virtual Shift? Shift { get; set; } = null!;

    public virtual User? User { get; set; } = null!;
}
