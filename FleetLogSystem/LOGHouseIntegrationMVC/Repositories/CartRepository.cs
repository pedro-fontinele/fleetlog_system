using iTextSharp.text.pdf;
using iTextSharp.text;
using LOGHouseSystem.Models;
using LOGHouseSystem.Repositories.Interfaces;
using LOGHouseSystem.ViewModels;
using Microsoft.EntityFrameworkCore;
using System.Drawing.Imaging;
using System.Drawing;
using System.Runtime.InteropServices;
using ZXing.QrCode;
using ZXing;
using System.IO;

namespace LOGHouseSystem.Repositories
{
    public class CartRepository : RepositoryBase, ICartRepository
    {
        public async Task<Cart> AddAsync(Cart cart)
        {
            if (cart == null) throw new ArgumentNullException("Não foi possível adicionar esse carrinho pois ele está nulo!");

                  _db.Carts.Add(cart);
            await _db.SaveChangesAsync();

            return cart;
        }

        public async Task DeleteAsync(Cart cart)
        {
            if (cart == null) throw new ArgumentNullException("Não foi possível deletar esse carrinho pois ele está nulo!");
            
                  _db.Carts.Remove(cart);
            await _db.SaveChangesAsync();
        }

        public byte[] GeneratePdfWithLabels(List<CartIdViewModel> itens)
        {
            using (MemoryStream memoryStream = new MemoryStream())
            {
                // Defina o tamanho da página para A4 (210mm x 297mm)
                Document doc = new Document(PageSize.A4);

                PdfWriter writer = PdfWriter.GetInstance(doc, memoryStream);

                doc.Open();

                foreach (var item in itens)
                {
                    PdfContentByte cb = writer.DirectContent;

                    // Crie o código de barras grande
                    var barcodeWriter = new BarcodeWriterPixelData
                    {
                        Format = BarcodeFormat.CODE_128,
                        Options = new QrCodeEncodingOptions
                        {
                            Width = 400, // Ajuste o tamanho do código de barras conforme necessário
                            Height = 100 // Ajuste o tamanho do código de barras conforme necessário
                        }
                    };

                    var barcodeData = barcodeWriter.Write(item.Id.ToString());

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
                            barcodeImage.ScaleToFit(PageSize.A4.Width - 40, PageSize.A4.Height / 3);

                            // Defina a posição do código de barras no PDF
                            barcodeImage.SetAbsolutePosition(20f, PageSize.A4.Height / 2 - barcodeImage.ScaledHeight / 2);

                            // Adicione a imagem do código de barras diretamente ao conteúdo PDF
                            cb.AddImage(barcodeImage);

                            // Adicione a descrição grande
                            var descriptionFont = new iTextSharp.text.Font(iTextSharp.text.Font.FontFamily.HELVETICA, 16f);

                            // Ajuste a posição vertical da descrição
                            var positionY = PageSize.A4.Height / 2 - barcodeImage.ScaledHeight / 2 - 40; // Ajuste conforme necessário

                            Phrase skuPhrase = new Phrase(item.Description, descriptionFont);
                            ColumnText.ShowTextAligned(cb, Element.ALIGN_CENTER, skuPhrase, PageSize.A4.Width / 2, positionY, 0);
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

        public async Task<List<Cart>> GetAllAsync()
        {
            return await _db.Carts
                         .AsNoTracking()
                         .ToListAsync();
        }

        public async Task<Cart> GetByIdAsync(int id)
        {
            if (id <= 0) throw new ArgumentNullException("Não foi possível atualizar esse carrinho pois ele não existe!");

            return await _db.Carts.FirstOrDefaultAsync(x => x.Id == id);
                
        }

        public async Task<Cart> UpdateAsync(Cart cart)
        {
            if (cart == null) throw new ArgumentNullException("Não foi possível atualizar esse carrinho pois não possui informações o suficiente!");

            _db.Carts.Update(cart);
            await _db.SaveChangesAsync();

            return cart;

        }

        public async Task<Cart> UpdateByIdAsync(int id, Cart newCart)
        {
            if (id <= 0) throw new ArgumentNullException("Não foi possível atualizar esse carrinho pois ele não existe!");
            Cart cart = await GetByIdAsync(id);

            cart.Description = newCart.Description;
            
            await UpdateAsync(cart);

            return cart;

        }
    }
}
