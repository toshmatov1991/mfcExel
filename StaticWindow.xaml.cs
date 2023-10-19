using exel_for_mfc;
using exel_for_mfc.SupportClass;
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
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace exel_for_mfc
{
    /// <summary>
    /// Логика взаимодействия для StaticWindow.xaml
    /// </summary>
    public partial class StaticWindow : Window
    {
        public StaticWindow()
        {
            InitializeComponent();
            StartapStatic();
        }

        void StartapStatic()
        {
            using ExDbContext db = new();

            //Общее количество сертификатов

            Sert.Text += db.Registries
                .Where(u => u.SerialAndNumberSert != null || string.IsNullOrEmpty(u.SerialAndNumberSert))
                .Count().ToString();

            //Размер выплат
            var getNamePays = db.PayAmounts.ToList();
            List<PayClass> names = new();
            foreach (var item in getNamePays)
            {
                names.Add(new PayClass(item.Id, item.Pay, db.Registries.Where(u => u.PayAmountFk == item.Id).Count()));
            }
            payFilter.ItemsSource = names.ToList();

            //Общее количество выплат
            payCount.Text += db.Registries.Where(u => u.PayAmountFk != null).Count().ToString();

            //Решения
            var getNameSoul = db.SolutionTypes.ToList();
            List<SolutionClass> names1 = new();
            foreach (var item in getNameSoul)
            {
                names1.Add(new SolutionClass(item.Id, item.SolutionName, db.Registries.Where(u => u.SolutionFk == item.Id).Count()));
            }
            solFilter.ItemsSource = names1.ToList();

            //Общее количество Решений
            solCount.Text += db.Registries.Where(u => u.SolutionFk != null).Count().ToString();



        }

    }
}
