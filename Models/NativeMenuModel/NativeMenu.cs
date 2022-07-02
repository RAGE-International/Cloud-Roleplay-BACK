using Backend.Models.NativeMenuModel.Data;
using System;
using System.Collections.Generic;
using System.Text;

namespace Backend.Models.NativeMenuModel
{
    public class NativeMenu
    {
        public string Title { get; set; }
        public string Description { get; set; }
        public List<NativeItem> Items { get; set; }
        public bool SearchBar { get; set; }

        public NativeMenu(string title, string description, List<NativeItem> items, bool searchBar = false)
        {
            this.Title = title;
            this.Description = description;
            this.Items = items;
            this.SearchBar = searchBar;
        }
    }
}
