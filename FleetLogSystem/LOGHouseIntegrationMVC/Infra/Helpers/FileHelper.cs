namespace LOGHouseSystem.Infra.Helpers
{
    public static class FileHelper
    {
        public static void SaveFile(string path, byte[] bytes)
        {
            using (Stream stream = new MemoryStream())
            {
                stream.Write(bytes, 0, bytes.Length);
                stream.Position = 0;
                FileStream fileStream = File.Create(path, (int)stream.Length);
                byte[] bytesInStream = new byte[stream.Length];
                stream.Read(bytesInStream, 0, bytesInStream.Length);
                fileStream.Write(bytesInStream, 0, bytesInStream.Length);
                fileStream.Close();
            }
        }

        public static void RenameFile(string originalName, string newName)
        {
            File.Move(originalName, newName);
        }
    }
}
