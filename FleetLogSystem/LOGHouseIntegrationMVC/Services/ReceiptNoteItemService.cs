using iTextSharp.text;
using iTextSharp.text.pdf;
using LOGHouseSystem.Models;
using LOGHouseSystem.Repositories.Interfaces;
using LOGHouseSystem.Services.Interfaces;
using LOGHouseSystem.ViewModels;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Runtime.InteropServices;
using ZXing;
using ZXing.QrCode;
using ZXing.SkiaSharp;

namespace LOGHouseSystem.Services
{
    public class ReceiptNoteItemService : IReceiptNoteItemService
    {
        private readonly IWebHostEnvironment _webHostEnvironment;


        private readonly IReceiptNoteItemRepository _receiptNoteItemRepository;
        private readonly IPositionAndProductRepository _positionAndProductRepository;
        public ReceiptNoteItemService(IReceiptNoteItemRepository receiptNoteItemRepository, IWebHostEnvironment webHostEnvironment, IPositionAndProductRepository positionAndProductRepository)
        {
            _receiptNoteItemRepository = receiptNoteItemRepository;
            _webHostEnvironment = webHostEnvironment;
            _positionAndProductRepository = positionAndProductRepository;
        }

        public async Task<List<ReceiptNoteWithPositionViewModel>> GetReceiptNoteItemWithPosition(List<Product> items)
        {
            List<ReceiptNoteWithPositionViewModel> list = new List<ReceiptNoteWithPositionViewModel>();
            foreach (var item in items)
            {
                if (item == null)
                    continue;
                Models.PositionAndProduct position = await _positionAndProductRepository.GetLastProductAddressAsync(item.Id);
                string namePosition = null;

                if (position != null)
                {
                    namePosition = position.AddressingPosition.Name;
                }


                ReceiptNoteWithPositionViewModel receiptNoteWithPositionViewModel = new ReceiptNoteWithPositionViewModel()
                {
                    Id = item.Id,
                    Code = item.Code,
                    Description = item.Description,
                    Ean = item.Ean,
                    Quantity = item.StockQuantity,
                    Position = namePosition
                };

                list.Add(receiptNoteWithPositionViewModel);
            }

            return list;
        }

        public ReceiptNoteItem SearchItemToValidate(int ReceiptNoteId, string code)
        {
            return _receiptNoteItemRepository.GetAll()
                .Where(x => x.ReceiptNoteId == ReceiptNoteId && (x.Ean == code || x.Code == code))
                .FirstOrDefault();
        }


        public byte[] GeneratePdfWithLabels(List<ReceiptNoteItemTransferViewModel> itens)
        {
            using (MemoryStream memoryStream = new MemoryStream())
            {
                // Defina o tamanho da página para 4 cm x 3 cm (144 x 90 pontos)
                Document doc = new Document(new iTextSharp.text.Rectangle(144f, 90f));
                PdfWriter writer = PdfWriter.GetInstance(doc, memoryStream);

                doc.Open();

                foreach (var item in itens)
                {
                    for (int i = 0; i < int.Parse(item.Quantidade); i++)
                    {
                        PdfContentByte cb = writer.DirectContent;

                        var barcodeWriter = new BarcodeWriterPixelData
                        {
                            Format = BarcodeFormat.CODE_128,
                            Options = new QrCodeEncodingOptions
                            {
                                Width = 200,
                                Height = 50
                            }
                        };

                        var barcodeData = barcodeWriter.Write(item.Ean);

                        var barcodeBytes = barcodeData.Pixels;

                        using (MemoryStream bmpStream = new MemoryStream())
                        {
                            using (Bitmap barcodeBitmap = new Bitmap(barcodeData.Width, barcodeData.Height, PixelFormat.Format32bppRgb))
                            {
                                BitmapData bmpData = barcodeBitmap.LockBits(new System.Drawing.Rectangle(0, 0, barcodeData.Width, barcodeData.Height), ImageLockMode.WriteOnly, PixelFormat.Format32bppRgb);
                                Marshal.Copy(barcodeBytes, 0, bmpData.Scan0, barcodeBytes.Length);
                                barcodeBitmap.UnlockBits(bmpData);

                                // Ajuste o tamanho da imagem do código de barras
                                var barcodeImage = iTextSharp.text.Image.GetInstance(barcodeBitmap, BaseColor.WHITE);
                                barcodeImage.ScaleToFit(144f, 45f);

                                // Defina a posição do código de barras no PDF
                                float espacoSuperior = 10f;
                                barcodeImage.SetAbsolutePosition(0f, doc.PageSize.Height - espacoSuperior - barcodeImage.ScaledHeight);

                                // Adicione a imagem do código de barras diretamente ao conteúdo PDF
                                cb.AddImage(barcodeImage);

                                // Adicione o SKU abaixo do código de barras
                                Phrase skuPhrase = new Phrase(item.Id, new iTextSharp.text.Font(iTextSharp.text.Font.FontFamily.HELVETICA, 10f));
                                ColumnText.ShowTextAligned(cb, Element.ALIGN_CENTER, skuPhrase, doc.PageSize.Width / 2, 30f, 0);
                            }
                        }

                        if (i < int.Parse(item.Quantidade) - 1)
                        {
                            doc.NewPage(); // Adicione uma nova página para cada item, exceto o último na quantidade
                        }
                    }

                    // Adicione uma nova página após cada item, exceto o último na lista
                    if (item != itens.Last())
                    {
                        doc.NewPage();
                    }
                }

                doc.Close();
                return memoryStream.ToArray();
            }
        }

