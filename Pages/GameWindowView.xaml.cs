using GameStore.DataBase;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace GameStore.Pages
{
    /// <summary>
    /// Логика взаимодействия для GameWindowView.xaml
    /// </summary>
    public partial class GameWindowView : Window
    {
        public GameWindowView(Products products)
        {
            InitializeComponent();
            this.DataContext = this;
            Image.Source = InsertImage(products.Image);
            Description.Text = products.Description;
            Price.Text = products.Price.ToString() + " $";
            Name.Text = products.Name;   
        }

            private BitmapImage InsertImage(string URL)
        {
            try
            {
                byte[] buffer = File.ReadAllBytes(URL);
                using (MemoryStream stream = new MemoryStream(buffer))
                {
                    BitmapImage image = new BitmapImage();
                    image.BeginInit();
                    image.StreamSource = stream;
                    image.CacheOption = BitmapCacheOption.OnLoad;
                    image.EndInit();
                    return image;
                }
            }
            catch (FileNotFoundException ex)
            {
                MessageBox.Show("Файла " + GetFileNameFromMessage(ex.Message) + " не существует!");
                return null;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
                return null;
            }
        }
        private string GetFileNameFromMessage(string message)
        {
            int start = message.IndexOf("'") + 1;
            int end = message.IndexOf("'", start);
            if (start >= 0 && end >= 0)
            {
                return message.Substring(start, end - start);
            }
            return null;
        }
        private void ToolBar_MouseDown(object sender, MouseButtonEventArgs e) => this.DragMove();

        private void CloseWindowExecute(object sender, MouseButtonEventArgs e)
        {
            this.Close();
        }

        private void MinimizeWindowExecute(object sender, MouseButtonEventArgs e)
        {
            this.WindowState = WindowState.Minimized;
        }


    }
}
