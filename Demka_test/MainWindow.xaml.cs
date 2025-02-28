using Amazon.SimpleSystemsManagement;
using Amazon.SimpleSystemsManagement.Model;
using Demka_test.Pages;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Reactive;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Demka_test
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
           
        }

        private void btnLogin_Click(object sender, RoutedEventArgs e)
        {
            string login = loginText.Text;
            string password = passwordText.Password;

            var context = Helper.GetContext();

            try
            {
                var user = context.Users.SingleOrDefault(u => u.login == login && u.password == password);

                if (user != null)
                {
                    var role = context.TypeRole.SingleOrDefault(r => r.idTypeRole == user.idTypeRole);
                    if (role != null)
                    {
                        MessageBox.Show($"Добро пожаловать, {user.fio}! Ваша роль: {role.typeRole1}");

                        var specialistWindow = new specialistWindowOrder(user.fio, role.typeRole1);
                        specialistWindow.Show();
                        this.Close();
                    }
                    else
                    {
                        MessageBox.Show("Ошибка определения роли пользователя.");
                    }
                }
                else
                {
                    // ToolTip toolTip = new ToolTip();
                    // toolTip.Content = "Неверный логин или пароль!";
                    // loginText.ToolTip = toolTip;

                    MessageBox.Show("Неверный логин или пароль!");
                    // MessageBox.Show("Повторить попытку?", "Ошибка", MessageBoxButton.YesNo);
                    // MessageBox.Show("Продолжить?", "Ошибка", MessageBoxButton.OKCancel);
                }
            }
            finally
            {
               
            }




        }
    }
}
