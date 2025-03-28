using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace LOGHouseSystem.Infra.Enums
{
    public enum Status
    {
        Inativo = 0,
        Ativo = 1
    }

    public enum NoteStatus
    {
        [Display(Name = "Aguardando")]
        [Description("Aguardando")]
        Aguardando = 0,

        [Display(Name = "Em Aguardando")]
        [Description("Em Andamento")]
        EmAndamento = 1,

        [Display(Name = "Validado")]
        [Description("Validado")]
        NotaOk = 2,

        [Display(Name = "Divergente")]
        [Description("Divergente")]
        NotaDivergente = 3,

        [Display(Name = "Finalizado")]
        [Description("Finalizado")]
        Finalizada = 4,

        [Display(Name = "Rejeitado")]
        [Description("Rejeitado")]
        Rejeitada = 5,

        [Display(Name = "Aguardando Endereçamento")]
        [Description("Aguardando Endereçamento")]
        AguardandoEnderecamento = 6,

    }

    public enum NoteItemStatus
    {
        Aguardando = 0,
        EmAndamento = 1,
        ItemOk = 2,
        ItemDivergente = 3,
        AguardandoEnderecamento = 4,
        Finalizado = 5
    }

    public enum AddressingPositionStatus
    {
        Inativo = 0,
        Disponivel = 1,
        Ocupado = 2,
        Bloqueado = 3
    }

    public enum PickingListStatus
    {
        [Description("Gerado")]
        Gerado = 0,

        [Description("Em atendimento")]
        EmAtendimento = 1,

        [Description("Finalizado")]
        Finalizado = 2,

        [Description("Cancelado")]
        Cancelado = 3
    }

    public enum PickingListItemStatus
    {
        Gerado = 0,
        EmAndamento = 1,
        Finalizado = 2,
        Cancelado = 3
    }

    public enum PackingStatus
    {
        [Description("Gerado")]
        Gerado = 0,

        [Description("Em atendimento")]
        EmAtendimento = 1,

        [Description("Finalizado")]
        Finalizado = 2,

        [Description("Cancelado")]
        Cancelado = 3
    }

    public enum PackingItemStatus
    {
        [Description("Gerado")]
        Gerado = 0,

        [Description("Em andamento")]
        EmAndamento = 1,

        [Description("Finalizado")]
        Finalizado = 2,

        [Description("Cancelado")]
        Cancelado = 3
    }

    public enum PackingListTransportationStatus
    {
        [Description("Em andamento")]
        Gerado = 0,

        [Description("Finalizado")]
        Finalizado = 1
    }

    public enum LotStatus
    {
        [Description("Gerado")]
        Gerado = 0,

        [Description("Em andamento")]
        EmAndamento = 1,

        [Description("Finalizado")]
        Finalizado = 2
    }
}
