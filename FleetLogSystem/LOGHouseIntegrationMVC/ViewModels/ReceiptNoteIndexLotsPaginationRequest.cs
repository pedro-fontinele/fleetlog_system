using LOGHouseSystem.Infra.Pagination;

namespace LOGHouseSystem.ViewModels
{
    public class ReceiptNoteIndexLotsPaginationRequest : PaginationRequest
    {
        public int ClientId { get; set; }
        public int ProductId { get; set; }
    }
}
