using Hangfire;
using Hangfire.SqlServer;
using LOGHouseSystem;
using LOGHouseSystem.Adapters.API.MercadoLivre;
using LOGHouseSystem.Adapters.Extensions.BlingExtention;
using LOGHouseSystem.Adapters.Extensions.BlingOAUTHExtension;
using LOGHouseSystem.Adapters.Extensions.Kangu;
using LOGHouseSystem.Adapters.Extensions.Labelary;
using LOGHouseSystem.Adapters.Extensions.MelhorEnvioExtension;
using LOGHouseSystem.Adapters.Extensions.NFeExtension;
using LOGHouseSystem.Adapters.Extensions.ShopeeExtension;
using LOGHouseSystem.Adapters.Extensions.TinyExtension;
using LOGHouseSystem.Infra.Database;
using LOGHouseSystem.Models.SendEmails;
using LOGHouseSystem.Repositories;
using LOGHouseSystem.Repositories.Interfaces;
using LOGHouseSystem.Services;
using LOGHouseSystem.Services.HangFire;
using LOGHouseSystem.Services.HangFire.Interface;
using LOGHouseSystem.Services.Integrations;
using LOGHouseSystem.Services.Interfaces;
using LOGHouseSystem.Services.Reports;
using LOGHouseSystem.Services.Smartgo;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.StaticFiles;
using Microsoft.Extensions.Hosting;
using Microsoft.IdentityModel.Tokens;
using PdfSharp.Charting;
using System.Text;
using System.Globalization;
using DocumentFormat.OpenXml.Office2016.Drawing.ChartDrawing;
using LOGHouseSystem.Services.SmartgoImportation;
using LOGHouseSystem.Services.PositionAndProduct;
using Microsoft.Extensions.DependencyInjection;
using LOGHouseSystem.Services.RetryQueue;


var builder = WebApplication.CreateBuilder(args);

var cultureInfo = new CultureInfo("pt-BR"); 
CultureInfo.DefaultThreadCurrentCulture = cultureInfo;
CultureInfo.DefaultThreadCurrentUICulture = cultureInfo;


