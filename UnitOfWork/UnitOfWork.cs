using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GameStore.DataBase;
using GameStore.Repository;


namespace GameStore.UnitOfWork
{
    public class UnitOfWork : IDisposable
    {

        private MyDbContext db = new();
        private OrdersRepository OrdersRepository;
        private ProductsRepository productsRepository;
        private UsersRepository usersRepository;
        private BasketRepository basketRepository;

        public UsersRepository User
        { 
            get 
            {
                if (usersRepository == null)
                {
                    usersRepository = new(db);
                }
                return usersRepository;
            }
        }

        public ProductsRepository Produsct
        {
            get
            {
                if (productsRepository == null)
                {
                    productsRepository = new(db);
                }
                return productsRepository;
            }
        }

        public OrdersRepository Order
        {
            get
            {
                if (OrdersRepository == null)
                {
                    OrdersRepository = new(db);
                }
                return OrdersRepository;
            }
        }

        public BasketRepository Basket
        {
            get
            {
                if (basketRepository == null)
                {
                    basketRepository = new(db);
                }
                return basketRepository;
            }
        }

        public void Save()
        {
            db.SaveChanges();
        }
        public void Dispose()
        {
            db.Dispose();
        }
    }
}
