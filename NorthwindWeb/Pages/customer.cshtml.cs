using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using Packt.Shared;
using System.Linq;
using System;
using Microsoft.EntityFrameworkCore;    // Include...a new query with the related data included
using System.Threading.Tasks;

namespace NorthwindWeb.Pages
{
    public class CustomerModel: PageModel
    {
        private readonly Northwind _db;

        [BindProperty]
        public Customer Customer { get; set; }

        // automatically get the id from query string e.g. /customer?id=ALFKI
        [BindProperty(SupportsGet = true)]
        public string ID { get; set; }

        public CustomerModel(Northwind injectedContext)
        {
            _db = injectedContext;
        }

        public void OnGet()
        {
            ViewData["Title"] = "Northwind Web Site - Customer";
            //Customer = db.Customers.First(cust => cust.CustomerID == ID);

            Customer = _db.Customers
            .Where(c => c.CustomerID == ID)   // get the customer with this ID
            .Include(c => c.Orders)           // ...and their orders
            .ThenInclude(o => o.OrderDetails) // ...with order details
            .ThenInclude(d => d.Product)      // ...and product details
            .FirstOrDefault();               // limit to the first customer.
        }
    }
}

