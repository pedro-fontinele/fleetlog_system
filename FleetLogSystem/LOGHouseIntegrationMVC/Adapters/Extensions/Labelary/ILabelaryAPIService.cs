namespace LOGHouseSystem.Adapters.Extensions.Labelary
{
    public interface ILabelaryAPIService
    {
        Task<Stream?> ConvertZPLToPDF(string ZPLFile);
    }
}
