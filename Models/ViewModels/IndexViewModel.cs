namespace SD_310_W22SD_Assignment.Models.ViewModels
{
    public class IndexViewModel
    {
        public Song TopSong { get; set; }
        public Artist TopArtist { get; set; }
        public List<Music> Top3Rated { get; set; }
        public int TotalRevenue { get; set; }
        public int SelectedYear { get; set; }
        public int SelectedMonth { get; set; }
        public List<Collection> purchasedMusicByMonth { get; set; }

        public IndexViewModel(List<Music> musics, List<Collection> collections, List<Artist> artists, DateTime dateValue)
        {
            // Getting the top selling song
            Music currentMusic = musics.OrderByDescending(m => m.Collections.Count()).First();
            TopSong = currentMusic.Song;

            // Getting the top selling artist
            Dictionary<Artist, int> TopArtists = new Dictionary<Artist, int>();
            collections.ForEach(c =>
            {
                if (TopArtists.ContainsKey(c.Music.Artist))
                {
                    TopArtists[c.Music.Artist]++;
                } else
                {
                    TopArtists.Add(c.Music.Artist, 1);
                }
            });
            TopArtist = TopArtists.OrderByDescending(t => t.Value).ToDictionary(t => t.Key, t => t.Value).First().Key;

            // Getting the top 3 highest rating music
            Dictionary<Music, int> GetAvgMusic = new Dictionary<Music, int>();
            Dictionary<Music, int> FilteredTo3Music = new Dictionary<Music, int>();
            musics.ForEach(m =>
            {
                GetAvgMusic.Add(m, m.Collections.Count == 0 ? 0 : (int)m.Collections.Average(c => c.Rating));
            });
            FilteredTo3Music = GetAvgMusic.OrderByDescending(g => g.Value).Take(3).ToDictionary(g => g.Key, g => g.Value);
            Top3Rated = FilteredTo3Music.Keys.ToList();

            // Getting Getting the top Revenue
            int currentYear = dateValue.Year;
            int currentMonth = dateValue.Month;
            SelectedMonth = currentMonth;
            SelectedYear = currentYear;
            List<Collection> datedCollection = collections.Where(c => c.PurchaseDate.Year == currentYear && c.PurchaseDate.Month == currentMonth).ToList();
            purchasedMusicByMonth = datedCollection;
            TotalRevenue = (int)datedCollection.Sum(c => c.Music.Price);
        }
    }
}
