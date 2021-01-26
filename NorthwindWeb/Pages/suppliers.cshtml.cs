using Microsoft.AspNetCore.Mvc.RazorPages;      // gives ViewData available as this model inherits from PageModel
using Microsoft.AspNetCore.Mvc;                 // to enable response to HTTP post request
using System.Collections.Generic;
using System.Linq;
using Packt.Shared;

namespace NorthwindWeb.Pages
{
    public class SuppliersModel : PageModel
    {
        
        public IEnumerable<string> Suppliers { get; set; }
        private Northwind db;
                
        [BindProperty]  // enables HTML elements on web page to easily connect to properties in the Supplier class
        public Supplier Supplier { get; set; }

        public SuppliersModel(Northwind injectedContext)
        {
            db = injectedContext;
        }

        public void OnGet()
        {
            ViewData["Title"] = "Northwind Web Site - Suppliers";

            Suppliers = db.Suppliers.Select(s => s.CompanyName);
        }
        

        public IActionResult OnPost()
        {
            if (ModelState.IsValid) // checks all property values conform to validation rules
            {
                db.Suppliers.Add(Supplier);
                db.SaveChanges();
                return RedirectToPage("/suppliers");
            }
            return Page();
        }
    }

}