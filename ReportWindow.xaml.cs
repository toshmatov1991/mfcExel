using Microsoft.Win32;
using SpreadsheetLight;
using System;
using System.Collections.Generic;
using System.Data;
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

        private DateTime yearCodeBehind = DateTime.Today;

        public ReportWindow()
        {
            InitializeComponent();
            TotalAmountForAllTime.Text = yearCodeBehind.Year.ToString();
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





        private void CreateFile(string str)
        {

            #region Стили
            SLStyle titleStyle = new SLStyle();
            titleStyle.Font.FontName = "Arial";
            titleStyle.Font.FontSize = 16;
            titleStyle.Font.Bold = true;
            titleStyle.SetWrapText(true);
            titleStyle.SetVerticalAlignment(DocumentFormat.OpenXml.Spreadsheet.VerticalAlignmentValues.Center);
            titleStyle.SetHorizontalAlignment(DocumentFormat.OpenXml.Spreadsheet.HorizontalAlignmentValues.Center);
            #endregion




            if (str != string.Empty)
            {
                //Получил список выбранных Месяцев
                var listMouth = ListMouth();

                //Генерация колонок в зависимости от выбора Месяцев
                //Создаю объкт таблицы
                DataTable dt = new();

                //Затем в цикле надо задать колонки Месяцев
                foreach (var item in listMouth)
                {
                    dt.Columns.Add(item, typeof(string));
                }



                //var u = DateTime.Now.Month;





                using ExDbContext db = new();
                using SLDocument doc = new();
                doc.ImportDataTable(1, 1, dt, true);

                var getMyArea = db.Areas.Where(u => u.HidingArea == 1).OrderBy(u => u.AreaName).ToList();


                //doc.SetCellValue("A1", "Район");

                doc.SetColumnWidth(1, 40);
                doc.SetRowHeight(1, 30);
                doc.SetCellStyle(1, 1, titleStyle);

                int i = 2;
                foreach (var item in getMyArea)
                {
                    doc.SetCellValue($"A{i}", item.AreaName);
                    i++;
                }






                doc.SaveAs(str);

            }

        }







        #region Методы помошники
        //Вернем список выбранных месяцев
        private List<string> ListMouth()
        {
            List<string> strings = new();

            strings.Add("Район");

            if ((bool)checkBox1.IsChecked)
                strings.Add((string)checkBox1.Content);

            if ((bool)checkBox2.IsChecked)
                strings.Add((string)checkBox2.Content);

            if ((bool)checkBox3.IsChecked)
                strings.Add((string)checkBox3.Content);

            if ((bool)checkBox4.IsChecked)
                strings.Add((string)checkBox4.Content);

            if ((bool)checkBox5.IsChecked)
                strings.Add((string)checkBox5.Content);

            if ((bool)checkBox6.IsChecked)
                strings.Add((string)checkBox6.Content);

            if ((bool)checkBox7.IsChecked)
                strings.Add((string)checkBox7.Content);

            if ((bool)checkBox8.IsChecked)
                strings.Add((string)checkBox8.Content);

            if ((bool)checkBox9.IsChecked)
                strings.Add((string)checkBox9.Content);

            if ((bool)checkBox10.IsChecked)
                strings.Add((string)checkBox10.Content);

            if ((bool)checkBox11.IsChecked)
                strings.Add((string)checkBox11.Content);

            if ((bool)checkBox12.IsChecked)
                strings.Add((string)checkBox12.Content);


            return strings;
        }

        //Кнопка влево
        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            yearCodeBehind = yearCodeBehind.AddYears(-1);
            TotalAmountForAllTime.Text = yearCodeBehind.Year.ToString();
        }

        //Кнопка вправо
        private void Button_Click_2(object sender, RoutedEventArgs e)
        {
            yearCodeBehind = yearCodeBehind.AddYears(1);
            TotalAmountForAllTime.Text = yearCodeBehind.Year.ToString();
        }
        #endregion

    }
}
