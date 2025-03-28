using LOGHouseSystem.Infra.Enums;
using LOGHouseSystem.Infra.Pagination;

namespace LOGHouseSystem.ViewModels
{
    public class NotFoundedPaginationRequestViewModel : PaginationRequest
    {
        public ExpeditionOrdersLotNotFoundedStatusEnum? Status { get; set; }
    }
}
