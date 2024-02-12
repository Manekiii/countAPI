using System;
using System.Collections.Generic;

namespace countApi.Mailer;

public partial class CoreMailer
{
    public int Id { get; set; }

    public string SenderDisplayEmail { get; set; } = null!;

    public string SenderDisplayName { get; set; } = null!;

    public string Recipient { get; set; } = null!;

    public string? Ccrecipient { get; set; }

    public string? Bccrecipient { get; set; }

    public string MailSubject { get; set; } = null!;

    public string MailBody { get; set; } = null!;

    public string? FilePath { get; set; }

    public string MailFormat { get; set; } = null!;

    public string MailStatus { get; set; } = null!;

    public string? CreatedBy { get; set; }

    public DateTime Created { get; set; }
}
