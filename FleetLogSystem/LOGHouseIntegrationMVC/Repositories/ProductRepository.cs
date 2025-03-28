using LOGHouseSystem.Infra.Database;
using LOGHouseSystem.Infra.Helpers;
using LOGHouseSystem.Models;
using LOGHouseSystem.Repositories.Interfaces;
using LOGHouseSystem.ViewModels;
using Microsoft.Data.SqlClient.DataClassification;
using Microsoft.EntityFrameworkCore;

namespace LOGHouseSystem.Repositories
{
    public class ProductRepository : RepositoryBase, IProductRepository
    {
        private IClientsRepository _clientRepository;
        

        public ProductRepository(IClientsRepository clientsRepository)
        {
            _clientRepository = clientsRepository;
        }

        public async Task<Product> GetByCodeAsync(string code, int clientId)
        {
            return await _db.Products
                .Where(x => x.Code == code && x.ClientId == clientId)
                .FirstOrDefaultAsync();
        }

        public Product GetByEan(string ean, int clientId)
        {
            return _db.Products
                .Where(x => x.Ean == ean && x.ClientId == clientId)
                .FirstOrDefault();
        }

        public List<Product> GetByDate(DateTime date)
        {
            return _db.Products
                .AsNoTracking()
                .Where(x => x.CreatedAt == date)
                .ToList();
        }


        public List<Product> GetByClient()
        {
            
            Client client = _clientRepository.GetByUserLoged();


            return _db.Products
                .AsNoTracking()
                .Where(x => x.ClientId == client.Id)
                .ToList();
        }


        public Product GetById(int id)
        {
            return _db.Products
                .FirstOrDefault(x => x.Id == id);
        }

        public List<Product> GetAll()
        {
            return _db.Products
                .AsNoTracking()
                .ToList();
        }


        public Product Add(Product product)
        {
            _db.Products.Add(product);
            _db.SaveChanges();
            return product;
        }

        //public Product Update(Product product)
        //{
        //    Product productById = GetById(product.Id);
        //    if (productById == null)
        //        throw new System.Exception("Houve um erro na atualização do produto");

        //    productById.Code = product.Code;
        //    productById.Description = product.Description;
        //    productById.Ean = product.Ean;
        //    productById.StockQuantity = product.StockQuantity;
        //    productById.StockReservationQuantity = product.StockReservationQuantity;

        //    _db.Products.Update(productById);
        //    _db.SaveChanges();

        //    return productById;
        //}

        public bool Delete(int id)
        {
            Product productById = GetById(id);
            if (productById == null)
                throw new System.Exception("Houve um erro na remoção do produto");

            _db.Products.Remove(productById);
            _db.SaveChanges();

            return true;
        }

        public Task<Product> GetByEanAsync(string ean, int clientId)
        {
            return _db.Products
                .Where(x => x.Ean == ean && x.ClientId == clientId)
                .FirstOrDefaultAsync();
        }
        public object GetAutoComplete(string prefix)
        {
            Client clientByUser = _clientRepository.GetByUserLoged();

            var products = (from product in _db.Products
                            where product.StockQuantity > 0
                            where product.Description.Contains(prefix)
                            where product.Description.Contains(prefix) || product.Code.Contains(prefix)
                            select new
                            {
                                label = product.Code + " - " + product.Description,
                                val = product.Id,
                                valx = product.Ean,
                                valy = product.Description,
                                valId = 0
                            }).ToList();

            return (products);
        }


        //Return object to autocomplete method
        public object GetByClientAutoComplete(string prefix)
        {
            Client clientByUser = _clientRepository.GetByUserLoged();

            var products = (from product in _db.Products
                           where product.ClientId == clientByUser.Id
                           where product.StockQuantity > 0
                           where product.Description.Contains(prefix) || product.Code.Contains(prefix) || product.Ean.Contains(prefix)
                            select new
                           {
                               label = product.Code + " - " + product.Description,
                               val = product.Id,
                               valx = product.Ean,
                               valy = product.Description,
                               valId = product.ClientId
                           }).ToList();

            return (products);
        }

        //Return object to autocomplete method
        public object GetByClientAutoCompleteById(string prefix, int id)
        {

            var products = (from product in _db.Products
                            where product.ClientId == id
                            where product.StockQuantity > 0
                            where product.Description.Contains(prefix) || product.Code.Contains(prefix) || product.Ean.Contains(prefix)
                            select new
                            {
                                label = product.Code + " - " + product.Description,
                                val = product.Id,
                                valx = product.Ean,
                                valy = product.Description
                            }).ToList();

            return (products);
        }

        public List<Product> GetByClientId(int id)
        {
            return _db.Products
                .Where(x => x.ClientId == id )
                .ToList();
        }

        public async Task<Product> GetByIdAsync(int? id)
        {
            return await _db.Products
                .Where(x => x.Id == id)
                .FirstOrDefaultAsync();
        }

        public Product Update(Product product)
        {
            var entry = _db.Entry(product);

            if (entry.State == EntityState.Detached)
            {
                var set = _db.Set<Product>();
                Product attachedEntity = set.Find(product.Id);  // You need to have access to key

                if (attachedEntity != null)
                {
                    var attachedEntry = _db.Entry(attachedEntity);
                    attachedEntry.CurrentValues.SetValues(product);
                }
                else
                {
                    entry.State = EntityState.Modified; // This should attach entity
                }
            }

            _db.SaveChanges();

            return product;
        }

        public Product GetByCode(string code, int clientId)
        {
            return  _db.Products
            .Where(x => x.Code == code && x.ClientId == clientId)
            .FirstOrDefault();
        }

        public async Task<ProductValidateQuantityResponseViewModel> ValidateStockProductAsync(ProductValidateQuantityRequestViewModel viewModel)
        {
            Product product = await GetByIdAsync(viewModel.ProductId);

            ProductValidateQuantityResponseViewModel responseViewModel = new ProductValidateQuantityResponseViewModel();

            if (product.StockQuantity < viewModel.Quantity) 
            {
                responseViewModel.HaveInStock = false;
                responseViewModel.ProductName = product.Description;
                responseViewModel.AvailableQuantity = Convert.ToInt32(product.StockQuantity); ;
            }
            else
            {
                responseViewModel.HaveInStock = true;
                
            }

            return responseViewModel;
        }

        public async Task<List<Product>> GetByClientIdAsync(int clientId)
        {
            var product = await _db.Products.Where(e => e.ClientId == clientId).ToListAsync();

            return product;
        }
    }
}
