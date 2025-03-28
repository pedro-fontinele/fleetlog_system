
using LOGHouseSystem.Infra.Enums;
using LOGHouseSystem.Infra.Helpers;
using LOGHouseSystem.Models;
using LOGHouseSystem.Models.SendEmails;
using LOGHouseSystem.Repositories.Interfaces;
using LOGHouseSystem.Services.Interfaces;
using LOGHouseSystem.ViewModels;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace LOGHouseSystem.Controllers.MVC
{
    public class LoginController : Controller
    {
        private IUserRepository _userRepository;
        private SessionHelper _session = new SessionHelper();
        private readonly IEmailService _email;
        private readonly IConfiguration _configuration;
        private readonly IHttpContextAccessor _httpContextAccessor;


        public LoginController(IEmailService email, IUserRepository userRepository, IConfiguration configuration, IHttpContextAccessor httpContextAccessor)
        {
            _email = email;
            _userRepository = userRepository;
            _configuration = configuration;
            _httpContextAccessor = httpContextAccessor;
        }

        public ActionResult Index()
        {
            //Se o usuário estiver logado, redirecionar para Home
            var user = _session.SearchUserSession();
            if (user != null && user.Id > 0) return RedirectToAction("Index", "Home");

            _session.RemoveUserSession();

            return View();
        }

        [HttpPost]
        public IActionResult Login(Login login)
        {
            try
            {
                if(ModelState.IsValid)
                {

                   User user = _userRepository.SearchByLogin(login.LoginName, 0);

                   if(user == null)
                    {
                        user = _userRepository.SearchByEmail(login.LoginName, 0);
                    }

                    if(user != null)
                    {
                        if (user.ValidPassword(login.Password))
                        {
                            _session.CreateSession(user);
                            if(user.FirstAcess == YesOrNo.Yes)
                            {
                                return RedirectToAction("ChangePassword", "User");
                            }

                            if(user.PermissionLevel == PermissionLevel.Client)
                            {
                                return RedirectToAction("IndexClient", "Home");
                            }
                            return RedirectToAction("Index", "Home");
                        }

                        ViewBag.LoginError = "Senha inválida";
                    }
                    else
                    {
                        ViewBag.LoginError = "Usuário ou senha inválidos";
                        return View("Index", login);
                    }
                }
                return View("Index", login);
            }
            catch(Exception)
            {
                return RedirectToAction("Index");
            }
        }

        public IActionResult ForgotPassword()
        {
            return View();
        }

        [HttpPost]
        public IActionResult PasswordRedefinitionByEmail(ForgotPasswordViewModel model)
        {
            try
            {
                if (ModelState.IsValid)
                {

                    User user = _userRepository.SearchByEmail(model.Email, 0);


                    var urlUsed = $"{_httpContextAccessor.HttpContext.Request.Scheme}://{_httpContextAccessor.HttpContext.Request.Host}{_httpContextAccessor.HttpContext.Request.PathBase}";

                    if (user != null)
                    {
                       user.FirstAcess = YesOrNo.Yes;
                       string newPassword = user.GenerateNewPassword();
                       string message = $"Sua nova senha é: {newPassword}";
                       EmailData emailData = new EmailData()
                       {
                           EmailToId = user.Email,
                           EmailToName = user.Name,
                           EmailSubject = "Nova Senha",
                           EmailBody = $"<h1>Olá, {user.Name} </h1>" + "<br/>" +
                                       "<p> Você está recebendo este e-mail da LogHouse porque solicitou a redefinição de sua senha através do nosso sistema.\r\nSe você não solicitou essa alteração, pode ignorar este e-mail tranquilamente. </p>" +
                                       "<br/>" + "Essa é sua nova senha temporária, será solicitado uma nova senha em seu primeiro acesso." + "<br/>" + $"{newPassword}" + "<br/>" +
                                       "<button style=\"background-color: #496FDC; border: none; border-radius: 4px;color: white; padding: 10px 20px; text-align: center; text-decoration: none; display: inline-block; font-size: 16px; margin: 4px 2px; cursor: pointer;\">" +
                                       "<a style=\"color: white; text-decoration: none; display: inline-block; font-size: 16px; margin: 4px 2px; cursor: pointer;\" href=\"" + urlUsed + "\">Clique aqui</a>" +
                                       "</button>" + "<br/>" + $"Url: {urlUsed}" + "<br/>" + "<p>Você pode alterar sua senha novamente a qualquer momento, através do painel de configuração de sua conta.</p>"
                                       + "<br/>" + "Atenciosamente,\r\nEquipe LogHouse"
                       };

                        bool sendEmail = _email.SendEmail(emailData, user, 0);

                        if (sendEmail) 
                        {
                            _userRepository.UpdatePassword(user);
                            TempData["SuccessMessage"] = "Enviamos uma nova senha para o seu email, faça login com ela para entrar na sua conta e altera-la.";
                            return RedirectToAction("Index", "Login");
                        }
                        else
                        {
                            ViewBag.Error = "Não foi possível enviar o email com a sua senha. Por favor, tente novamente!";
                        }
                        return RedirectToAction("Index");
                    }

                    TempData["ErrorMessage"] = "Email não cadastrado! Para realizar seu cadastro, entre em contato conosco";
                    return View("ForgotPassword");
                }

                ViewBag.Error = "Digite corretamente o email do usuário";
                return View("ForgotPassword");
            }
            catch (Exception)
            {
                ViewBag.LoginError = "Não foi possível redefinir sua senha";
                return RedirectToAction("Index");
            }

        }

        public ActionResult Logout()
        {
            _session.RemoveUserSession();
            return RedirectToAction("Index", "Login");

        }
    }
}
