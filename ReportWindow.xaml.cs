using Microsoft.Win32;
using SpreadsheetLight;
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
    public partial class ReportWindow : Window
    {
        public ReportWindow()
        {
            InitializeComponent();
        }

        //Генерация шаблона для отчета
        private void Button_Click(object sender, RoutedEventArgs e)
        {
            SaveFileDialog saveFile = new()
            {
                DefaultExt = "xlsx"
            };

            if (saveFile.ShowDialog() == true)
            {
                CreateFile(saveFile.FileName);

            }
        }

        private static void CreateFile(string str)
        {
            if(str != string.Empty)
            {
                using SLDocument doc = new();

                doc.SetCellValue("A1", "Тимур");
                doc.SaveAs(str);



            }



        }
    }
}
