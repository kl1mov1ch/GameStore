using Azure;
using GameStore.Commands;
using GameStore.DataBase;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
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
    /// Логика взаимодействия для UserMainWindowView.xaml
    /// </summary>
    public partial class UserMainWindowView : Window
    {

        UnitOfWork.UnitOfWork db = new UnitOfWork.UnitOfWork();
        public Users CurrentUser;
        private List<Products> BasketGames;
        private bool KillProcess = true;
        public UserMainWindowView(Users currentUser)
        {
            InitializeComponent();
            InitializeDataBase();
            DataContext = this;
            BasketGames = new List<Products>();
            CurrentUser = currentUser;
        }

        private void InitializeDataBase()
        {
            var items = db.Produsct.GetAll().ToList();
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
                    item.Image = System.IO.Path.Combine(System.IO.Path.GetDirectoryName(exePath), "Images", item.Image);
                }
                source.Add(item);
            }
            ItemBox.ItemsSource = source;
        }

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

        private void Border_MouseDown(object sender, MouseButtonEventArgs e) { }/*=> this.DragMove()*/

        private void BasketOpenWindow(object sender, RoutedEventArgs e)
        {

            BasketWindowView basketWindowView = new BasketWindowView(this);
            basketWindowView.ShowDialog();
        }

        #region ExitButton
        private DelegateCommand? ExitCommand;

        public ICommand Exit
        {
            get
            {
                if (ExitCommand == null)
                {
                    ExitCommand = new DelegateCommand(ExitExecute);
                }
                return ExitCommand;
            }
        }

        private void ExitExecute()
        {
            EntryWindowView entryWindow = new EntryWindowView();
            KillProcess = false;
            this.Close();
            entryWindow.Show();
        }
        #endregion

        #region SearchFilterBGame

        private DelegateCommand? SearchFilterGameCommand;

        public ICommand SearchFilterGame
        {
            get
            {
                if (SearchFilterGameCommand == null)
                {
                    SearchFilterGameCommand = new DelegateCommand(SearchFilterGameExecute);
                }
                return SearchFilterGameCommand;
            }
        }

        private void SearchFilterGameExecute()
        {

            List<Products> SearchGame = new List<Products>() { };
            try
            {
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

                            SearchGame.Add(game);
                        }

                    }
                    ItemBox.ItemsSource = SearchGame;
                    ItemBox.Items.Refresh();
                }


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

        #region AddBasketGame

        private DelegateCommand? AddBasketGameCommand;

        public ICommand AddBasketGame
        {
            get
            {
                if (AddBasketGameCommand == null)
                {
                    AddBasketGameCommand = new DelegateCommand(AddBasketGameExecute);
                }
                return AddBasketGameCommand;
            }
        }

        private void AddBasketGameExecute()
        {
            try
            {
               Products product = ItemBox.SelectedItem as Products;
                if(product != null)
                {
                    foreach (var item in db.Basket.GetAll().ToList())
                    {
                        if (item.User_Id == CurrentUser.Id && item.Product_Id == product.Id)
                            throw new NullReferenceException();
                        //break;
                    }
                    Basket basket = new()
                    {
                        Product_Id = db.Produsct.Get(product.Id).Id,
                        Price = product.Price,
                        Description = product.Description,
                        Name = product.Name,
                        User_Id = CurrentUser.Id,
                        Image = product.Image
                    };
                    db.Basket.Create(basket);
                    db.Save();
                    ItemBox.SelectedIndex = -1;
                }
                
            }
            catch(NullReferenceException)
            {
                return;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void ItemBox_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            Products productToOpen = ItemBox.SelectedItem as Products;
            if (productToOpen != null)
            {
                GameWindowView gameWindow = new GameWindowView(productToOpen);
                gameWindow.ShowDialog();
            }
            ItemBox.SelectedIndex = -1;
        }
    }
    #endregion

}
