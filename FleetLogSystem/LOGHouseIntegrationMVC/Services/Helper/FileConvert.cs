using LOGHouseSystem.Infra.Enums;

namespace LOGHouseSystem.Services.Helper
{
    public class FileConvert
    {
        public int OrderNumber { get; set; }
        public FileTypeEnum Type { get; set; }
        public FileFormatEnum Format { get; set; }
        public string Content { get; set; }
    }
}
