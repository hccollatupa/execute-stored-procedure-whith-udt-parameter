using SPWhithUDTParameter.Entities;
using SPWhithUDTParameter.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SPWhithUDTParameter
{
    class Program
    {
        static void Main(string[] args)
        {
            log4net.Config.XmlConfigurator.Configure();

            List<Product> products = new List<Product>() {
                new Product(){ Id = 1, Description = "Producto 1", Price = 1 },
                new Product(){ Id = 2, Description = "Producto 2", Price = 2 }
            };

            Category category = new Category();
            category.Description = "Prueba";
            category.Products = products;

            CategoryRepository categoryRepository = new CategoryRepository();
            long id = categoryRepository.Create(category);
            Console.WriteLine("Registro generado con código: " + id);

            Console.WriteLine("Presione una tecla para finalizar...");
            Console.ReadLine();
        }
    }
}