using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using QuickInvoiceSaaS.Data;
using QuickInvoiceSaaS.Models;

namespace QuickInvoiceSaaS.Controllers
{
    [Authorize] // 🔥 حماية الـ Controller بالكامل (لن يدخل أحد إلا بتسجيل دخول)
    public class InvoicesWebController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        public InvoicesWebController(ApplicationDbContext context, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        // --- 1. عرض فواتير الشركة الخاصة بالمستخدم فقط ---
        public async Task<IActionResult> Index()
        {
            // جلب بيانات المستخدم الحالي المسجل دخوله
            var user = await _userManager.GetUserAsync(User);
            if (user == null) return Challenge();

            var tenantId = user.TenantId;
            ViewData["CurrentTenant"] = tenantId;

            // عزل حقيقي: جلب الفواتير التي تطابق TenantId الخاص بالمستخدم فقط!
            var invoices = await _context.Invoices
                .IgnoreQueryFilters()
                .Where(i => i.TenantId == tenantId)
                .ToListAsync();

            // حساب الإحصائيات للواجهة بناءً على بيانات الشركة الحالية
            ViewData["TotalInvoicesCount"] = invoices.Count;
            ViewData["UniqueClientsCount"] = invoices.Select(i => i.ClientName).Distinct().Count();

            return View(invoices);
        }
        // --- 4. صفحة تعديل الفاتورة (HttpGet) ---
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();

            // جلب المستخدم الحالي لمعرفة شركته
            var user = await _userManager.GetUserAsync(User);
            if (user == null) return Challenge();

            // جلب الفاتورة بشرط أن تكون تابعة لنفس الـ TenantId لحماية البيانات 🔐
            var invoice = await _context.Invoices
                .IgnoreQueryFilters()
                .FirstOrDefaultAsync(i => i.Id == id && i.TenantId == user.TenantId);

            if (invoice == null) return Forbid(); // منع الوصول إذا كانت الفاتورة لشركة أخرى

            return View(invoice);
        }

        // --- 5. حفظ تعديل الفاتورة (HttpPost) ---
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Invoice invoice)
        {
            if (id != invoice.Id) return NotFound();

            var user = await _userManager.GetUserAsync(User);
            if (user == null) return Challenge();

            // التأكد قبل الحفظ أن الفاتورة الأصلية تابعة للمستخدم الحالي
            var existingInvoice = await _context.Invoices
                .IgnoreQueryFilters()
                .AsNoTracking()
                .FirstOrDefaultAsync(i => i.Id == id && i.TenantId == user.TenantId);

            if (existingInvoice == null) return Forbid();

            if (ModelState.IsValid)
            {
                try
                {
                    invoice.TenantId = user.TenantId; // إعادة حقن الـ TenantId لضمان عدم التلاعب
                    _context.Update(invoice);
                    await _context.SaveChangesAsync();
                    return RedirectToAction(nameof(Index));
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!_context.Invoices.Any(e => e.Id == invoice.Id)) return NotFound();
                    else throw;
                }
            }
            return View(invoice);
        }

        // --- 6. حذف الفاتورة مباشرة (HttpPost لحماية أمنية أفضل) ---
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null) return Challenge();

            // جلب الفاتورة الخاصة بالشركة فقط لحذفها
            var invoice = await _context.Invoices
                .IgnoreQueryFilters()
                .FirstOrDefaultAsync(i => i.Id == id && i.TenantId == user.TenantId);

            if (invoice == null) return Forbid();

            _context.Invoices.Remove(invoice);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        // --- 2. صفحة إنشاء فاتورة جديدة (HttpGet) ---
        public IActionResult Create() => View();

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Invoice invoice)
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null) return Challenge();

            // 🔒 فحص خطة الاشتراك والحد الأقصى للفواتير
            if (user.SubscriptionPlan == "Free")
            {
                // حساب عدد فواتير هذه الشركة حالياً في قاعدة البيانات
                var currentInvoicesCount = await _context.Invoices
                    .IgnoreQueryFilters()
                    .CountAsync(i => i.TenantId == user.TenantId);

                if (currentInvoicesCount >= 3)
                {
                    // إظهار رسالة خطأ تمنع الحفظ وتطلب الترقية
                    ModelState.AddModelError("", "⚠️ لقد استهلكت الحد الأقصى للباقة المجانية (3 فواتير). يرجى الترقية إلى الباقة الممتازة لإضافة المزيد!");
                    return View(invoice);
                }
            }

            invoice.TenantId = user.TenantId;

            try
            {
                _context.Invoices.Add(invoice);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", "حدث خطأ أثناء الحفظ: " + ex.Message);
            }

            return View(invoice);
        }
    }
}