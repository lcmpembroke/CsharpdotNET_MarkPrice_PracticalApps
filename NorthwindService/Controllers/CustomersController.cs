using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using NorthwindService.Repositories;
using Packt.Shared;

namespace NorthwindService.Controllers
{
    // base address api/customers
    [Route("api/[controller]")]
    [ApiController]
    public class CustomersController : ControllerBase
    {
        private readonly ICustomerRepository _customerRepository;

        public CustomersController(ICustomerRepository injectedRepo)
        {
            this._customerRepository = injectedRepo;
        }

        // GET: api/customers/
        // GET: api/customers/?country=[country]
        [HttpGet]
        [ProducesResponseType(200, Type = typeof(IEnumerable<Customer>))]
        public async Task<IEnumerable<Customer>> GetCustomers(string country)
        {
            if (string.IsNullOrWhiteSpace(country))
            {
                return (await _customerRepository.RetrieveAllAsync());
            }
            else
            {
                return (await _customerRepository.RetrieveAllAsync()).Where(cust => cust.Country == country);
            }
        }

        // GET: api/customers/[id]
        [HttpGet("{id}", Name = nameof(GetCustomer))]     // named route so it can be used to generate URL after inserting a new customer
        [ProducesResponseType(200, Type = typeof(Customer))]
        [ProducesResponseType(404)]
        public async Task<IActionResult> GetCustomer(string id)
        {
            Customer c = await _customerRepository.RetrieveAsync(id);
            if (c == null)
            {
                return NotFound();
            }
            return Ok(c);
        }
        
        // POST: api/customers
        [HttpPost]
        [ProducesResponseType(201, Type = typeof(Customer))]
        [ProducesResponseType(400)]
        public async Task<IActionResult> Create([FromBody] Customer c)  // param is decorated with [FromBody to tell model binder to populate it with values from body of HTTP POST request]
        {
            if (c == null)
            {
                return BadRequest();    // 400 Bad request
            }

            if (ModelState.IsValid == false)
            {
                return BadRequest(ModelState);
            }

            try
            {
                Customer added = await _customerRepository.CreatAsync(c);
                return CreatedAtRoute(
                    routeName: nameof(GetCustomer),
                    routeValues: new { id = added.CustomerID.ToLower() },
                    value: added);
            }
            catch (System.Exception ex)
            {
                return BadRequest(ex.Message);    // 400 Bad request
            }
        }

        // PUT: api/customers/[id]
        [HttpPut("{id}")]
        [ProducesResponseType(204)]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        public async Task<IActionResult> Update(string id, [FromBody] Customer c)
        {
            if (id == null)
            {
                return BadRequest("No customer id provided for update");    // 400 Bad request
            }

            if (!ModelState.IsValid)
            {
                return BadRequest("Invalid model oject");    // 400 Bad request
            }

            id = id.ToUpper();
            c.CustomerID = c.CustomerID.ToUpper();

            if (c == null || c.CustomerID != id)
            {
                return BadRequest();    // 400 Bad request
            }

            if (ModelState.IsValid == false)
            {
                return BadRequest(ModelState);  // 400 Bad request
            }

            var existing = await _customerRepository.RetrieveAsync(id);

            if (existing == null)
            {
                return NotFound();  // 404 Resource not found
            }

            await _customerRepository.UpdateAsync(id, c);
            return new NoContentResult();   // 204 No content
        }
        
            
        // DELETE: api/customers/[id]
        [HttpDelete("{id}")]
        [ProducesResponseType(204)]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        public async Task<IActionResult> Delete(string id)
        {

            try
            {
                if (id == "bad")
                {
                    var problemDetails = new ProblemDetails
                    {
                        Status = StatusCodes.Status400BadRequest,
                        Type = "https://localhost:5001/customers/failed-to-delete",
                        Title = $"Customer ID {id} found but failed to delete",
                        Detail = "More details like Company name, Country and so on.",
                        Instance = HttpContext.Request.Path
                    };
                    return BadRequest(problemDetails);  // 400 Bad request
                }

                var existing = await _customerRepository.RetrieveAsync(id);

                if (existing == null)
                {
                    return NotFound();  // 404 Resource not found
                }

                bool? deleted = await _customerRepository.DeleteAsync(id);

                if (deleted.HasValue && deleted.Value)  // short circuit AND
                {
                    return new NoContentResult();   // 204 No content
                }
                else
                {
                    return BadRequest($"Customer {id} was found but failed to delete.");    // 400 Bad request
                }
            }
            catch (System.Exception ex)
            {
                return BadRequest($"Customer {id} failed to delete: {ex.Message}, {ex.InnerException.Message}");    // 400 Bad request
            }

        }

    }
}
