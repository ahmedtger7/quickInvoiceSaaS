using System.ComponentModel.DataAnnotations;

namespace QuickInvoiceSaaS.Models
{
    public class Tenant
    {
        [Key]
        public string Id { get; set; } = Guid.NewGuid().ToString(); // معرف فريد لكل شركة مشتركة

        [Required]
        [MaxLength(150)]
        public string BusinessName { get; set; } = string.Empty;

        public string? LogoUrl { get; set; }

        [Required]
        [MaxLength(10)]
        public string Currency { get; set; } = "USD"; // العملة الافتراضية للفواتير

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}