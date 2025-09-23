using Microsoft.AspNetCore.Mvc;

namespace WebApp.Controllers
{
    [Route("trang-chu")]
    public class HomeController() : Controller
    {
        [HttpGet("")]
        public IActionResult Index()
        {           
            return View("~/Views/Home/Index.cshtml");
        }
    }
}