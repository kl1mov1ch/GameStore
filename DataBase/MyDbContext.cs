using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace GameStore.DataBase
{
    public class MyDbContext : DbContext
    {

        public virtual DbSet<Orders> Orders { get; set; } = null!;
        public virtual DbSet<Users> Users { get; set; } = null!;
        public virtual DbSet<Products> Products { get; set; } = null!;
        public virtual DbSet<Basket> Baskets { get; set; } = null!;

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                optionsBuilder.UseSqlServer(@"Server=ANTON\SQLEXPRESS;Database=GameStore;Trusted_Connection=True; TrustServerCertificate = True");
            }
        }

    }
}
