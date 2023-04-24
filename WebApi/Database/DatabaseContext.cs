using Microsoft.EntityFrameworkCore;
using System.Xml;

namespace WebApi.Database
{
    public class DatabaseContext : DbContext
    {
        public DbSet<EmployeeEntity> Employees { get; set; }
        public DbSet<OrderEntity> Orders { get; set; }


        protected override void OnConfiguring(DbContextOptionsBuilder options)
            => options.UseSqlServer("Server=DESKTOP-SDVVQNB\\SQLEXPRESS;Initial Catalog=Spedition;Integrated Security=SSPI;TrustServerCertificate=true;");
    }
}

