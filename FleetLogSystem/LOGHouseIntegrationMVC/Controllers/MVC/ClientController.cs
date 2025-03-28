using LOGHouseSystem.Controllers.API.PipedriveHook.Requests;
using LOGHouseSystem.Infra.Database;
using LOGHouseSystem.Infra.Filters;
using LOGHouseSystem.Infra.Helpers;
using LOGHouseSystem.Migrations;
using LOGHouseSystem.Models;
using LOGHouseSystem.Models.SendEmails;
using LOGHouseSystem.Repositories;
using LOGHouseSystem.Repositories.Interfaces;
using LOGHouseSystem.Services;
using LOGHouseSystem.Services.Interfaces;
using LOGHouseSystem.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json.Linq;
using System.Diagnostics.Contracts;

namespace LOGHouseSystem.Controllers.MVC
{
    
    public class ClientController : Controller
    {
        private IClientsRepository _clientRepository;
        private readonly IClientContractsRepository _clientContractRepository;
        private readonly IUserRepository _userRepository;
        private readonly IEmailService _email;
        private readonly IPipedriveService _pipedriveService;
        private readonly IHttpContextAccessor _httpContextAccessor;


        public ClientController(IEmailService email, IClientContractsRepository clientContractsRepository, IClientsRepository clientsRepository, IUserRepository userRepository, IPipedriveService pipedriveService, IHttpContextAccessor httpContextAccessor)
        {
            _email = email;
            _clientContractRepository = clientContractsRepository;
            _clientRepository = clientsRepository;
            _userRepository = userRepository;
            _pipedriveService = pipedriveService;
            _httpContextAccessor = httpContextAccessor;
        }

        //Autocomplete
        [HttpPost]
        [PageForAdmin]
        public JsonResult AutoComplete(string prefix)
        {
            var result = _clientRepository.GetIfContainsPrefix(prefix);

            return Json(result);
        }

        [PageForAdmin]
        public IActionResult Index()
        {
            List<Client> clients = _clientRepository.GetAll();
            
            foreach(var item in clients)
            {
                item.Cnpj = MaskHelper.FormatCNPJ(item.Cnpj);
            }

            return View(clients);
        }

        [PageForAdmin]
        public IActionResult ViewMore(int id)
        {
            try
            {
                Client clientById = _clientRepository.GetById(id);

                if(clientById != null)
                {
                    ClientContract contract = _clientRepository.GetContract(clientById.Id);
                    User user = _userRepository.GetById(clientById.UserId);

                    ClientAndYourContractViewModel viewModel = new()
                    {
                        ClientId = clientById.Id,
                        Cnpj = clientById.Cnpj,
                        Email = clientById.Email,
                        Adress = clientById.Adress,
                        SocialReason = clientById.SocialReason,
                        Phone = clientById.Phone,
                        StateRegistration = clientById.StateRegistration,
                        Storage = contract.Storage,
                        SurplusStorage = contract.SurplusStorage,
                        StorageValue = contract.StorageValue,
                        Requests = contract.Requests,
                        RequestsValue = contract.RequestsValue,
                        ShippingUnits = contract.ShippingUnits,
                        ContractValue = contract.ContractValue,
                        InsurancePercentage = contract.InsurancePercentage,
                        ExcessOrderValue = contract.ExcessOrderValue,
                        Login = user.Username,
                        Password = user.Password
                    };

                    return View(viewModel);
                }

                TempData["ErrorMessage"] = "O usuário não foi encontrado";
                return RedirectToAction("Index", "Client");
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = "Ocorreu um erro ao tentar buscar esse usuário. Detalhes do erro: " + ex;
                return RedirectToAction("Index", "Client");
            }

        }
        [PageForAdmin]
        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [PageForAdmin]
        public IActionResult Create(ClientAndYourContractViewModel clientContractViewModel)
        {
            try
            {
                if (ModelState.IsValid)
                {

                    clientContractViewModel.Cnpj = MaskHelper.RemoveMask(clientContractViewModel.Cnpj);
                    bool checkedCnpj = _clientRepository.CheckClient(clientContractViewModel.Cnpj, null, 0);

                    if(checkedCnpj == false)
                    {
                        ViewBag.CnpjError = "Esse CNPJ já está em uso";
                        return View();
                    }

                    bool checkedEmail = _clientRepository.CheckClient(null, clientContractViewModel.Email, 0);

                    if(checkedEmail == false)
                    {
                        ViewBag.EmailError = "Esse email já está em uso";
                        return View();
                    }

                    TransactionService transactionService = new TransactionService();
                    int clientId = transactionService.CreateNewClientUserAndClientContract(clientContractViewModel);

                    TempData["SuccessMessage"] = "Cliente cadastrado com sucesso";
                    return RedirectToAction("ViewMore", "Client", new {id = clientId});
                }

                return View("Create", clientContractViewModel);

            }
            catch (Exception erro)
            {
                TempData["ErrorMessage"] = $"Ops, não conseguimos cadastrar seu cliente, " +
                    $"tente novamente! Detalhe do erro: {erro.Message}";
                return RedirectToAction("Index", "Client");
            }

        }

