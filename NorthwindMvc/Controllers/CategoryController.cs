using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Packt.Shared;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace NorthwindMvc.Controllers
{
    public class CategoryController : Controller
    {

        private readonly ILogger<HomeController> _logger;
        private Northwind db;

        public CategoryController(ILogger<HomeController> logger, Northwind injectedContext)
        {
            _logger = logger;
            db = injectedContext;
        }

        // default route would be category/index/1 so can simplify that to category/1
        [Route("category/{id:int?}")]
        public async Task<IActionResult> Index(int? id)
        {
            if (id.HasValue)
            {
                var model = await db.Categories.SingleOrDefaultAsync(c => c.CategoryID == id);

                if (model == null)
                {
                    return NotFound($"Category with ID {id} was not found.");
                }

                return View(model);
            }
            else
            {
                return NotFound("You must pass a category ID in the route eg /Home/category/21");
            }
        }



    }
}
