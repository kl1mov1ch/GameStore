using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.RightsManagement;
using System.Text;
using System.Threading.Tasks;

namespace GameStore.DataBase
{
    public class Basket
    {
        public int Id { get; set; }
        public int User_Id { get; set; }
        public int Product_Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public decimal Price { get; set; }
        public string Image { get; set; }
    }
}
