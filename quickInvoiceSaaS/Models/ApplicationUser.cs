using Microsoft.AspNetCore.Identity;

namespace QuickInvoiceSaaS.Models
{
    public class ApplicationUser : IdentityUser
    {
        // إضافة حقل الشركة ليكون جزءاً أساسياً من هويّة المستخدم
        public string TenantId { get; set; } = string.Empty;
        public string SubscriptionPlan { get; set; } = "Free";
    }
}