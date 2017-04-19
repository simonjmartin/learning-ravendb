using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Raven.Client.Document;

namespace Northwind
{
    class Program
    {
        static void Main(string[] args)
        {
            var documentStore = new DocumentStore
            {
                Url = "http://localhost:8080",
                DefaultDatabase = "Northwind"
            };

            documentStore.Initialize();
        }
    }
}
