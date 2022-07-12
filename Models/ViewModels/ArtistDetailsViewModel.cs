namespace SD_310_W22SD_Assignment.Models.ViewModels
{
    public class ArtistDetailsViewModel
    {
        public string Name { get; set; }
        public Dictionary<string, int> ListOfMusic { get; set; }
        public List<Collection> Collections { get; set; }

        public ArtistDetailsViewModel(Artist artist, List<Collection> collection)
        {
            Name = artist.Name;
            ListOfMusic = new Dictionary<string, int>();
            Collections = collection;
            artist.Musics.ToList().ForEach(m =>
            {
                int countCollections = 0;
                foreach(Collection collection in Collections)
                {
                    if(collection.MusicId == m.Id)
                    {
                        countCollections++;
                    }
                }
                ListOfMusic.Add(m.Song.Title, countCollections);
            });
        }
    }
}
