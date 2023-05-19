using GameStore.Commands;
using GameStore.DataBase;
using GameStore.Repository;
using System;
using System.Collections.Generic;
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
using GameStore.UnitOfWork;
using System.Diagnostics;
using GameStore.Utilites;
using Azure;
using System.IO;

namespace GameStore.Pages
{
    /// <summary>
    /// Логика взаимодействия для AdminMainWindowView.xaml
    /// </summary>

    public partial class AdminMainWindowView : Window
    {
        private bool KillProcess = true;
        private List<Products> products;

        UnitOfWork.UnitOfWork db = new UnitOfWork.UnitOfWork();

        public AdminMainWindowView()
        {
            DataContext = this;

            InitializeComponent();
            SyncData();

            products = new List<Products>();


        }

        public void SyncData()
        {
            
            var items = db.Produsct.GetAll().ToList();
            var source = new List<Products>();
            string exePath = System.Reflection.Assembly.GetExecutingAssembly().Location;
            foreach (var item in items)
            {
                if (item.Image == "")
                {
                    item.Image = System.IO.Path.Combine(exePath ,"../../../../","Source/ClosePageIconDark.png");
                }
                else
                {
                    item.Image = System.IO.Path.Combine(System.IO.Path.GetDirectoryName(path: exePath), "Images", item.Image);
                }
                source.Add(item);
            }
            // products = source;
            products = source;
            ItemBox.ItemsSource = db.Produsct.GetAll().ToList();
            db.Save();
            ItemBox.Items.Refresh();
            
        }

        public void SyncDataUpdate(ref List<Products> products)
        {
            var items = products.ToList();
            var source = new List<Products>();
            string exePath = System.Reflection.Assembly.GetExecutingAssembly().Location;
            foreach (var item in items)
            {
                if (item.Image == "")
                {
                    item.Image = System.IO.Path.Combine(exePath, "../../../../", "Source/ClosePageIconDark.png");
                }
                else
                {
                    item.Image = System.IO.Path.Combine(System.IO.Path.GetDirectoryName(path: exePath), "Images", item.Image);
                }
                source.Add(item);
            }
            products = new List<Products>(source);
            ItemBox.ItemsSource = products;
            db.Save();
            ItemBox.Items.Refresh();

        }

        private DelegateCommand AddItemWindowCommand;

        public ICommand AddItemWindow
        {
            get
            {
                if (AddItemWindowCommand == null)
                {
                    AddItemWindowCommand = new DelegateCommand(OpenAddItemWindowExecute);
                }
                return AddItemWindowCommand;
            }
        }

        private void OpenAddItemWindowExecute()
        {
            AddingWindowView addingWindowView = new AddingWindowView(this);
            addingWindowView.ShowDialog();
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

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            if(KillProcess)
                Process.GetCurrentProcess().Kill();
        }


        private void CloseButtonThis()
        {
            ItemBox.Items.Refresh();
        }

        private void RelodeButton(object sender, MouseButtonEventArgs e)
        {

            CloseButtonThis();
        }

        #region RemoveGame

        private DelegateCommand? RemoveGameCommand;

        public ICommand RemoveGame
        {
            get
            {
                if (RemoveGameCommand == null)
                {
                    RemoveGameCommand = new DelegateCommand(RemoveGameExecute);
                }
                return RemoveGameCommand;
            }
        }

        private void RemoveGameExecute()
        {
            try
            {
                
                Products product = ItemBox.SelectedItem as Products;
                if (products == null)
                    throw new Exception("Элемент не выбран!");
                db.Produsct.Delete(product.Id);
                db.Save();
                var baskets = db.Basket.GetAll().ToList();
                foreach (var basket in baskets)
                {
                    if(basket.Product_Id == product.Id)
                    {
                        db.Basket.Delete(basket.Id);
                        products.Remove(db.Produsct.Get(product.Id));
                    }
                }
                db.Save();

                SyncData();
                ItemBox.SelectedIndex = -1;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
        #endregion

        #region SearchFilterGame

        private DelegateCommand? searchFilterGameCommand;

        public ICommand SearchFilterGame
        {
            get
            {
                if (searchFilterGameCommand == null)
                {
                    searchFilterGameCommand = new DelegateCommand(SearchFilterGameExecute);
                }
                return searchFilterGameCommand;
            }
        }

        private void SearchFilterGameExecute()
        {
            try
            {
                List<Products> searchGame = new List<Products>();
                UnitOfWork.UnitOfWork db = new UnitOfWork.UnitOfWork();

                if (SearchLine.Text.Trim() == "")
                {
                    ItemBox.ItemsSource = db.Produsct.GetAll().ToList();
                }
                else
                {
                    foreach (var game in db.Produsct.GetAll().ToList())
                    {
                        if (game.Name.Contains(SearchLine.Text) ||
                            game.Description.Contains(SearchLine.Text))
                        {
                            searchGame.Add(game);
                        }
                    }
                    ItemBox.ItemsSource = searchGame;
                }

                ItemBox.Items.Refresh();
            }
            catch (NullReferenceException ex)
            {
                MessageBox.Show("Выбран пустой элемент!");
                ItemBox.SelectedItem = null;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        #endregion

        //private DelegateCommand? SearchFilterGameCommand;

        //public ICommand SearchFilterGame
        //{
        //    get
        //    {
        //        if (SearchFilterGameCommand == null)
        //        {

        //        }
        //        return SearchFilterGameCommand = new DelegateCommand(SearchFilterGameExecute); ;
        //    }
        //}

        //private void SearchFilterGameExecute()
        //{

        //    List<Products> SearchGame = new List<Products>() { };
        //    try
        //    {
        //        if (SearchLine.Text.Trim() == "")
        //        {
        //            ItemBox.ItemsSource = db.Produsct.GetAll().ToList();
        //        }
        //        else
        //        {
        //            foreach (var game in db.Produsct.GetAll().ToList())
        //            {

        //                if (game.Name.Contains(SearchLine.Text) ||
        //                    game.Description.Contains(SearchLine.Text))
        //                {

        //                    SearchGame.Add(game);
        //                }

        //            }
        //            SyncDataUpdate(SearchGame);
        //            //ItemBox.ItemsSource = SearchGame;
        //            //ItemBox.Items.Refresh();

        //        ItemBox.Items.Refresh();
        //        }


        //    }
        //    catch (NullReferenceException ex)
        //    {
        //        MessageBox.Show("Выбран пустой элемент!");
        //        ItemBox.SelectedItem = null;

        //    }
        //    catch (Exception ex)
        //    {
        //        MessageBox.Show(ex.Message);
        //    }

        //}

        #region Change
        private DelegateCommand? ChangeGameCommand;

        public ICommand ChangeGame
        {
            get
            {
                if (ChangeGameCommand == null)
                {
                    ChangeGameCommand = new DelegateCommand(ChangeGameExecute);
                }
                return ChangeGameCommand;
            }
        }

        private void ChangeGameExecute()
        {
            try
            {
                Products? product = ItemBox.SelectedItem as Products;
                if (products == null)
                    throw new Exception("Элемент не выбран!");
                ChangeWindowView changeWindowView = new ChangeWindowView(products: product, owner: this);
                changeWindowView.ShowDialog();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Элемент не выбран!");
            }
        }

        #endregion




    }


}
