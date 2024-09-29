using E_commerce.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;

namespace E_commerce.Data
{
    public class EcommerceContext : DbContext
    {
        public DbSet<Product>Products { get; set; }
        public DbSet<User> Users { get; set; }
        public DbSet<Order> Orders { get; set; }
        public DbSet<LineItem> LineItems { get; set; }


        public EcommerceContext(DbContextOptions<EcommerceContext> options) : base(options) { }

        
    }
}
