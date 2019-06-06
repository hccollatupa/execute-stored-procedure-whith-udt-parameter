using System;
using System.Collections.Generic;

namespace SPWhithUDTParameter.Entities
{
    public class Category
    {
        public long Id { get; set; }
        public string Description { get; set; }
        public List<Product> Products { get; set; }

        public Category()
        {
            this.Products = new List<Product>();
        }
    }
}
