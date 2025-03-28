using LOGHouseSystem.Infra.Enums;
using LOGHouseSystem.Models;
using System.ComponentModel;

namespace LOGHouseSystem.Controllers.API.BarcodeColectorApi.Response
{
    public class GetProductsToAddressingResponse
    {
        public int ReceiptNoteID { get; set; }

        public List<ProductAddressing> Products { get; set; }
    }

    public class ProductAddressing
    {
        public int ProductId { get; set; }

        public int ReceiptNoteItemID { get; set; }

        public string Code { get; set; }

        public string? Description { get; set; }

        public string? Ean { get; set; }

        public double StockQuantity { get; set; }
        public NoteItemStatus Status { get; internal set; }
    }


}
