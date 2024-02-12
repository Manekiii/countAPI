using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace countApi.Models;

public partial class CountdbContext : DbContext
{
    public CountdbContext()
    {
    }

    public CountdbContext(DbContextOptions<CountdbContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Branch> Branches { get; set; }

    public virtual DbSet<BranchSetting> BranchSettings { get; set; }

    public virtual DbSet<Company> Companies { get; set; }

    public virtual DbSet<Group> Groups { get; set; }

    public virtual DbSet<Inventory> Inventories { get; set; }

    public virtual DbSet<InventoryObjectValue> InventoryObjectValues { get; set; }

    public virtual DbSet<Item> Items { get; set; }

    public virtual DbSet<Location> Locations { get; set; }

    public virtual DbSet<Menu> Menus { get; set; }

    public virtual DbSet<Object> Objects { get; set; }

    public virtual DbSet<Setting> Settings { get; set; }

    public virtual DbSet<Shift> Shifts { get; set; }

    public virtual DbSet<Token> Tokens { get; set; }

    public virtual DbSet<Transaction> Transactions { get; set; }

    public virtual DbSet<TransactionObjectValue> TransactionObjectValues { get; set; }

    public virtual DbSet<User> Users { get; set; }

    public virtual DbSet<UserBranchAccess> UserBranchAccesses { get; set; }

    public virtual DbSet<UserDetail> UserDetails { get; set; }

    public virtual DbSet<UserForgotPassword> UserForgotPasswords { get; set; }

    public virtual DbSet<UserMenu> UserMenus { get; set; }

    public virtual DbSet<UserShift> UserShifts { get; set; }

    public virtual DbSet<VBranch> VBranches { get; set; }

    public virtual DbSet<VBranchSetting> VBranchSettings { get; set; }

    public virtual DbSet<VInventory> VInventories { get; set; }

    public virtual DbSet<VInventoryObjectValue> VInventoryObjectValues { get; set; }

    public virtual DbSet<VMenu> VMenus { get; set; }

    public virtual DbSet<VObject> VObjects { get; set; }

    public virtual DbSet<VUser> VUsers { get; set; }

    public virtual DbSet<VUserMenu> VUserMenus { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see http://go.microsoft.com/fwlink/?LinkId=723263.
        => optionsBuilder.UseSqlServer("Server=core.private.fast.com.ph;Database=COUNTDB;User Id=countuser;Password=countpassword;TrustServerCertificate=True;");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Branch>(entity =>
        {
            entity.ToTable("Branch");

            entity.HasIndex(e => e.Code, "IX_Branch").IsUnique();

            entity.Property(e => e.Address)
                .HasMaxLength(300)
                .IsUnicode(false);
            entity.Property(e => e.Alias)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.Area)
                .HasMaxLength(100)
                .IsUnicode(false);
            entity.Property(e => e.Code)
                .HasMaxLength(10)
                .IsUnicode(false);
            entity.Property(e => e.ContactEmail)
                .HasMaxLength(100)
                .IsUnicode(false);
            entity.Property(e => e.ContactId)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.ContactName)
                .HasMaxLength(200)
                .IsUnicode(false);
            entity.Property(e => e.ContactNumber)
                .HasMaxLength(100)
                .IsUnicode(false);
            entity.Property(e => e.CreatedDate).HasColumnType("datetime");
            entity.Property(e => e.Latitude)
                .HasMaxLength(30)
                .IsUnicode(false);
            entity.Property(e => e.Longitude)
                .HasMaxLength(30)
                .IsUnicode(false);
            entity.Property(e => e.MapReference)
                .HasMaxLength(500)
                .IsUnicode(false);
            entity.Property(e => e.ModifiedDate).HasColumnType("datetime");
            entity.Property(e => e.Name)
                .HasMaxLength(100)
                .IsUnicode(false);
            entity.Property(e => e.Region)
                .HasMaxLength(100)
                .IsUnicode(false);

            entity.HasOne(d => d.Company).WithMany(p => p.Branches)
                .HasForeignKey(d => d.CompanyId)
                .HasConstraintName("FK_Branch_Company");

            entity.HasOne(d => d.Group).WithMany(p => p.Branches)
                .HasForeignKey(d => d.GroupId)
                .HasConstraintName("FK_Branch_Group");

            entity.HasOne(d => d.UserBranchAccesses1).WithMany(p => p.branch1)
               .HasPrincipalKey(p => p.BranchCode)
               .HasForeignKey(d => d.Code)
               .OnDelete(DeleteBehavior.ClientSetNull)
               .HasConstraintName("FK_UserBranchAccess_Branch");
        });

