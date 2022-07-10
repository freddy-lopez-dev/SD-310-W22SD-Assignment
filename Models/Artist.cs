using System;
using System.Collections.Generic;

namespace SD_310_W22SD_Assignment.Models
{
    public partial class Artist
    {
        public Artist()
        {
            Musics = new HashSet<Music>();
        }

        public int Id { get; set; }
        public string? Name { get; set; }

        public virtual ICollection<Music> Musics { get; set; }
    }
}
