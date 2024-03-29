﻿using Microsoft.AspNetCore.Mvc.Rendering;

namespace SD_310_W22SD_Assignment.Models.ViewModels
{
    public class CollectionViewModel
    {
        public User CurrentUser { get; set; }
        public List<SelectListItem> UserList { get; set; }
        public List<SelectListItem> MusicList { get; set; }
        public List<Collection> Collections { get; set; }

        public CollectionViewModel(List<User> userList, List<Music> music, List<Collection> collections)
        {
            UserList = new List<SelectListItem>();
            MusicList = new List<SelectListItem>();
            userList.ForEach(u =>
            {
                UserList.Add(new SelectListItem(u.Name, u.Id.ToString()));
            });
            Collections = collections;
            if (collections.Count() == 0)
            {
                List<Music> orderedMusic = music.OrderBy(m => m.Artist.Name).ToList();
                orderedMusic.ForEach(m =>
                {
                    string txt = $"{m.Song.Title} - {m.Artist.Name} ({m.Price})";
                    MusicList.Add(new SelectListItem(txt, m.Id.ToString()));
                });
            } else
            {
                List<Music> checkedMusic = music.Where(m => !collections.Any(c => c.MusicId == m.Id)).OrderBy(m => m.Artist.Name).ToList();
                
                checkedMusic.ForEach(m =>
                {
                    string txt = $"{m.Song.Title} - {m.Artist.Name} ({m.Price})";
                    MusicList.Add(new SelectListItem(txt, m.Id.ToString()));
                });
            }
        }
    }
}
