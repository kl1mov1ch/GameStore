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
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace GameStore.Pages
{
    /// <summary>
    /// Логика взаимодействия для MessageWindowView.xaml
    /// </summary>
    public partial class MessageWindowView : Window
    {
        public MessageWindowView()
        {
            InitializeComponent();

            DoubleAnimation animation = new DoubleAnimation
            {
                From = 0,
                To = 100,
                Duration = TimeSpan.FromSeconds(4)
            };

            animation.Completed += Animation_Completed;
            Storyboard.SetTargetName(animation, "MyProgressBar");
            Storyboard.SetTargetProperty(animation, new PropertyPath(ProgressBar.ValueProperty));
            Storyboard storyboard = new Storyboard();
            storyboard.Children.Add(animation);     
            storyboard.Begin(this);
        }

        private void Animation_Completed(object sender, EventArgs e)
        {
            MyTextBox.Text = "Сообщение с игрой отправлено на почту!";
        }

        private void ToolBar_MouseDown(object sender, MouseButtonEventArgs e) => this.DragMove();

        private void CloseWindowExecute(object sender, MouseButtonEventArgs e)
        {
            this.Close();
        }
    }
}
