using GameStore.Commands;
using GameStore.DataBase;
using System;
using System.Threading.Tasks;
using System.Net.Mail;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Input;
using MimeKit;
using System.Net;
using GameStore.Utilites;
using Org.BouncyCastle.Tls.Crypto.Impl.BC;
using System.Collections.Specialized;
using Org.BouncyCastle.Tls;
using Org.BouncyCastle.Crypto.Macs;
using MailKit.Net.Smtp;
using MailKit.Security;
using SmtpClient = MailKit.Net.Smtp.SmtpClient;
using Org.BouncyCastle.Bcpg;

namespace GameStore.Pages
{
    /// <summary>
    /// Логика взаимодействия для BasketWindowView.xaml
    /// </summary>
    public partial class BasketWindowView : Window
    {
        private UnitOfWork.UnitOfWork db;

        private UserMainWindowView owner;
        private List<Basket> baskets;
        private List<Products> products;
        private List<Users> _users;

        public BasketWindowView(UserMainWindowView owner)
        {
            this.owner = owner;
            DataContext = this;
            InitializeComponent();
            InitializeDataBase();
            baskets = new List<Basket>();
        }

        public void InitializeDataBase()
        {
            db = new UnitOfWork.UnitOfWork();
            List<Products> basket = new List<Products>();
            foreach (var item in db.Basket.GetAll().ToList())
            {
                if (owner.CurrentUser.Id == item.User_Id)
                {

                    basket.Add(db.Produsct.Get(item.Product_Id));
                }
            }
            var newBasket = new List<Products>();
            foreach (var item in basket)
            {
                if (item != null)
                    newBasket.Add(item);
            }
            products = newBasket;
            ItemBox.ItemsSource = products;

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


        #region DeleteGame
        private DelegateCommand? DeleteGameCommand;

        public ICommand DeleteGame
        {
            get
            {
                if (DeleteGameCommand == null)
                {
                    DeleteGameCommand = new DelegateCommand(DeleteGameExecute);
                }
                return DeleteGameCommand;
            }
        }

        private void DeleteGameExecute()
        {
            try
            {
                Products productToRemove = ItemBox.SelectedItem as Products;

                if (productToRemove != null)
                {
                    foreach (var basket in db.Basket.GetAll().ToList())
                    {
                        if (owner.CurrentUser.Id == basket.User_Id && basket.Product_Id == productToRemove.Id)
                        {
                            products.Remove(db.Produsct.Get(basket.Product_Id));
                            db.Basket.Delete(basket.Id);
                            db.Save();
                            ItemBox.Items.Refresh();
                            break;
                        }
                    }
                    ItemBox.SelectedIndex = -1;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
        #endregion

        #region BuyGame
        private DelegateCommand? BuyGameCommand;

        public ICommand BuyGame
        {
            get
            {
                if (BuyGameCommand == null)
                {
                    BuyGameCommand = new DelegateCommand(BuyGameExecute);
                }
                return BuyGameCommand;
            }
        }

        //Tmkmz2TaxLY2HTSgAeyc

        private async void BuyGameExecute()
        {
            try
            {
                MessageWindowView messageWindow = new MessageWindowView();
                messageWindow.ShowDialog();
                Basket basket = ItemBox.SelectedItem as Basket;
                if (baskets != null)
                {
                    var emailMessage = new MimeMessage();
                    emailMessage.From.Add(new MailboxAddress("Game Store", "gamestore03@mail.ru"));
                    emailMessage.To.Add(new MailboxAddress("", owner.CurrentUser.Email));
                    emailMessage.Subject = "Ваш заказ в магазине GameStore";

                    var bodyBuilder = new BodyBuilder();
                    bodyBuilder.HtmlBody = $@"<!DOCTYPE html>
                                                <html>
                                                <head>
                                                    <meta charset=""utf-8"">
                                                    <title>Заголовок письма</title>
                                                </head>
                                                <body>
                                                    <h2>Здравствуйте {owner.CurrentUser.Email}!</h2>
                                                    <p>Благодарим вас за заказ в магазине GameStore, когда ключ от игры будет доступен, мы вам напишем.</p>
                                                    <p>Спасибо за доверие!</p>
                                                    <p>С уважением,<h1><br/>Команда GameStore</h1></p>
                                                </body>
                                                </html>";

                    emailMessage.Body = bodyBuilder.ToMessageBody();
                    using (var client = new SmtpClient())
                    {
                        await client.ConnectAsync("smtp.mail.ru", 465, true);
                        await client.AuthenticateAsync("gamestore03@mail.ru", "Tmkmz2TaxLY2HTSgAeyc");
                        await client.SendAsync(emailMessage);
                        await client.DisconnectAsync(true);
                    }
                }
                else
                {
                    throw new InvalidOperationException("Элемент не выбран");
                }
            }
            catch (SmtpException ex)
            {
                MessageBox.Show("Ошибка при отправке сообщения: " + ex.Message);
            }
            catch (InvalidOperationException ex)
            {
                MessageBox.Show("Ошибка: " + ex.Message);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Произошла ошибка: " + ex.Message);
            }
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
            List<Basket> searchGame = new List<Basket>();

            try
            {
                if (SearchLine.Text.Trim() == "")
                {
                    searchGame = db.Basket.GetAll().Where(item => owner.CurrentUser.Id == item.User_Id).ToList();
                }
                else
                {
                    searchGame = db.Basket.GetAll()
                        .Where(game => owner.CurrentUser.Id == game.User_Id &&
                                       (game.Name.Contains(SearchLine.Text) || game.Description.Contains(SearchLine.Text))).ToList();
                }
                ItemBox.ItemsSource = searchGame;
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
                Basket basket = ItemBox.SelectedItem as Basket;
                if (basket == null)
                    throw new Exception("Элемент не выбран!");
                foreach (var item in db.Basket.GetAll().ToList())
                {
                    if (owner.CurrentUser.Id == item.User_Id)
                    {
                    }
                    db.Produsct.Delete(basket.Id);
                }
                db.Save();
                ItemBox.SelectedIndex = -1;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
        #endregion

    }
} 