        modelBuilder.Entity<BranchSetting>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK_BranchSetting_1");

            entity.ToTable("BranchSetting");

            entity.Property(e => e.BranchCode)
                .HasMaxLength(10)
                .IsUnicode(false);
            entity.Property(e => e.CreatedDate).HasColumnType("datetime");
            entity.Property(e => e.ModifiedDate).HasColumnType("datetime");
            entity.Property(e => e.Value)
                .HasMaxLength(50)
                .IsUnicode(false);

            entity.HasOne(d => d.BranchCodeNavigation).WithMany(p => p.BranchSettings)
                .HasPrincipalKey(p => p.Code)
                .HasForeignKey(d => d.BranchCode)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_BranchSetting_Branch");
        });

        modelBuilder.Entity<Company>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK_BusinessUnit");

            entity.ToTable("Company");

            entity.Property(e => e.Code)
                .HasMaxLength(10)
                .IsUnicode(false);
            entity.Property(e => e.ContactNumber)
                .HasMaxLength(100)
                .IsUnicode(false);
            entity.Property(e => e.CreatedDate).HasColumnType("datetime");
            entity.Property(e => e.ModifiedDate).HasColumnType("datetime");
            entity.Property(e => e.Name)
                .HasMaxLength(100)
                .IsUnicode(false);
        });

        modelBuilder.Entity<Group>(entity =>
        {
            entity.ToTable("Group");

            entity.Property(e => e.Code)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.ContactEmail)
                .HasMaxLength(100)
                .IsUnicode(false);
            entity.Property(e => e.ContactId)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.ContactName)
                .HasMaxLength(200)
                .IsUnicode(false);
            entity.Property(e => e.ContactNumber)
                .HasMaxLength(100)
                .IsUnicode(false);
            entity.Property(e => e.CreatedDate).HasColumnType("datetime");
            entity.Property(e => e.ModifiedDate).HasColumnType("datetime");
            entity.Property(e => e.Name)
                .HasMaxLength(100)
                .IsUnicode(false);
            entity.Property(e => e.Status)
                .HasMaxLength(1)
                .IsUnicode(false)
                .IsFixedLength();
        });

        modelBuilder.Entity<Inventory>(entity =>
        {
            entity.ToTable("Inventory");

            entity.Property(e => e.BranchCode)
                .HasMaxLength(10)
                .IsUnicode(false);
            entity.Property(e => e.CreatedDate).HasColumnType("datetime");
            entity.Property(e => e.ItemCode)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.LocationCode)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.ModifiedDate).HasColumnType("datetime");
            entity.Property(e => e.Quantity).HasColumnType("decimal(18, 2)");
            entity.Property(e => e.ScanCode)
                .HasMaxLength(100)
                .IsUnicode(false);
        });

        modelBuilder.Entity<InventoryObjectValue>(entity =>
        {
            entity.ToTable("InventoryObjectValue");

            entity.Property(e => e.CreatedDate).HasColumnType("datetime");
            entity.Property(e => e.ModifiedDate).HasColumnType("datetime");
            entity.Property(e => e.Value)
                .HasMaxLength(255)
                .IsUnicode(false);
        });

        modelBuilder.Entity<Item>(entity =>
        {
            entity.ToTable("Item");

            entity.Property(e => e.BranchCode)
                .HasMaxLength(10)
                .IsUnicode(false);
            entity.Property(e => e.Code)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.CreatedDate).HasColumnType("datetime");
            entity.Property(e => e.Description)
                .HasMaxLength(100)
                .IsUnicode(false);
            entity.Property(e => e.ModifiedDate).HasColumnType("datetime");
        });

        modelBuilder.Entity<Location>(entity =>
        {
            entity.ToTable("Location");

            entity.Property(e => e.BranchCode)
                .HasMaxLength(10)
                .IsUnicode(false);
            entity.Property(e => e.Code)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.CreatedDate).HasColumnType("datetime");
            entity.Property(e => e.Description)
                .HasMaxLength(100)
                .IsUnicode(false);
            entity.Property(e => e.ModifiedDate).HasColumnType("datetime");
        });

        modelBuilder.Entity<Menu>(entity =>
        {
            entity.ToTable("Menu");

            entity.Property(e => e.CreatedDate).HasColumnType("datetime");
            entity.Property(e => e.Icon)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.MenuName)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.ModifiedDate).HasColumnType("datetime");
        });

        modelBuilder.Entity<Object>(entity =>
        {
            entity.ToTable("Object");

            entity.Property(e => e.Alias)
                .HasMaxLength(20)
                .IsUnicode(false);
            entity.Property(e => e.BranchCode)
                .HasMaxLength(10)
                .IsUnicode(false);
            entity.Property(e => e.CreatedDate).HasColumnType("datetime");
            entity.Property(e => e.Form)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.InputType)
                .HasMaxLength(10)
                .IsUnicode(false)
                .HasComment("Text, Number, Date");
            entity.Property(e => e.ModifiedDate).HasColumnType("datetime");
            entity.Property(e => e.Name)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.Placeholder)
                .HasMaxLength(50)
                .IsUnicode(false);
        });

        modelBuilder.Entity<Setting>(entity =>
        {
            entity.ToTable("Setting");

            entity.Property(e => e.CreatedDate).HasColumnType("datetime");
            entity.Property(e => e.Default)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.Detail)
                .HasMaxLength(255)
                .IsUnicode(false);
            entity.Property(e => e.ModifiedDate).HasColumnType("datetime");
            entity.Property(e => e.Name)
                .HasMaxLength(50)
                .IsUnicode(false);
        });

        modelBuilder.Entity<Shift>(entity =>
        {
            entity.ToTable("Shift");

            entity.Property(e => e.BranchCode)
                .HasMaxLength(10)
                .IsUnicode(false);
            entity.Property(e => e.CreatedDate).HasColumnType("datetime");
            entity.Property(e => e.ModifiedDate).HasColumnType("datetime");
            entity.Property(e => e.Name)
                .HasMaxLength(50)
                .IsUnicode(false);

            entity.HasOne(d => d.BranchCodeNavigation).WithMany(p => p.Shifts)
                .HasPrincipalKey(p => p.Code)
                .HasForeignKey(d => d.BranchCode)
                .HasConstraintName("FK_Shift_Branch");
        });

        modelBuilder.Entity<Token>(entity =>
        {
            entity.HasKey(e => e.TokenId).HasName("PK_Tokens");

            entity.ToTable("tokens");

            entity.Property(e => e.AuthToken).HasMaxLength(250);
            entity.Property(e => e.ExpiresOn).HasColumnType("datetime");
            entity.Property(e => e.IssuedOn).HasColumnType("datetime");
            entity.Property(e => e.UserId)
                .HasMaxLength(100)
                .IsUnicode(false);
        });

        modelBuilder.Entity<Transaction>(entity =>
        {
            entity.ToTable("Transaction");

            entity.HasIndex(e => e.Id, "IX_Transaction");

            entity.Property(e => e.BranchCode)
                .HasMaxLength(10)
                .IsUnicode(false);
            entity.Property(e => e.CreatedDate).HasColumnType("datetime");
            entity.Property(e => e.ItemCode)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.LocationCode)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.ModifiedDate).HasColumnType("datetime");
            entity.Property(e => e.Quantity).HasColumnType("decimal(18, 2)");
            entity.Property(e => e.ScanCode)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.ScanDate).HasColumnType("datetime");

            entity.HasOne(d => d.BranchCodeNavigation).WithMany(p => p.Transactions)
                .HasPrincipalKey(p => p.Code)
                .HasForeignKey(d => d.BranchCode)
                .HasConstraintName("FK_Transaction_Branch");

            entity.HasOne(d => d.CreatedByUser).WithMany(p => p.TransactionCreatedByUsers)
                .HasForeignKey(d => d.CreatedByUserId)
                .HasConstraintName("FK_Transaction_User");

            entity.HasOne(d => d.ModifiedByUser).WithMany(p => p.TransactionModifiedByUsers)
                .HasForeignKey(d => d.ModifiedByUserId)
                .HasConstraintName("FK_Transaction_User1");
        });

        modelBuilder.Entity<TransactionObjectValue>(entity =>
        {
            entity.ToTable("TransactionObjectValue");

            entity.Property(e => e.CreatedDate).HasColumnType("datetime");
            entity.Property(e => e.ModifiedDate).HasColumnType("datetime");
            entity.Property(e => e.Value)
                .HasMaxLength(255)
                .IsUnicode(false);
        });

        modelBuilder.Entity<User>(entity =>
        {
            entity.ToTable("User");

            entity.Property(e => e.ContactNumber)
                .HasMaxLength(100)
                .IsUnicode(false);
            entity.Property(e => e.CreatedDate).HasColumnType("datetime");
            entity.Property(e => e.DeactivateReason)
                .HasMaxLength(255)
                .IsUnicode(false);
            entity.Property(e => e.EmailAddress)
                .HasMaxLength(100)
                .IsUnicode(false);
            entity.Property(e => e.Firstname)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.HashCode)
                .HasMaxLength(100)
                .IsUnicode(false);
            entity.Property(e => e.Lastname)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.Middlename)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.ModifiedDate).HasColumnType("datetime");
            entity.Property(e => e.Nickname)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.Password)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.Type)
                .HasMaxLength(5)
                .IsUnicode(false);
            entity.Property(e => e.Username)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.VerifiedDate).HasColumnType("datetime");
        });

        modelBuilder.Entity<UserBranchAccess>(entity =>
        {
            entity.ToTable("UserBranchAccess");

            entity.HasIndex(e => new { e.BranchCode, e.UserId }, "IX_UserBranchAccess").IsUnique();

            entity.Property(e => e.BranchCode)
                .HasMaxLength(10)
                .IsUnicode(false);
            entity.Property(e => e.CreatedDate).HasColumnType("datetime");
            entity.Property(e => e.ModifiedDate).HasColumnType("datetime");

            entity.HasOne(d => d.BranchCodeNavigation).WithMany(p => p.UserBranchAccesses)
                .HasPrincipalKey(p => p.Code)
                .HasForeignKey(d => d.BranchCode)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_UserBranchAccess_Branch");

            entity.HasOne(d => d.CreatedByUser).WithMany(p => p.UserBranchAccessCreatedByUsers)
                .HasForeignKey(d => d.CreatedByUserId)
                .HasConstraintName("FK_UserBranchAccess_UserBranchAccess");

            entity.HasOne(d => d.ModifiedByUser).WithMany(p => p.UserBranchAccessModifiedByUsers)
                .HasForeignKey(d => d.ModifiedByUserId)
                .HasConstraintName("FK_UserBranchAccess_User1");

            entity.HasOne(d => d.User).WithMany(p => p.UserBranchAccessUsers)
                .HasForeignKey(d => d.UserId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_UserBranchAccess_User");
        });

        modelBuilder.Entity<UserDetail>(entity =>
        {
            entity.ToTable("UserDetail");

            entity.Property(e => e.Address)
                .HasMaxLength(255)
                .IsUnicode(false);
            entity.Property(e => e.CreatedDate).HasColumnType("datetime");
            entity.Property(e => e.ModifiedDate).HasColumnType("datetime");

            entity.HasOne(d => d.Company).WithMany(p => p.UserDetails)
                .HasForeignKey(d => d.CompanyId)
                .HasConstraintName("FK_UserDetail_Company");

            entity.HasOne(d => d.User).WithMany(p => p.UserDetails)
                .HasForeignKey(d => d.UserId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_UserDetail_User");
        });

        modelBuilder.Entity<UserForgotPassword>(entity =>
        {
            entity.ToTable("UserForgotPassword");

            entity.Property(e => e.Code)
                .HasMaxLength(100)
                .IsUnicode(false);
            entity.Property(e => e.CreatedDate).HasColumnType("datetime");
            entity.Property(e => e.Email)
                .HasMaxLength(100)
                .IsUnicode(false);
        });

        modelBuilder.Entity<UserMenu>(entity =>
        {
            entity.ToTable("UserMenu").HasKey(x => new { x.MenuId, x.UserId });
            /*  entity
                  .HasNoKey()
                  .ToTable("UserMenu");*/

            entity.Property(e => e.BranchCode)
                .HasMaxLength(10)
                .IsUnicode(false);
            entity.Property(e => e.CreatedDate).HasColumnType("datetime");
            entity.Property(e => e.ModifiedDate).HasColumnType("datetime");

            entity.HasOne(d => d.BranchCodeNavigation).WithMany()
                .HasPrincipalKey(p => p.Code)
                .HasForeignKey(d => d.BranchCode)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_UserMenu_Branch");

            entity.HasOne(d => d.CreatedByUser).WithMany()
                .HasForeignKey(d => d.CreatedByUserId)
                .HasConstraintName("FK_UserMenu_User1");

            entity.HasOne(d => d.Menu).WithMany()
                .HasForeignKey(d => d.MenuId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_UserMenu_Menu");

            entity.HasOne(d => d.ModifiedByUser).WithMany()
                .HasForeignKey(d => d.ModifiedByUserId)
                .HasConstraintName("FK_UserMenu_User2");

            entity.HasOne(d => d.User).WithMany()
                .HasForeignKey(d => d.UserId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_UserMenu_User");
        });

        modelBuilder.Entity<UserShift>(entity =>
        {
            entity.ToTable("UserShift");

            entity.Property(e => e.CreatedDate).HasColumnType("datetime");
            entity.Property(e => e.ModifiedDate).HasColumnType("datetime");

            entity.HasOne(d => d.Shift).WithMany(p => p.UserShifts)
                .HasForeignKey(d => d.ShiftId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_UserShift_Shift");

            entity.HasOne(d => d.User).WithMany(p => p.UserShifts)
                .HasForeignKey(d => d.UserId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_UserShift_User");
        });

        modelBuilder.Entity<VBranch>(entity =>
        {
            entity
                .HasNoKey()
                .ToView("vBranch");

            entity.Property(e => e.Address)
                .HasMaxLength(300)
                .IsUnicode(false);
            entity.Property(e => e.Alias)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.Area)
                .HasMaxLength(100)
                .IsUnicode(false);
            entity.Property(e => e.Code)
                .HasMaxLength(10)
                .IsUnicode(false);
            entity.Property(e => e.Company)
                .HasMaxLength(100)
                .IsUnicode(false);
            entity.Property(e => e.ContactEmail)
                .HasMaxLength(100)
                .IsUnicode(false);
            entity.Property(e => e.ContactId)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.ContactName)
                .HasMaxLength(200)
                .IsUnicode(false);
            entity.Property(e => e.ContactNumber)
                .HasMaxLength(100)
                .IsUnicode(false);
            entity.Property(e => e.CreatedBy)
                .HasMaxLength(104)
                .IsUnicode(false);
            entity.Property(e => e.CreatedDate).HasColumnType("datetime");
            entity.Property(e => e.Group)
                .HasMaxLength(100)
                .IsUnicode(false);
            entity.Property(e => e.Latitude)
                .HasMaxLength(30)
                .IsUnicode(false);
            entity.Property(e => e.Longitude)
                .HasMaxLength(30)
                .IsUnicode(false);
            entity.Property(e => e.MapReference)
                .HasMaxLength(500)
                .IsUnicode(false);
            entity.Property(e => e.ModifiedBy)
                .HasMaxLength(104)
                .IsUnicode(false);
            entity.Property(e => e.ModifiedDate).HasColumnType("datetime");
            entity.Property(e => e.Name)
                .HasMaxLength(100)
                .IsUnicode(false);
            entity.Property(e => e.Region)
                .HasMaxLength(100)
                .IsUnicode(false);
        });

        modelBuilder.Entity<VBranchSetting>(entity =>
        {
            entity
                .HasNoKey()
                .ToView("vBranchSetting");

            entity.Property(e => e.BranchCode)
                .HasMaxLength(10)
                .IsUnicode(false);
            entity.Property(e => e.BranchName)
                .HasMaxLength(100)
                .IsUnicode(false);
            entity.Property(e => e.CreatedBy)
                .HasMaxLength(104)
                .IsUnicode(false);
            entity.Property(e => e.CreatedDate).HasColumnType("datetime");
            entity.Property(e => e.ModifiedBy)
                .HasMaxLength(104)
                .IsUnicode(false);
            entity.Property(e => e.ModifiedDate).HasColumnType("datetime");
            entity.Property(e => e.SettingName)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.StatusName)
                .HasMaxLength(8)
                .IsUnicode(false);
            entity.Property(e => e.Value)
                .HasMaxLength(50)
                .IsUnicode(false);
        });

        modelBuilder.Entity<VInventory>(entity =>
        {
            entity
                .HasNoKey()
                .ToView("vInventory");

            entity.Property(e => e.BranchAlias)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.BranchCode)
                .HasMaxLength(10)
                .IsUnicode(false);
            entity.Property(e => e.BranchName)
                .HasMaxLength(100)
                .IsUnicode(false);
            entity.Property(e => e.CreatedBy)
                .HasMaxLength(104)
                .IsUnicode(false);
            entity.Property(e => e.CreatedDate).HasColumnType("datetime");
            entity.Property(e => e.ItemCode)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.LocationCode)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.ModifiedBy)
                .HasMaxLength(104)
                .IsUnicode(false);
            entity.Property(e => e.ModifiedDate).HasColumnType("datetime");
            entity.Property(e => e.Quantity).HasColumnType("decimal(18, 2)");
            entity.Property(e => e.ScanCode)
                .HasMaxLength(100)
                .IsUnicode(false);
        });

        modelBuilder.Entity<VInventoryObjectValue>(entity =>
        {
            entity
                .HasNoKey()
                .ToView("vInventoryObjectValue");

            entity.Property(e => e.CreatedBy)
                .HasMaxLength(104)
                .IsUnicode(false);
            entity.Property(e => e.CreatedDate).HasColumnType("datetime");
            entity.Property(e => e.ModifiedBy)
                .HasMaxLength(104)
                .IsUnicode(false);
            entity.Property(e => e.ModifiedDate).HasColumnType("datetime");
            entity.Property(e => e.ObjectName)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.Value)
                .HasMaxLength(255)
                .IsUnicode(false);
        });

        modelBuilder.Entity<VMenu>(entity =>
        {
            entity
                .HasNoKey()
                .ToView("vMenu");

            entity.Property(e => e.CreatedBy)
                .HasMaxLength(104)
                .IsUnicode(false);
            entity.Property(e => e.CreatedDate).HasColumnType("datetime");
            entity.Property(e => e.Icon)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.Id).ValueGeneratedOnAdd();
            entity.Property(e => e.ModifiedBy)
                .HasMaxLength(104)
                .IsUnicode(false);
            entity.Property(e => e.ModifiedDate).HasColumnType("datetime");
            entity.Property(e => e.Name)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.ParentMenu)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.StatusName)
                .HasMaxLength(8)
                .IsUnicode(false);
        });

        modelBuilder.Entity<VObject>(entity =>
        {
            entity
                .HasNoKey()
                .ToView("vObject");

            entity.Property(e => e.Alias)
                .HasMaxLength(20)
                .IsUnicode(false);
            entity.Property(e => e.BranchCode)
                .HasMaxLength(10)
                .IsUnicode(false);
            entity.Property(e => e.BranchName)
                .HasMaxLength(100)
                .IsUnicode(false);
            entity.Property(e => e.CreatedBy)
                .HasMaxLength(104)
                .IsUnicode(false);
            entity.Property(e => e.CreatedDate).HasColumnType("datetime");
            entity.Property(e => e.Form)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.InputType)
                .HasMaxLength(10)
                .IsUnicode(false);
            entity.Property(e => e.ModifiedBy)
                .HasMaxLength(104)
                .IsUnicode(false);
            entity.Property(e => e.ModifiedDate).HasColumnType("datetime");
            entity.Property(e => e.Name)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.Placeholder)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.StatusName)
                .HasMaxLength(8)
                .IsUnicode(false);
        });

        modelBuilder.Entity<VUser>(entity =>
        {
            entity
                .HasNoKey()
                .ToView("vUser");

            entity.Property(e => e.ContactNumber)
                .HasMaxLength(100)
                .IsUnicode(false);
            entity.Property(e => e.CreatedBy)
                .HasMaxLength(104)
                .IsUnicode(false);
            entity.Property(e => e.CreatedDate).HasColumnType("datetime");
            entity.Property(e => e.DeactivateReason)
                .HasMaxLength(255)
                .IsUnicode(false);
            entity.Property(e => e.EmailAddress)
                .HasMaxLength(100)
                .IsUnicode(false);
            entity.Property(e => e.Firstname)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.Fullname)
                .HasMaxLength(153)
                .IsUnicode(false);
            entity.Property(e => e.Fullname2)
                .HasMaxLength(104)
                .IsUnicode(false);
            entity.Property(e => e.HashCode)
                .HasMaxLength(100)
                .IsUnicode(false);
            entity.Property(e => e.Id).ValueGeneratedOnAdd();
            entity.Property(e => e.Lastname)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.Middlename)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.ModifiedBy)
                .HasMaxLength(104)
                .IsUnicode(false);
            entity.Property(e => e.ModifiedDate).HasColumnType("datetime");
            entity.Property(e => e.Nickname)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.Password)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.Type)
                .HasMaxLength(5)
                .IsUnicode(false);
            entity.Property(e => e.Username)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.VerifiedBye)
                .HasMaxLength(104)
                .IsUnicode(false);
            entity.Property(e => e.VerifiedDate).HasColumnType("datetime");
        });

        modelBuilder.Entity<VUserMenu>(entity =>
        {
            entity
                .HasNoKey()
                .ToView("vUserMenu");

            entity.Property(e => e.CreatedBy)
                .HasMaxLength(104)
                .IsUnicode(false);
            entity.Property(e => e.CreatedDate).HasColumnType("datetime");
            entity.Property(e => e.Icon)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.MenuName)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.ModifiedBy)
                .HasMaxLength(104)
                .IsUnicode(false);
            entity.Property(e => e.ModifiedDate).HasColumnType("datetime");
            entity.Property(e => e.ParentIcon)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.ParentMenu)
                .HasMaxLength(50)
                .IsUnicode(false);
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
