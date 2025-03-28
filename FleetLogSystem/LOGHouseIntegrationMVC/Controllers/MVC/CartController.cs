using iTextSharp.text.pdf;
using iTextSharp.text;
using LOGHouseSystem.Infra.Helpers;
using LOGHouseSystem.Models;
using LOGHouseSystem.Repositories.Interfaces;
using LOGHouseSystem.ViewModels;
using LOGHouseSystem.ViewModels.Cart;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Drawing;
using System.Drawing.Imaging;
using Zen.Barcode;
using ZXing;
using static iTextSharp.text.pdf.AcroFields;

namespace LOGHouseSystem.Controllers.MVC
{
    public class CartController : Controller
    {
        private readonly ICartRepository _cartRepository;

        public CartController(ICartRepository cartRepository)
        {
            _cartRepository = cartRepository;
        }
        public async Task<IActionResult> Index()
        {
            List<Cart> carts = await _cartRepository.GetAllAsync();
            return View(carts);
        }

        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Create(Cart cart)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    cart.CreatedAt = DateTime.Now;
                    await _cartRepository.AddAsync(cart);
                    TempData["SuccessMessage"] = "Carrinho cadastrado com sucesso!";

                    return RedirectToAction("Index");
                }

                TempData["ErrorMessage"] = "Ops! Não foi possível cadastrar o carrinho, observe se preencheu os campos corretamente!";
                return View(cart);

            } catch (Exception ex)
            {
                TempData["ErrorMessage"] = "Ops! Não foi possível cadastrar o carrinho no momento, por favor, tente novamente mais tarde";
                throw new Exception("Erro:"+ ex);
            }
            
        }

        [HttpPost]
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                Cart cart = await _cartRepository.GetByIdAsync(id);

                if(cart != null) 
                { 
                    await _cartRepository.DeleteAsync(cart);
                    TempData["SuccessMessage"] = "Carrinho deletado com sucesso!";
                    
                }
                else
                {
                    TempData["ErrorMessage"] = "Ops! Não foi possível deletar o carrinho no momento, por favor, tente novamente mais tarde";
                }

                return RedirectToAction("Index");

            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = "Ops! Não foi possível deletar o carrinho no momento, por favor, tente novamente mais tarde. Erro: " + ex;
                return RedirectToAction("Index");
            }

        }

        [HttpPost]
        public async Task<IActionResult> Update(int id, string description)
        {
            try
            {
                Cart cart = await _cartRepository.GetByIdAsync(id);

                if (cart != null)
                {
                    cart.Description = description;
                    await _cartRepository.UpdateAsync(cart);
                    TempData["SuccessMessage"] = "Carrinho atualizado com sucesso!";

                }
                else
                {
                    TempData["ErrorMessage"] = "Ops! Não foi possível atualizar o carrinho no momento, por favor, tente novamente mais tarde";
                }

                return RedirectToAction("Index");

            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = "Ops! Não foi possível atualizar o carrinho no momento, por favor, tente novamente mais tarde. Erro: " + ex;
                return RedirectToAction("Index");
            }

        }

        public async Task<IActionResult> BarCode(int id)
        {
            try
            {
                Cart cart = await _cartRepository.GetByIdAsync(id);
                if (cart == null) throw new Exception("Id não encontrado");

                byte[] barcodeBytes = CartHelper.GenerateBarCode(cart.Id);

                return View(new BarcodeViewModel
                {
                    Image = barcodeBytes,
                    Name = cart.Description
                });
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = "Ops! Não foi possível cadastrar o carrinho no momento, por favor, tente novamente mais tarde";
                throw new Exception("Erro:" + ex);
            }

        }

        public IActionResult ImpressLabels(string labels)
        {
            try
            {
                List<CartIdViewModel> items = JsonConvert.DeserializeObject<List<CartIdViewModel>>(labels);

                byte[] binaryData = _cartRepository.GeneratePdfWithLabels(items);

                return File(binaryData, "application/pdf;");

            } catch (Exception e) 
            {
                TempData["ErrorMessage"] = "Não foi possível imprimir os códigos de barras dos carrinhos selecionados. Erro: "+ e;

                return RedirectToAction("Index");
            }


        }
    }
}
