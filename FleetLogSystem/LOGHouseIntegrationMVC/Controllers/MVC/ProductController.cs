using LOGHouseSystem.Infra.Filters;
using LOGHouseSystem.Models;
using LOGHouseSystem.Repositories;
using LOGHouseSystem.Repositories.Interfaces;
using LOGHouseSystem.Services;
using LOGHouseSystem.Services.Interfaces;
using LOGHouseSystem.ViewModels;
using Microsoft.AspNetCore.Mvc;

namespace LOGHouseSystem.Controllers.MVC
{

    public class ProductController : Controller
    {
        private IProductRepository _productRepository;
        private IClientsRepository _clientRepository;
        private IPositionAndProductRepository _positionAndProductRepository;
        private IProductStockService _productStockService;

        public ProductController(IProductRepository productRepository, IClientsRepository clientsRepository, IPositionAndProductRepository positionAndProductRepository, IProductStockService productStockService)
        {
            _productRepository = productRepository;
            _clientRepository = clientsRepository;
            _positionAndProductRepository = positionAndProductRepository;
            _productStockService = productStockService;
        }

        [PageForClient]
        public IActionResult IndexClient()
        {
            List<Product> products = _productRepository.GetByClient();

            return View(products);
        }

        [PageForClient]
        public IActionResult Create()
        {
            
            return View();
        }

        // POST: Products2/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Code,Description,Ean")] Product product)
        {

            product.ClientId = _clientRepository.GetByUserLoged().Id;
            product.CreatedAt = DateTime.Now;
            product.StockQuantity = 0;

            try
            {
                ProductService productService = new ProductService();
                productService.CheckAndAddProduct(product.Ean, product);
                TempData["SuccessMessage"] = "Produto Criado com sucesso!";
                return RedirectToAction("IndexClient");
            }catch(Exception ex)
            {
                TempData["ErrorMessage"] = $"Falha ao criar produto. Err: {ex.Message}";
            }
            
            return View(product);
        }

        [HttpPost]
        public IActionResult AutoCompleteByClient(string prefix)
        {
            var result = _productRepository.GetByClientAutoComplete(prefix);

            return Json(result);
        }

        [HttpPost]
        public IActionResult AutoComplete(string prefix)
        {
            var result = _productRepository.GetAutoComplete(prefix);

            return Json(result);
        }

        [HttpPost]
        public IActionResult AutoCompleteById(string prefix, int id)
        {
            var result = _productRepository.GetByClientAutoCompleteById(prefix, id);

            return Json(result);
        }

        [PageForAdmin]
        public IActionResult ViewProductsByClient()
        {
            return View();
        }

        public async Task Reprocess(int id)
        {
            await _productStockService.ProcessByProductId(id, new DateTime(1900, 1, 1, 0, 0, 0));
        }

        public void ReprocessClient(int id)
        {
            _productStockService.CreateHangFireProcessByClientId(id, new DateTime(1900, 1, 1, 0, 0, 0));
        }

        [PageForAdmin]
        public async Task<IActionResult> SearchClientProducts(int id)  
         {
            List<Product> products = _productRepository.GetByClientId(id);

            List<ProductWithStockAdress> productList = new List<ProductWithStockAdress>();

            foreach (Product product in products)
            {
                ProductWithStockAdress productWithAdress = new ProductWithStockAdress();

                PositionAndProduct positionaAndProduct = await _positionAndProductRepository.GetFirstProductAddressAsync(product.Id);

                productWithAdress.StockQuantity = product.StockQuantity;
                productWithAdress.StockReservationQuantity = product.StockReservationQuantity;
                productWithAdress.TotalStock = product.StockReservationQuantity + product.StockQuantity;
                
                productWithAdress.Id = product.Id;
                productWithAdress.Ean = product.Ean;
                productWithAdress.Code = product.Code;
                productWithAdress.Description = product.Description;
                productWithAdress.ClientId = product.ClientId;


                if (positionaAndProduct == null)
                {
                    
                    productWithAdress.PositionName = "-";
                }
                else
                {
                    productWithAdress.PositionName = positionaAndProduct.AddressingPosition.Name;
                }

                productList.Add(productWithAdress);
            }

            return View("ViewProductsByClient", productList);
        }

        [HttpPost]
        public IActionResult CreateProductToDevolutio(string codigo, string descricao, string ean, int clientId)
        {
            try
            {
                Product product = new Product()
                {
                    Code = codigo,
                    Description = descricao,
                    Ean = ean,
                    StockQuantity = 0,
                    StockReservationQuantity = 0,
                    CreatedAt = DateTime.Now,
                    ClientId = clientId
                };

                _productRepository.Add(product);

                // Operação bem-sucedida
                return Json(new { success = true, message = "Produto criado com sucesso" });
            }
            catch (Exception ex)
            {
                // Operação falhou
                return Json(new { success = false, message = "Erro ao criar o produto: " + ex.Message });
            }
        }

        [HttpPost]
        public async Task<ProductValidateQuantityResponseViewModel> ValidateProductStockQuantity(ProductValidateQuantityRequestViewModel viewModel)
        {
            ProductValidateQuantityResponseViewModel response = await _productRepository.ValidateStockProductAsync(viewModel);

            return response;
        }




    }
}

