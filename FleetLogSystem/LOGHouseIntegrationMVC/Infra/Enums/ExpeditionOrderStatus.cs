using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace LOGHouseSystem.Infra.Enums
{
    public enum ExpeditionOrderStatus
    {
        [Description("AGUARDANDO_PROCESSAMENTO")]
        [Display(Name = "Aguardando processamento")]
        ProcessingPendenting = 1,

        [Description("PROCESSADO")]
        [Display(Name = "Processado")]
        Processed = 2,

        [Description("EM_PICKING_LIST")]
        [Display(Name = "Em picking list")]
        InPickingList = 3,

        [Description("SEPARADO")]
        [Display(Name = "Separado")]
        Separated = 4,

        [Description("EM_EMPACOTAMENTO")]
        [Display(Name = "Em Empacotamento")]
        InPacking = 5,

        [Description("EMPACOTADO")]
        [Display(Name = "Empacotado")]
        Packed = 6,

        [Description("EXPEDIDO")]
        [Display(Name = "Expedido")]
        Dispatched = 7,

        [Description("BIPAGEM_SEPARACAO")]
        [Display(Name = "Bipagem da Separação")]
        BeepingPickingList = 8,

        [Description("ERRO_INTEGRACAO")]
        [Display(Name = "Erro de integração")]
        ErrorIntegration = 100,

        [Description("ERRO_PROCESSAMENTO")]
        [Display(Name = "Erro de processamento")]
        ErrorProcessing = 101,

        [Description("ERRO_PICKING_LIST")]
        [Display(Name = "Erro de picking list")]
        ErrorPickingList = 102,

        [Description("PROCESSAMENTO_RECUSADO")]
        [Display(Name = "Processamento Recusado")]
        ProcessingRefused = 103,

        [Description("CANCELADO")]
        [Display(Name = "Cancelado")]
        Canceled = 104
    }
}
