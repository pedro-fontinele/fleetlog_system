using System.ComponentModel;

namespace LOGHouseSystem.Infra.Enums
{
    public enum OrderOrigin
    {
        [Description("Tiny")]
        Tiny = 0,

        [Description("Bling")]
        Bling = 1,

        [Description("NFE")]
        NFE = 2,

        [Description("Criação Manual")]
        ClientPanel = 3,

        [Description("Criação por XML")]
        XMLCreation = 4
    }
}
