using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using QuickInvoiceSaaS.Models;

namespace QuickInvoiceSaaS.Data
{
    // يرث من IdentityDbContext المخصص لـ ApplicationUser لإدارة هويات مستخدمي الشركات
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        // جدول الفواتير الرئيسي
        public DbSet<Invoice> Invoices { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // ضروري جداً لتأسيس جداول الـ Identity (المستخدمين، الأدوار، الصلاحيات)
            base.OnModelCreating(modelBuilder);

            // تأكيد ربط اسم العمود بالكابيتال وتثبيته في قاعدة البيانات لمنع أي تعارض
            modelBuilder.Entity<Invoice>(entity =>
            {
                entity.ToTable("Invoices");
                entity.Property(e => e.TenantId)
                    .HasColumnName("TenantId")
                    .IsRequired(); // يمكن إلغاؤها إذا كنت تسمح بفواتير بدون شركة
            });

            modelBuilder.Entity<ApplicationUser>(entity =>
            {
                entity.Property(e => e.TenantId)
                    .HasColumnName("TenantId")
                    .IsRequired(false); // نجعله اختياري للمشرفين العامين (Admin) مثلاً
            });
        }
    }
}