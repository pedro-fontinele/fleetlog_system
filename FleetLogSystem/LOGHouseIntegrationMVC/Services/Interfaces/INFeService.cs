using LOGHouseSystem.Adapters.Extensions.NFeExtension;
using LOGHouseSystem.Controllers;
using LOGHouseSystem.Models;

namespace LOGHouseSystem.Services.Interfaces
{
    public interface INFeService
    {
        ResponseDTO ImportNfe(string fileText, int statusDevolution, int isScheduling = 0, bool validateStock = false);

        Task<ResponseDTO> ImportNfeAsync(string fileText, int statusDevolution, int isScheduling = 0, bool validateStock = false);

        ResponseDTO ValidatingXmlData(string fileText, int statusDevolution, int isScheduling = 0);

        ResponseDTO GenerateNfeProc(string fileText, int statusDevolution = 0, int isScheduling = 0, bool validateStock = false, bool validarNota = true);
    }
}
