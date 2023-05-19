using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.RightsManagement;
using System.Text;
using System.Threading.Tasks;

namespace GameStore.DataBase
{
    public class Orders
    {
        public int Id { get; set; }
        public int Product_Id { get; set; }
        public int User_Id { get; set; }
        public DateTime Order_Date { get; set; }
        public decimal Total_Price { get; set; }
        public Products Products { get; set; }
        public Users Users { get; set; }
        public Basket Basket { get; set; }
    }
}
