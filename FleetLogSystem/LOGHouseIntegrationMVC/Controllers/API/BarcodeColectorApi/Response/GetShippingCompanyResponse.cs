using LOGHouseSystem.Infra.Enums;
using System.ComponentModel;

namespace LOGHouseSystem.Controllers.API.BarcodeColectorApi.Response
{
    public class GetShippingCompanyResponse
    {
        public int Id { get; set; }

        [DisplayName("Nome")]
        public string Name { get; set; }

    }
}
