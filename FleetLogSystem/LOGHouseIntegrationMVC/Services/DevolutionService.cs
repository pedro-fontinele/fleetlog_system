using LOGHouseSystem.Models;
using LOGHouseSystem.Models.SendEmails;
using LOGHouseSystem.Repositories;
using LOGHouseSystem.Repositories.Interfaces;
using LOGHouseSystem.Services.Interfaces;
using LOGHouseSystem.ViewModels;
using Newtonsoft.Json;
using Org.BouncyCastle.Crypto.Operators;
using PagedList;
using System.Net.Mail;

namespace LOGHouseSystem.Services
{
    public class DevolutionService : IDevolutionService
    {
        private readonly IDevolutionRepository _devolutionRepository;
        private readonly IDevolutionImageRepository _imageRepository;
        private readonly IEmailService _emailService;
        private readonly IProductRepository _productRepository;
        private readonly IClientsRepository _clientsRepository;

        public DevolutionService(IDevolutionRepository devolutionRepository, IDevolutionImageRepository imageRepository, IEmailService emailService, IProductRepository productRepository, IClientsRepository clientsRepository )
        {
            _devolutionRepository = devolutionRepository;
            _imageRepository = imageRepository;
            _emailService = emailService;
            _productRepository = productRepository;
            _clientsRepository = clientsRepository;
        }


        public async Task<Devolution> AddAsync(DevolutionCreateAndUpdateViewModel note)
        {

            Devolution dev = new Devolution()
            {
                SenderName = note.SenderName,
                InvoiceNumber = note.InvoiceNumber,
                PostNumber = note.PostNumber,
                Observation = note.Observation,
                Status = note.Status,
                ClientId = note.ClientId,
            };

           await _devolutionRepository.AddAsync(dev);
           return dev;
        }

        public async Task<bool> DeleteAsync(int id)
        {
            return await _devolutionRepository.Delete(id);
        }

        public  PagedList<DevolutionCreateAndUpdateViewModel> GetAllPaged(int Page)
        {
            int pageSize = 50;
            return _devolutionRepository.GetAllPaged(Page, pageSize);
        }

        public async Task<Devolution> GetByIdAsync(int id)
        {
            return await _devolutionRepository.GetById(id);
        }

        public async Task<Devolution> UpdateAsync(DevolutionCreateAndUpdateViewModel note)
        {
            Devolution devolutionById = await GetByIdAsync(note.Id);

            devolutionById.SenderName = note.SenderName;
            devolutionById.InvoiceNumber = note.InvoiceNumber;
            devolutionById.PostNumber = note.PostNumber;
            devolutionById.Observation = note.Observation;
            devolutionById.ClientId = note.ClientId;
            devolutionById.Status = note.Status;

            await _devolutionRepository.UpdateAsync(devolutionById);
            return devolutionById;
        }

        public async Task SendDevolutionImage(int id, List<IFormFile> formFile, string imageBase64 = "")
        {
            var devolution = await _devolutionRepository.GetById(id);

            string filePath = "";

            if (string.IsNullOrEmpty(imageBase64))
            {
                if (formFile.Count <= 0)
                {
                    throw new Exception("É necessário ao menos um arquivo");
                }

                var file = formFile[0];

                if (file.Length < 0)
                {
                    throw new Exception("É necessário ao menos um arquivo");
                }

                string[] allowedFormats = { ".jpg", ".jpeg", ".png", ".gif" };
                string fileExtension = Path.GetExtension(file.FileName).ToLowerInvariant();

                if (!allowedFormats.Contains(fileExtension))
                {
                    throw new Exception("O arquivo enviado não é uma imagem válida.");
                }


                if (devolution == null)
                {
                    throw new Exception("Nenhum pacote foi encontrado com esse Id");
                }

                Guid guid = Guid.NewGuid(); // Substitua isso pelo seu próprio GUID

                string guidSubstring = guid.ToString("N").Substring(0, 5);

                // new file name
                filePath = $"{Environment.DevolutionPhotoPath}/{id}.{guidSubstring}" + fileExtension;

                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    file.CopyTo(stream);
                }

                DevolutionImage devolutionImagePath = new DevolutionImage()
                {
                    FilePath = filePath.Replace("wwwroot/", "~/"),
                    DevolutionId = id
                };

                await _imageRepository.AddImageAsync(devolutionImagePath);
            }
            else
            {
                imageBase64 = imageBase64.Replace("data:image/png;base64,", "");

                byte[] bytesImagem = Convert.FromBase64String(imageBase64);

                Guid guid = Guid.NewGuid(); // Substitua isso pelo seu próprio GUID

                string guidSubstring = guid.ToString("N").Substring(0, 5);

                // new file name
                filePath = $"{Environment.DevolutionPhotoPath}/{id}.{guidSubstring}.png";

                File.WriteAllBytes(filePath, bytesImagem);

                DevolutionImage devolutionImage = new DevolutionImage()
                {
                    FilePath = filePath.Replace("wwwroot/", "~/"),
                    DevolutionId = id
                };

                await _imageRepository.AddImageAsync(devolutionImage);
            }
        }

