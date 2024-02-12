using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace countApi.Mailer;

public partial class CoredbMailContext : DbContext
{
    public CoredbMailContext()
    {
    }

    public CoredbMailContext(DbContextOptions<CoredbMailContext> options)
        : base(options)
    {
    }

    public virtual DbSet<CoreMailer> CoreMailers { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see http://go.microsoft.com/fwlink/?LinkId=723263.
        => optionsBuilder.UseSqlServer("Server=core.private.fast.com.ph;Database=COREDB_MAIL;User Id=countuser;Password=countpassword;TrustServerCertificate=True;");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<CoreMailer>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK_core_mail");

            entity.ToTable("core_mailer");

            entity.HasIndex(e => e.MailStatus, "idx_core_mailer_recipient");

            entity.HasIndex(e => e.MailStatus, "idx_core_mailer_status");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.Bccrecipient)
                .IsUnicode(false)
                .HasColumnName("bccrecipient");
            entity.Property(e => e.Ccrecipient)
                .IsUnicode(false)
                .HasColumnName("ccrecipient");
            entity.Property(e => e.Created)
                .HasColumnType("datetime")
                .HasColumnName("created");
            entity.Property(e => e.CreatedBy)
                .HasMaxLength(100)
                .IsUnicode(false)
                .HasColumnName("createdBy");
            entity.Property(e => e.FilePath)
                .HasMaxLength(500)
                .IsUnicode(false)
                .HasColumnName("filePath");
            entity.Property(e => e.MailBody)
                .IsUnicode(false)
                .HasColumnName("mailBody");
            entity.Property(e => e.MailFormat)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("mailFormat");
            entity.Property(e => e.MailStatus)
                .HasMaxLength(1)
                .IsUnicode(false)
                .HasColumnName("mailStatus");
            entity.Property(e => e.MailSubject)
                .HasMaxLength(100)
                .IsUnicode(false)
                .HasColumnName("mailSubject");
            entity.Property(e => e.Recipient)
                .IsUnicode(false)
                .HasColumnName("recipient");
            entity.Property(e => e.SenderDisplayEmail)
                .HasMaxLength(100)
                .IsUnicode(false)
                .HasColumnName("senderDisplayEmail");
            entity.Property(e => e.SenderDisplayName)
                .HasMaxLength(100)
                .IsUnicode(false)
                .HasColumnName("senderDisplayName");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
