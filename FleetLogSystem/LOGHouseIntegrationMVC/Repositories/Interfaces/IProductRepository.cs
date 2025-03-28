using LOGHouseSystem.Models;
using LOGHouseSystem.ViewModels;
using Microsoft.AspNetCore.Http;

namespace LOGHouseSystem.Repositories.Interfaces
{
    public interface IProductRepository
    {
        Task<Product> GetByCodeAsync(string code, int clientId);

        Task<ProductValidateQuantityResponseViewModel> ValidateStockProductAsync(ProductValidateQuantityRequestViewModel viewModel);

        Product GetByCode(string code, int clientId);

        Product GetByEan(string ean, int clientId);

        Task<Product> GetByEanAsync(string ean, int clientId);

        List<Product> GetByDate(DateTime date);

        List<Product> GetByClient();

        List<Product> GetByClientId(int id);

        Product GetById(int id);
        Task<Product> GetByIdAsync(int? id);

        List<Product> GetAll();

        Product Add(Product product);

        Product Update(Product product);

        object GetByClientAutoComplete(string prefix);

        object GetByClientAutoCompleteById(string prefix, int id);

        object GetAutoComplete(string prefix);


        bool Delete(int id);
        Task<List<Product>> GetByClientIdAsync(int clientId);
        //Task<Product> UpdateAsync(Product product);
    }
}
