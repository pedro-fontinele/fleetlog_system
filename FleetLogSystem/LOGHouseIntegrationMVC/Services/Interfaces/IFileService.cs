namespace LOGHouseSystem.Services.Interfaces
{
    public interface IFileService
    {
        string GetXMLContent(string filesPath);
        string DeleteXMLFile(string accessKey);
        void IsEmpty(List<IFormFile> files);
        void IsXmlFile(IFormFile file);
        string ReadFile(IFormFile file);
        Task<string> SaveXMLFile(string accessKey, string xml);
    }
}
