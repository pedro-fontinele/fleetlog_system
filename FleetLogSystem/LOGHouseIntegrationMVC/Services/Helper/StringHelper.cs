namespace LOGHouseSystem.Services.Helper
{
    public static class StringHelper
    {
        public static async Task<string> GetZplFileContentAsync(this string urlFile)
        {
            if(File.Exists(urlFile))
            {
                return await File.ReadAllTextAsync(urlFile);
            }

            return null;
            
        }
        public static async Task<string> GetImageFileContentAsync(this string urlFile)
        {
            if(File.Exists(urlFile))
            {
                byte[] bytes = await File.ReadAllBytesAsync(urlFile);
                string file = Convert.ToBase64String(bytes);

                return file;
            }

            return null;
            
        }
    }
}
