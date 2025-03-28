using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace LOGHouseSystem.Infra.Enums
{
    public enum ShippingMethodEnum
    {
        [Display(Name = "Outro")]
        [Description("Outro")]
        Other = 0,

        [Display(Name = "Melhor Envio")]
        [Description("Melhor Envio")]
        MelhorEnvio = 1,

        [Display(Name = "Mercado Livre")]
        [Description("Mercado Livre")]
        MercadoLivre = 2,

        [Display(Name = "Shopee")]
        [Description("Shopee")]
        Shopee = 3,

        [Display(Name = "Enviado manualmente")]
        [Description("Enviado manualmente")]
        Uploaded = 4,

        [Display(Name = "Kangu")]
        [Description("Kangu")]
        Kangu = 5,

        [Display(Name = "Olist")]
        [Description("Olist")]
        Olist = 6,

        [Display(Name = "Raia Drogasil")]
        [Description("Raia Drogasil")]
        RaiaDrogasil = 7,

        [Display(Name = "Magalu")]
        [Description("Magalu")]
        Magalu = 8,

        [Display(Name = "Amazon")]
        [Description("Amazon")]
        Amazon = 9,

        [Display(Name = "Americanas")]
        [Description("Americanas")]
        Americanas = 10,

        [Display(Name = "Shein")]
        [Description("Shein")]
        Shein = 11
    }
}