builder.Services.AddDbContext<AppDbContext>();
builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
}).AddJwtBearer(o =>
{
    o.TokenValidationParameters = new TokenValidationParameters
    {
        ValidIssuer = builder.Configuration["Jwt:Issuer"],
        ValidAudience = builder.Configuration["Jwt:Audience"],
        IssuerSigningKey = new SymmetricSecurityKey
        (Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"])),
        ValidateIssuer = false,
        ValidateAudience = false,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
    };
});

builder.Services.AddControllersWithViews();

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddCors();
builder.Services.AddDistributedMemoryCache();

builder.Services.AddSwaggerGen();


builder.Services.AddHangfire(configuration => configuration
    .SetDataCompatibilityLevel(CompatibilityLevel.Version_170)
    .UseSimpleAssemblyNameTypeSerializer()
    .UseRecommendedSerializerSettings()
    .UseSqlServerStorage(LOGHouseSystem.Environment.DefaultConnection, new SqlServerStorageOptions
    {
        CommandBatchMaxTimeout = TimeSpan.FromMinutes(5),
        SlidingInvisibilityTimeout = TimeSpan.FromMinutes(5),
        QueuePollInterval = TimeSpan.Zero,
        UseRecommendedIsolationLevel = true,
        DisableGlobalLocks = true
    }));



//Injeções de Dependência
//Repositorys
builder.Services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();
builder.Services.AddScoped<IAddressingPositionRepository, AddressingPositionRepository>();
builder.Services.AddScoped<IAddressingStreetRepository, AddressingStreetRepository>();
builder.Services.AddScoped<IClientContractsRepository, ClientContractsRepository>();
builder.Services.AddScoped<IClientsRepository, ClientsRepository>();
builder.Services.AddScoped<IProductRepository, ProductRepository>();
builder.Services.AddScoped<IReceiptNoteItemRepository, ReceiptNoteItemRepository>();
builder.Services.AddScoped<IReceiptNoteRepository, ReceiptNoteRepository>();
builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<IInventoryMovementRepository, InventoryMovementRepository>();
builder.Services.AddScoped<IReceiptSchedulingRepository, ReceiptSchedulingRepository>();
builder.Services.AddScoped<IPositionAndProductRepository, PositionAndProductRepository>();
builder.Services.AddScoped<ICaixaMastersRepository, CaixaMasterRepository>();
builder.Services.AddScoped<ITinyOrdersRepository, TinyOrdersRepository>();
builder.Services.AddScoped<IBlingOrdersRepository, BlingOrdersRepository>();
builder.Services.AddScoped<IIntegrationRepository, IntegrationRepository>();
builder.Services.AddScoped<IIntegrationVariableRepository, IntegrationVariableRepository>();
builder.Services.AddScoped<IExpeditionOrderItemsRepository, ExpeditionOrderItemsRepository>();
builder.Services.AddScoped<IExpeditionOrderRepository, ExpeditionOrderRepository>();
builder.Services.AddScoped<IShippingDetailsRepository, ShippingDetailsRepository>();
builder.Services.AddScoped<IExpeditionOrderTagShippingRepository, ExpeditionOrderTagShippingRepository>();
builder.Services.AddScoped<IPickingListRepository, PickingListRepository>();
builder.Services.AddScoped<IPickingListItemRepository, PickingListItemRepository>();
builder.Services.AddScoped<IPackingRepository, PackingRepository>();
builder.Services.AddScoped<IPackingItemRepository, PackingItemRepository>();
builder.Services.AddScoped<IHookInputRepository, HookInputRepository>();
builder.Services.AddScoped<IInvoiceRepository, InvoiceRepository>();
builder.Services.AddScoped<ITransportationPersonRepository, TransportationPersonRepository>();
builder.Services.AddScoped<IShippingCompanyRepository, ShippingCompanyRepository>();
builder.Services.AddScoped<IPackingListTransportationRepository, PackingListTransportationRepository>();
builder.Services.AddScoped<IReturnInvoiceRepository, ReturnInvoiceRepository>();
builder.Services.AddScoped<IReturnInvoiceItemRepository, ReturnInvoiceItemRepository>();
builder.Services.AddScoped<IReceiptNoteLotsRepository, ReceiptNoteLotsRepository>();
builder.Services.AddScoped<ICartRepository, CartRepository>();
builder.Services.AddScoped<ISentEmailRepository, SentEmailRepository>();
builder.Services.AddScoped<IPickingListHistoryRepository, PickingListHistoryRepository>();
builder.Services.AddScoped<IPackingHistoryRepository, PackingHistoryRepository>();
builder.Services.AddScoped<IExpeditionOrderHistoryRepository, ExpeditionOrderHistoryRepository>();
builder.Services.AddScoped<ILabelBillingRepository, LabelBillingRepository>();
builder.Services.AddScoped<IDevolutionRepository, DevolutionRepository>();
builder.Services.AddScoped<IDevolutionAndProductRepository, DevolutionAndProductRepository>();
builder.Services.AddScoped<IDevolutionImageRepository, DevolutionImageRepository>();
builder.Services.AddScoped<IDevolutionAndReceiptNoteRepository, DevolutionAndReceiptNoteRepository>();
builder.Services.AddScoped<ISmartgoImportationRepository, SmartgoImportationRepository>();
builder.Services.AddScoped<IPackingListTransportationHistoryRepository, PackingListTransportationHistoryRepository>();
builder.Services.AddScoped<IAppVersionRepository, AppVersionRepository>();
builder.Services.AddScoped<IHangfireExecutionRepository, HangfireExecutionRepository>();
builder.Services.AddScoped<IRetryQueueRepository, RetryQueueRepository>();
builder.Services.AddScoped<IExpeditionOrdersLotNotFoundedRepository, ExpeditionOrdersLotNotFoundedRepository>();
builder.Services.AddScoped<IReturnInvoiceOrdersRepository, ReturnInvoiceOrdersRepository>();
builder.Services.AddScoped<IReturnInvoiceProductInvoicesRepository, ReturnInvoiceProductInvoicesRepository>();            
builder.Services.AddScoped<IProductStockService, ProductStockService>();


builder.Services.Configure<EmailSettings>(builder.Configuration.GetSection("EmailSettings"));
builder.Services.AddScoped<IEmailService, EmailService>();
builder.Services.AddScoped<ITinyHookService, TinyHookService>();
builder.Services.AddScoped<IBlingCallbackService, BlingCallbackService>();
builder.Services.AddScoped<IPipedriveService, PipedriveService>();
builder.Services.AddScoped<IReceiptNoteItemService, ReceiptNoteItemService>();
builder.Services.AddScoped<IShopeeService, ShopeeService>();
builder.Services.AddScoped<IDataShopeeService, DataShopeeService>();
builder.Services.AddScoped<IExpeditionOrderTagShippingService, ExpeditionOrderTagShippingService>();
builder.Services.AddScoped<IPickingListItemService, PickingListItemService>();
builder.Services.AddScoped<IPickingListService, PickingListService>();
builder.Services.AddScoped<IDataMelhorEnvioService, DataMelhorEnvioService>();
builder.Services.AddScoped<IPackingService, PackingService>();
builder.Services.AddScoped<IPackingItemService, PackingItemService>();
builder.Services.AddScoped<IIntegrationService, IntegrationService>();
builder.Services.AddScoped<ISimplifiedDanfeService, SimplifiedDanfeService>();
builder.Services.AddScoped<INFeService, NFeService>();
builder.Services.AddScoped<IHookInputService, HookInputService>();
builder.Services.AddScoped<IHookInputRoutine, HookInputRoutine>();
builder.Services.AddScoped<IShippingTagsRoutine, ShippingTagsRoutine>();
builder.Services.AddScoped<IInvoiceService, InvoiceService>();
builder.Services.AddScoped<IZplToPdfService, ZplToPdfService>();
builder.Services.AddScoped<IReceiptNoteLotsService, ReceiptNoteLotsService>();
builder.Services.AddScoped<ITransportationPersonService, TransportationPersonService>();
builder.Services.AddScoped<IShippingCompanyService, ShippingCompanyService>();
builder.Services.AddScoped<IPackingListTransportationService, PackingListTransportationService>();
builder.Services.AddScoped<IBlingNFeService, BingNFeService>();
builder.Services.AddScoped<IDataBlingService, DataBlingService>();
builder.Services.AddScoped<IAPIBlingService, APIBlingService>();
builder.Services.AddScoped<IPickingListHistoryService, PickingListHistoryService>();
builder.Services.AddScoped<IPackingHistoryService, PackingHistoryService>();
builder.Services.AddScoped<IExpeditionOrderHistoryService, ExpeditionOrderHistoryService>();
builder.Services.AddScoped<IKanguExtensionService, KanguExtensionService>();
builder.Services.AddScoped<IReceiptNoteService, ReceiptNoteService>();
builder.Services.AddScoped<IDevolutionService, DevolutionService>();
builder.Services.AddScoped<IReceiptNoteReportService, ReceiptNoteReportService>();
builder.Services.AddScoped<IDevolutionAndReceiptNoteService, DevolutionAndReceiptNoteService>();
builder.Services.AddScoped<IExpeditionOrderReportService, ExpeditionOrderReportService>();
builder.Services.AddScoped<ISmartgoImportationService, SmartgoImportationService>();
builder.Services.AddScoped<IPositionAndProductService, PositionAndProductService>();
builder.Services.AddScoped<IHangfireExecutionService, HangfireExecutionService>();
builder.Services.AddScoped<ITagService, TagService>();
builder.Services.AddScoped<IRetryQueueService, RetryQueueService>();
builder.Services.AddScoped<IExpeditionOrdersLotNotFoundedService, ExpeditionOrdersLotNotFoundedService>();
builder.Services.AddScoped<IReturnInvoiceService, ReturnInvoiceService>();
builder.Services.AddScoped<IFileService, FileService>();


builder.Services.AddScoped<IHookInputRoutine, HookInputRoutine>();
builder.Services.AddScoped<IReturnInvoiceRoutine, ReturnInvoiceRoutine>();
builder.Services.AddScoped<IReceptNoteLotsRoutine, ReceptNoteLotsRoutine>();
builder.Services.AddScoped<ISentEmailService, SentEmailService>();
builder.Services.AddScoped<ISmartGoService, SmartGoService>();
builder.Services.AddScoped<IKanguService, KanguService>();
builder.Services.AddScoped<INFeRoutine, NFeRoutine>();

// Extensions
builder.Services.AddTinyExtension();
builder.Services.AddBlingExtension();
builder.Services.UseLabelaryAPI(LOGHouseSystem.Environment.ZplConfiguration.LabelaryAPIBaseRoute);
builder.Services.AddScoped<IAPIShopeeService, ClientShopeeService>();
builder.Services.AddScoped<IMelhorEnvioAPIServices, ClientMelhorEnvioService>();
builder.Services.AddScoped<IMelhorEnvioService, MelhorEnvioService>();
builder.Services.AddScoped<IMercadoLivreExtension, MercadoLivreExtension>();
builder.Services.AddScoped<IRefreshTokenRoutine, RefreshTokenRoutine>();
builder.Services.AddScoped<IIntegrationMercadoLivreService, IntegrationMercadoLivreService>();
builder.Services.AddScoped<IExpeditionOrderService, ExpeditionOrderService>();
builder.Services.AddScoped<IBlingService, BlingService>();
builder.Services.AddScoped<INFeExtension, NFeExtension>();

// Hooks related Return Invoice flow
/*services.AddScoped<IHookInputRoutine, HookInputRoutine>();
services.AddScoped<IReturnInvoiceRoutine, ReturnInvoiceRoutine>();
services.AddScoped<IReceptNoteLotsRoutine, ReceptNoteLotsRoutine>();*/

//ServiceExtensionHookReturnInvoiceConfiguration.AddReturnInvoiceHook();



builder.Services.AddSession(options =>
{
    options.Cookie.IsEssential = true;
    options.IdleTimeout = TimeSpan.FromMinutes(50);
    //options.Cookie.Expiration = TimeSpan.FromMinutes(50);
});

var app = builder.Build();


if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHangfireServer();

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseSwagger();
app.UseSwaggerUI();
app.UseCors(x => x
   .AllowAnyOrigin()
   .AllowAnyMethod()
   .AllowAnyHeader());

app.UseAuthentication();
app.UseAuthorization();

app.UseSession();

app.UseSwagger();
app.UseSwaggerUI();


//RecurringJob.AddOrUpdate<IReturnInvoiceRoutine>(x => x.ReturnInvoiceRoutineMethod(), Cron.Daily);
RecurringJob.AddOrUpdate<IRefreshTokenRoutine>(x => x.RefreshAccessTokensRoutine(), Cron.HourInterval(3));
RecurringJob.AddOrUpdate<IHookInputRoutine>(x => x.HookRoutine(), "* * * * *");
RecurringJob.AddOrUpdate<IHookInputRoutine>(x => x.ForceAllIntegration(), "*/50 6-18 * * *");
RecurringJob.AddOrUpdate<IHookInputRoutine>(x => x.HookInvoiceRoutine(), "* * * * *");
//RecurringJob.AddOrUpdate<INFeRoutine>(x => x.Routine(), "00 01 * * *");
RecurringJob.AddOrUpdate<LogService>(x => x.CleanLogger(), Cron.DayInterval(3));
RecurringJob.AddOrUpdate<IHookInputRoutine>(x => x.DeleteOldHooks(), Cron.Daily(23));
RecurringJob.AddOrUpdate<IShippingTagsRoutine>(x => x.GetBlockedTags(), "*/20 * * * *");


FileExtensionContentTypeProvider contentTypes = new FileExtensionContentTypeProvider();
contentTypes.Mappings[".apk"] = "application/vnd.android.package-archive";
app.UseStaticFiles(new StaticFileOptions
{
    ContentTypeProvider = contentTypes
});


app.UseHangfireDashboard("/hangfire", new DashboardOptions
{
    Authorization = new[] { new MyHangfireDashboardAuthorizationFilter() },
});
GlobalJobFilters.Filters.Add(new AutomaticRetryAttribute { Attempts = 2 });

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Login}/{action=Index}/{id?}");

app.Run();


