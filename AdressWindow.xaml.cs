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
        }

        public AdressWindow(ref string str)
        {
            InitializeComponent();
        }

        //Добавить адрес
        private void Button_Click(object sender, RoutedEventArgs e)
        {
            /*
             * ulOrDom - Улица или Дом
             name - Улица название
            dom - дом
            numberDom - номер дома
            numCorpus - номер корпуса
            Stroenie - строение
            kvartira - квартира
             */

            if(string.IsNullOrEmpty(name.Text) 
                || string.IsNullOrWhiteSpace(name.Text)
                && string.IsNullOrEmpty(numberDom.Text)
                && string.IsNullOrWhiteSpace(numberDom.Text)
                || string.IsNullOrEmpty(name.Text) 
                || string.IsNullOrEmpty(numberDom.Text))
            {
                MessageBox.Show("Вы пропустили обязательные поля для заполнения!", "Название улицы или номер дома");
            }

            else
            {
                if(string.IsNullOrEmpty(Stroenie.Text) && string.IsNullOrEmpty(numCorpus.Text))
                    TableWindow.temp1 = $"{ulOrDom.Text} {name.Text}, {dom.Text} {numberDom.Text}, кв.{kvartira.Text}";
                if(!string.IsNullOrEmpty(Stroenie.Text))
                    TableWindow.temp1 = $"{ulOrDom.Text} {name.Text}, {dom.Text} {numberDom.Text}/{Stroenie.Text}, кв.{kvartira.Text}";
            }

            Close();

        }
    }
}
