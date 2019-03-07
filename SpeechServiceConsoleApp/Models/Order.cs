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
            OrderItems = new List<MenuItem>();
        }

        public void AddToOrder(List<MenuItem> items)
        {
            OrderItems.AddRange(items);
        }

        public void RemoveFromOrder(List<MenuItem> items) 
        {
            // Can't manipulate the list you're iterating.
            // This code is bad and I should feel bad.
            List<MenuItem> itemsToRemove = new List<MenuItem>();

            foreach(var itemInOrder in OrderItems)
            {
                foreach(var itemToRemove in items) 
                {
                    if(itemInOrder.Name == itemToRemove.Name)
                    {
                        itemsToRemove.Add(itemInOrder);
                    }
                }
            }
            foreach(var itemToRemove in itemsToRemove) 
            {
                OrderItems.Remove(itemToRemove);
            }
        }
    }
}
