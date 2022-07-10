using Microsoft.AspNetCore.Mvc;

namespace SD_310_W22SD_Assignment.Controllers
{
    public class MySpotifyController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
