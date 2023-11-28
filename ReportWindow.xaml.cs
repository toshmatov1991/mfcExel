﻿using Microsoft.Win32;
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
        private bool flag = true;

        private DateTime yearCodeBehind = DateTime.Today;

        private List<string> listMouth = new();

        public ReportWindow()
        {
            InitializeComponent();
            TotalAmountForAllTime.Text = yearCodeBehind.Year.ToString();
        }

        //Генерация шаблона для отчета
        private void Button_Click(object sender, RoutedEventArgs e)
        {
            // Получил список выбранных Месяцев
            listMouth = ListMouth();


            if (listMouth.Count == 1)
                MessageBox.Show("Нужно выбрать хотя бы один месяц для отчета\n Или выбрать все!", "Внимание", MessageBoxButton.OK, MessageBoxImage.Information);

            else if (listMouth.Count > 1)
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

        }





        private void CreateFile(string str)
        {

            #region Стили
            //Стиль главного заголовка
            SLStyle titleStyle = new SLStyle();
            titleStyle.Font.FontName = "Arial";
            titleStyle.Font.FontSize = 16;
            titleStyle.Font.Bold = true;
            titleStyle.SetWrapText(true);
            titleStyle.SetVerticalAlignment(DocumentFormat.OpenXml.Spreadsheet.VerticalAlignmentValues.Center);
            titleStyle.SetHorizontalAlignment(DocumentFormat.OpenXml.Spreadsheet.HorizontalAlignmentValues.Center);

            //Стиль месяца
            SLStyle itemRowHeaderStyle = new SLStyle();
            itemRowHeaderStyle.Font.FontName = "Arial";
            itemRowHeaderStyle.Font.FontSize = 14;
            itemRowHeaderStyle.SetWrapText(true);
            titleStyle.Font.Bold = true;
            itemRowHeaderStyle.SetVerticalAlignment(DocumentFormat.OpenXml.Spreadsheet.VerticalAlignmentValues.Center);
            itemRowHeaderStyle.SetHorizontalAlignment(DocumentFormat.OpenXml.Spreadsheet.HorizontalAlignmentValues.Center);
            itemRowHeaderStyle.Border.BottomBorder.BorderStyle = itemRowHeaderStyle.Border.TopBorder.BorderStyle = itemRowHeaderStyle.Border.LeftBorder.BorderStyle = itemRowHeaderStyle.Border.RightBorder.BorderStyle = DocumentFormat.OpenXml.Spreadsheet.BorderStyleValues.Thin;
            itemRowHeaderStyle.Border.BottomBorder.Color = itemRowHeaderStyle.Border.TopBorder.Color = itemRowHeaderStyle.Border.LeftBorder.Color = itemRowHeaderStyle.Border.RightBorder.Color = System.Drawing.Color.Black;



            #endregion

            if (str != string.Empty)
            {
                // Создаю документ
                using SLDocument doc = new();


               

                // Генерация колонок в зависимости от выбора Месяцев
                // Создаю объкт таблицы
                DataTable dt = new();

                //Затем в цикле надо задать колонки Месяцев
                foreach (var item in listMouth)
                {
                    dt.Columns.Add(item, typeof(string));
                    
                }

                // Задать стиль района Главного Заголовка
                doc.SetColumnWidth(1, 35);
                doc.SetRowHeight(1, 30);
                doc.SetCellStyle(1, 1, titleStyle);


                // Задать стили заголовков месяцев колонок
                for (int j = 2; j < listMouth.Count + 1; j++)
                {
                   doc.SetColumnWidth(j, 15);
                   doc.SetCellStyle(1, j, itemRowHeaderStyle);
                }



                //var u = DateTime.Now.Month;





                using ExDbContext db = new();

                doc.ImportDataTable(1, 1, dt, true);

                var getMyArea = db.Areas.Where(u => u.HidingArea == 1).OrderBy(u => u.AreaName).ToList();


                //doc.SetCellValue("A1", "Район");

              


                

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

        //Галочки поставить все и убрать
        private void AllCheckOrNo_Click(object sender, RoutedEventArgs e)
        {
            if (flag)
            {
                checkBox1.IsChecked = true;
                checkBox2.IsChecked = true;
                checkBox3.IsChecked = true;
                checkBox4.IsChecked = true;
                checkBox5.IsChecked = true;
                checkBox6.IsChecked = true;
                checkBox7.IsChecked = true;
                checkBox8.IsChecked = true;
                checkBox9.IsChecked = true;
                checkBox10.IsChecked = true;
                checkBox11.IsChecked = true;
                checkBox12.IsChecked = true;
                allCheckOrNo.Content = "Убрать все галочки";
                flag = false;
            }
            
            else if (!flag)
            {
                checkBox1.IsChecked = false;
                checkBox2.IsChecked = false;
                checkBox3.IsChecked = false;
                checkBox4.IsChecked = false;
                checkBox5.IsChecked = false;
                checkBox6.IsChecked = false;
                checkBox7.IsChecked = false;
                checkBox8.IsChecked = false;
                checkBox9.IsChecked = false;
                checkBox10.IsChecked = false;
                checkBox11.IsChecked = false;
                checkBox12.IsChecked = false;
                allCheckOrNo.Content = "Отметить все галочки";
                flag = true;
            }

        }
        #endregion


    }
}
