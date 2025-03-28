using LOGHouseSystem.Infra.Enums;
using Microsoft.Build.Framework;
using System.ComponentModel;
using System.Security.Policy;
using RequiredAttribute = System.ComponentModel.DataAnnotations.RequiredAttribute;

namespace LOGHouseSystem.Models
{
    public class ReceiptScheduling
    {
        public int Id { get; set; }

        [DisplayName("Fornecedor")]
        [Required(ErrorMessage = "É necessário informar o nome do fornecedor")]
        public string Supplier { get; set; }

        [DisplayName("Número da NF")]
        [Required(ErrorMessage = "É necessário informar o número da NF")]
        public string? NfNumber { get; set; }

        [DisplayName("Quantidade de Volumes")]
        [Required(ErrorMessage = "É necessário informar a quantidade de volumes")]
        public decimal? VolumesQuantity { get; set; }

        [DisplayName("Data de Recebimento")]
        [Required(ErrorMessage = "É necessário informar a data de recebimento")]
        public DateTime ReceiptDate { get; set; }

        [DisplayName("Data de Criação")]
        public DateTime CreationDate { get; set; } = DateTime.Now;

        [DisplayName("Caminho do XML")]
        public string? FilesPath { get; set; } 

        public int ClientId { get; set; }

        [DisplayName("Cliente")]
        public virtual Client? Client { get; set; }

        public YesOrNo GeneratedReceiptNote { get; set; } = YesOrNo.No;


    }
}
