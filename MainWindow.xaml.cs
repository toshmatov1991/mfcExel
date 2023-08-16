using exel_for_mfc.PassModels;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;

namespace exel_for_mfc
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            TableWindow tableWindow = new();
            tableWindow.Show();
            Close();
            //Start();
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
                    login_text.BorderBrush = Brushes.Red;


                if (string.IsNullOrWhiteSpace(password_text.Password))
                    password_text.BorderBrush = Brushes.Red;

            }
            else
            {
                int temp = 0;
                using (PassContext db = new())
                {
                    var GetUserLogPass = await db.Passwords.Where(u => u.Id == 1).FirstOrDefaultAsync();
                   
                    if (GetUserLogPass.Login == login_text.Text && GetUserLogPass.Pass == MD5Hash(password_text.Password) && GetUserLogPass.Id == 1)
                    {
                        MessageBox.Show("Пользователь");
                        temp = 1;
                    }

                    if(temp == 0)
                    {
                        var GetAdminLogPass = await db.Passwords.Where(u => u.Id == 2).FirstOrDefaultAsync();
                        if (GetAdminLogPass.Login == login_text.Text && GetAdminLogPass.Pass == MD5Hash(password_text.Password) && GetAdminLogPass.Id == 2)
                        {
                            MessageBox.Show("Админ");
                            temp = 1;
                        }
                    }

                    if (temp == 0)
                    {
                        MessageBox.Show("Повторите попытку", "Неправильный логин или пароль", MessageBoxButton.OK);
                    }
                }
            }
        }

        //Метод хэширования вводимого пароля
        private string MD5Hash(string input)
        {
            var md5 = MD5.Create();
            var hash = md5.ComputeHash(Encoding.UTF8.GetBytes(input));
            return Convert.ToBase64String(hash);
        }



        //Метод хорошего старта
        private async void Start()
        {
            using (PassContext db = new())
            {
                var start = await db.Passwords.ToListAsync();
                foreach (var item in start) { }
            }
            login_text.Focus();
        }



        //Методы подсвечивают рамки красным при неправильном вводе
        private void Pa(object sender, MouseEventArgs e)
        {
            password_text.BorderBrush = Brushes.Black;
        }

        private void Bo(object sender, MouseEventArgs e)
        {
            login_text.BorderBrush = Brushes.Black;
        }


        private void Log(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
                Button_Click(sender, e);
        }

        private void Pas(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
                Button_Click(sender, e);
        }
    }
}