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
            RequestAnOrderNumber();
        }

        private static void RequestAnOrderNumber()
        {
            while (true)
            {
                Console.WriteLine("Please, enter a company id (0 to exit):");

                int companyId;
                if(!int.TryParse(Console.ReadLine(), out companyId))
                {
                    Console.WriteLine("Company id is invalid!");
                    continue;
                }

                if (companyId == 0) break;

                QueryCompanyOrders(companyId);
            }

            Console.WriteLine("Goodbye!");

        }
        private static void QueryCompanyOrders(int companyId)
        {
            using (var session = DocumentStoreHolder.Store.OpenSession())
            {
                var orders = (
                    from order in session.Query<Order>()
                        .Include(o => o.Company)
                    where order.Company == $"companies/{companyId}"
                    select order
                    ).ToList();

                var company = session.Load<Company>(companyId);

                if (company == null)
                {
                    Console.WriteLine("Company not found");
                    return;
                }

                Console.WriteLine($"Orders for {company.Name}");

                foreach (var order in orders)
                {
                    Console.WriteLine($"{order.Id} - {order.OrderedAt}");
                }
            }
        }

        private static void PrintOrder(int orderNumber)
        {
            using (var session = DocumentStoreHolder.Store.OpenSession())
            {
                var order = session
                    .Include<Order>(o => o.Company)
                    .Include(o => o.Employee)
                    .Include(o => o.Lines.Select(l => l.Product))
                    .Load(orderNumber);

                if (order == null)
                {
                    Console.WriteLine($"Order # {orderNumber} not found.");
                    return;
                }

                Console.WriteLine($"Order #{orderNumber}");

                var c = session.Load<Company>(order.Company);
                Console.WriteLine($"Company: {c.Id} - {c.Name}");

                var e = session.Load<Employee>(order.Employee);
                Console.WriteLine($"Employee: {e.Id} - {e.LastName}, {e.FirstName}");

                foreach (var orderLine in order.Lines)
                {
                    var p = session.Load<Product>(orderLine.Product);
                    Console.WriteLine($"  - {orderLine.ProductName}, " +
                        $" {orderLine.Quantity} x {p.QuantityPerUnit}");
                }
            }
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
