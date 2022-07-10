using System;
using System.Collections.Generic;

namespace SD_310_W22SD_Assignment.Models
{
    public partial class Music
    {
        public Music()
        {
            Collections = new HashSet<Collection>();
        }

        public int Id { get; set; }
        public int? SongId { get; set; }
        public int? ArtistId { get; set; }

        public virtual Artist? Artist { get; set; }
        public virtual Song? Song { get; set; }
        public virtual ICollection<Collection> Collections { get; set; }
    }
}
