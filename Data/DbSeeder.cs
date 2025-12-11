using Microsoft.AspNetCore.Identity;
using MyGYM.Models;

namespace MyGYM.Data;

public class DbSeeder
{
    public static async Task SeedRolesAndAdminAsync(IServiceProvider service)
    {
        // Kullanıcı ve Rol Yöneticilerini çağırıyoruz
        var userManager = service.GetService<UserManager<IdentityUser>>();
        var roleManager = service.GetService<RoleManager<IdentityRole>>();

        // 1. ROLLERİ EKLE (Admin ve Member)
        await roleManager.CreateAsync(new IdentityRole("Admin"));
        await roleManager.CreateAsync(new IdentityRole("Member"));

        // 2. ADMİN KULLANICISI VAR MI KONTROL ET
        var adminUser = await userManager.FindByEmailAsync("ogrencinumarasi@sakarya.edu.tr");

        if (adminUser == null)
        {
            // Yoksa yeni oluştur
            var newAdmin = new IdentityUser
            {
                UserName = "g201210306@sakarya.edu.tr",
                Email = "g201210306@sakarya.edu.tr",
                EmailConfirmed = true
            };

            // Kullanıcıyı "sau" şifresiyle oluştur
            await userManager.CreateAsync(newAdmin, "sau");

            // Kullanıcıya "Admin" rolünü ver
            await userManager.AddToRoleAsync(newAdmin, "Admin");
        }
    }
}
