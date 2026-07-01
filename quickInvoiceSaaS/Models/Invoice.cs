using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace QuickInvoiceSaaS.Models
{
    public class Invoice : IMustHaveTenant
    {
        [Key]
        public int Id { get; set; }

        // تطبيق الواجهة لفصل البيانات
       
        [Column("TenantId")]

        public string TenantId { get; set; } = string.Empty;

        [Required]
        [MaxLength(50)]
        public string InvoiceNumber { get; set; } = string.Empty; // مثل INV-2026-001

        [Required]
        [MaxLength(150)]
        public string ClientName { get; set; } = string.Empty;

        public string? ClientEmail { get; set; }

        public DateTime IssueDate { get; set; } = DateTime.UtcNow;

        [Column(TypeName = "decimal(18,2)")]
        public decimal TotalAmount { get; set; }

        [Required]
        public string Status { get; set; } = "Unpaid"; // Unpaid, Paid, Draft

        // علاقة One-to-Many مع تفاصيل الفاتورة
        public List<InvoiceItem> Items { get; set; } = new();
    }
}