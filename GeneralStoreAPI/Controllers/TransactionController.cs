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
    public class TransactionController : ApiController
    {
        ApplicationDbContext _context = new ApplicationDbContext();

        //Create(POST)
        [HttpPost]
        public async Task<IHttpActionResult> CreateTransaction(Transaction model)
        {
            //validate request
            if (model is null)
                return BadRequest("Request cannot be empty");
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            //Make sure customer and products exist.
            var customer = await _context.Customers.FindAsync(model.CustomerId);
            if (customer is null)
                return BadRequest($"No customer found with ID of {model.CustomerId}");
            var product = await _context.Products.FindAsync(model.ProductId);
            if (product is null)
                return BadRequest($"No Product Found with SKU{model.ProductId}");

            if (!product.IsInStock)
                return BadRequest("Item Out Of Stock");

            if (product.NumberInInventory < model.ItemCount)
                return BadRequest($"Not enough inventory in stock only {product.NumberInInventory} available");

            product.NumberInInventory -= model.ItemCount;
            _context.Transactions.Add(model);
            await _context.SaveChangesAsync();
            return Ok();
        }

        //Get All Transactions(GET)
        [HttpGet]
        public async Task<IHttpActionResult> GetAllTransactions()
        {
            return Ok(await _context.Transactions.ToListAsync());
        }

        //Get Transaction By Id
        [HttpGet]
        public async Task<IHttpActionResult> GetTransactionById(int id)
        {
            Transaction transaction = await _context.Transactions.FindAsync(id);
            if (transaction != null)
                return Ok(transaction);
            return NotFound();
        }

        // Get All Transactions By CustomerId
        [HttpGet]
        public async Task<IHttpActionResult> GetAllTransactionsByCustomerId(int CustomerId)
        {
            List<Transaction> transactions = new List<Transaction>();
            foreach (var t in await _context.Transactions.ToListAsync())
                if (t.CustomerId == CustomerId)
                    transactions.Add(t);
            return Ok(transactions);
        }

        // Update existing transaction by its ID (PUT)
        [HttpPut]
        public async Task<IHttpActionResult> UpdateTransactionById(int id, Transaction updatedTransaction)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);
            Transaction transaction = await _context.Transactions.FindAsync(id);
            var product = await _context.Products.FindAsync(transaction.ProductId);
            if (transaction == null)
                return NotFound();

            if (updatedTransaction.ItemCount - transaction.ItemCount > product.NumberInInventory)
                return BadRequest("Not Enough Product in Inventory");

            product.NumberInInventory -= (updatedTransaction.ItemCount - transaction.ItemCount);
            transaction.ItemCount = updatedTransaction.ItemCount;
            transaction.DateOfTransaction = updatedTransaction.DateOfTransaction;
            await _context.SaveChangesAsync();
            return Ok("Transaction Updated");
        }

        //Delete Existing Transaction By ID(DELETE)
        public async Task<IHttpActionResult> DeleteTransactionById(int id)
        {
            var entity = await _context.Transactions.FindAsync(id);
            if (entity is null)
                return NotFound();

            var product = await _context.Products.FindAsync(entity.ProductId);
            product.NumberInInventory += entity.ItemCount;
            _context.Transactions.Remove(entity);
            await _context.SaveChangesAsync();
            return Ok("Transaction Deleted");
        }
    }
}
