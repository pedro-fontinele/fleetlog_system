using LOGHouseSystem.Infra.Filters;
using LOGHouseSystem.Infra.Helpers;
using LOGHouseSystem.Models;
using LOGHouseSystem.Repositories;
using LOGHouseSystem.Repositories.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace LOGHouseSystem.Controllers.MVC
{
    [PageForLogedUser]
    public class AddressController : Controller
    {
        private readonly IAddressingStreetRepository _addressRepository;
        private readonly IAddressingPositionRepository _positionRepository;
        private readonly IPositionAndProductRepository _positionAndProductRepository;

        public AddressController(IAddressingPositionRepository positionRepository, IAddressingStreetRepository adressRepository, IPositionAndProductRepository positionAndProductRepository)
        {
            _positionRepository = positionRepository;
            _addressRepository = adressRepository;
            _positionAndProductRepository = positionAndProductRepository;
        }

        public IActionResult Index()
        {
            List<AddressingStreet> ads = _addressRepository.GetAll();
            return View(ads);
        }

        public IActionResult AddStreet(string Name)
        {
            var adress = _addressRepository.CheckByName(Name);

            if (adress == true)
            {
                AddressingStreet address = new AddressingStreet();
                address.Name = Name;
                address.Status = Infra.Enums.Status.Ativo;

                _addressRepository.Add(address);

                return RedirectToAction("Index");

            }
            else
            {
                TempData["ErrorMessage"] = "Ops! Já existe uma rua cadastrada com esse nome, tente um nome diferente!";
                return RedirectToAction("Index");
            }
        }

        public IActionResult AddPosition(int AddressingStreetID, string Name)
        {
            
                AddressingPosition addressPosition = new AddressingPosition();
                addressPosition.Name = Name;
                addressPosition.AddressingStreetID = AddressingStreetID;
                addressPosition.Status = Infra.Enums.AddressingPositionStatus.Disponivel;

                _positionRepository.Add(addressPosition);

                return RedirectToAction("Index");
            
        }
        
        public IActionResult DeletePosition(int AddressingPositionId)
        {

            _positionRepository.Delete(AddressingPositionId);

            return RedirectToAction("Index");
        }

        
        public IActionResult DeletePositions(string x)
        {
           List<string> ids = x.Split(",").ToList();

           List<int> newIds = new List<int>();

            foreach (string id in ids)
            {
                var intId = id.ConvertToInt();
                newIds.Add(intId);
            }

           var delete =  _positionRepository.DeleteArrange(newIds);

            if(delete == true)
            {
                TempData["SuccessMessage"] = "As posições selecionadas foram apagadas com sucesso!";
                return RedirectToAction("Index");
            }

            TempData["ErrorMessage"] = "Ops! Não foi possível apagar as posições selecionadas, tente novamente!";
            return RedirectToAction("Index");

        }

        public IActionResult DeleteStreet(int AddressingStreetID)
        {
            var hasPositions = _positionRepository.GetByStreetId(AddressingStreetID).Count > 0;
            if(hasPositions)
            {
                TempData["ErrorMessage"] = "Não é possível excluir uma rua com posições";
                return RedirectToAction("Index");
            }

            _addressRepository.Delete(AddressingStreetID);

            return RedirectToAction("Index");
        }

        public IActionResult CopyAdress(int AddressingStreetID, string message)
        {
           //Valida nome da street
           var adress =  _addressRepository.CheckByName(message);

            if(adress == true) { 

            //StreetCopiada
            AddressingStreet addressingStreet = _addressRepository.GetById(AddressingStreetID);

            //Positions copiadas
            List<AddressingPosition> positions = _positionRepository.GetByStreetId(AddressingStreetID);

            //Cópia das positions
            List<AddressingPosition> newPositions = new List<AddressingPosition>();

            //NovaRua
            AddressingStreet newStreet = new AddressingStreet()
            {
                Name = message,
                Positions = newPositions,
                Status = addressingStreet.Status
            };

            AddressingStreet ads = _addressRepository.Add(newStreet);

            //Foreach trocando o nome
            foreach (var position in positions)
            {
                position.AddressingStreetID = ads.AddressingStreetID;
                position.AddressingStreet = ads;
                position.AddressingPositionID = 0;
                var nameAfter = position.Name.Remove(0, 1);
                position.Name = $"{message}{nameAfter}";
                position.Status = Infra.Enums.AddressingPositionStatus.Disponivel;

                _positionRepository.Add(position);
            }

            return RedirectToAction("Index");

            }
            else
            {
                TempData["ErrorMessage"] = "Ops! Já existe uma rua cadastrada com esse nome, tente um nome diferente!";
                return RedirectToAction("Index");
            }
        }

        [HttpGet]
        public List<AddressingPosition> GetAll()
        {
            List< AddressingPosition> positions =  _positionRepository.GetAll();

            return positions;
        }

        [HttpPost]
        public async Task<IActionResult> CopyAdress(int productId, int positionId, int clientId)
        {
            PositionAndProduct position = await _positionAndProductRepository.GetFirstProductAddressAsync(productId);

            if (position == null)
            {
                PositionAndProduct newPos = new PositionAndProduct()
                {
                    ProductId = productId,
                    AddressingPositionId = positionId
                };
                
                _positionAndProductRepository.Add(newPos);
            }
            else if (position.ProductId == productId && position.AddressingPositionId == positionId)
            {
                TempData["ErrorMessage"] = "O produto selecionado já está ocupando a posição informada!";
            }
            else
            {
                position.AddressingPositionId = positionId;
                await _positionAndProductRepository.Update(position);
            }

            return RedirectToAction("SearchClientProducts", "Product", new { id = clientId });

        }


    }
}
