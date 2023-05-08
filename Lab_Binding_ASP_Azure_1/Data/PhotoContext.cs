using Lab_Binding_ASP_Azure_1.Models;
using Microsoft.EntityFrameworkCore;

namespace Lab_Binding_ASP_Azure_1.Data
{
    public class PhotoContext : DbContext
    {
        public DbSet<Photo> Photos { get; set; } = default!;
        public PhotoContext(DbContextOptions<PhotoContext> options) : base(options) 
        { 
            Database.EnsureCreated();
        }
    }
}
