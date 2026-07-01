using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using QuickInvoiceSaaS.Data;
using QuickInvoiceSaaS.Models;

namespace QuickInvoiceSaaS.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class InvoicesController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public InvoicesController(ApplicationDbContext context)
        {
            _context = context;
        }

        // 1. دالة جلب الفواتير (ستجلب تلقائياً فواتير الشركة الحالية فقط بفضل الـ Filter)
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var invoices = await _context.Invoices.Include(i => i.Items).ToListAsync();
            return Ok(invoices);
        }

        // 2. دالة إنشاء فاتورة جديدة (سيتم حقن الـ TenantId تلقائياً قبل الحفظ)
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] Invoice invoice)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            _context.Invoices.Add(invoice);
            await _context.SaveChangesAsync();

            return Ok(new { message = "تم إنشاء الفاتورة بنجاح!", invoiceId = invoice.Id });
        }
    }
}