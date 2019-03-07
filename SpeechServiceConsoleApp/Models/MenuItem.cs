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
        public string Size { get; set; }
        public string Description { get; set; }
        public float Price { get; set; }
        public List<string> Ingredients { get; set; }
        public List<string> Modifiers { get; set; }

        public MenuItem() {
            this.Ingredients = new List<string>();
            this.Modifiers = new List<string>();
        }
    }
}
