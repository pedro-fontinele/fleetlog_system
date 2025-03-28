namespace LOGHouseSystem.Adapters.Extensions.NFeExtension
{
    public interface INFeExtension
    {
        NfeProc? DeserializeNFe(string fileStream);
        Task<NfeProc?> GetNfeByPathAsync(string xmlPath);
        T DeserializeFromXml<T>(string xml);
    }
}
