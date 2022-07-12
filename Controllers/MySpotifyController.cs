using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using SD_310_W22SD_Assignment.Models;
using SD_310_W22SD_Assignment.Models.ViewModels;
using System.Linq;

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
            ViewBag.ArtistList = _db.Artists;
            ViewBag.Music = _db.Musics.Take(5).Include(m => m.Artist).Include(m => m.Song);
            return View(_db.Musics.Take(9).Include(m => m.Artist).Include(m => m.Song));
        }

        public IActionResult UserCollection(int? id = 1)
        {
            SelectList userList = new SelectList(_db.Users, "Id", "Name");
            ViewBag.UserList = userList;
            List<SelectListItem> musicList = new List<SelectListItem>();
            List<Music> myMusic = _db.Musics.Include(m => m.Song).Include(m => m.Artist).ToList();
            myMusic.ForEach(m =>
            {
                string music = $"{m.Song.Title} - {m.Artist.Name}";
                musicList.Add(new SelectListItem(music, m.Id.ToString()));
            });
            ViewBag.MusicList = musicList;
            ViewBag.userId = id;
            if (id.HasValue)
            {
                User currentUser = _db.Users.First(u => u.Id == id);
                IEnumerable<Collection> collections = _db.Collections.Where(c => c.UserId == currentUser.Id).OrderBy(c => c.Music.Artist.Name).Include(c => c.Music).ThenInclude(m => m.Song).Include(m => m.Music).ThenInclude(m => m.Artist);
                if(collections.Count() == 0)
                {
                    return View();
                } else
                {
                    return View(collections);
                }
            } else
            {
                return View();
            }            
        }

        [HttpPost]
        public IActionResult UserCollection(int id, int userId)
        {
            Music currentMusic = _db.Musics.First(m => m.Id == id);
            User currentUser = _db.Users.First(u => u.Id == userId);
            Collection newCollection = new Collection(currentUser, currentMusic);
            _db.Collections.Add(newCollection);
            currentMusic.Collections.Add(newCollection);
            currentUser.Collections.Add(newCollection);
            _db.SaveChanges();

            SelectList userList = new SelectList(_db.Users, "Id", "Name");
            SelectList musicList = new SelectList(_db.Songs, "Id", "Title");
            ViewBag.UserList = userList;
            ViewBag.MusicList = musicList;
            return View(_db.Collections.Where(c => c.UserId == currentUser.Id).OrderBy(c => c.Music.Artist.Name).Include(c => c.Music).ThenInclude(m => m.Song).Include(m => m.Music).ThenInclude(m => m.Artist));
        }

        public IActionResult FindArtist()
        {
            ArtistSelectViewModel vm = new ArtistSelectViewModel(_db.Artists.ToList());
            return View(vm);
        }

        public IActionResult ArtistDetails(int? artistId)
        {
            
            if (artistId != null)
            {
                try
                {
                    Artist currentArtist = _db.Artists.Include(a => a.Musics).ThenInclude(m => m.Song).First(c => c.Id == artistId);
                    ArtistDetailsViewModel vm = new ArtistDetailsViewModel(currentArtist, _db.Collections.ToList());
                    return View(vm);
                }
                catch
                {
                    return RedirectToAction("Error", "Home");
                }
            }
            else
            {
                return RedirectToAction("Error", "Home");
            }

        }

    }
}
