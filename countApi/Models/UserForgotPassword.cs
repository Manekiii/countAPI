using System;
using System.Collections.Generic;

namespace countApi.Models;

public partial class UserForgotPassword
{
    public int Id { get; set; }

    public string? Email { get; set; }

    public string Code { get; set; } = null!;

    public byte IsUsed { get; set; }

    public DateTime CreatedDate { get; set; }
}
