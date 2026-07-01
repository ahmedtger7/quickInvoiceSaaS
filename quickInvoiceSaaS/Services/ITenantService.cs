namespace QuickInvoiceSaaS.Services
{
    public interface ITenantService
    {
        string GetTenantId();
    }

    public class TenantService : ITenantService
    {
        private readonly IHttpContextAccessor _httpContextAccessor;

        public TenantService(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
        }

        public string GetTenantId()
        {
            // كبداية سنأتي بالـ TenantId من الـ Claims الخاصة بالمستخدم المسجل
            // أو من الـ Headers الخاصة بالطلب (مثلاً تضع Header باسم X-Tenant-Id)
            var httpContext = _httpContextAccessor?.HttpContext;
            if (httpContext != null)
            {
                // البحث في الـ Claims (إذا كان العميل مسجل دخول عبر Identity)
                var tenantClaim = httpContext.User?.FindFirst("TenantId")?.Value;
                if (!string.IsNullOrEmpty(tenantClaim)) return tenantClaim;

                // كخيار بديل مؤقت للاختبار عبر الـ Header
                if (httpContext.Request.Headers.TryGetValue("X-Tenant-Id", out var tenantId))
                {
                    return tenantId.ToString();
                }
            }

            return string.Empty;
        }
    }
}