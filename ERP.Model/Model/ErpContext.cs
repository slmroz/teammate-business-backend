using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace ERP.Model.Model;

public partial class ErpContext : DbContext
{
    public ErpContext()
    {
    }

    public ErpContext(DbContextOptions<ErpContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Contact> Contacts { get; set; }

    public virtual DbSet<Currency> Currencies { get; set; }

    public virtual DbSet<Customer> Customers { get; set; }

    public virtual DbSet<Lead> Leads { get; set; }

    public virtual DbSet<LeadSource> LeadSources { get; set; }

    public virtual DbSet<LeadStatus> LeadStatuses { get; set; }

    public virtual DbSet<PriceList> PriceLists { get; set; }

    public virtual DbSet<PriceListItem> PriceListItems { get; set; }

    public virtual DbSet<Priority> Priorities { get; set; }

    public virtual DbSet<Product> Products { get; set; }

    public virtual DbSet<ProductGroup> ProductGroups { get; set; }

    public virtual DbSet<Quote> Quotes { get; set; }

    public virtual DbSet<QuoteItem> QuoteItems { get; set; }

    public virtual DbSet<QuoteStatus> QuoteStatuses { get; set; }

    public virtual DbSet<User> Users { get; set; }

    public virtual DbSet<UserRole> UserRoles { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.HasDefaultSchema("slmroz_dbadmin");

        modelBuilder.Entity<Contact>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Contact__3214EC074553CEC5");

            entity.ToTable("Contact", "Crm");

            entity.Property(e => e.CreatedAt).HasColumnType("datetime");
            entity.Property(e => e.Email).HasMaxLength(100);
            entity.Property(e => e.FirstName).HasMaxLength(50);
            entity.Property(e => e.LastName).HasMaxLength(50);
            entity.Property(e => e.LastUpdatedAt).HasColumnType("datetime");
            entity.Property(e => e.PhoneNo).HasMaxLength(50);
            entity.Property(e => e.RemovedAt).HasColumnType("datetime");

            entity.HasOne(d => d.Customer).WithMany(p => p.Contacts)
                .HasForeignKey(d => d.CustomerId)
                .HasConstraintName("FK__Contact__Custome__182C9B23");
        });

        modelBuilder.Entity<Currency>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Currency__3214EC07C9AB04A0");

            entity.ToTable("Currency", "Catalog");