        [PageForAdmin]
        public IActionResult Update(int id) 
        {
            Client clientById = _clientRepository.GetById(id);
            ClientContract contractById = _clientRepository.GetContract(clientById.Id);

            ClientAndYourContractViewModel viewModel = new()
            {
                ClientId = clientById.Id,
                Cnpj = clientById.Cnpj,
                Email = clientById.Email,
                Adress = clientById.Adress,
                SocialReason = clientById.SocialReason,
                Phone = clientById.Phone,
                StateRegistration = clientById.StateRegistration,
                Storage = contractById.Storage,
                SurplusStorage = contractById.SurplusStorage,
                StorageValue = contractById.StorageValue,
                Requests = contractById.Requests,
                RequestsValue = contractById.RequestsValue,
                ShippingUnits = contractById.ShippingUnits,
                ExcessOrderValue = contractById.ExcessOrderValue,
                InsurancePercentage = contractById.InsurancePercentage,
                ContractValue = contractById.ContractValue
            };


            return View(viewModel);
        }

        [HttpPost]
        public IActionResult Update(ClientAndYourContractViewModel viewModel)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    viewModel.Cnpj = MaskHelper.RemoveMask(viewModel.Cnpj);
                    bool checkedCnpj = _clientRepository.CheckClient(viewModel.Cnpj, null, viewModel.ClientId);

                    if (checkedCnpj == false)
                    {
                        ViewBag.CnpjError = "Esse CNPJ já está em uso";
                        return View();
                    }

                    bool checkedEmail = _clientRepository.CheckClient(null, viewModel.Email, viewModel.ClientId);

                    if (checkedEmail == false)
                    {
                        ViewBag.EmailError = "Esse email já está em uso";
                        return View();
                    }

                    _clientRepository.Update(viewModel);
                    TempData["SuccessMessage"] = "Cliente atualizado com sucesso";
                    return RedirectToAction("Index");
                }

