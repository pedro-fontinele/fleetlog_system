using LOGHouseSystem.Infra.Enums;
using System.ComponentModel;

namespace LOGHouseSystem.Models
{
    public class PackingListTransportation
    {
        public int Id { get; set; }

        [DisplayName("Data")]
        public DateTime CreatedAt { get; set; }

        public int? TransportationPersonId { get; set; }
        public virtual TransportationPerson? TransportationPerson { get; set; }

        public int? ShippingCompanyId { get; set; }
        public virtual ShippingCompany? ShippingCompany { get; set; }

        public PackingListTransportationStatus Status { get; set; }

        public virtual IEnumerable<ExpeditionOrder> ExpeditionOrders { get; set;}

        public string? SignatureImgPath { get; set; }

        public string? VehiclePlate { get; set; }

        public string? Observation { get; set; }

        public List<PackingListTransportationHistory> PackingListTransportationHistories { get; set; }
    }
}
