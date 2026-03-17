using Microsoft.EntityFrameworkCore;
using Restify.BackOffice.Domain.Entities;
using Restify.Core.Application.Interfaces;
using Restify.Core.Domain.Entities;

namespace Restify.BackOffice.Infrastructure.Persistence;

public class BackOfficeDbContext : DbContext
{
    private readonly ICurrentUserService? _currentUserService;
    private readonly Guid? _currentTenantId;

    public BackOfficeDbContext(DbContextOptions<BackOfficeDbContext> options) : base(options)
    {
    }

    public BackOfficeDbContext(DbContextOptions<BackOfficeDbContext> options, ICurrentUserService currentUserService)
        : base(options)
    {
        _currentUserService = currentUserService;
        _currentTenantId = currentUserService.TenantId;
    }

    public DbSet<Category> Categories => Set<Category>();
    public DbSet<Product> Products => Set<Product>();
    public DbSet<ProductModifier> ProductModifiers => Set<ProductModifier>();
    public DbSet<GlobalModifier> GlobalModifiers => Set<GlobalModifier>();
    public DbSet<GlobalModifierProduct> GlobalModifierProducts => Set<GlobalModifierProduct>();
    public DbSet<Table> Tables => Set<Table>();
    public DbSet<Order> Orders => Set<Order>();
    public DbSet<OrderItem> OrderItems => Set<OrderItem>();
    public DbSet<OrderItemModifier> OrderItemModifiers => Set<OrderItemModifier>();
    public DbSet<Invoice> Invoices => Set<Invoice>();
    public DbSet<InvoiceItem> InvoiceItems => Set<InvoiceItem>();
    public DbSet<InvoiceItemModifier> InvoiceItemModifiers => Set<InvoiceItemModifier>();
    public DbSet<CashRegister> CashRegisters => Set<CashRegister>();
    public DbSet<CashRegisterSession> CashRegisterSessions => Set<CashRegisterSession>();
    public DbSet<CashRegisterMovement> CashRegisterMovements => Set<CashRegisterMovement>();
    public DbSet<Supplier> Suppliers => Set<Supplier>();
    public DbSet<InventoryItem> InventoryItems => Set<InventoryItem>();
    public DbSet<InventoryMovement> InventoryMovements => Set<InventoryMovement>();
    public DbSet<PurchaseOrder> PurchaseOrders => Set<PurchaseOrder>();
    public DbSet<PurchaseOrderItem> PurchaseOrderItems => Set<PurchaseOrderItem>();
    public DbSet<Customer> Customers => Set<Customer>();
    public DbSet<Payment> Payments => Set<Payment>();
    public DbSet<PaymentItem> PaymentItems => Set<PaymentItem>();
    public DbSet<DispatchApproval> DispatchApprovals => Set<DispatchApproval>();
    public DbSet<QRCode> QRCodes => Set<QRCode>();
    public DbSet<DeliveryCooperative> DeliveryCooperatives => Set<DeliveryCooperative>();
    public DbSet<DeliveryDriver> DeliveryDrivers => Set<DeliveryDriver>();
    public DbSet<Delivery> Deliveries => Set<Delivery>();
    public DbSet<DriverDocument> DriverDocuments => Set<DriverDocument>();
    public DbSet<PoolDeliveryCommission> PoolDeliveryCommissions => Set<PoolDeliveryCommission>();
    public DbSet<AIImagePromptTemplate> AIImagePromptTemplates => Set<AIImagePromptTemplate>();
    public DbSet<AIImageGeneration> AIImageGenerations => Set<AIImageGeneration>();

    // Accounting
    public DbSet<AccountingAccount> AccountingAccounts => Set<AccountingAccount>();
    public DbSet<AccountingPeriod> AccountingPeriods => Set<AccountingPeriod>();
    public DbSet<JournalEntry> JournalEntries => Set<JournalEntry>();
    public DbSet<JournalEntryLine> JournalEntryLines => Set<JournalEntryLine>();

    // Payroll
    public DbSet<Employee> Employees => Set<Employee>();
    public DbSet<PayrollPeriod> PayrollPeriods => Set<PayrollPeriod>();
    public DbSet<PayrollEntry> PayrollEntries => Set<PayrollEntry>();
    public DbSet<DeductionType> DeductionTypes => Set<DeductionType>();

