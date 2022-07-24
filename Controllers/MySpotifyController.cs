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

        //Studied dropdown before class.
        //Took some ideas here https://stackoverflow.com/questions/33667310/convert-my-listint-into-a-listselectlistitem
        public IActionResult UserCollection(int? id = 1)
        {
            User currentUser = _db.Users.First(u => u.Id == id);
            IEnumerable<Collection> collections = _db.Collections.Where(c => c.UserId == currentUser.Id).OrderBy(c => c.Music.Artist.Name).Include(c => c.Music).ThenInclude(m => m.Song).Include(m => m.Music).ThenInclude(m => m.Artist);
            CollectionViewModel cvm = new CollectionViewModel(_db.Users.ToList(), _db.Musics.Include(m => m.Song).Include(m => m.Artist).ToList(), collections.ToList());
            cvm.CurrentUser = currentUser;
            return View(cvm);
        }

        [HttpPost]
        public IActionResult UserCollection(int musicId, int userId)
        {
            Music currentMusic = _db.Musics.Include(m => m.Song).First(m => m.Id == musicId);
            User currentUser = _db.Users.First(u => u.Id == userId);
            bool checkCollection = _db.Collections.Any(c => (c.MusicId == musicId) && (c.UserId == userId));
            if (checkCollection != true)
            {
                Collection newCollection = new Collection(currentUser, currentMusic);
                _db.Collections.Add(newCollection);
                currentMusic.Collections.Add(newCollection);
                currentUser.Collections.Add(newCollection);
                _db.SaveChanges();
                ViewBag.Notification = $"{currentMusic.Song.Title} successfully added to the collection!";
            } else
            {
                ViewBag.Notification = $"{currentMusic.Song.Title} is already in the collection!";
            }
            ViewBag.isValidEntry = checkCollection;
            SelectList userList = new SelectList(_db.Users.OrderByDescending(u => u.Id == userId), "Id", "Name");
            ViewBag.UserList = userList;
            List<SelectListItem> musicList = new List<SelectListItem>();
            List<Music> myMusic = _db.Musics.Include(m => m.Song).Include(m => m.Artist).ToList();
            myMusic.ForEach(m =>
            {
                string music = $"{m.Song.Title} - {m.Artist.Name}";
                musicList.Add(new SelectListItem(music, m.Id.ToString()));
            });
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
