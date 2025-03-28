using LOGHouseSystem.Services;
using LOGHouseSystem.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;
using NLog.Targets;
using System.Text;

namespace LOGHouseSystem.Controllers.MVC
{
    public class UploadController : Controller
    {
        public INFeService _nfeService;
        public UploadController(INFeService NFeService)
        {
            _nfeService = NFeService;
        }

        public IActionResult Index()
        {
            return View();
        }

        public async Task<IActionResult> UploadFiles(List<IFormFile> files)
        {
            List<string> errors = new List<string>();
            List<string> results = new List<string>();

            if (files.Count == 0)
            {
                errors.Add("Erro: Arquivo(s) não selecionado(s)");
                ViewData["Error"] = errors;
                ViewData["Result"] = results;
                return View(ViewData);
            }

            foreach (var file in files)
            {
                //verifica se existem arquivos 
                if (file == null || file.Length == 0)
                {
                    errors.Add($"Erro: Corpo do arquivo não encontrado");
                    continue;
                }


                if (!file.FileName.Contains(".xml"))
                {
                    errors.Add($"Erro: O arquivo {file.FileName} não é um XML, selecione um arquivo válido.");
                    continue;
                }

                Stream fileStream = file.OpenReadStream();

                using var sr = new StreamReader(fileStream, Encoding.UTF8);

                string content = sr.ReadToEnd();


                //NFE
                var result = _nfeService.ImportNfe(content, 0, 1);

                if (result.Success)
                {
                    results.Add($"{file.FileName} - Arquivo enviado com sucesso!");
                }
                else
                {
                    errors.Add($"Erro: Não foi possível importar o arquivo {file.FileName}. {result.Message}");
                }
            }

            ViewData["Error"] = errors;
            ViewData["Result"] = results;

            return View(ViewData);
        }
    }
}
