using LOGHouseSystem.Infra.Enums;
using LOGHouseSystem.Models;
using System.ComponentModel;

namespace LOGHouseSystem.ViewModels
{
    public class IntegrationViewModel
    {
        public int Id { get; set; }

        [DisplayName("Nome")]
        public string Name { get; set; }

        [DisplayName("Tipo")]
        public IntegrationType Type { get; set; }

        public Status Status { get; set; }

        public string UrlToBling { get; set; }
        public string UrlToTiny { get; set; }

        public List<IntegrationVariable> Variables { get; set; }
    }
}
