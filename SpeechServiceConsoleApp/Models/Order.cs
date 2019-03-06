using System;
using System.Collections.Generic;
using System.Text;

namespace SpeechServiceConsoleApp.Models
{
    class Order
    {
        public List<MenuItem> OrderItems { get; set; }

        public List<MenuItem> AddToOrder(MenuItem item)
        {
            OrderItems.Add(item);
            return OrderItems;
        }
    }
}
