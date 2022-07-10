using System;
using System.Collections.Generic;

namespace SD_310_W22SD_Assignment.Models
{
    public partial class Collection
    {
        public int Id { get; set; }
        public int? MusicId { get; set; }
        public int? UserId { get; set; }

        public virtual Music? Music { get; set; }
        public virtual User? User { get; set; }
    }
}
