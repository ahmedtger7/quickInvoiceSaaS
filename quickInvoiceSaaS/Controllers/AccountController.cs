using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using QuickInvoiceSaaS.Models;

namespace QuickInvoiceSaaS.Controllers
{
    public class AccountController : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;

        public AccountController(UserManager<ApplicationUser> userManager, SignInManager<ApplicationUser> signInManager)
        {
            _userManager = userManager;
            _signInManager = signInManager;
        }

        // --- شاشة إنشاء حساب جديد ---
        [HttpGet]
        public IActionResult Register() => View();

        [HttpPost]
        public async Task<IActionResult> Register(RegisterViewModel model)
        {
            if (ModelState.IsValid)
            {
                var user = new ApplicationUser
                {
                    UserName = model.Email,
                    Email = model.Email,
                    TenantId = model.TenantId,// ربط المستخدم بالشركة هنا 🎯
                    SubscriptionPlan = model.SubscriptionPlan // 🔥 حفظ الباقة هنا
                };

                var result = await _userManager.CreateAsync(user, model.Password);
                if (result.Succeeded)
                {
                    await _signInManager.SignInAsync(user, isPersistent: false);
                    return RedirectToAction("Index", "InvoicesWeb");
                }

                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError("", error.Description);
                }
            }
            return View(model);
        }

        // --- شاشة تسجيل الدخول ---
        [HttpGet]
        public IActionResult Login() => View();

        [HttpPost]
        public async Task<IActionResult> Login(LoginViewModel model)
        {
            if (ModelState.IsValid)
            {
                var result = await _signInManager.PasswordSignInAsync(model.Email, model.Password, model.RememberMe, false);
                if (result.Succeeded)
                {
                    return RedirectToAction("Index", "InvoicesWeb");
                }
                ModelState.AddModelError("", "محاولة دخول غير صحيحة، تأكد من البيانات.");
            }
            return View(model);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UpgradeToPremium()
        {
            // 1. جلب المستخدم الحالي المسجل دخوله
            var user = await _userManager.GetUserAsync(User);
            if (user == null) return Challenge();

            // 2. تغيير خطته إلى الباقة الممتازة
            user.SubscriptionPlan = "Premium";

            // 3. حفظ التعديل في قاعدة البيانات
            var result = await _userManager.UpdateAsync(user);

            // 4. إعادة توجيهه إلى صفحة الفواتير مجدداً بعد الترقية
            return RedirectToAction("Index", "InvoicesWeb");
        }

        // --- تسجيل الخروج ---
        [HttpPost]
        public async Task<IActionResult> Logout()
        {
            await _signInManager.SignOutAsync();
            return RedirectToAction("Login", "Account");
        }
    }
}