using MassTransit;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Restify.BackOffice.Application.Interfaces;
using Restify.BackOffice.Infrastructure.BenefitHub;
using Restify.BackOffice.Infrastructure.Consumers;
using Restify.BackOffice.Infrastructure.Messaging;
using Restify.BackOffice.Infrastructure.Notifications;
using Restify.BackOffice.Infrastructure.Persistence;
using Restify.BackOffice.Infrastructure.Persistence.Repositories;
using Restify.BackOffice.Infrastructure.Services;
using Restify.BackOffice.Infrastructure.Services.Gateways;
using Restify.BackOffice.Infrastructure.Services.Sri;
using Restify.Core.Application.Interfaces;
using Restify.Core.Infrastructure.Extensions;
using Restify.Core.Infrastructure.Services;

namespace Restify.BackOffice.Infrastructure.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddBackOfficeInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        // DbContext para BackOffice
        services.AddDbContext<BackOfficeDbContext>(options =>
            options.UseNpgsql(
                configuration.GetConnectionString("DefaultConnection"),
                b => b.MigrationsAssembly(typeof(BackOfficeDbContext).Assembly.FullName)
            ));

        // Agregar servicios de Core (incluye ICurrentUserService, IGridConfigurationService, etc.)
        services.AddCoreInfrastructure(configuration);

        // Repositories
        services.AddScoped<ICategoryRepository, CategoryRepository>();
        services.AddScoped<IProductRepository, ProductRepository>();
        services.AddScoped<IGlobalModifierRepository, GlobalModifierRepository>();
        services.AddScoped<ITableRepository, TableRepository>();
        services.AddScoped<IOrderRepository, OrderRepository>();
        services.AddScoped<IInvoiceRepository, InvoiceRepository>();
        services.AddScoped<IReportRepository, ReportRepository>();
        services.AddScoped<ICashRegisterRepository, CashRegisterRepository>();
        services.AddScoped<IInventoryRepository, InventoryRepository>();
        services.AddScoped<ISupplierRepository, SupplierRepository>();
        services.AddScoped<IPurchaseOrderRepository, PurchaseOrderRepository>();
        services.AddScoped<ICustomerRepository, CustomerRepository>();
        services.AddScoped<IPaymentRepository, PaymentRepository>();
        services.AddScoped<IDispatchRepository, DispatchRepository>();
        services.AddScoped<IQRCodeRepository, QRCodeRepository>();

        // Services
        services.AddScoped<ICategoryService, CategoryService>();
        services.AddScoped<IProductService, ProductService>();
        services.AddScoped<IGlobalModifierService, GlobalModifierService>();
        services.AddScoped<ITableService, TableService>();
        services.AddScoped<IOrderService, OrderService>();
        services.AddScoped<IInvoiceService, InvoiceService>();
        services.AddScoped<IReportService, ReportService>();
        services.AddScoped<ICashRegisterService, CashRegisterService>();
        services.AddScoped<IInventoryService, InventoryService>();
        services.AddScoped<ISupplierService, SupplierService>();
        services.AddScoped<IPurchaseOrderService, PurchaseOrderService>();
        services.AddScoped<ICustomerService, CustomerService>();

        // Payment Gateway
        services.AddScoped<IPaymentGateway, ManualPaymentGateway>();
        services.AddScoped<IPaymentGatewayFactory, PaymentGatewayFactory>();

        // Payment & Dispatch Services
        services.AddScoped<IPaymentService, PaymentService>();
        services.AddScoped<IDispatchService, DispatchService>();

        // QR Code & Customer Menu Services
        services.AddScoped<IQRCodeService, QRCodeService>();
        services.AddScoped<ICustomerMenuService, CustomerMenuService>();

        // Table Assignment Service
        services.AddScoped<ITableAssignmentService, TableAssignmentService>();

        // Delivery Repositories
        services.AddScoped<IDeliveryCooperativeRepository, DeliveryCooperativeRepository>();
        services.AddScoped<IDeliveryDriverRepository, DeliveryDriverRepository>();
        services.AddScoped<IDeliveryRepository, DeliveryRepository>();

        // Delivery Services
        services.AddScoped<IDeliveryCooperativeService, DeliveryCooperativeService>();
        services.AddScoped<IDeliveryDriverService, DeliveryDriverService>();
        services.AddScoped<IDeliveryService, DeliveryService>();

        // Pool Driver Services
        services.AddScoped<IPoolDriverService, PoolDriverService>();
        services.AddSingleton<IFileStorageService>(new LocalFileStorageService());

        // Delivery Assignment & Pool Commission Services
        services.AddScoped<IDeliveryAssignmentService, DeliveryAssignmentService>();
        services.AddScoped<IPoolCommissionService, PoolCommissionService>();

        // AI Image Repositories
        services.AddScoped<IAIImagePromptTemplateRepository, AIImagePromptTemplateRepository>();
        services.AddScoped<IAIImageGenerationRepository, AIImageGenerationRepository>();

        // AI Image Services
        services.AddScoped<IAIImagePromptTemplateService, AIImagePromptTemplateService>();
        services.AddScoped<IAIImageService, AIImageService>();

        // HttpClientFactory for VisualCreative API
        services.AddHttpClient("VisualCreative", client =>
        {
            var baseUrl = configuration["VisualCreative:BaseUrl"] ?? "http://localhost:5600";
            client.BaseAddress = new Uri(baseUrl);
            client.Timeout = TimeSpan.FromSeconds(60);
        });

        // Accounting Repositories
        services.AddScoped<IAccountingAccountRepository, AccountingAccountRepository>();
        services.AddScoped<IAccountingPeriodRepository, AccountingPeriodRepository>();
        services.AddScoped<IJournalEntryRepository, JournalEntryRepository>();

        // Accounting Services
        services.AddScoped<IAccountingAccountService, AccountingAccountService>();
        services.AddScoped<IAccountingPeriodService, AccountingPeriodService>();
        services.AddScoped<IJournalEntryService, JournalEntryService>();
        services.AddScoped<IFinancialReportService, FinancialReportService>();

        // Payroll Repositories
        services.AddScoped<IEmployeeRepository, EmployeeRepository>();
        services.AddScoped<IPayrollPeriodRepository, PayrollPeriodRepository>();
        services.AddScoped<IPayrollEntryRepository, PayrollEntryRepository>();
        services.AddScoped<IDeductionTypeRepository, DeductionTypeRepository>();

        // Payroll Services
        services.AddScoped<IEmployeeService, EmployeeService>();
        services.AddScoped<IPayrollPeriodService, PayrollPeriodService>();
        services.AddScoped<IDeductionTypeService, DeductionTypeService>();

        // Electronic Invoicing (SRI Ecuador) Repositories
        services.AddScoped<IElectronicDocumentRepository, ElectronicDocumentRepository>();
        services.AddScoped<ICreditNoteRepository, CreditNoteRepository>();
        services.AddScoped<IWithholdingVoucherRepository, WithholdingVoucherRepository>();
        services.AddScoped<IFiscalConfigurationRepository, FiscalConfigurationRepository>();

        // Electronic Invoicing Services
        services.AddScoped<IAccessKeyGenerator, AccessKeyGenerator>();
        services.AddScoped<IXmlSigningService, XAdESBESSigningService>();
        services.AddScoped<ISriSoapClient, SriSoapClient>();
        services.AddScoped<IRideGeneratorService, RideGeneratorService>();
        services.AddScoped<IFiscalConfigurationService, FiscalConfigurationService>();
        services.AddScoped<IElectronicInvoiceService, ElectronicInvoiceService>();

        // HttpClient for SRI SOAP
        services.AddHttpClient("SRI");

        // Transfer Approval
        services.AddScoped<ITransferPaymentRequestRepository, TransferPaymentRequestRepository>();
        services.AddScoped<ITransferApprovalService, TransferApprovalService>();

        // Multi-Sucursal
        services.AddScoped<IBranchService, BranchService>();
        services.AddScoped<IManagerAssignmentService, ManagerAssignmentService>();

        // Cash Closing y Turnos (Fase 9)
        services.AddScoped<ICashClosingService, CashClosingService>();
        services.AddScoped<IShiftService, ShiftService>();

        // Fase 11 — Features Operativos Criticos
        services.AddScoped<IRecipeService, RecipeService>();
        services.AddScoped<ITableReservationService, TableReservationService>();
        services.AddScoped<IPromotionService, PromotionService>();
        services.AddScoped<IStockAlertService, StockAlertService>();

        // Fase 12 — Franquicias y Webhooks
        services.AddScoped<IFranchiseService, FranchiseService>();
        services.AddScoped<IWebhookService, WebhookService>();

        // Fase 13 — Split Payment
        services.AddScoped<ISplitPaymentService, SplitPaymentService>();

        // Notification Client (SMS) — Null Object si no esta configurado
        var notificationBaseUrl = configuration["NotificationService:BaseUrl"];
        var notificationApiKey = configuration["NotificationService:ApiKey"];

        if (!string.IsNullOrEmpty(notificationBaseUrl))
        {
            services.AddHttpClient<INotificationClient, HttpNotificationClient>(client =>
            {
                client.BaseAddress = new Uri(notificationBaseUrl);
                client.Timeout = TimeSpan.FromMilliseconds(2000);
                if (!string.IsNullOrEmpty(notificationApiKey))
                    client.DefaultRequestHeaders.Add("X-Api-Key", notificationApiKey);
            });
        }
        else
        {
            services.AddScoped<INotificationClient, NullNotificationClient>();
        }

        // HttpClient para despacho de webhooks outbound
        services.AddHttpClient("Webhook", client =>
        {
            client.DefaultRequestHeaders.Add("User-Agent", "Restify-Webhook/1.0");
        });

        // BenefitHub Integration (Fase 10)
        var benefitHubBaseUrl = configuration["BenefitHub:BaseUrl"];
        var benefitHubApiKey = configuration["BenefitHub:ApiKey"];

        if (!string.IsNullOrEmpty(benefitHubBaseUrl) && !string.IsNullOrEmpty(benefitHubApiKey))
        {
            services.AddHttpClient<IBenefitHubClient, HttpBenefitHubClient>(client =>
            {
                client.BaseAddress = new Uri(benefitHubBaseUrl);
                client.DefaultRequestHeaders.Add("X-BenefitHub-Key", benefitHubApiKey);
                client.Timeout = TimeSpan.FromMilliseconds(500); // fail fast
            });
        }
        else
        {
            services.AddScoped<IBenefitHubClient, NullBenefitHubClient>();
        }

        // MassTransit + RabbitMQ Event Publisher
        services.AddScoped<IEventPublisher, RabbitMqPublisher>();
        services.AddMassTransit(x =>
        {
            // Consumers para eventos de FElectonica
            x.AddConsumer<DocumentAuthorizedConsumer>();
            x.AddConsumer<DocumentRejectedConsumer>();
            x.AddConsumer<DocumentProcessingFailedConsumer>();

            x.UsingRabbitMq((context, cfg) =>
            {
                var host = configuration["RabbitMQ:Host"] ?? "localhost";
                var user = configuration["RabbitMQ:Username"] ?? "restaurant";
                var pass = configuration["RabbitMQ:Password"] ?? "restaurant123";

                cfg.Host(host, "/", h =>
                {
                    h.Username(user);
                    h.Password(pass);
                });

                cfg.ConfigureEndpoints(context);
            });
        });

        return services;
    }
}