        public async Task<List<DevolutionImage>> GetAllImagesByIdAsync(int id)
        {
            return await _imageRepository.GetAllImagesByDevolutionIdAsync(id);
        }

        public List<int> GetProductsIdList(string json)
        {
            List<int> intList = new List<int>();

            List<string> stringList = JsonConvert.DeserializeObject<List<string>>(json);

            foreach (string strNumero in stringList)
            {
                if (int.TryParse(strNumero, out int valorConvertido))
                {
                    intList.Add(valorConvertido);
                }
            }

            return intList;
        }

        public void SendDevolutionEmail(Devolution dev, List<DevolutionAndProduct> products, List<byte[]> images = null)
        {
            string prodMessage = "";

            foreach (DevolutionAndProduct product in products)
            {
                Product prodById = _productRepository.GetById(product.ProductId);
                prodMessage += prodById.Description + "<br/>";
            }


            _emailService.SendEmailToDevolution(new EmailData
            {
                EmailBody = $@"
            <html>
            <head>
                <style>
                    /* Estilos CSS para a formatação do e-mail */
                    body {{ font-family: Arial, sans-serif; }}
                    .container {{ padding: 20px; }}
                    .header {{ background-color: #007BFF; color: white; padding: 10px; }}
                    .content {{ padding: 20px; }}
                    .signature {{ font-style: italic; }}
                </style>
            </head>
            <body>
                <div class='container'>
                    <div class='header'>
                        <h2>Devolução de Produto</h2>
                    </div>
                    <div class='content'>
                        <p>Prezado(a),</p>
                        <p>Segue abaixo os dados da devolução recepcionada no centro de distribuição da LogHouse.</p>
                        <p>Solicitamos que o retorno seja enviado em até 5 dias corridos para que possamos dar o devido tratamento ao seu produto. Caso ultrapasse este período, os produtos serão adicionados manualmente ao estoque.</p>
                        <p>Devoluções</p>
                        <ul>
                            <li>Remetente: {dev.SenderName}</li>
                            <li>NF: {dev.InvoiceNumber}</li>
                            <li>Produto: {prodMessage}</li>
                        </ul>
                        <p class='signature'>Atenciosamente, LogHouse</p>
                    </div>
                </div>
            </body>
            </html>",
                EmailSubject = $"Devolução de Produto",
                EmailToId = dev.Client.Email,
                EmailToName = dev.Client.SocialReason
            }, null, dev.ClientId, images, dev.InvoiceNumber);
        }

        public PagedList<DevolutionCreateAndUpdateViewModel> GetAllPagedByUserLoged(int Page)
        {
            int clientId = _clientsRepository.GetByUserLoged().Id;

            int pagesize = 100;
            return _devolutionRepository.GetAllPagedByUserLoged(Page, pagesize, clientId);
        }

        public async Task<DevolutionCreateAndUpdateViewModel> GetViewmodelByIdAsync(int id, List<DevolutionAndProduct> list)
        {
            Devolution devolutionById = await GetByIdAsync(id);

            DevolutionCreateAndUpdateViewModel vm = new DevolutionCreateAndUpdateViewModel()
            {
                SenderName = devolutionById.SenderName,
                InvoiceNumber = devolutionById.InvoiceNumber,
                PostNumber = devolutionById.PostNumber,
                Observation = devolutionById.Observation,
                ClientId = devolutionById.ClientId,
                Status = devolutionById.Status
            };

            vm.Images = await _imageRepository.GetAllImagesByDevolutionIdAsync(id);
            List<string> prodsNames = new List<string>();

            if(list != null)
            {
                foreach(var prod in list) 
                {
                    prodsNames.Add(prod.Product.Description);
                }
            }

            vm.Products = prodsNames;

            return vm;
        }

        public PagedList<DevolutionCreateAndUpdateViewModel> GetByFilter(FilterDevolutionViewModel filter)
        {
            return _devolutionRepository.GetByFilters(filter);
        }
    }
}
