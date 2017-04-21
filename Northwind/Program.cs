﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Raven.Client;
using Raven.Client.Document;

namespace Northwind
{
    class Program
    {
        static void Main(string[] args)
        {
            RequestAnOrderNumber();
        }

        private static void RequestAnOrderNumber()
        {
            while (true)
            {
                Console.WriteLine("Please, enter an order # (0 to exit):");

                int orderNumber;
                if(!int.TryParse(Console.ReadLine(), out orderNumber))
                {
                    Console.WriteLine("Order # is invalid!");
                    continue;
                }

                if (orderNumber == 0) break;

                PrintOrder(orderNumber);
            }
            
        }
        private static void PrintOrder(int orderNumber)
        {
            
        }

        private static void LoadRelatedData()
        {
            using (var session = DocumentStoreHolder.Store.OpenSession())
            {
                var order = session
                    .Include<Order>(x => x.Company)
                    .Include(x => x.Employee)
                    .Include(x => x.Lines.Select(l => l.Product))
                    .Load("orders/1");

            }
        }

        private static void LoadMultipleTypes()
        {
            //using (var session = DocumentStoreHolder.Store.OpenSession())
            //{
            //    object[] items = session.Load<object>("products/1", "categories/2");

            //    Product product = (Product)items[0];
            //    Category category = (Category)items[1];
            //}
        }

        private static void GetMultipleDocuments()
        {
            using (var session = DocumentStoreHolder.Store.OpenSession())
            {
                Product[] p = session.Load<Product>(1, 2, 3);

                foreach (var item in p)
                {
                    Console.WriteLine(item.Name);
                }
                Console.ReadLine();
            }
        }

        private static void GetProductByConvention()
        {
            using (var session = DocumentStoreHolder.Store.OpenSession())
            {
                Product p = session.Load<Product>("products/3");
                Console.WriteLine(p.Name);
                Console.ReadLine();
            }
        }

        private static void SimpleGetProduct()
        {
            using (var session = DocumentStoreHolder.Store.OpenSession())
            {
                var p = session.Load<dynamic>("products/3");
                System.Console.WriteLine(p.Name);
                Console.ReadLine();
            }
        }
    }

    public static class DocumentStoreHolder
    {
        private static readonly Lazy<IDocumentStore> LazyStore = new Lazy<IDocumentStore>(() =>
        {
            var store = new DocumentStore
            {
                ConnectionStringName = "RavenDB"
            };

            return store.Initialize();
        });

        public static IDocumentStore Store => LazyStore.Value;
    }
}
