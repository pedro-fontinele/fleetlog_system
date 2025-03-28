
using LOGHouseSystem.Infra.Filters;
using LOGHouseSystem.Infra.Helpers;
using LOGHouseSystem.Models;
using LOGHouseSystem.Models.SendEmails;
using LOGHouseSystem.Repositories;
using LOGHouseSystem.Repositories.Interfaces;
using LOGHouseSystem.Services.Interfaces;
using LOGHouseSystem.ViewModels;
using Microsoft.AspNetCore.Mvc;

namespace LOGHouseSystem.Controllers.MVC
{

    public class UserController : Controller
    {
        private IUserRepository _userRepository;
        private readonly IEmailService _email;
        private SessionHelper _session = new SessionHelper();

        public UserController(IEmailService email, IUserRepository userRepository)
        {
            _email= email;
            _userRepository= userRepository;
        }

        [PageForAdmin]
        //GET: Users
        public IActionResult Index()
        {
           List<User> users =  _userRepository.GetAllNotClients();
           return View(users);
        }

        [PageForAdmin]
        public IActionResult Create()
        {
            return View();
        }

        [PageForAdmin]
        [HttpPost]
        public IActionResult Create(User user)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    bool checkedLogin = _userRepository.CheckUserByLogin(user.Username, user.Id);

                    if (checkedLogin == false)
                    {
                        TempData["ErrorMessage"] = "Ops! Parece que já existe um usuário cadastrado com esse Login. Tente utilizar outro!";
                        return View();
                    }

                    bool checkedEmail = _userRepository.CheckUserByLogin(user.Email, user.Id);

                    if (checkedEmail == false)
                    {
                        TempData["ErrorMessage"] = "Ops! Parece que já existe um usuário cadastrado com esse Email. Tente utilizar outro!";
                        return View();
                    }

                    _userRepository.Add(user);                    
                    
                    TempData["SuccessMessage"] = "Usuário criado com sucesso!";
                    return RedirectToAction("Index");
                }

                TempData["ErrorMessage"] = "Ops! Parece que você esqueceu de preencher e/ou preencheu incorretamente algum campo, por favor, tente novamente!";

                return View(user);

            }
            catch(Exception ex)
            {
                TempData["ErrorMessage"] = "Algo de errado aconteceu, não foi possível cadastrar o seu usuário. Detalhe do erro: " + ex;
                return RedirectToAction("Index");
            }
        }

        [PageForAdmin]
        public IActionResult Edit(int id)
        {
            User user = _userRepository.GetById(id);
            EditUserViewModel userVw = new EditUserViewModel()
            {
                Username = user.Username,
                Name = user.Name,
                Email = user.Email,
                Password = user.Password,
                PermissionLevel = user.PermissionLevel,
                IsActive = user.IsActive,

            };
            return View(userVw);
        }

        [PageForAdmin]
        [HttpPost]
        public IActionResult Edit(EditUserViewModel user)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    bool checkedLogin = _userRepository.CheckUserByLogin(user.Username, user.Id);

                    if (checkedLogin == false)
                    {
                        ViewBag.UsernameError = "Esse usuário já está em uso";
                        return View();
                    }

                    bool checkedEmail = _userRepository.CheckUserByLogin(user.Email, user.Id);

                    if (checkedEmail == false)
                    {
                        ViewBag.EmailError = "Esse email já está em uso";
                        return View();
                    }

                    _userRepository.Update(user);
                    TempData["SuccessMessage"] = "Usuário editado com sucesso!";
                    return RedirectToAction("Index");
                }

                return View("Edit", user);
            }
            catch(Exception ex)
            {
                TempData["ErrorMessage"] = "Algo de errado aconteceu, não foi possível editar o seu usuário. Detalhe do erro: " + ex;
                return RedirectToAction("Index");
            }
        }

        [PageForLogedUser]
        public IActionResult ChangePassword()
        {
            return View();
        }

        [PageForLogedUser]
        [HttpPost]
        public IActionResult PasswordRedefinition(PasswordRedefinitionViewModel changePassword)
        {
            try
            {
                User userLoged = _session.SearchUserSession();
                changePassword.Id = userLoged.Id;

                if (ModelState.IsValid)
                {
                    _userRepository.ChangePassword(changePassword);
                    TempData["SuccessMessage"] = "Senha alterada com sucesso!";

                    if(userLoged.PermissionLevel != Infra.Enums.PermissionLevel.Client)
                    {
                        return RedirectToAction("Index", "Home");
                    }

                    return RedirectToAction("IndexClient", "Home");
                }

                return View("ChangePassword", changePassword);
            }
            catch(Exception ex)
            {
                TempData["ErrorMessage"] = $"Ops, não conseguimos realizar a troca de senha, por favor, tente novamente! Detalhe do erro: {ex.Message}";

                return View("ChangePassword", changePassword);
            }

        }

    }
}
