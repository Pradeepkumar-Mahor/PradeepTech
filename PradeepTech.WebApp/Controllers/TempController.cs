using Microsoft.AspNetCore.Mvc;
using PradeepTech.WebApp.Models;
using System.Diagnostics;
using static PradeepTech.WebApp.Controllers.BaseController;

namespace PradeepTech.WebApp.Controllers
{
    public class TempController : BaseController
    {
        private readonly ILogger<TempController> _logger;

        public TempController(ILogger<TempController> logger)
        {
            _logger = logger;
        }

        public IActionResult Index()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

        [HttpGet]
        public IActionResult Create()
        {
            try
            {
                int a = 0;
                int b = 0;
                int c = a / b;
                Notify("Success", "Login Success", NotificationType.success);
                return View();
            }
            catch (Exception ex)
            {
                Notify("Exception", ex.Message, NotificationType.error);
                _logger.LogCritical(ex.Message);
                return View();
            }
        }

        [HttpPost]
        public IActionResult Create(IFormCollection formCollection)
        {
            try
            {
                int a = 0;
                int b = 0;
                int c = a / b;
                Notify("Success", "Login Success", NotificationType.success);
                return View();
            }
            catch (Exception ex)
            {
                Notify("Exception", ex.Message, NotificationType.error);
                _logger.LogCritical(ex.Message);
                return View();
            }
        }
    }
}