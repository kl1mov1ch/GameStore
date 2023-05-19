using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.TextFormatting;
using GameStore.DataBase;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;

namespace GameStore.Repository
{
    public class UsersRepository : IRepository<Users>
    {
        private MyDbContext db;

        public UsersRepository(MyDbContext context)
        {
            this.db = context;
        }


        public void Create(Users entity)
        {
          db.Users.Add(entity);
        }

        public void Delete(int id)
        {
            Users users = db.Users.Find(id);
            if (users != null)
            {
                db.Users.Remove(users);
            }
        }

        public Users Get(int id)
        {
            return db.Users.Find(id);
        }

        public IEnumerable<Users> GetAll()
        {
            return db.Users;
        }

        public void Update(Users entity)
        {
            db.Entry(entity).State = EntityState.Modified;
        }
    }
}
