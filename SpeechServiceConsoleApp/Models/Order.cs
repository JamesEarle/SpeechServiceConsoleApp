using System;
using System.Collections.Generic;
using System.Text;

namespace SpeechServiceConsoleApp.Models
{
    class Order
    {
        public List<MenuItem> OrderItems { get; set; }
        public Order() 
        {
            // Empty constructor
        }

        public Boolean AddToOrder(MenuItem item)
        {
            try {
                OrderItems.Add(item);
                return true;
            } catch(Exception e) {
                return false;
            }
        }

        public Boolean RemoveFromOrder(MenuItem item) 
        {
            try {
                // Perform name equality check, if not in list return false
                OrderItems.Remove(item);
                return true;
            } catch(Exception e) {
                return false;
            }
        }
    }
}
