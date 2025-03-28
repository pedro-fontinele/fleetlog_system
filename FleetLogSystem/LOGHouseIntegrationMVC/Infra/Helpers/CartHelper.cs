using System.Drawing;
using System.Drawing.Imaging;
using Zen.Barcode;

namespace LOGHouseSystem.Infra.Helpers
{
    public static class CartHelper
    {
        public static byte[] GenerateBarCode(int idCarrinho)
        {
            BarcodeDraw barcodeDraw = BarcodeDrawFactory.Code128WithChecksum;
            Image barcodeImage = barcodeDraw.Draw(idCarrinho.ToString(), 200, 5); // 50 é o tamanho da imagem, ajuste conforme necessário


            using (var ms = new MemoryStream())
            {
                barcodeImage.Save(ms, ImageFormat.Png);
                return ms.ToArray();
            }
        }
    }
}
