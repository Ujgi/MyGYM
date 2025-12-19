using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using MyGYM.Data;
AppContext.SetSwitch("Npgsql.EnableLegacyTimestampBehavior", true);
var builder = WebApplication.CreateBuilder(args);

builder.Services.AddRazorPages();

var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");

builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseNpgsql(connectionString)); // PostgreSQL kullandığımız için UseNpgsql

// Identity (Kullanıcı Girişi) Ayarları
// Identity nin varsayılan ayarlarının değiştirilmesi gerekti şifreyi sau yapabilmek için
builder.Services.AddDefaultIdentity<IdentityUser>(options =>
    {
        options.SignIn.RequireConfirmedAccount = false; // Mail onayı isteme
        options.Password.RequireDigit = false; // Sayı zorunluluğu yok
        options.Password.RequiredLength = 3; // En az 3 karakter (sau için)
        options.Password.RequireLowercase = false;
        options.Password.RequireNonAlphanumeric = false; // Sembol (!, *) yok
        options.Password.RequireUppercase = false; // Büyük harf yok
    })
    .AddRoles<IdentityRole>()
    .AddEntityFrameworkStores<ApplicationDbContext>();

// Add services to the container.
builder.Services.AddControllersWithViews();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.MapRazorPages();

// --- SEED KODU BAŞLANGICI ---
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    // DbSeeder sınıfındaki metodu çalıştır
    await DbSeeder.SeedRolesAndAdminAsync(services);
}

app.Run();