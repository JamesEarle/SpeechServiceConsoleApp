using System;
using System.Collections.Generic;
using System.Text;

namespace SpeechServiceConsoleApp.Models
{
    class MenuItem
    {
        private string _sku;
        private string _id;
        public string Name { get; set; }
        public string Description { get; set; }
        public float Price { get; set; }
        public List<string> Ingredients { get; set; }

        public MenuItem(string Name) {
            this.Name = Name;
            // Pull additional information from item catalog lookup
        }
    }
}
