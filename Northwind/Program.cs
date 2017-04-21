using System;
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
            GetMultipleDocuments();
        }

        private static void GetMultipleDocuments()
        {
            using (var session = DocumentStoreHolder.Store.OpenSession())
            {
                Product[] p = session.Load<Product>(1,2,3);

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

    public class Product
    {
        public string Name { get; set; }
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
