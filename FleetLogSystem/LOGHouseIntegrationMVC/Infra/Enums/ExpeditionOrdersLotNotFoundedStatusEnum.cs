using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace LOGHouseSystem.Infra.Enums
{
    public enum ExpeditionOrdersLotNotFoundedStatusEnum
    {
        [Description("Criado")]
        [Display(Name = "Criado")]
        Created = 0,

        [Description("Pendente")]
        [Display(Name = "Pendente")]
        Pendenting = 1,

        [Description("Enviado")]
        [Display(Name = "Enviado")]
        Sended = 2
    }
}
