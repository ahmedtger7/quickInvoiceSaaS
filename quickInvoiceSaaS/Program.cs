
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using QuickInvoiceSaaS.Data;
using QuickInvoiceSaaS.Models;
using QuickInvoiceSaaS.Services;

var builder = WebApplication.CreateBuilder(args);

// ==========================================
// 1. تسجيل الخدمات (Configure Services)
// ==========================================

// تفعيل الوصول إلى بيانات الـ HTTP Request (مثل الـ Headers والـ Claims)
builder.Services.AddHttpContextAccessor();

// تسجيل خدمة جلب الـ Tenant الحالي وتحديد عمرها بـ Scoped (لكل طلب مستقل)
builder.Services.AddScoped<ITenantService, TenantService>();

// تسجيل الـ DbContext وربطه بـ SQL Server ونص الاتصال من الـ appsettings.json
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// تسجيل خدمات الـ Controllers ودعم الـ Swagger لتجربة الـ APIs
builder.Services.AddControllers();
builder.Services.AddControllersWithViews();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new Microsoft.OpenApi.Models.OpenApiInfo { Title = "QuickInvoice SaaS API", Version = "v1" });

    options.AddSecurityDefinition("TenantId", new Microsoft.OpenApi.Models.OpenApiSecurityScheme
    {
        In = Microsoft.OpenApi.Models.ParameterLocation.Header,
        Name = "X-Tenant-Id", // اسم الهيدر الذي يبحث عنه الكود لديك
        Type = Microsoft.OpenApi.Models.SecuritySchemeType.ApiKey,
        Description = "الرجاء إدخال معرف الشركة هنا (مثل: Company-A)"
    });

    options.AddSecurityRequirement(new Microsoft.OpenApi.Models.OpenApiSecurityRequirement
    {
        {
            new Microsoft.OpenApi.Models.OpenApiSecurityScheme
            {
                Reference = new Microsoft.OpenApi.Models.OpenApiReference
                {
                    Type = Microsoft.OpenApi.Models.ReferenceType.SecurityScheme,
                    Id = "TenantId"
                }
            },
            new string[] {}
        }
    });
});
// إضافة خدمات نظام الهوية المخصص للمستخدمين والشركات
builder.Services.AddIdentity<ApplicationUser, IdentityRole>(options =>
{
    // إعدادات اختيارية لكلمة المرور (يمكنك تعديلها حسب رغبتك)
    options.Password.RequireDigit = false;
    options.Password.RequiredLength = 6;
    options.Password.RequireNonAlphanumeric = false;
    options.Password.RequireUppercase = false;
    options.Password.RequireLowercase = false;
})
.AddEntityFrameworkStores<ApplicationDbContext>()
.AddDefaultTokenProviders();

// ضبط مسارات الـ Cookie في حال حاول مستخدم غير مسجل الدخول دخول صفحات محمية
builder.Services.ConfigureApplicationCookie(options =>
{
    options.LoginPath = "/Account/Login";
    options.AccessDeniedPath = "/Account/AccessDenied";
});

var app = builder.Build();

// ==========================================
// 2. خط أنابيب الطلبات (Configure HTTP Pipeline)
// ==========================================

// تفعيل واجهة Swagger للاختبار في بيئة التطوير (Development)
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

// ربط مسارات الـ Controllers بالـ Routing الأساسي للتطبيق
app.MapControllers();
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=InvoicesWeb}/{action=Index}/{id?}");
app.UseRouting();

// 🔥 السطرين الأهم للتأكد من فحص هوية وصلاحيات المستخدمين
app.UseAuthentication();
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=InvoicesWeb}/{action=Index}/{id?}");

// تشغيل التطبيق
app.Run();
