using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using Packt.Shared;
using System.Linq;
using System;

namespace NorthwindWeb.Pages
{
    public class CustomersByCountryModel : PageModel
    {
        public IEnumerable<Customer> Customers { get; set; }
        private readonly Northwind _db;

        [BindProperty]
        public Customer Customer { get; set; }

        public CustomersByCountryModel(Northwind injectedContext)
        {
                _db = injectedContext;
        }

        public void OnGet()
        {
            ViewData["Title"] = "Northwind Web Site - Customers Grouped by  Country";

            Customers = _db.Customers.ToArray();
        }
    }
}

