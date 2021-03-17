using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using NorthwindMvc.Models;
using Packt.Shared;
using Microsoft.EntityFrameworkCore;    // To use Include extension method for related entities
//using Microsoft.AspNetCore.Razor.TagHelpers;


namespace NorthwindMvc.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private Northwind db;
        //private readonly IHttpClientFactory _clientFactory;

        public HomeController(ILogger<HomeController> logger, Northwind injectedContext)
        {
            _logger = logger;
            db = injectedContext;
            //_clientFactory = httpClientFactory;

        }

        public async Task<IActionResult> Index()
        {
            var rand = new Random();

            HomeIndexViewModel viewModel = new HomeIndexViewModel
            {
                VisitorCount = rand.Next(1, 1001),
                Categories = await db.Categories.ToListAsync(),
                Products = await db.Products.ToListAsync()
            };
            
            return View(viewModel);
        }

        public IActionResult Privacy()
        {
            return View();
        }

        public IActionResult ProductsThatCostMoreThan(decimal? price)
        {
            if (price.HasValue)
            {
                var model = db.Products
                    .Include(p => p.Category)
                    .Include(p => p.Supplier)
                    .AsEnumerable() // switch to client side
                    .Where(p => p.UnitPrice > price);

                if (model.Count() == 0)
                {
                    return NotFound($"No products that cost more than {price:C}.");
                }
                ViewData["PriceThreshold"] = price.Value;    // add the price to the view data dictionary
                return View(model);
            }
            else
            {
                return NotFound("A product price must be passed into the query string eg /Home/ProductsThatCostMoreThan?price=50");
            }
            
        }

        public async Task<IActionResult> ProductDetail(int? id)
        {
            if (id.HasValue)
            {
                var model = await db.Products.SingleOrDefaultAsync(p => p.ProductID == id);

                if (model == null)
                {
                    return NotFound($"Product with ID {id} not found");
                }

                return View(model);
            }
            else
            {
                return NotFound("You must pass a product ID in the route eg /Home/ProductDetail/21");
            }
        }

        // MOVED TO CustomersController
        //public async Task<IActionResult> Customers(string country)
        //{
        //    string uri;

        //    if (string.IsNullOrEmpty(country))
        //    {
        //        ViewData["Title"] = "All customers worldwide";
        //        uri = "api/customers";
        //    }
        //    else
        //    {
        //        ViewData["Title"] = $"Customers in {country}";
        //        uri = $"api/customers/?country={country}";
        //    }

        //    var client = _clientFactory.CreateClient(name: "NorthwindService");
        //    var request = new HttpRequestMessage(method: HttpMethod.Get, requestUri: uri);
        //    HttpResponseMessage response = await client.SendAsync(request);

        //    string jsonString = await response.Content.ReadAsStringAsync();

        //    IEnumerable<Customer> model = JsonConvert.DeserializeObject<IEnumerable<Customer>>(jsonString);

        //    return View(model);
        //}


        public IActionResult ModelBinding()
        {
            return View();  // page with a form to submit
        }


        [HttpPost]
        public IActionResult ModelBinding(Thing thing)
        {
            var model = new HomeModelBindingViewModel
            {
                Thing = thing,
                HasErrors = !ModelState.IsValid,
                ValidationErrors = ModelState.Values
                .SelectMany(state => state.Errors)
                .Select(error => error.ErrorMessage)
            };

            //return View(thing);  // show the model bound thing
            return View(model);  // show the model bound thing
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
