using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using PradeepTech.DataAccess.DBTran.Interface;
using PradeepTech.DataAccess.DBTran.Repositories;
using PradeepTech.DataAccess.Models;
using PradeepTech.DataAccess.Models.Auth;
using PradeepTech.WebApp.Areas.User.Models;
using PradeepTech.WebApp.Controllers;

namespace PradeepTech.WebApp.Areas.User.Controllers
{
    [Area("User")]
    public class AuthController : BaseController
    {
        // GET: AuthController
        private IAuthService _authService;

        private readonly ILogger<TempController> _logger;

        public AuthController(IAuthService authService, ILogger<TempController> logger)
        {
            _authService = authService;
            _logger = logger;
        }

        //New Ad
        public ActionResult Index()
        {
            return View();
        }

        // GET: AuthController/Create
        public ActionResult Login()
        {
            Notify("Success", "Success Message", NotificationType.success);
            LoginModel loginModel = new LoginModel();
            return View(loginModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Login(LoginModel loginModel)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    Notify("Error", "User name and Password required", NotificationType.error);
                    return View(loginModel);
                }
                var result = _authService.Login(loginModel.UserName, loginModel.Password);
                if (result.IsFaulted)
                {
                    Notify("Error", "Invalid user or password", NotificationType.error);
                }
                else
                {
                    Notify("Success", "Login Success", NotificationType.success);
                    return RedirectToAction(nameof(Index));
                }
                return View(loginModel);
            }
            catch (Exception ex)
            {
                Notify("Exception", ex.Message, NotificationType.error);
                _logger.LogCritical(ex.Message);
                return View(loginModel);
            }
        }
    }
}