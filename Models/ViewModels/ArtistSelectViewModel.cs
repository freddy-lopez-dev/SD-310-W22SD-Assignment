using Microsoft.AspNetCore.Mvc.Rendering;

namespace SD_310_W22SD_Assignment.Models.ViewModels
{
    public class ArtistSelectViewModel
    {
        public List<SelectListItem> ArtistsList { get; set; }
        

        public ArtistSelectViewModel(List<Artist> artists)
        {
            ArtistsList = new List<SelectListItem>();
            artists.ForEach(a =>
            {
                ArtistsList.Add(new SelectListItem(a.Name, a.Id.ToString()));
            });
        }
    }
}
