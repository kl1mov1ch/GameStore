using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace GameStore.Utilites
{
    public static class ImageManager
    {
        static ImageManager()
        {
            if (!Directory.Exists("Images"))
                Directory.CreateDirectory("Images");
        }

        public static string SaveImage(string path)
        {
            string name = Path.GetFileName(path);

            if (File.Exists(Path.Combine("Images", name)))
                return name;

            File.Copy(path, Path.Combine("Images", name));

            return name;
        }

        public static ImageSource? LoadImage(string name)
        {
            string exePath = System.Reflection.Assembly.GetExecutingAssembly().Location;
            string imageUri = Path.Combine(Path.GetDirectoryName(exePath), "Images", name);

            if (!File.Exists(imageUri))
                return null;

            BitmapImage image = new BitmapImage();

            image.BeginInit();
            image.UriSource = new Uri(imageUri, UriKind.Absolute);
            image.EndInit();

            return image;
        }

        public static ImageSource GetNoImage()
        {
            BitmapImage image = new BitmapImage();

            image.BeginInit();
            image.UriSource = new Uri("/Source/ClosePageIconDark.png", UriKind.Relative);
            image.EndInit();

            return image;
        }
    }
}
