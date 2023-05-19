using GameStore.DataBase;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameStore.Repository
{
    public class BasketRepository : IRepository<Basket>
    {
        private MyDbContext db;

        public BasketRepository(MyDbContext context)
        {
            this.db = context;
        }

        public void Create(Basket entity)
        {
            db.Baskets.Add(entity);
        }

        public void Delete(int id)
        {
            Basket basket = db.Baskets.Find(id);
            if (basket != null)
            {
                db.Baskets.Remove(basket);
            }
        }

        public Basket Get(int id)
        {
            return db.Baskets.Find(id);
        }

        public IEnumerable<Basket> GetAll()
        {
            return db.Baskets;
        }

        public void Update(Basket entity)
        {
            db.Entry(entity).State = EntityState.Modified;
        }
    }
}
