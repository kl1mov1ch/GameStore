using Azure;
using GameStore.Utilites;
using GameStore.Commands;
using GameStore.DataBase;
using GameStore.UnitOfWork;
using Microsoft.Win32;
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
using System.Diagnostics;

namespace GameStore.Pages
{
    /// <summary>
    /// Логика взаимодействия для AddingWindowView.xaml
    /// </summary>
    public partial class AddingWindowView : Window
    {
        private AdminMainWindowView owner;

        public AddingWindowView(AdminMainWindowView owner)
        {
            this.owner = owner;

            DataContext = this;
            InitializeComponent();
        }

        private void CloseWindowExecute(object sender, MouseButtonEventArgs e)
        {
            this.Close();
        }

        private void BorderWindowStyle_MouseDown(object sender, MouseButtonEventArgs e) => this.DragMove();

        #region AddImage
        private DelegateCommand? AddImageURLCommand;

        public ICommand AddImageURL
        {
            get
            {
                if (AddImageURLCommand == null)
                {
                    AddImageURLCommand = new DelegateCommand(AddImageURLExecute);
                }
                return AddImageURLCommand;
            }
        }

        private void AddImageURLExecute()
        {
            try
            {
                OpenFileDialog openFileDialog = new OpenFileDialog();
                openFileDialog.Filter = "Image files (*.png, *.jpg)|*.png;*.jpg";
                Nullable<bool> result = openFileDialog.ShowDialog();

                if (result == true)
                {
                    InputImageUrl.Text = ImageManager.SaveImage(openFileDialog.FileName);
                }

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
        #endregion

        #region AddGame

        UnitOfWork.UnitOfWork db =  new UnitOfWork.UnitOfWork();

        private DelegateCommand? AddGameCommand;

        public ICommand AddGame
        {
            get
            {
                if (AddGameCommand == null)
                {
                    AddGameCommand = new DelegateCommand(AddGameExecute);
                }
                return AddGameCommand;
            }
        }

        private void AddGameExecute()
        {

            Products products = new Products();
            try
            {
                products.Name = NameInput.Text.Trim();
                products.Description = DescriptionInput.Text.Trim();
                products.Image = InputImageUrl.Text.Trim();
                string valet = " $";
                if (PriceInput.Text.Trim() != "")
                {
                    products.Price = Convert.ToDecimal(PriceInput.Text);
                }
                else
                    throw new Exception("Некорректная цена");

                if (CheckGame(products))
                {
                    db.Produsct.Create(products);
                    db.Save();
                    owner.ItemBox.Items.Refresh();
                    owner.SyncData();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private bool CheckGame
            (Products products)
        {
            try
            {
                if (String.IsNullOrEmpty(products.Name))
                    throw new Exception("Имя игры не узазано!");
                if (String.IsNullOrEmpty(products.Description))
                    throw new Exception("Описание игры не указано!");
                if (!String.IsNullOrEmpty(products.Image))
                {
                    //if (!File.Exists(products.Image))
                    //{
                    //    throw new Exception("Файл " + products.Image + " не существует!");
                    //}  
                }
                if(products.Price < 0)
                    throw new Exception("Цена меньше нуля");
                this.Close();
                return true;
            }
            catch (IOException ex)
            {
                MessageBox.Show("Файл " + GetFileNameFromMessage(ex.Message) + " уже используется другой книгой!");
                return false;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
                return false;
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


        #endregion

    }
}
