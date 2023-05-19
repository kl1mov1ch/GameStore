using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.TextFormatting;
using GameStore.DataBase;
using Microsoft.EntityFrameworkCore;

namespace GameStore.Repository
{
    public class ProductsRepository : IRepository<Products>
    {
        private MyDbContext db;
        public ProductsRepository(MyDbContext context) 
        { 
            this.db = context;
        }
        public void Create(Products entity)
        {
            db.Products.Add(entity);
        }

        public void Delete(int id)
        {
            Products products = db.Products.Find(id);
            if (products != null)
            {
                db.Products.Remove(products);
            }
        }

        public Products Get(int id)
        {
            return db.Products.Find(id);
        }

        public IEnumerable<Products> GetAll()
        {
            return db.Products;
        }

        public void Update(Products entity)
        {
                Products oldEntiry = db.Products.Find(entity.Id);
            if(oldEntiry != null)
            {
                db.Products.Remove(oldEntiry);
            };
                db.Products.Add(entity);
                db.SaveChanges();
        }
    }
}
