using Microsoft.EntityFrameworkCore;

using System.Collections.Generic;
using System.Diagnostics;
using System.Net.NetworkInformation;
using System.Reflection.Emit;
using System.Xml.Linq;
using Cofefe.Models;
using Microsoft.EntityFrameworkCore.Infrastructure;
namespace Cofefe
{
    public class ApplicationContext : DbContext
    {
        private readonly string _connection = "Data Source=DESKTOP-3FU748J;Database=DiplomCofefe;Integrated Security = sspi; Encrypt=False;";

        //private readonly string _connection = "Data Source=192.168.221.12;User ID = user04;Password=04;Database=DiplomCofefe 2;TrustServerCertificate=true";

        public DbSet<Order> Orders { get; set; }
        public DbSet<User> Users { get; set; }
        public DbSet<Product> Products { get; set; }
        public DbSet<ShoppingCart> ShoppingCarts { get; set; }
        public DbSet<FavoriteProduct> FavoriteProducts { get; set; }
        public DbSet<Comment> Comments { get; set; }
        public DbSet<Grade> Grades { get; set; }
        public DbSet<Status> Statuses { get; set; }
        public DbSet<Category> Categoryes { get; set; }
        public DbSet<CategoryProduct> CategoryProducts { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlServer(_connection);
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
        }
        public ApplicationContext()
        {
            //Database.EnsureCreated();
        }
        public ApplicationContext(DbContextOptions<ApplicationContext> options)
        : base(options)
        {
        }
    }
}
