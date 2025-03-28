using LOGHouseSystem.Services.Helper;
using PdfSharp.Pdf;

namespace LOGHouseSystem.Services
{
    public interface IZplToPdfService
    {
        Task<List<string>> ConvertFilesToPDFs(List<FileConvert> files);
        PdfDocument? MargeSimplifiedDanfesPdfsFiles(List<string> files);
    }
}