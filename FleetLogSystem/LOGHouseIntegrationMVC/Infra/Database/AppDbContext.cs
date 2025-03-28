using LOGHouseSystem.Models;
using Microsoft.EntityFrameworkCore;

namespace LOGHouseSystem.Infra.Database
{
    public class AppDbContext : DbContext
    {
        public DbSet<User> Users { get; set; }
        public DbSet<Client> Clients { get; set; }
        public DbSet<ClientContract> ClientContracts { get; set; }
        public DbSet<ReceiptNote> ReceiptNotes { get; set; }
        public DbSet<ReceiptNoteItem> ReceiptNoteItems { get; set; }
        public DbSet<Product> Products { get; set; }
        public DbSet<InventoryMovement> InventoryMovements { get; set; }
        public DbSet<AddressingStreet> AddressingStreets { get; set; }
        public DbSet<AddressingPosition> AddressingPositions { get; set; }
        public DbSet<CaixaMaster> CaixaMasters { get; set; }
        public DbSet<ReceiptScheduling> ReceiptSchedulings { get; set; }
        public DbSet<PositionAndProduct> PositionsAndProducts { get; set; }
        public DbSet<Integration> Integrations { get; set; }
        public DbSet<IntegrationVariable> IntegrationVariables { get; set; }        
        public DbSet<TinyOrder> TinyOrder { get; set; }
        public DbSet<TinyOrderItem> TinyOrderItem { get; set; }
        public DbSet<BlingOrder> BlingOrders { get; set; }
        public DbSet<BlingOrderItem> BlingOrderItems { get; set; }
        public DbSet<Logger> Logger { get; set; }
        public DbSet<ExpeditionOrder> ExpeditionOrders { get; set; }
        public DbSet<ExpeditionOrderItem> ExpeditionOrderItems { get; set; }
        public DbSet<ShippingDetails> ShippingDetails { get; set; }
        public DbSet<ExpeditionOrderTagShipping> ExpeditionOrderTagShipping { get; set; }
        public DbSet<PickingList> PickingLists { get; set; }
        public DbSet<PickingListItem> PickingListItems { get; set; }
        public DbSet<Packing> Packings { get; set; }
        public DbSet<PackingItem> PackingItems { get; set; }
        public DbSet<HookInput> HookInputs { get; set; }
        public DbSet<Invoice> Invoices { get; set; }
        public DbSet<InvoiceItem> InvoiceItems { get; set; }
        public DbSet<TransportationPerson> TransportationPeople { get; set; }
        public DbSet<ShippingCompany> ShippingCompanies { get; set; }
        public DbSet<PackingListTransportation> PackingListTransportations { get; set; }
        public DbSet<ReturnInvoice> ReturnInvoices { get; set; }
        public DbSet<ReceiptNoteLots> ReceiptNoteLots { get; set; }
        public DbSet<ReturnInvoiceItem> ReturnInvoiceItems { get; set; }
        public DbSet<Cart> Carts { get; set; }
        public DbSet<SentEmail> SentEmails { get; set; }
        public DbSet<PickingListHistory> PickingListHistories { get; set; }
        public DbSet<PackingHistory> PackingHistories { get; set; }
        public DbSet<ExpeditionOrderHistory> ExpeditionOrderHistories { get; set; }
        public DbSet<LabelBilling> LabelBillings { get; set; }
        public DbSet<Devolution> Devolutions { get; set; }
        public DbSet<DevolutionImage> DevolutionImages { get; set; }
        public DbSet<DevolutionAndProduct> DevolutionAndProducts { get; set; }
        public DbSet<DevolutionAndReceiptNote> DevolutionAndReceiptNote { get; set; }
        public DbSet<SmartgoImportation> SmartgoImportations { get; set; }
        public DbSet<PackingListTransportationHistory> PackingListTransportationHistories { get; set; }
        public DbSet<AppVersion> AppVersions { get; set; }
        public DbSet<HangfireExecutionControl> HangfireExecutionControls { get; set; }
        public DbSet<ExpeditionOrdersLotNotFounded> ExpeditionOrdersLotNotFounded { get; set; }
        public DbSet<ReturnInvoiceOrders> ReturnInvoiceOrders { get; set; }
        public DbSet<ReturnInvoiceProductInvoices> ReturnInvoiceProductInvoices { get; set; }
        public DbSet<RetryQueue> RetryQueues { get; set; }
        


        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder) => optionsBuilder.UseSqlServer(Environment.DefaultConnection); 
    }
}
 