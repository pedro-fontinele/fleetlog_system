using System.ComponentModel;

namespace LOGHouseSystem.Infra.Enums
{
    public enum DevolutionStatus
    {
        [Description("Aguardando Resposta")]
        AguardandoResposta = 0,

        [Description("Finalizado")]
        Finalizado = 1,
    }
}
