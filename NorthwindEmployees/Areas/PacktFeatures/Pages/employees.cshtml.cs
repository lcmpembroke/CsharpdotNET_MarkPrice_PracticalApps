using System;
using System.Collections.Generic;           // IEnumerable<T>
using System.Linq;                          // ToArray()
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;  // PageModel
using Packt.Shared;                         // Employee

namespace PacktFeatures.Pages
{
    public class EmployeesPageModel : PageModel
    {
        private Northwind db;
        public IEnumerable<Employee> Employees { get; set; }

        public EmployeesPageModel(Northwind injectedContext)
        {
            db = injectedContext;
        }
        public void OnGet()
        {
            ViewData["Title"] = "Northwind Web Site - Employees";
            Employees = db.Employees.ToArray();
        }
    }
}