                return View(viewModel);
                
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = $"Ops, não conseguimos atualizar seu cliente, " +
                    $"tente novamente! Detalhe do erro: {ex.Message}";
                return RedirectToAction("Index");
            }
        }

        [HttpPost] //Atualiza apenas o cliente
        public IActionResult MyData(ClientViewModel viewModel)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    viewModel.Cnpj = MaskHelper.RemoveMask(viewModel.Cnpj);
                    bool checkedCnpj = _clientRepository.CheckClient(viewModel.Cnpj, null, viewModel.Id);

                    if (checkedCnpj == false)
                    {
                        ViewBag.CnpjError = "Esse CNPJ já está em uso";
                        return View();
                    }

                    bool checkedEmail = _clientRepository.CheckClient(null, viewModel.Email, viewModel.Id);

                    if (checkedEmail == false)
                    {
                        ViewBag.EmailError = "Esse email já está em uso";
                        return View();
                    }

                    _clientRepository.UpdateClient(viewModel);
                    TempData["SuccessMessage"] = "Cliente atualizado com sucesso";
                    return View();
                }

                return View(viewModel);

            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = $"Ops, não conseguimos atualizar seu cliente, " +
                    $"tente novamente! Detalhe do erro: {ex.Message}";
                return View();
            }
        }

        [PageForAdmin]
        public IActionResult ResetPassword(int id)
        {
            try
            {
                Client clientById = _clientRepository.GetById(id);

                if (clientById == null)
                    throw new Exception("Não foi possível restar a senha do usuário");

                User user = _userRepository.GetById(clientById.UserId);
                if (clientById == null)
                    throw new Exception("Não foi possível restar a senha do usuário");

                var urlUsed = $"{_httpContextAccessor.HttpContext.Request.Scheme}://{_httpContextAccessor.HttpContext.Request.Host}{_httpContextAccessor.HttpContext.Request.PathBase}";

                string newPassword = user.GenerateNewPassword();

                EmailData emailData = new EmailData()
                {
                    EmailToId = user.Email,
                    EmailToName = user.Name,
                    EmailSubject = "Nova Senha",
                    EmailBody = $"<h1>Olá, {user.Name} </h1>" + "<br/>" +
                                       "<p> Você está recebendo este e-mail da LogHouse porque foi solicitado a redefinição de sua senha através do nosso sistema.\r\nSe você não solicitou essa alteração, pode ignorar este e-mail tranquilamente. </p>" +
                                       "<br/>" + "Essa é sua nova senha temporária, será solicitado uma nova senha ao acessar o sistema." + "<br/>" + $"{newPassword}" + "<br/>" +
                                       "<button style=\"background-color: #496FDC; border: none; border-radius: 4px;color: white; padding: 10px 20px; text-align: center; text-decoration: none; display: inline-block; font-size: 16px; margin: 4px 2px; cursor: pointer;\">" +
                                       "<a style=\"color: white; text-decoration: none; display: inline-block; font-size: 16px; margin: 4px 2px; cursor: pointer;\" href=\"" + urlUsed + "\">Clique aqui</a>" +
                                       "</button>" + "<br/>" + $"Url: {urlUsed}" + "<br/>" + "<p>Você pode alterar sua senha novamente a qualquer momento, através do painel de configuração de sua conta.</p>"
                                       + "<br/>" + "Atenciosamente,\r\nEquipe LogHouse"
                };

                bool sendEmail = _email.SendEmail(emailData, user, 0);

                if(sendEmail)
                {
                    user.FirstAcess = Infra.Enums.YesOrNo.Yes;
                    _userRepository.UpdatePassword(user);
                    TempData["SuccessMessage"] = "A senha desse usuário foi resetada e sua nova senha foi enviada para o e-mail cadastrado";
                    return RedirectToAction("Index", "Client");
                }
                else
                {
                    TempData["ErrorMessage"] = "Ops, não foi possível resetar essa senha, tente novamente!";
                    return RedirectToAction("Index", "Client");
                }

            }
            catch (Exception)
            {
                TempData["ErrorMessage"] = $"Ops, não conseguimos resetar a senha desse cliente";
                return RedirectToAction("Index");
            }
        }

        [PageForClient]
        public IActionResult MyData()
        {
           Client clientLoged = _clientRepository.GetByUserLoged();

            ClientViewModel viewModel = new()
            {
                Id = clientLoged.Id,
                Cnpj = clientLoged.Cnpj,
                Email = clientLoged.Email,
                Adress = clientLoged.Adress,
                SocialReason = clientLoged.SocialReason,
                Phone = clientLoged.Phone,
                StateRegistration = clientLoged.StateRegistration,
            };

            return View(viewModel);
        }


        [PageForAdmin]
        public IActionResult UpdateStatus(int id)
        {
            try
            {
                Client clientById = _clientRepository.GetById(id);

                if (clientById == null)
                    throw new Exception("Não foi possivel atualizar o status desse cliente, por favor, tente novamente");

                if (clientById.IsActive == Infra.Enums.YesOrNo.Yes)
                {
                    _clientRepository.UpdateStatus(clientById, Infra.Enums.YesOrNo.No);
                }
                else
                {
                    _clientRepository.UpdateStatus(clientById, Infra.Enums.YesOrNo.Yes);
                }

                return RedirectToAction("Index");
            } 
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = $"Erro: + {ex}";
                return RedirectToAction("Index");
            }
        }

    }

}