    // Electronic Invoicing (SRI Ecuador)
    public DbSet<ElectronicDocument> ElectronicDocuments => Set<ElectronicDocument>();
    public DbSet<CreditNote> CreditNotes => Set<CreditNote>();
    public DbSet<CreditNoteItem> CreditNoteItems => Set<CreditNoteItem>();
    public DbSet<WithholdingVoucher> WithholdingVouchers => Set<WithholdingVoucher>();
    public DbSet<WithholdingDetail> WithholdingDetails => Set<WithholdingDetail>();
    public DbSet<FiscalConfiguration> FiscalConfigurations => Set<FiscalConfiguration>();

    // Transfer Approvals
    public DbSet<TransferPaymentRequest> TransferPaymentRequests => Set<TransferPaymentRequest>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Esquema separado para BackOffice
        modelBuilder.HasDefaultSchema("backoffice");

        // Aplicar configuraciones
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(BackOfficeDbContext).Assembly);

        // Query filter para multi-tenancy
        modelBuilder.Entity<Category>()
            .HasQueryFilter(x => _currentTenantId == null || x.TenantId == _currentTenantId);
        
        modelBuilder.Entity<Product>()
            .HasQueryFilter(x => _currentTenantId == null || x.TenantId == _currentTenantId);
        
        modelBuilder.Entity<ProductModifier>()
            .HasQueryFilter(x => _currentTenantId == null || x.TenantId == _currentTenantId);
        
        modelBuilder.Entity<GlobalModifier>()
            .HasQueryFilter(x => _currentTenantId == null || x.TenantId == _currentTenantId);
        
        modelBuilder.Entity<GlobalModifierProduct>()
            .HasQueryFilter(x => _currentTenantId == null || x.TenantId == _currentTenantId);
        
        modelBuilder.Entity<Table>()
            .HasQueryFilter(x => _currentTenantId == null || x.TenantId == _currentTenantId);
        
        modelBuilder.Entity<Order>()
            .HasQueryFilter(x => _currentTenantId == null || x.TenantId == _currentTenantId);
        
        modelBuilder.Entity<OrderItem>()
            .HasQueryFilter(x => _currentTenantId == null || x.TenantId == _currentTenantId);
        
        modelBuilder.Entity<Invoice>()
            .HasQueryFilter(x => _currentTenantId == null || x.TenantId == _currentTenantId);
        
        modelBuilder.Entity<InvoiceItem>()
            .HasQueryFilter(x => _currentTenantId == null || x.TenantId == _currentTenantId);
        
        modelBuilder.Entity<CashRegister>()
            .HasQueryFilter(x => _currentTenantId == null || x.TenantId == _currentTenantId);
        
        modelBuilder.Entity<CashRegisterSession>()
            .HasQueryFilter(x => _currentTenantId == null || x.TenantId == _currentTenantId);
        
        modelBuilder.Entity<CashRegisterMovement>()
            .HasQueryFilter(x => _currentTenantId == null || x.TenantId == _currentTenantId);
        
        modelBuilder.Entity<Supplier>()
            .HasQueryFilter(x => _currentTenantId == null || x.TenantId == _currentTenantId);
        
        modelBuilder.Entity<InventoryItem>()
            .HasQueryFilter(x => _currentTenantId == null || x.TenantId == _currentTenantId);
        
        modelBuilder.Entity<InventoryMovement>()
            .HasQueryFilter(x => _currentTenantId == null || x.TenantId == _currentTenantId);
        
        modelBuilder.Entity<PurchaseOrder>()
            .HasQueryFilter(x => _currentTenantId == null || x.TenantId == _currentTenantId);
        
        modelBuilder.Entity<PurchaseOrderItem>()
            .HasQueryFilter(x => _currentTenantId == null || x.TenantId == _currentTenantId);

        modelBuilder.Entity<Customer>()
            .HasQueryFilter(x => _currentTenantId == null || x.TenantId == _currentTenantId);

        modelBuilder.Entity<Payment>()
            .HasQueryFilter(x => _currentTenantId == null || x.TenantId == _currentTenantId);

        modelBuilder.Entity<DispatchApproval>()
            .HasQueryFilter(x => _currentTenantId == null || x.TenantId == _currentTenantId);

        modelBuilder.Entity<QRCode>()
            .HasQueryFilter(x => _currentTenantId == null || x.TenantId == _currentTenantId);

        modelBuilder.Entity<DeliveryCooperative>()
            .HasQueryFilter(x => _currentTenantId == null || x.TenantId == _currentTenantId);

        modelBuilder.Entity<DeliveryDriver>()
            .HasQueryFilter(x => _currentTenantId == null || x.TenantId == _currentTenantId);

        modelBuilder.Entity<Delivery>()
            .HasQueryFilter(x => _currentTenantId == null || x.TenantId == _currentTenantId);

        modelBuilder.Entity<PoolDeliveryCommission>()
            .HasQueryFilter(x => _currentTenantId == null || x.TenantId == _currentTenantId);

        modelBuilder.Entity<AIImagePromptTemplate>()
            .HasQueryFilter(x => _currentTenantId == null || x.TenantId == _currentTenantId);

        modelBuilder.Entity<AIImageGeneration>()
            .HasQueryFilter(x => _currentTenantId == null || x.TenantId == _currentTenantId);

        // Accounting
        modelBuilder.Entity<AccountingAccount>()
            .HasQueryFilter(x => _currentTenantId == null || x.TenantId == _currentTenantId);

        modelBuilder.Entity<AccountingPeriod>()
            .HasQueryFilter(x => _currentTenantId == null || x.TenantId == _currentTenantId);

        modelBuilder.Entity<JournalEntry>()
            .HasQueryFilter(x => _currentTenantId == null || x.TenantId == _currentTenantId);

        modelBuilder.Entity<JournalEntryLine>()
            .HasQueryFilter(x => _currentTenantId == null || x.TenantId == _currentTenantId);

        // Payroll
        modelBuilder.Entity<Employee>()
            .HasQueryFilter(x => _currentTenantId == null || x.TenantId == _currentTenantId);

        modelBuilder.Entity<PayrollPeriod>()
            .HasQueryFilter(x => _currentTenantId == null || x.TenantId == _currentTenantId);

        modelBuilder.Entity<PayrollEntry>()
            .HasQueryFilter(x => _currentTenantId == null || x.TenantId == _currentTenantId);

        modelBuilder.Entity<DeductionType>()
            .HasQueryFilter(x => _currentTenantId == null || x.TenantId == _currentTenantId);

        // Electronic Invoicing
        modelBuilder.Entity<ElectronicDocument>()
            .HasQueryFilter(x => _currentTenantId == null || x.TenantId == _currentTenantId);

        modelBuilder.Entity<CreditNote>()
            .HasQueryFilter(x => _currentTenantId == null || x.TenantId == _currentTenantId);

        modelBuilder.Entity<CreditNoteItem>()
            .HasQueryFilter(x => _currentTenantId == null || x.TenantId == _currentTenantId);

        modelBuilder.Entity<WithholdingVoucher>()
            .HasQueryFilter(x => _currentTenantId == null || x.TenantId == _currentTenantId);

        modelBuilder.Entity<WithholdingDetail>()
            .HasQueryFilter(x => _currentTenantId == null || x.TenantId == _currentTenantId);

        modelBuilder.Entity<FiscalConfiguration>()
            .HasQueryFilter(x => _currentTenantId == null || x.TenantId == _currentTenantId);

        // Transfer Approvals
        modelBuilder.Entity<TransferPaymentRequest>()
            .HasQueryFilter(x => _currentTenantId == null || x.TenantId == _currentTenantId);
    }

    public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        foreach (var entry in ChangeTracker.Entries<BaseEntity>())
        {
            switch (entry.State)
            {
                case EntityState.Added:
                    entry.Entity.CreatedAt = DateTime.UtcNow;
                    entry.Entity.CreatedBy = _currentUserService?.Email;
                    break;
                case EntityState.Modified:
                    entry.Entity.UpdatedAt = DateTime.UtcNow;
                    entry.Entity.UpdatedBy = _currentUserService?.Email;
                    break;
            }
        }

        foreach (var entry in ChangeTracker.Entries<TenantEntity>())
        {
            if (entry.State == EntityState.Added && entry.Entity.TenantId == Guid.Empty)
            {
                entry.Entity.TenantId = _currentTenantId ?? Guid.Empty;
            }
        }

        return base.SaveChangesAsync(cancellationToken);
    }
}
