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
            List<Music> musics = _db.Musics.Include(m => m.Collections).Include(m => m.Song).ToList();
            List<Collection> collections = _db.Collections.Include(c => c.Music).ThenInclude(m => m.Artist).ToList();
            List<Artist> artists = _db.Artists.Include(a => a.Musics).ToList();
            IndexViewModel IVM = new IndexViewModel(musics, collections, artists);
            return View(IVM);
        }

        //Refactored into ViewModel from ViewBag Part 1
        public IActionResult UserCollection(int? userId = 1)
        {
            User currentUser = _db.Users.First(u => u.Id == userId);
            IEnumerable<Collection> collections = _db.Collections.Where(c => c.UserId == currentUser.Id).OrderBy(c => c.Music.Artist.Name).Include(c => c.Music).ThenInclude(m => m.Song).Include(m => m.Music).ThenInclude(m => m.Artist);
            CollectionViewModel cvm = new CollectionViewModel(_db.Users.OrderByDescending(u => u.Id == userId).ToList(), _db.Musics.Include(m => m.Song).Include(m => m.Artist).ToList(), collections.ToList());
            cvm.CurrentUser = currentUser;
            //Studied tempdata
            //https://www.tutorialsteacher.com/mvc/tempdata-in-asp.net-mvc
            ViewBag.Alert = TempData["ValidPurchase"];
            ViewBag.Notification = TempData["notification"];
            return View(cvm);
        }

        [HttpPost]
        public IActionResult UserCollection(int musicId, int userId)
        {
            Music currentMusic = _db.Musics.Include(m => m.Song).First(m => m.Id == musicId);
            User currentUser = _db.Users.First(u => u.Id == userId);
            if(currentUser.Wallet > currentMusic.Price)
            {
                Collection newCollection = new Collection(currentUser, currentMusic);
                _db.Collections.Add(newCollection);
                currentMusic.Collections.Add(newCollection);
                currentUser.Collections.Add(newCollection);
                currentUser.Wallet -= currentMusic.Price;
                _db.SaveChanges();
                TempData["validPurchase"] = true;
                TempData["notification"] = $"{currentMusic.Song.Title} successfully brought and added to the collection!. Current wallet value: ${currentUser.Wallet.Value.ToString("0.00")}";
                
            } else
            {
                TempData["validPurchase"] = false;
                TempData["notification"] = $"Not enough money to purchase: {currentMusic.Song.Title}";
            }
            

            return RedirectToAction("UserCollection", new { userId });
        }

        [HttpPost]           
        public IActionResult RefundMusic(int collectionId, int userId)
        {
            Collection currentCollection = _db.Collections.Include(c => c.Music).ThenInclude(m => m.Song).First(c => c.Id == collectionId);
            User currentUser = _db.Users.First(u => u.Id == userId);
            _db.Collections.Remove(currentCollection);
            currentUser.Wallet += currentCollection.Music.Price;
            _db.SaveChanges();
            TempData["validPurchase"] = true;
            TempData["notification"] = $"{currentCollection.Music.Song.Title} has successfully been refunded!. Current wallet value: ${currentUser.Wallet.Value.ToString("0.00")}";
            return RedirectToAction("UserCollection", new { userId });
        }

        /* Rating music is implemented in the same page as UserCollection. 
         * This will solve the validation of user should not be able to rate the music that has not been purchased.
         * RateMusic will get the user selection by clicking of stars in the table (UserCollection View).
         * Stars defined in a loop index to have a value from 0 to 4.
         * Adding 1 and assigning it to the collection rating value.
         */
        [HttpPost]
        public IActionResult RateMusic(int rateCount, int userId, int collectionId)
        {
            Collection currentCollection = _db.Collections.First(c => c.Id == collectionId);
            currentCollection.Rating = rateCount + 1;
            _db.SaveChanges();
            return RedirectToAction("UserCollection", new { userId });
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
