using GameStore.DataBase;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;

namespace GameStore.Utilites
{
    public class ProductVM
    {
        private Products products;

        private object? owner;

        public int Id { get => products.Id; }
        public string Name { get => products.Name; }
        public string Desctiption { get => products.Description; }
        public string Price { get => products.Price.ToString() + " $"; }

        public ImageSource Image
        {
            get
            {
                return ImageManager.LoadImage(products.Image) ?? ImageManager.GetNoImage();
            }
        }

        public ProductVM(Products products, object? owner = null)
        {
            this.products = products;
            this.owner = owner;
        }




    }
}
