using Microsoft.VisualStudio.Web.CodeGenerators.Mvc.Razor;
using System.ComponentModel;

namespace LOGHouseSystem.Infra.Enums
{
    public enum IntegrationType
    {
        [Description("ERP")]
        ERP = 0,

        [Description("Marketplace")]
        Marketplace = 1
    }
}
