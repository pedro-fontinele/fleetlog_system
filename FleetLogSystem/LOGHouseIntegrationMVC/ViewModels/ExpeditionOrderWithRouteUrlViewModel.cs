using Hangfire.Annotations;
using LOGHouseSystem.Infra.Enums;
using LOGHouseSystem.Models;

namespace LOGHouseSystem.ViewModels
{
    public class ExpeditionOrderWithRouteUrlViewModel
    {
        public ExpeditionOrder? Order { get; set; }

        public List<ExpeditionOrder>? ExpeditionOrders { get; set; }

        public string? RouteUrl { get; set; }

        public User UserLoged { get; set; }

        public PackingListTransportation? PackingListTransportation { get; set; }

        public OrderPackingInfo PackingInfo { get; set; }

        public string? Ids { get; set; }
    }

    public class OrderPackingInfo
    {
        public int PackingId { get; set; }

        public string Photo { get; set; }

        public PackingStatus Status { get; set; }
        public string Observation { get; set; }
    }
}
