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
    public class ProductController : ApiController
    {
        ApplicationDbContext _context = new ApplicationDbContext();

        //Create(POST)
        [HttpPost]
        public async Task<IHttpActionResult> AddNewProduct(Product model)
        {
            if (model is null)
                return BadRequest("Your request cannot be empty");

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            _context.Products.Add(model);

            if (await _context.SaveChangesAsync() == 1)
                return Ok("Product Added");
            return InternalServerError();
        }

        //Get All Products(GET)
        [HttpGet]
        public async Task<IHttpActionResult> GetAllProducts()
        {
            return Ok(await _context.Products.ToListAsync());
        }

        //Get Product By Id(GET)
        [HttpGet]
        public async Task<IHttpActionResult> GetProductById(string id)
        {
            Product product = await _context.Products.FindAsync(id);

            if (product is null)
                return NotFound();
            return Ok(product);
        }

        //Update an existing object by ID(PUT)
        [HttpPut]
        public async Task<IHttpActionResult> UpdateProductById([FromUri] string id, [FromBody]Product updatedProduct)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);
            if (updatedProduct is null)
                return BadRequest("Request Cannot Be Empty");

            Product product = await _context.Products.FindAsync(id);
            if (product is null)
                return NotFound();
            product.Cost = updatedProduct.Cost;
            product.Name = updatedProduct.Name;
            product.NumberInInventory = updatedProduct.NumberInInventory;

            if (await _context.SaveChangesAsync() == 1)
                return Ok($"Product {product.SKU} Updated");
            return InternalServerError();
        }
        //Delete Product By Id(DELETE)
        [HttpDelete]
        public async Task<IHttpActionResult> DeleteProductById(string id)
        {
            Product entity = await _context.Products.FindAsync(id);
            if (entity is null)
                return NotFound();

            _context.Products.Remove(entity);
            if (await _context.SaveChangesAsync() == 1)
                return Ok("Product Deleted");
            return InternalServerError();
        }
    }
}
