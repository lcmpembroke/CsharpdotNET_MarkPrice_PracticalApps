using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Packt.Shared;
using System.Net.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using NorthwindMvc.Models;
using System.Diagnostics;

namespace NorthwindMvc.Controllers
{
    public class CustomersController : Controller
    {
        private readonly ILogger<CustomersController> _logger;
        private Northwind db;
        private readonly IHttpClientFactory _clientFactory;

        public CustomersController(ILogger<CustomersController> logger, Northwind injectedContext, IHttpClientFactory httpClientFactory)
        {
            _logger = logger;
            db = injectedContext;
            _clientFactory = httpClientFactory;
        }


        public async Task<IActionResult> Index(string country)
        {
            string uri;

            if (string.IsNullOrEmpty(country))
            {
                ViewData["Title"] = "All customers worldwide";
                uri = "api/customers";
            }
            else
            {
                ViewData["Title"] = $"Customers in {country}";
                uri = $"api/customers/?country={country}";
            }

            try
            {
                var client = _clientFactory.CreateClient(name: "NorthwindService");
                var request = new HttpRequestMessage(method: HttpMethod.Get, requestUri: uri);
                HttpResponseMessage response = await client.SendAsync(request);

                string jsonString = await response.Content.ReadAsStringAsync();

                IEnumerable<Customer> model = JsonConvert.DeserializeObject<IEnumerable<Customer>>(jsonString);

                return View(model);
            }
            catch (System.Net.Http.HttpRequestException ex)
            {
                return View();
            }
        }

    }
}