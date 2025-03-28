using LOGHouseSystem.Infra.Pagination;
﻿using LOGHouseSystem.Infra.Enums;
using LOGHouseSystem.Models;
using System.ComponentModel;

namespace LOGHouseSystem.ViewModels
{
    public class ExpeditionOrderIndexViewModel : PaginationRequest
    {
        public PaginationBase<ExpeditionOrderWithPickingListViewModel>? Orders { get; set; }
        public ExpeditionOrdersPage Page { get; set; }

        public User UserLoged { get; set; }

        public ExpeditionOrderFilterViewModel? Filter { get; set; }

        public readonly List<ExpeditionOrderStatus> IntegrationStatus = new List<ExpeditionOrderStatus>(){ ExpeditionOrderStatus.ProcessingPendenting };
        public readonly List<ExpeditionOrderStatus> OrdersStatus = new List<ExpeditionOrderStatus>() { ExpeditionOrderStatus.Dispatched, ExpeditionOrderStatus.Processed, ExpeditionOrderStatus.InPickingList, ExpeditionOrderStatus.Separated, ExpeditionOrderStatus.InPacking , ExpeditionOrderStatus.Packed, ExpeditionOrderStatus.ErrorIntegration, ExpeditionOrderStatus.ErrorProcessing, ExpeditionOrderStatus.ErrorPickingList };
    }

    public enum ExpeditionOrdersPage
    {
        [Description("Integração")]
        Integration = 0,
        [Description("Pedidos")]
        Orders = 1
    }
}


