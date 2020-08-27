using GeneralStoreAPI.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;

namespace GeneralStoreAPI.Controllers
{
    public class CustomerController : ApiController
    {
        //Crud
        private readonly ApplicationDbContext _context = new ApplicationDbContext();

        //Create New Customers(POST)
        [HttpPost]
        public async Task<IHttpActionResult> CreateCustomer(Customer model)
        {
            if (model is null)
                return BadRequest("Your request cannot be empty");

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            _context.Customers.Add(model);
            await _context.SaveChangesAsync();
            return Ok("Customer Added");
        }

        //All Customers(GET)
        [HttpGet]
        public async Task<IHttpActionResult> GetAllCustomers()
        {
            List<Customer> customers = await _context.Customers.ToListAsync();
            return Ok(customers);
        }
        //Single Customer(GET)
        [HttpGet]
        public async Task<IHttpActionResult> GetCustomerById(int id)
        {
            Customer customer = await _context.Customers.FindAsync(id);
            if (customer != null)
                return Ok(customer);
            return NotFound();
        }

        //Update existing customer By ID(Put)
        [HttpPut]
        public async Task<IHttpActionResult> UpdateCustomerById([FromUri]int id,[FromBody] Customer updatedCustomer)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            Customer customer = await _context.Customers.FindAsync(id);
            if (customer is null)
                return NotFound();

            customer.FirstName = updatedCustomer.FirstName;
            customer.LastName = updatedCustomer.LastName;

            await _context.SaveChangesAsync();
            return Ok($"Customer {id} updated");
        }

        //Delete Customer By ID(DELETE)
        [HttpDelete]
        public async Task<IHttpActionResult> DeleteCustomerById(int id)
        {
            Customer entity = await _context.Customers.FindAsync(id);

            if (entity is null)
                return NotFound();

            _context.Customers.Remove(entity);

            if (await _context.SaveChangesAsync() == 1)
                return Ok($"Customer {id} Deleted");

            return InternalServerError();
        }
    }
}
