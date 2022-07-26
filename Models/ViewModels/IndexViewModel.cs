namespace SD_310_W22SD_Assignment.Models.ViewModels
{
    public class IndexViewModel
    {
        public Song TopSong { get; set; }
        public Artist TopArtist { get; set; }
        List<Music> Top3Rated { get; set; }


        public IndexViewModel(List<Music> musics, List<Collection> collections, List<Artist> artists)
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
            Dictionary<Music, int> TopRatedMusic = new Dictionary<Music, int>();
            Dictionary<Music, int> FilteredTo3Music = new Dictionary<Music, int>();
            collections.ForEach(c =>
            {
                if (TopRatedMusic.ContainsKey(c.Music))
                {
                    TopRatedMusic[c.Music] += c.Rating;
                }
                else
                {
                    TopRatedMusic.Add(c.Music, c.Rating);
                }
            });
            FilteredTo3Music = TopRatedMusic.OrderByDescending(t => t.Value).ToDictionary(t => t.Key, t => t.Value);
            foreach(KeyValuePair<Music, int> pair in FilteredTo3Music)
            {
                Top3Rated.Add(pair.Key);
            }
        }
    }
}
