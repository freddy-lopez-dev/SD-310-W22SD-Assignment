using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using SD_310_W22SD_Assignment.Models;

namespace SD_310_W22SD_Assignment.Controllers
{
    public class MySpotifyController : Controller
    {
        private MySpotifyDBContext _db { get; set; }

        public MySpotifyController(MySpotifyDBContext context)
        {
            _db = context;
        }
        public IActionResult Index()
        {
            return View();
        }

        public IActionResult UserCollection(int? id)
        {
            SelectList userList = new SelectList(_db.Users, "Id", "Name");
            ViewBag.UserList = userList;
            if (id.HasValue)
            {
                User currentUser = _db.Users.First(u => u.Id == id);
                return View(_db.Collections.Where(c => c.UserId == currentUser.Id).Include(c => c.Music).ThenInclude(m => m.Song).Include(m => m.Music).ThenInclude(m => m.Artist));
            } else
            {
                return View();
            }
                        
        }

    }
}
