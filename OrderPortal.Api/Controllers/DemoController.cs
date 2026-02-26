using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using OrderPortal.Api.Data;
using OrderPortal.Domain.Models;

namespace OrderPortal.Api.Controllers
{
    [ApiController]
    [Route("api/demo")]
    public class DemoController : ControllerBase
    {
        private readonly AppDbContext _db;

        public DemoController(AppDbContext db) => _db = db;

        [HttpPost("seed")]
        public async Task<IActionResult> Seed()
        {
            if (await _db.Customers.AnyAsync())
                return Ok("Already seeded.");

            var customer = new Customer { Name = "Acme Ltd" };

            var p1 = new Product { Name = "Keyboard", UnitPrice = 49.99m };
            var p2 = new Product { Name = "Mouse", UnitPrice = 19.99m };

            var invoice = new Invoice
            {
                Customer = customer,
                InvoiceDate = DateTime.UtcNow,
                Lines =
            {
                new InvoiceLine { Product = p1, Quantity = 2, UnitPrice = p1.UnitPrice },
                new InvoiceLine { Product = p2, Quantity = 1, UnitPrice = p2.UnitPrice }
            }
            };

            _db.Invoices.Add(invoice);
            await _db.SaveChangesAsync();

            return Ok("Seeded 1 customer, 2 products, 1 invoice, 2 lines.");
        }

        [HttpGet("invoices")]
        public async Task<IActionResult> GetInvoices()
        {
            var data = await _db.Invoices
                .Include(i => i.Customer)
                .Include(i => i.Lines)
                    .ThenInclude(l => l.Product)
                .Select(i => new
                {
                    i.Id,
                    i.InvoiceDate,
                    Customer = i.Customer!.Name,
                    Total = i.Lines.Sum(l => l.Quantity * l.UnitPrice),
                    Lines = i.Lines.Select(l => new
                    {
                        Product = l.Product!.Name,
                        l.Quantity,
                        l.UnitPrice,
                        LineTotal = l.Quantity * l.UnitPrice
                    })
                })
                .ToListAsync();

            return Ok(data);
        }
    }
}
