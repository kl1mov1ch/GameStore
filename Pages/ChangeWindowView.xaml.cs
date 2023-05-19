using GameStore.Commands;
using GameStore.DataBase;
using GameStore.Utilites;
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

namespace GameStore.Pages
{
    /// <summary>
    /// Логика взаимодействия для ChangeWindowView.xaml
    /// </summary>
    public partial class ChangeWindowView : Window
    {

        private AdminMainWindowView owner;
        private int CurrentId = 0;
        public ChangeWindowView(Products products, AdminMainWindowView owner)
        {
            InitializeComponent();
            this.owner = owner;
            DataContext = this;
            DescriptionInput.Text = products.Description;
            PriceInput.Text = products.Price.ToString();
            InputImageUrl.Text = products.Image;
            NameInput.Text = products.Name;
            CurrentId = products.Id;

        }
        private void CloseWindowExecute(object sender, MouseButtonEventArgs e)
        {
            this.Close();
        }
        private void BorderWindowStyle_MouseDown(object sender, MouseButtonEventArgs e) => this.DragMove();

        #region UpdateGame

        UnitOfWork.UnitOfWork db = new UnitOfWork.UnitOfWork();

        private DelegateCommand? addGameCommand;

        public ICommand AddGame
        {
            get
            {
                if (addGameCommand == null)
                {
                    addGameCommand = new DelegateCommand(AddGameExecute);
                }
                return addGameCommand;
            }
        }

        private void AddGameExecute()
        {
            try
            {
                Products products = new Products
                {
                    Name = NameInput.Text.Trim(),
                    Description = DescriptionInput.Text.Trim(),
                    Image = InputImageUrl.Text.Trim(),
                    Id = CurrentId
                };

                if (!decimal.TryParse(PriceInput.Text, out decimal price) || price < 0)
                {
                    throw new Exception("Некорректная цена игры!");
                }

                products.Price = price;

                if (CheckGame(products))
                {
                    db.Produsct.Update(products);
                    db.Save();
                    var items = db.Produsct.GetAll().ToList();
                    owner.SyncDataUpdate(ref items);
                    owner.ItemBox.SelectedIndex = -1;
                    this.Close();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private bool CheckGame(Products products)
        {
            try
            {
                if (string.IsNullOrEmpty(products.Name))
                    throw new Exception("Имя игры не указано!");

                if (string.IsNullOrEmpty(products.Description))
                    throw new Exception("Описание игры не указано!");

                if (products.Price < 0)
                    throw new Exception("Цена игры не может быть отрицательной!");

                return true;
            }
            catch (IOException ex)
            {
                MessageBox.Show("Файл " + GetFileNameFromMessage(ex.Message) + " уже используется игрой");
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
        //UnitOfWork.UnitOfWork db = new UnitOfWork.UnitOfWork();

        //private DelegateCommand? AddGameCommand;

        //public ICommand AddGame
        //{
        //    get
        //    {
        //        if (AddGameCommand == null)
        //        {
        //            AddGameCommand = new DelegateCommand(AddGameExecute);
        //        }
        //        return AddGameCommand;
        //    }
        //}

        //private void AddGameExecute()
        //{

        //    Products products = new Products();
        //    try
        //    {
        //        products.Name = NameInput.Text.Trim();
        //        products.Description = DescriptionInput.Text.Trim();
        //        products.Image = InputImageUrl.Text.Trim();
        //        products.Id = CurrentId;
        //        if (PriceInput.Text.Trim() != "")
        //        {
        //            products.Price = Convert.ToDecimal(PriceInput.Text);
        //        }
        //        if (CheckGame(products))
        //        {
        //            if (products == null)
        //                throw new Exception("Элемент не выбран!");
        //            db.Produsct.Update(products);
        //            db.Save();
        //            var itemss = db.Produsct.GetAll().ToList();
        //            owner.SyncDataUpdate(itemss);
        //            owner.ItemBox.SelectedIndex = -1;
        //            this.Close();
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        MessageBox.Show(ex.Message);
        //    }
        //}

        //private bool CheckGame
        //    (Products products)
        //{
        //    try
        //    {
        //        if (String.IsNullOrEmpty(products.Name))
        //            throw new Exception("Имя игры не узазано!");
        //        if (String.IsNullOrEmpty(products.Description))
        //            throw new Exception("Описание игры не указано!");
        //        if (products.Price < 0)
        //            throw new Exception("Цена меньше нуля");
        //        return true;
        //    }
        //    catch (IOException ex)
        //    {
        //        MessageBox.Show("Файл " + GetFileNameFromMessage(ex.Message) + " уже используется игрой");
        //        return false;
        //    }
        //    catch (Exception ex)
        //    {
        //        MessageBox.Show(ex.Message);
        //        return false;
        //    }
        //}

        //private string GetFileNameFromMessage(string message)
        //{
        //    int start = message.IndexOf("'") + 1;
        //    int end = message.IndexOf("'", start);
        //    if (start >= 0 && end >= 0)
        //    {
        //        return message.Substring(start, end - start);
        //    }
        //    return null;
        //}
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

    }
}
