using PdfSharp.Drawing;
using PdfSharp.Pdf;
using PdfSharp.Pdf.IO;

namespace LOGHouseSystem.Services.Helper
{
    public static class PdfHelper
    {
        public static byte[] GetByteByPdfDocument(this PdfDocument? pdf)
        {
            byte[] byteData = null;
            
            using (MemoryStream stream = new MemoryStream())
            {
                int pages = pdf.PageCount; // There are a error in PDF Sharp that is just solved when we put this line in the code.
                pdf.Save(stream, true);
                byteData = stream.ToArray();
            }

            return byteData;
        }

        public static void SaveImageAsPdf(string imageFileName, string pdfFileName, int width = 600, bool deleteImage = false)
        {
            using (var document = new PdfDocument())
            {
                PdfPage page = document.AddPage();
                using (XImage img = XImage.FromFile(imageFileName))
                {
                    // Calculate new height to keep image ratio
                    var height = (int)(((double)width / (double)img.PixelWidth) * img.PixelHeight);

                    // Change PDF Page size to match image
                    page.Width = width;
                    page.Height = height;

                    XGraphics gfx = XGraphics.FromPdfPage(page);
                    gfx.DrawImage(img, 0, 0, width, height);
                }
                document.Save(pdfFileName);
            }

            if (deleteImage)
                File.Delete(imageFileName);
        }
    }
}
