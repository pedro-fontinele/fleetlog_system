using LOGHouseSystem.Services.Interfaces;
using Microsoft.AspNetCore.Http;
using System.Text;

namespace LOGHouseSystem.Services
{
    public class FileService : IFileService
    {
        public string DeleteXMLFile(string accessKey)
        {
            var xmlPath = $"{Environment.XmlUploadPath}/{accessKey}.xml";

            if (File.Exists(xmlPath))
            {
                File.Delete(xmlPath);
            }

            return xmlPath;
        }

        public string GetXMLContent(string filesPath)
        {
            string content = File.ReadAllText(filesPath);

            return content;
        }

        public void IsEmpty(List<IFormFile> files)
        {
            if (files.Count == 0)
            {
                throw new Exception("Erro: Arquivo(s) não selecionado(s)");                
            }
        }

        public void IsXmlFile(IFormFile file)
        {
            //verifica se existem arquivos 
            if (file == null || file.Length == 0)
            {
                throw new Exception($"Erro: Corpo do arquivo não encontrado");
            }

            if (!file.FileName.Contains(".xml"))
            {
                throw new Exception($"Erro: O arquivo {file.FileName} não é um XML, selecione um arquivo válido.");                
            }
        }

        public string ReadFile(IFormFile file)
        {
            Stream fileStream = file.OpenReadStream();

            using var sr = new StreamReader(fileStream, Encoding.UTF8);

            string content = sr.ReadToEnd();

            return content;
        }

        public async Task<string> SaveXMLFile(string accessKey, string xml)
        {
            var xmlPath = DeleteXMLFile(accessKey);

            await File.WriteAllTextAsync(xmlPath, xml);

            return xmlPath;
        }
    }
}
