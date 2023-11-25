using DocumentFormat.OpenXml.Drawing;
using exel_for_mfc.SupportClass;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Windows;

namespace exel_for_mfc
{
    public partial class StaticWindow : Window
    {
        private int yearCodeBehind = DateTime.Now.Year;
        public StaticWindow()
        {
            InitializeComponent();
            StartapStatic();
        }

        void StartapStatic()
        {
            using ExDbContext db = new();

            YearXaml.Text = DateTime.Now.Year.ToString();


            //Общее количество сертификатов

            Sert.Text += db.Registries
                .Where(u => u.SerialAndNumberSert != null || string.IsNullOrEmpty(u.SerialAndNumberSert))
                .Count().ToString();

            //Размер выплат
            var getNamePays = db.PayAmounts.Where(u => u.Pay != null).ToList();
            List<PayClass> names = new();
            foreach (var item in getNamePays)
            {
                names.Add(new PayClass(item.Id, item.Pay, db.Registries.Where(u => u.PayAmountFk == item.Id).Count()));
            }
            payFilter.ItemsSource = names.ToList();

            //Общее количество выплат
            var AllPays = from r in db.Registries.Where(u => u.PayAmountFk != null)
                          join p in db.PayAmounts.Where(u => u.Pay != null) on r.PayAmountFk equals p.Id
                          select new
                          {
                              p.Pay,
                              r.DateGetSert
                          };

            decimal? allSummPays = 0;

            foreach (var item in AllPays)
            {
                allSummPays += item.Pay;
            }



            payCount.Text = "Общая сумма выплат за год: " + allSummPays.ToString() + " рублей";

            //Решения
            var getNameSoul = db.SolutionTypes.Where(u => u.SolutionName != "").ToList();
            List<SolutionClass> names1 = new();
            foreach (var item in getNameSoul)
            {
                names1.Add(new SolutionClass(item.Id, item.SolutionName, db.Registries.Where(u => u.SolutionFk == item.Id).Count()));
            }
            solFilter.ItemsSource = names1.ToList();

        }

        //Кнопка вправо
        private void Button_Click(object sender, RoutedEventArgs e)
        {
            yearCodeBehind++;
            YearXaml.Text = yearCodeBehind.ToString();
            StartapStatic();
        }

        //Кнопка влево
        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            yearCodeBehind--;
            YearXaml.Text = yearCodeBehind.ToString();
            StartapStatic();
        }

    }
}