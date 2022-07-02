using System;
using System.Collections.Generic;
using System.Text;

namespace Backend.Modules.ItemShop.Data
{
    public class ClientItemShopModel
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public int Price { get; set; }
        public int Quantity { get; set; }
    }
}
