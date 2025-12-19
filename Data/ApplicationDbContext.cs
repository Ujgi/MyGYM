using MyGYM.Models;

namespace MyGYM.Data;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

public class ApplicationDbContext:IdentityDbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options)
    {
    }

    // Eklediğimiz tablolar
    public DbSet<Antrenor> Antrenors { get; set; }
    public DbSet<SporBrans> SporBranslar { get; set; }

    public DbSet<Appointment> Appointments { get; set; }

}