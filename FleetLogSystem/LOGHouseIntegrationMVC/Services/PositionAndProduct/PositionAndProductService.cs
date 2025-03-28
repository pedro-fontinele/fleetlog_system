using LOGHouseSystem.Models;
using LOGHouseSystem.Repositories.Interfaces;
using LOGHouseSystem.Services.Interfaces;

namespace LOGHouseSystem.Services.PositionAndProduct
{
    public class PositionAndProductService : IPositionAndProductService
    {
        private IPositionAndProductRepository _positionAndProductRepository;
        private IAddressingPositionRepository _addressingPositionRepository;

        public PositionAndProductService(IPositionAndProductRepository positionAndProductRepository, IAddressingPositionRepository addressingPositionRepository)
        {
            _positionAndProductRepository = positionAndProductRepository;
            _addressingPositionRepository = addressingPositionRepository;
        }


        public Models.PositionAndProduct AssociateProductToPosition(int productId, string addressPosition)
        {
            AddressingPosition addressingPosition = _addressingPositionRepository.SearchByName(addressPosition).FirstOrDefault();

            if (addressingPosition == null)
                throw new Exception($"Posição {addressingPosition} não encontrada");

            if(!_positionAndProductRepository.ProductAlreadyAssociated(addressingPosition.AddressingPositionID, productId))
            {
                return _positionAndProductRepository.Add(new Models.PositionAndProduct
                {
                    AddressingPositionId = addressingPosition.AddressingPositionID,
                    ProductId = productId
                });
            }

            return null;
        }
    }
}
