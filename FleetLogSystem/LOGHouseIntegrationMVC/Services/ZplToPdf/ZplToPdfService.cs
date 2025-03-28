using LOGHouseSystem.Adapters.Extensions.Labelary;
using LOGHouseSystem.Infra.Enums;
using LOGHouseSystem.Services.Helper;
using PdfSharp.Pdf;
using PdfSharp.Pdf.IO;

namespace LOGHouseSystem.Services
{
    public class ZplToPdfService : IZplToPdfService
    {
        private ILabelaryAPIService _labelaryAPIService;

        public ZplToPdfService(ILabelaryAPIService labelaryAPIService)
        {
            _labelaryAPIService = labelaryAPIService;
        }
        public async Task<List<string>> ConvertFilesToPDFs(List<FileConvert> files)
        {
            var pathResponse = new List<string>();

            foreach (var file in files)
            {
                if (file.Format == FileFormatEnum.Zpl || file.Format == FileFormatEnum.Txt)
                {
                    
                    string filePath;

                    if (file.Type == FileTypeEnum.SimplifiedDanfe)
                    {
                        Log.Info(string.Format("Processado danfe {0} ZPL.", file.OrderNumber));
                        filePath = $"{Environment.ZplConfiguration.SimplifiedDanfePdfPath}/Simplified-Danfe-{file.OrderNumber}.pdf";
                    }
                    else
                    {
                        Log.Info(string.Format("Processado danfe {0} PDF.", file.OrderNumber));
                        filePath = $"{Environment.ZplConfiguration.SimplifiedDanfePdfPath}/Tag-{file.OrderNumber}.pdf";
                    }

                    if (File.Exists(filePath))
                    {
                        File.Delete(filePath);
                    }

                    Thread.Sleep(1000);
                    Log.Info(string.Format("Pedido {0} convertendo ZPL to PDF.", file.OrderNumber));
                    var data = await _labelaryAPIService.ConvertZPLToPDF(file.Content);                    

                    var fileStream = File.Create(filePath);
                    await data.CopyToAsync(fileStream);
                    data.Close();
                    fileStream.Close();

                    pathResponse.Add(filePath);
                }
                else if (file.Format == FileFormatEnum.Image)
                {
                    var filePath = $"{Environment.TagUploaded}/{file.OrderNumber}.pdf";

                    Log.Info(string.Format("Processado Imagem {0}.", file.OrderNumber));

                    PdfHelper.SaveImageAsPdf(file.Content, filePath);

                    pathResponse.Add(filePath);

                }
                else
                {
                    Log.Info(string.Format("Processado Arquivo {0}.", file.OrderNumber));

                    if (File.Exists(file.Content))
                    {
                        pathResponse.Add(file.Content);
                    }                    
                }
            }

            return pathResponse;
        }

        public PdfDocument? MargeSimplifiedDanfesPdfsFiles(List<string> files)
        {

            Log.Info(string.Format("Inificando PDFs.", string.Join(',', files)));
            PdfDocument? merged = null;

            foreach (var file in files)
            {
                if (merged == null)
                {
                    merged = PdfReader.Open(file, PdfDocumentOpenMode.Modify);
                    continue;
                }

                using (PdfDocument filePdf = PdfReader.Open(file, PdfDocumentOpenMode.Import))
                {
                    CopyPages(filePdf, merged);
                }
            }

            merged.Close();

            return merged;
        }

        private void CopyPages(PdfDocument from, PdfDocument to)
        {
            for (int i = 0; i < from.PageCount; i++)
            {
                to.AddPage(from.Pages[i]);
            }
        }
    }
}
