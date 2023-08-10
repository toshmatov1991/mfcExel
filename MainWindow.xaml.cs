using exel_for_mfc.PassModels;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
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

namespace exel_for_mfc
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }
        

        private async void Button_Click(object sender, RoutedEventArgs e)
        {
            //При нажатии на кнопку Войти
            //Вход
            //Проверка на пустые поля
            //Проверка входных значений
            if (string.IsNullOrWhiteSpace(login_text.Text)
            && string.IsNullOrWhiteSpace(password_text.Password)
            || string.IsNullOrWhiteSpace(login_text.Text)
            || string.IsNullOrWhiteSpace(password_text.Password))
            {
                MessageBox.Show("Не заполнено одно или несколько полей");
                if (string.IsNullOrWhiteSpace(login_text.Text))
                    Keyboard.ClearFocus();
           
                if (string.IsNullOrWhiteSpace(password_text.Password))
                    Keyboard.ClearFocus();
                
            }
            else
            {
                using (PassContext db = new())
                {
                    var GetAllLogPass = await db.Passwords.ToListAsync();
                    int temp = 0;
                    foreach (var item in GetAllLogPass)
                    {
                        if (item.Login == login_text.Text && item.Pass == password_text.Password)
                        {
                            //temp++;
                            //User user = new($"{item.Firstname} {item.Name} {item.Lastname}", (int)item.Id);
                            //user.Show();
                            //Close();
                            //break;
                        }
                    }
                    if (temp == 0)
                    {
                        MessageBox.Show("Повторите попытку", "Неправильный логин или пароль", MessageBoxButton.OK, MessageBoxImage.Stop);
                    }
                }
            }
        }

        //private void GoOpen(object sender, KeyEventArgs e)
        //{
        //    if (e.Key == Key.Enter)
        //        Button_Click(sender, e);
        //}

        //private void GoOpenLog(object sender, KeyEventArgs e)
        //{
        //    if (e.Key == Key.Enter)
        //        Button_Click(sender, e);
        //}




        //Метод хэширования вводимого пароля
        private string MD5Hash(string input)
        {
            var md5 = MD5.Create();
            var hash = md5.ComputeHash(Encoding.UTF8.GetBytes(input));
            return Convert.ToBase64String(hash);
        }
    }
}
