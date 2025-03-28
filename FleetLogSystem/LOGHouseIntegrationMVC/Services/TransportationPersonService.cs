using LOGHouseSystem.Controllers.API.BarcodeColectorApi.Request;
using LOGHouseSystem.Infra.Pagination;
using LOGHouseSystem.Models;
using LOGHouseSystem.Repositories.Interfaces;
using LOGHouseSystem.Services.Interfaces;

namespace LOGHouseSystem.Services
{
    public class TransportationPersonService : ITransportationPersonService
    {
        private ITransportationPersonRepository _transportationPersonRepository;
        

        public TransportationPersonService(ITransportationPersonRepository transportationPersonRepository)
        {
            _transportationPersonRepository = transportationPersonRepository;
        }

        public async Task<TransportationPerson> Generate(CreateTransportationPersonRequest data)
        {

            var transportationPerson = new TransportationPerson() {
                Name = data.Name,
                Cpf = data.Cpf,
                Rg = data.Rg                
            };

            transportationPerson = await _transportationPersonRepository.AddAsync(transportationPerson);

            return transportationPerson;
        }

        public async Task UpdateImage(int id, List<IFormFile> frontImage, List<IFormFile> backImage)
        {

            var transportationPerson = await _transportationPersonRepository.GetById(id);

            if (transportationPerson == null)
            {
                throw new Exception("Nenhum pacote foi encontrado com esse Id");
            }

            if (frontImage.Count > 0)
            {
                var file = frontImage[0];

                if (file.Length > 0)
                {
                    string[] allowedFormats = { ".jpg", ".jpeg", ".png", ".gif" };
                    string fileExtension = Path.GetExtension(file.FileName).ToLowerInvariant();

                    if (!allowedFormats.Contains(fileExtension))
                    {
                        throw new Exception("O arquivo enviado não é uma imagem válida.");
                    }

                    // new file name
                    var filePath = $"{Environment.TransportationPhotoPath}/Front-{id}" + fileExtension;

                    // Delete image if exists
                    if (File.Exists(filePath))
                    {
                        // Excluir o arquivo existente
                        File.Delete(filePath);
                    }

                    // save file
                    using (var stream = new FileStream(filePath, FileMode.Create))
                    {
                        file.CopyTo(stream);
                    }

                    // removing wwwroot path because is not part of the route file
                    transportationPerson.PathDocumentFront = filePath.Replace("wwwroot/", "~/");
                }
            }

            if (backImage.Count > 0)
            {
                var file = backImage[0];

                if (file.Length > 0)
                {
                    string[] allowedFormats = { ".jpg", ".jpeg", ".png", ".gif" };
                    string fileExtension = Path.GetExtension(file.FileName).ToLowerInvariant();

                    if (!allowedFormats.Contains(fileExtension))
                    {
                        throw new Exception("O arquivo enviado não é uma imagem válida.");
                    }

                    // new file name
                    var filePath = $"{Environment.TransportationPhotoPath}/Back-{id}" + fileExtension;

                    // Delete image if exists
                    if (File.Exists(filePath))
                    {
                        // Excluir o arquivo existente
                        File.Delete(filePath);
                    }

                    // save file
                    using (var stream = new FileStream(filePath, FileMode.Create))
                    {
                        file.CopyTo(stream);
                    }

                    // removing wwwroot path because is not part of the route file
                    transportationPerson.PathDocumentBack = filePath.Replace("wwwroot/", "~/");
                }
            }

            await _transportationPersonRepository.UpdateAsync(transportationPerson);
        }


        public async Task<PaginationBase<TransportationPerson>> GetPaginationBaseAsync(PaginationRequest request)
        {
            if(request.PageNumber <= 0)
                request.PageNumber = 1;

            
            return await _transportationPersonRepository.GetByPagination(request);
        }
    }
}
