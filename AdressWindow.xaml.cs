using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace exel_for_mfc
{
    public partial class AdressWindow : Window
    {
        public AdressWindow()
        {
            InitializeComponent();
            name.Focus();
            StartAdress();
        }

        public AdressWindow(ref string str)
        {
            InitializeComponent();
            name.Focus();
            StartAdress();
        }

        private async void StartAdress()
        {
            using ExDbContext db = new();
            Xmkr.ItemsSource = await db.PayAmounts.Where(u => u.Mkr != null).Select(s => s.Mkr).ToListAsync();
            ulicaX.ItemsSource = await db.PayAmounts.Where(u => u.Ulica != null).Select(s => s.Ulica).ToListAsync();
            kv.ItemsSource = await db.PayAmounts.Where(u => u.Kvartira != null).Select(s => s.Kvartira).ToListAsync();
        }




        //Добавить адрес
        private void Button_Click(object sender, RoutedEventArgs e)
        {
            /*
             * Xmkr - Выбор типа Микрорайона
             * nameMKR - Наименование микрорайона
             * ulicaX - тип улицы
             * name - наименование улицы
             * dom - тип дома
             * numberDom - номер дома
             * ------------------
             * Stroenie - строение
             * numCorpus - корпус
             * kv - тип квартиры 
             * kvartira - номер квартиры
             */


            if (string.IsNullOrEmpty(nameMKR.Text) || string.IsNullOrWhiteSpace(nameMKR.Text) 
                || string.IsNullOrEmpty(name.Text) || string.IsNullOrWhiteSpace(name.Text))
            {
                MessageBox.Show("Вы пропустили обязательные поля для заполнения!", "Название улицы или Название микрорайона");
            }

            else
            {
                TableWindow.temp1 += string.IsNullOrEmpty(nameMKR.Text.Trim()) ? null : $"{Xmkr.Text} {nameMKR.Text}, ";
                TableWindow.temp1 += string.IsNullOrEmpty(name.Text) ? null : $"{ulicaX.Text} {name.Text}, ";
                TableWindow.temp1 += string.IsNullOrEmpty(numberDom.Text) ? null : $"{dom.Text} {numberDom.Text}, ";
                TableWindow.temp1 += string.IsNullOrEmpty(Stroenie.Text) ? null : $"/{Stroenie.Text}, ";
                TableWindow.temp1 += string.IsNullOrEmpty(numCorpus.Text) ? null : $"корп. {numCorpus.Text}, ";
                TableWindow.temp1 += string.IsNullOrEmpty(kvartira.Text) ? null : $"{kv.Text} {kvartira.Text}";
                Close();
            }
        }



        //Очистить поля
        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            nameMKR.Clear();
            name.Clear();
            numberDom.Clear();
            Stroenie.Clear();
            numCorpus.Clear();
            kvartira.Clear();
            name.Focus();
        }
    }
}