            entity.Property(e => e.BaseCurrency).HasMaxLength(5);
            entity.Property(e => e.CreatedAt).HasColumnType("datetime");
            entity.Property(e => e.LastUpdatedAt).HasColumnType("datetime");
            entity.Property(e => e.Rate).HasColumnType("decimal(18, 6)");
            entity.Property(e => e.RemovedAt).HasColumnType("datetime");
            entity.Property(e => e.TargetCurrency).HasMaxLength(5);
        });

        modelBuilder.Entity<Customer>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Customer__3214EC073F3A12EF");

            entity.ToTable("Customer", "Crm");

            entity.Property(e => e.Address).HasMaxLength(100);
            entity.Property(e => e.City).HasMaxLength(50);
            entity.Property(e => e.Country).HasMaxLength(50);
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.Facebook).HasMaxLength(100);
            entity.Property(e => e.LastUpdatedAt).HasColumnType("datetime");
            entity.Property(e => e.Name).HasMaxLength(200);
            entity.Property(e => e.RemovedAt).HasColumnType("datetime");
            entity.Property(e => e.TaxId).HasMaxLength(100);
            entity.Property(e => e.Www).HasMaxLength(100);
            entity.Property(e => e.ZipCode).HasMaxLength(10);
        });

        modelBuilder.Entity<Lead>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Lead__3214EC07D72D75F3");

            entity.ToTable("Lead", "Crm");

            entity.Property(e => e.CreatedAt).HasDefaultValueSql("(getdate())");
            entity.Property(e => e.Description).HasMaxLength(1000);
            entity.Property(e => e.EstimatedValue).HasColumnType("decimal(18, 2)");
            entity.Property(e => e.LastUpdatedAt).HasColumnType("datetime");
            entity.Property(e => e.RemovedAt).HasColumnType("datetime");
            entity.Property(e => e.Subject).HasMaxLength(200);

            entity.HasOne(d => d.Contact).WithMany(p => p.Leads)
                .HasForeignKey(d => d.ContactId)
                .HasConstraintName("FK__Lead__ContactId__398D8EEE");

            entity.HasOne(d => d.CreatedByNavigation).WithMany(p => p.LeadCreatedByNavigations)
                .HasForeignKey(d => d.CreatedBy)
                .HasConstraintName("FK__Lead__CreatedBy__3E52440B");

            entity.HasOne(d => d.Customer).WithMany(p => p.Leads)
                .HasForeignKey(d => d.CustomerId)
                .HasConstraintName("FK__Lead__CustomerId__38996AB5");

            entity.HasOne(d => d.LastUpdatedByNavigation).WithMany(p => p.LeadLastUpdatedByNavigations)
                .HasForeignKey(d => d.LastUpdatedBy)
                .HasConstraintName("FK__Lead__LastUpdate__3F466844");

            entity.HasOne(d => d.LeadSource).WithMany(p => p.Leads)
                .HasForeignKey(d => d.LeadSourceId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Lead__LeadSource__3A81B327");

            entity.HasOne(d => d.Priority).WithMany(p => p.Leads)
                .HasForeignKey(d => d.PriorityId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Lead__PriorityId__3C69FB99");

            entity.HasOne(d => d.RemovedByNavigation).WithMany(p => p.LeadRemovedByNavigations)
                .HasForeignKey(d => d.RemovedBy)
                .HasConstraintName("FK__Lead__RemovedBy__403A8C7D");

            entity.HasOne(d => d.Status).WithMany(p => p.Leads)
                .HasForeignKey(d => d.StatusId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Lead__StatusId__3B75D760");
        });

        modelBuilder.Entity<LeadSource>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__LeadSour__3213E83F62D7B58D");

            entity.ToTable("LeadSource", "Dictionary");

            entity.HasIndex(e => e.Label, "leadsource_label_unique").IsUnique();

            entity.Property(e => e.Id)
                .ValueGeneratedNever()
                .HasColumnName("id");
            entity.Property(e => e.Label)
                .HasMaxLength(100)
                .HasColumnName("label");
        });

        modelBuilder.Entity<LeadStatus>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__LeadStat__3213E83F5BF1A05E");

            entity.ToTable("LeadStatus", "Dictionary");

            entity.Property(e => e.Id)
                .ValueGeneratedNever()
                .HasColumnName("id");
            entity.Property(e => e.Label)
                .HasMaxLength(100)
                .HasColumnName("label");
        });

        modelBuilder.Entity<PriceList>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__PriceLis__3214EC0773DA311F");

            entity.ToTable("PriceList", "Catalog");

            entity.Property(e => e.CreatedAt).HasColumnType("datetime");
            entity.Property(e => e.Description).HasMaxLength(1000);
            entity.Property(e => e.DiscountPercentage).HasColumnType("decimal(18, 2)");
            entity.Property(e => e.LastUpdatedAt).HasColumnType("datetime");
            entity.Property(e => e.Name).HasMaxLength(150);
            entity.Property(e => e.RemovedAt).HasColumnType("datetime");

            entity.HasOne(d => d.CreatedByNavigation).WithMany(p => p.PriceListCreatedByNavigations)
                .HasForeignKey(d => d.CreatedBy)
                .HasConstraintName("FK__PriceList__Creat__25869641");

            entity.HasOne(d => d.Currency).WithMany(p => p.PriceLists)
                .HasForeignKey(d => d.CurrencyId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__PriceList__Curre__300424B4");

            entity.HasOne(d => d.LastUpdatedByNavigation).WithMany(p => p.PriceListLastUpdatedByNavigations)
                .HasForeignKey(d => d.LastUpdatedBy)
                .HasConstraintName("FK__PriceList__LastU__276EDEB3");

            entity.HasOne(d => d.RemovedByNavigation).WithMany(p => p.PriceListRemovedByNavigations)
                .HasForeignKey(d => d.RemovedBy)
                .HasConstraintName("FK__PriceList__Remov__267ABA7A");
        });

        modelBuilder.Entity<PriceListItem>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__PriceLis__3214EC0780A8A9A8");

            entity.ToTable("PriceListItem", "Catalog");

            entity.Property(e => e.CreatedAt).HasColumnType("datetime");
            entity.Property(e => e.LastUpdatedAt).HasColumnType("datetime");
            entity.Property(e => e.Price).HasColumnType("decimal(18, 2)");
            entity.Property(e => e.RemovedAt).HasColumnType("datetime");

            entity.HasOne(d => d.CreatedByNavigation).WithMany(p => p.PriceListItemCreatedByNavigations)
                .HasForeignKey(d => d.CreatedBy)
                .HasConstraintName("FK__PriceList__Creat__2C3393D0");

            entity.HasOne(d => d.LastUpdatedByNavigation).WithMany(p => p.PriceListItemLastUpdatedByNavigations)
                .HasForeignKey(d => d.LastUpdatedBy)
                .HasConstraintName("FK__PriceList__LastU__2E1BDC42");

            entity.HasOne(d => d.PriceList).WithMany(p => p.PriceListItems)
                .HasForeignKey(d => d.PriceListId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__PriceList__Price__2F10007B");

            entity.HasOne(d => d.Product).WithMany(p => p.PriceListItems)
                .HasForeignKey(d => d.ProductId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__PriceList__Produ__2B3F6F97");

            entity.HasOne(d => d.RemovedByNavigation).WithMany(p => p.PriceListItemRemovedByNavigations)
                .HasForeignKey(d => d.RemovedBy)
                .HasConstraintName("FK__PriceList__Remov__2D27B809");
        });

        modelBuilder.Entity<Priority>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Priority__3213E83F7368CCA4");

            entity.ToTable("Priority", "Dictionary");

            entity.HasIndex(e => e.Label, "priority_label_unique").IsUnique();

            entity.Property(e => e.Id)
                .ValueGeneratedNever()
                .HasColumnName("id");
            entity.Property(e => e.Label)
                .HasMaxLength(100)
                .HasColumnName("label");
        });

        modelBuilder.Entity<Product>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Products__3214EC078DD09984");

            entity.ToTable("Product", "Catalog");

            entity.Property(e => e.CreatedAt).HasDefaultValueSql("(getdate())");
            entity.Property(e => e.Description).HasMaxLength(500);
            entity.Property(e => e.LastUpdatedAt).HasColumnType("datetime");
            entity.Property(e => e.ListPrice).HasColumnType("decimal(18, 2)");
            entity.Property(e => e.Name).HasMaxLength(200);
            entity.Property(e => e.Oembrand)
                .HasMaxLength(100)
                .HasColumnName("OEMBrand");
            entity.Property(e => e.PartNumber).HasMaxLength(50);
            entity.Property(e => e.RemovedAt).HasColumnType("datetime");
            entity.Property(e => e.WeightKg).HasColumnType("decimal(10, 3)");

            entity.HasOne(d => d.ProductGroup).WithMany(p => p.Products)
                .HasForeignKey(d => d.ProductGroupId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Products__Produc__1DE57479");
        });

        modelBuilder.Entity<ProductGroup>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__ProductG__3214EC072A8E1E23");

            entity.ToTable("ProductGroup", "Catalog");

            entity.Property(e => e.CreatedAt).HasDefaultValueSql("(getdate())");
            entity.Property(e => e.Description).HasMaxLength(500);
            entity.Property(e => e.LastUpdatedAt).HasColumnType("datetime");
            entity.Property(e => e.Name).HasMaxLength(100);
            entity.Property(e => e.RemovedAt).HasColumnType("datetime");
        });

        modelBuilder.Entity<Quote>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Quote__3214EC07B91D9A17");

            entity.ToTable("Quote", "Sales");

            entity.HasIndex(e => e.QuoteNumber, "UQ__Quote__8A47966A5FE01B8F").IsUnique();

            entity.Property(e => e.CreatedAt).HasDefaultValueSql("(getdate())");
            entity.Property(e => e.Description).HasMaxLength(1000);
            entity.Property(e => e.DiscountPercentage)
                .HasDefaultValue(0m)
                .HasColumnType("decimal(5, 2)");
            entity.Property(e => e.IssueDate).HasDefaultValueSql("(getdate())");
            entity.Property(e => e.QuoteNumber).HasMaxLength(50);
            entity.Property(e => e.QuoteStatusId).HasDefaultValue(1);
            entity.Property(e => e.Subject).HasMaxLength(200);
            entity.Property(e => e.Subtotal)
                .HasDefaultValue(0m)
                .HasColumnType("decimal(18, 2)");
            entity.Property(e => e.TaxAmount)
                .HasDefaultValue(0m)
                .HasColumnType("decimal(18, 2)");
            entity.Property(e => e.TotalAmount).HasColumnType("decimal(18, 2)");

            entity.HasOne(d => d.Contact).WithMany(p => p.Quotes)
                .HasForeignKey(d => d.ContactId)
                .HasConstraintName("FK__Quote__ContactId__7D439ABD");

            entity.HasOne(d => d.CreatedByNavigation).WithMany(p => p.QuoteCreatedByNavigations)
                .HasForeignKey(d => d.CreatedBy)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Quote__CreatedBy__04E4BC85");

            entity.HasOne(d => d.Customer).WithMany(p => p.Quotes)
                .HasForeignKey(d => d.CustomerId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Quote__CustomerI__7B5B524B");

            entity.HasOne(d => d.LastUpdatedByNavigation).WithMany(p => p.QuoteLastUpdatedByNavigations)
                .HasForeignKey(d => d.LastUpdatedBy)
                .HasConstraintName("FK__Quote__LastUpdat__05D8E0BE");

            entity.HasOne(d => d.PriceList).WithMany(p => p.Quotes)
                .HasForeignKey(d => d.PriceListId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Quote__PriceList__7C4F7684");

            entity.HasOne(d => d.QuoteStatus).WithMany(p => p.Quotes)
                .HasForeignKey(d => d.QuoteStatusId)
                .HasConstraintName("FK__Quote__QuoteStat__7E37BEF6");

            entity.HasOne(d => d.RemovedByNavigation).WithMany(p => p.QuoteRemovedByNavigations)
                .HasForeignKey(d => d.RemovedBy)
                .HasConstraintName("FK__Quote__RemovedBy__06CD04F7");
        });

        modelBuilder.Entity<QuoteItem>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__QuoteIte__3214EC07B697749C");

            entity.ToTable("QuoteItem", "Sales");

            entity.Property(e => e.CreatedAt).HasDefaultValueSql("(getdate())");
            entity.Property(e => e.Description).HasMaxLength(500);
            entity.Property(e => e.DiscountPercentage)
                .HasDefaultValue(0m)
                .HasColumnType("decimal(5, 2)");
            entity.Property(e => e.LineNumber).HasDefaultValue(1);
            entity.Property(e => e.LineTotal).HasColumnType("decimal(18, 2)");
            entity.Property(e => e.Quantity)
                .HasDefaultValue(1m)
                .HasColumnType("decimal(12, 3)");
            entity.Property(e => e.UnitPrice).HasColumnType("decimal(18, 2)");

            entity.HasOne(d => d.CreatedByNavigation).WithMany(p => p.QuoteItemCreatedByNavigations)
                .HasForeignKey(d => d.CreatedBy)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__QuoteItem__Creat__10566F31");

            entity.HasOne(d => d.LastUpdatedByNavigation).WithMany(p => p.QuoteItemLastUpdatedByNavigations)
                .HasForeignKey(d => d.LastUpdatedBy)
                .HasConstraintName("FK__QuoteItem__LastU__114A936A");

            entity.HasOne(d => d.ProductGroup).WithMany(p => p.QuoteItems)
                .HasForeignKey(d => d.ProductGroupId)
                .HasConstraintName("FK__QuoteItem__Produ__0B91BA14");

            entity.HasOne(d => d.Product).WithMany(p => p.QuoteItems)
                .HasForeignKey(d => d.ProductId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__QuoteItem__Produ__0A9D95DB");

            entity.HasOne(d => d.Quote).WithMany(p => p.QuoteItems)
                .HasForeignKey(d => d.QuoteId)
                .HasConstraintName("FK__QuoteItem__Quote__09A971A2");

            entity.HasOne(d => d.RemovedByNavigation).WithMany(p => p.QuoteItemRemovedByNavigations)
                .HasForeignKey(d => d.RemovedBy)
                .HasConstraintName("FK__QuoteItem__Remov__123EB7A3");
        });

        modelBuilder.Entity<QuoteStatus>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__QuoteSta__3213E83F29577E5F");

            entity.ToTable("QuoteStatus", "Dictionary");

            entity.HasIndex(e => e.Label, "quotestatus_label_unique").IsUnique();

            entity.Property(e => e.Id)
                .ValueGeneratedNever()
                .HasColumnName("id");
            entity.Property(e => e.Label)
                .HasMaxLength(100)
                .HasColumnName("label");
        });

        modelBuilder.Entity<User>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__User__3214EC07F1B366FD");

            entity.ToTable("User", "Auth");

            entity.Property(e => e.Email).HasMaxLength(100);
            entity.Property(e => e.FirstName).HasMaxLength(50);
            entity.Property(e => e.LastName).HasMaxLength(50);
            entity.Property(e => e.LastUpdatedAt).HasColumnType("datetime");
            entity.Property(e => e.Password).HasMaxLength(200);
            entity.Property(e => e.PasswordResetExpires).HasColumnType("datetime");
            entity.Property(e => e.PasswordResetToken).HasMaxLength(200);
            entity.Property(e => e.RemovedAt).HasColumnType("datetime");
            entity.Property(e => e.Role).HasDefaultValue(1);
            entity.Property(e => e.DarkModeEnabled).HasDefaultValue(false);
        });

        modelBuilder.Entity<UserRole>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__UserRole__3213E83F83083160");

            entity.ToTable("UserRole", "Dictionary");

            entity.HasIndex(e => e.Label, "userrole_label_unique").IsUnique();

            entity.Property(e => e.Id)
                .ValueGeneratedNever()
                .HasColumnName("id");
            entity.Property(e => e.Label)
                .HasMaxLength(100)
                .HasColumnName("label");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
