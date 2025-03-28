using System.Net;
using System.Text;

namespace LOGHouseSystem.Adapters.Extensions.Labelary
{
    public class LabelaryAPIService : ILabelaryAPIService
    {
        public LabelaryAPIService()
        {
        }

        public async Task<Stream?> ConvertZPLToPDF(string ZPLFile)
        {
            byte[] zpl = Encoding.UTF8.GetBytes(ZPLFile);

            // adjust print density (8dpmm), label width (4 inches), label height (6 inches), and label index (0) as necessary
            var request = (HttpWebRequest)WebRequest.Create($"{ServiceCollectionExtention.LabelaryBaseURL}/v1/printers/8dpmm/labels/4x6/0/");
            request.Method = "POST";
            request.Accept = "application/pdf"; // omit this line to get PNG images back
            request.ContentType = "application/x-www-form-urlencoded";
            request.ContentLength = zpl.Length;

            var requestStream = await request.GetRequestStreamAsync();
            requestStream.Write(zpl, 0, zpl.Length);
            requestStream.Close();

            var response = (HttpWebResponse)await request.GetResponseAsync();
            var responseStream = response.GetResponseStream();

            return responseStream;
        }
    }
}
