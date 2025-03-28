using System.ComponentModel;

namespace LOGHouseSystem.Models
{
    public class TransportationPerson
    {
        public int Id { get; set; }

        [DisplayName("Nome")]
        public string Name { get; set; }

        [DisplayName("CPF")]
        public string Cpf { get; set; }

        [DisplayName("RG")]
        public string Rg { get; set; }

        [DisplayName("Frente")]
        public string? PathDocumentFront { get; set; }

        [DisplayName("Verso")]
        public string? PathDocumentBack { get; set; }

    }
}
