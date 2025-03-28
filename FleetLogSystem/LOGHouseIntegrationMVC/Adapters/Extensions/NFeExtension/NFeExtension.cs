using System.IO;
using System.Xml;
using System.Xml.Serialization;

namespace LOGHouseSystem.Adapters.Extensions.NFeExtension
{
    public class NFeExtension : INFeExtension
    {
        public NFeExtension() { }

        private string xmlBasePath = Environment.NFeXmlBasePath;
        public NfeProc? DeserializeNFe(string fileStream)
        {
            return DeserializeFromXml<NfeProc>(fileStream);
        }

        public T DeserializeFromXml<T>(string xml)
        {
            T result;
            XmlSerializer ser = new XmlSerializer(typeof(T));
            using (TextReader tr = new StringReader(xml))
            {
                result = (T)ser.Deserialize(tr);               
            }

            return result;
        }

        public async Task<NfeProc?> GetNfeByPathAsync(string xmlPath)
        {
            if (File.Exists(xmlPath))
            {
                var xml = await File.ReadAllTextAsync(xmlPath);
                return DeserializeFromXml<NfeProc>(xml);
            }

            return null;            
        }
    }
}
