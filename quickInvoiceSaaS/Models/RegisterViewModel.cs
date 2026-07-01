namespace QuickInvoiceSaaS.Models
{
    public class RegisterViewModel
    {
        public string Email { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
        public string TenantId { get; set; } = string.Empty; // اسم الشركة المسجل عليها
        public string SubscriptionPlan { get; set; } = "Free";
    
}
}