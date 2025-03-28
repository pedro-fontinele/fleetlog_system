using LOGHouseSystem.Infra.Enums;
using LOGHouseSystem.Models;

namespace LOGHouseSystem.Controllers.API.BarcodeColectorApi.Response
{
    public class PackingListTransportationResponse
    {
        public int Id { get; set; }

        public DateTime CreatedAt { get; set; }

        public int? TransportationPersonId { get; set; }

        public int? ShippingCompanyId { get; set; }

        public string? Observation { get; set; }

        public string? VehiclePlate { get; set; }

        public PackingListTransportationStatus Status { get; set; }

    }
}
