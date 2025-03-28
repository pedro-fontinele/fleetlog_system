using NLog.Config;
using System.ComponentModel;

namespace LOGHouseSystem.Infra.Enums
{
    public enum PermissionLevel
    {
        [Description("Administrador")]
        Admin = 0,

        [Description("Gerente")]
        Manager = 1,

        [Description("Funcionário")]
        Employee = 2,

        [Description("Cliente")]
        Client = 3
    }

    public enum PermissionLevelTest
    {
        [Description("Administrador")]
        Administrador = 0,

        [Description("Gerente")]
        Gerente = 1,

        [Description("Funcionário")]
        Funcionario = 2,

        [Description("Cliente")]
        Cliente = 3
    }
}
