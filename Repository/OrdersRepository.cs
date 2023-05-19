using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.NetworkInformation;
using System.Text;
using System.Threading.Tasks;
using GameStore.DataBase;
using Microsoft.EntityFrameworkCore;

namespace GameStore.Repository
{
    public class OrdersRepository : IRepository<Orders>
    {
        private MyDbContext db;

        public OrdersRepository(MyDbContext context)
        {
            this.db = context;
        }

        public void Create(Orders entity)
        {
            db.Orders.Add(entity);
        }

        public void Delete(int id)
        {
            Orders orders = db.Orders.Find(id);
            if (orders != null)
            {
                db.Orders.Remove(orders);
            }
        }

        public Orders Get(int id)
        {
            return db.Orders.Find(id);
        }

        public IEnumerable<Orders> GetAll()
        {
            return db.Orders;
        }

        public void Update(Orders entity)
        {
            db.Entry(entity).State = EntityState.Modified;
        }
    }
}
