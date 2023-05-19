using GameStore.Commands;
using GameStore.DataBase;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
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
    /// Логика взаимодействия для EntryWindowView.xaml
    /// </summary>
    public partial class EntryWindowView : Window
    {
        private bool KillProcess = true;
        private UnitOfWork.UnitOfWork db;
        private UserMainWindowView userMain;
        private AdminMainWindowView adminMain;

        public EntryWindowView()
        {
            DataContext = this;
            InitializeComponent();
            InitializeDataBase();
            OpenAuthorizationExecute();
        }

        private void InitializeDataBase()
        {
            db = new();
        }

        private void CloseWindow()
        {
            if (KillProcess)
                Process.GetCurrentProcess().Kill();
        }

        #region Command

        #region OpenAuthrization

        private DelegateCommand OpenAuthorizationCommand;

        public ICommand OpenAuthorization
        {
            get
            {
                if (OpenAuthorizationCommand == null)
                {
                    OpenAuthorizationCommand = new DelegateCommand(OpenAuthorizationExecute);
                }
                return OpenAuthorizationCommand;
            }
        }

        private void OpenAuthorizationExecute()
        {
            EntryTab.SelectedItem = AuthorizationTab;
        }


        #endregion

        #region OpenRegistration

        private DelegateCommand OpenRegistrationCommand;

        public ICommand OpenRegistration
        {
            get
            {
                if (OpenRegistrationCommand == null)
                {
                    OpenRegistrationCommand = new DelegateCommand(OpenRegistrationExecute);
                }
                return OpenRegistrationCommand;
            }
        }

        private void OpenRegistrationExecute()
        {
            EntryTab.SelectedItem = RegistrationTab;
        }


        #endregion

        #region Registration

        private DelegateCommand RegistrationCommand;

        public ICommand Registration
        {
            get
            {
                if (RegistrationCommand == null)
                {
                    RegistrationCommand = new DelegateCommand(RegistrationExecute);
                }
                return RegistrationCommand;
            }
        }

        private void RegistrationExecute()
        {
            try
            {
                Users user = new Users();
                user.Name = "User";
                user.Email = GmailTextBoxRegistration.Text;

                Regex regex = new Regex("^((\\w+([-+.]\\w+)*@\\w+([-.]\\w+)*\\.\\w+([-.]\\w+)*)\\s*[;]{0,1}\\s*)+$");
                if (!regex.IsMatch(user.Email))
                    throw new Exception("Email введен некорректно!");

                foreach (var _user in db.User.GetAll().ToList())
                {
                    if (_user.Email == user.Email)
                        throw new Exception("Такая почта уже существует в базе данных!");
                }

                if (string.IsNullOrWhiteSpace(PasswordtBoxStyleRegistration.Password))
                    throw new Exception("Пароль не может быть пустым!");

                int minPasswordLength = 6;
                if (PasswordtBoxStyleRegistration.Password.Length < minPasswordLength)
                    throw new Exception($"Пароль должен содержать минимум {minPasswordLength} символов!");

                if (PasswordtBoxStyleRegistration.Password != PasswordtRepeatBoxStyle.Password)
                    throw new Exception("Повторный ввод пароля неверен!");

                user.Password = PasswordtRepeatBoxStyle.Password;
                user.IsAdmin = false;
                db.User.Create(user);
                db.Save();

                KillProcess = false;
                userMain = new UserMainWindowView(user);
                userMain.Show();
                this.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }



        #endregion

        #region Authorization

        private DelegateCommand AuthorizationCommand;

        public ICommand Authorization
        {
            get
            {
                if (AuthorizationCommand == null)
                {
                    AuthorizationCommand = new DelegateCommand(AuthorizationExecute);
                }
                return AuthorizationCommand;
            }
        }

        private void AuthorizationExecute()
        {
            foreach (var _user in db.User.GetAll().ToList())
            {
                if(_user.Email == GmailTextBox.Text &&
                    _user.Password == PasswordtBoxStyle.Password)
                {
                    KillProcess = false;
                    if (_user.IsAdmin)
                    {
                        adminMain = new();
                        adminMain.Show();
                        this.Close();
                        return;
                    }
                    userMain = new UserMainWindowView(_user);
                    userMain.Show();
                    this.Close();
                    return;
                }
            }
            MessageBox.Show("Пользователь не найден");
            return;
        }
        #endregion

        #endregion

        private void BorderWindowStyle_MouseDown(object sender, MouseButtonEventArgs e) => this.DragMove();

        private void MinPageButton_MouseDown(object sender, MouseButtonEventArgs e)
        {
            this.WindowState = WindowState.Minimized;
        }

        private void ClosePageButton_MouseDown(object sender, MouseButtonEventArgs e)
        {
            CloseWindow();
        }

        private void MinPageButtonRegistration_MouseDown(object sender, MouseButtonEventArgs e)
        {
            this.WindowState = WindowState.Minimized;
        }

        private void ClosePageButtonRegistration_MouseDown(object sender, MouseButtonEventArgs e)
        {
            CloseWindow();
        }

        private void BorderWindowStyleRegistration_MouseDown(object sender, MouseButtonEventArgs e) => this.DragMove();

    }
}