        public byte[] GeneratePdfWithCaixMasterLabels(List<CaixaMasterLabelViewModel> itens)
        {
            using (MemoryStream memoryStream = new MemoryStream())
            {

                Document doc = new Document(new iTextSharp.text.Rectangle(213f, 106f));
                PdfWriter writer = PdfWriter.GetInstance(doc, memoryStream);

                doc.Open();

                foreach (var item in itens)
                {
                    PdfContentByte cb = writer.DirectContent;

                    var barcodeWriter = new BarcodeWriterPixelData
                    {
                        Format = BarcodeFormat.CODE_128,
                        Options = new QrCodeEncodingOptions
                        {
                            Width = 300,
                            Height = 75
                        }
                    };


                    var usingCode = item.CaixaMasterCode != "-" ? item.CaixaMasterCode : item.SKU;

                    var barcodeData = barcodeWriter.Write(usingCode);

                    var barcodeBytes = barcodeData.Pixels;

                    using (MemoryStream bmpStream = new MemoryStream())
                    {
                        using (Bitmap barcodeBitmap = new Bitmap(barcodeData.Width, barcodeData.Height, PixelFormat.Format32bppRgb))
                        {
                            BitmapData bmpData = barcodeBitmap.LockBits(new System.Drawing.Rectangle(0, 0, barcodeData.Width, barcodeData.Height), ImageLockMode.WriteOnly, PixelFormat.Format32bppRgb);
                            Marshal.Copy(barcodeBytes, 0, bmpData.Scan0, barcodeBytes.Length);
                            barcodeBitmap.UnlockBits(bmpData);

                            // Ajuste o tamanho da imagem do código de barras
                            var barcodeImage = iTextSharp.text.Image.GetInstance(barcodeBitmap, BaseColor.WHITE);
                            barcodeImage.ScaleToFit(213f, 70f);

                            // Defina a posição do código de barras no PDF
                            barcodeImage.SetAbsolutePosition((doc.PageSize.Width - barcodeImage.ScaledWidth) / 2, (doc.PageSize.Height - barcodeImage.ScaledHeight) / 2);

                            // Adicione a imagem do código de barras diretamente ao conteúdo PDF
                            cb.AddImage(barcodeImage);

                            // Adicione o SKU abaixo do código de barras
                            Phrase skuPhrase = new Phrase(usingCode, new iTextSharp.text.Font(iTextSharp.text.Font.FontFamily.HELVETICA, 10f));
                            ColumnText.ShowTextAligned(cb, Element.ALIGN_CENTER, skuPhrase, doc.PageSize.Width / 2, 17f, 0);

                            Phrase description = new Phrase(item.Description, new iTextSharp.text.Font(iTextSharp.text.Font.FontFamily.HELVETICA, 6f));
                            ColumnText.ShowTextAligned(cb, Element.ALIGN_CENTER, description, doc.PageSize.Width / 2, 10f, 0);
                        }
                    }

                    doc.NewPage(); // Adicione uma nova página para cada item, exceto o último na quantidade

                    // Adicione uma nova página após cada item, exceto o último na lista
                    if (item != itens.Last())
                    {
                        doc.NewPage();
                    }
                }

                doc.Close();
                return memoryStream.ToArray();
            }
        }

        public byte[] GeneratePdfWithIdentityLabels(List<string> itens)
        {
            using (MemoryStream memoryStream = new MemoryStream())
            {
                Document doc = new Document(new iTextSharp.text.Rectangle(150f * 2.83465f, 100f * 2.83465f));

                PdfWriter writer = PdfWriter.GetInstance(doc, memoryStream);

                doc.Open();

                PdfContentByte cb = writer.DirectContent;

                // Adicione a imagem no topo da página centralizada
                string imagePath = "wwwroot/img/logoazul.png";
                iTextSharp.text.Image img = iTextSharp.text.Image.GetInstance(imagePath);
                img.ScaleToFit(doc.PageSize.Width, doc.PageSize.Height);
                img.ScalePercent(50);

                img.Alignment = iTextSharp.text.Image.ALIGN_CENTER;
                img.SetAbsolutePosition(0, doc.PageSize.Height - img.ScaledHeight);

                doc.Add(img);

                float fontSize = 25f;
                float y = doc.PageSize.Top - img.ScaledHeight - 53f; // Espaço superior para o primeiro item abaixo da imagem

                foreach (var item in itens)
                {
                    // Adicione o texto do item ao PDF
                    Phrase itemPhrase = new Phrase(item, new iTextSharp.text.Font(iTextSharp.text.Font.FontFamily.HELVETICA, fontSize));

                    // Calcule a posição para listar uma abaixo da outra na mesma página, alinhando com a margem esquerda
                    float x = doc.Left - 30f;

                    // Adicione o texto na posição calculada
                    ColumnText.ShowTextAligned(cb, Element.ALIGN_LEFT, itemPhrase, x, y, 0);

                    // Atualize a posição vertical para a próxima linha
                    y -= fontSize + 10f; // Espaço entre as linhas (10 pontos)
                }

                doc.Close();
                return memoryStream.ToArray();
            }

        }
    }
}
