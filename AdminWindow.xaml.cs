using ClosedXML.Excel;
using DocumentFormat.OpenXml;
using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Spreadsheet;
using Microsoft.EntityFrameworkCore;
using Microsoft.Win32;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.IO;
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
using System.Windows.Shapes;

namespace exel_for_mfc
{
    public partial class AdminWindow : Window
    {
        public AdminWindow()
        {
            InitializeComponent();
            StartAdminWin();
        }
        #region Редактирование таблиц
        private void StartAdminWin()
        {
            using ExDbContext db = new();

            var AreaDataGrid = db.Areas.FromSqlRaw("SELECT * FROM Area").ToList();
            AreaX.ItemsSource = AreaDataGrid;

            var LocalDataGrid = db.Localities.FromSqlRaw("SELECT * FROM Locality").ToList();
            LocalX.ItemsSource = LocalDataGrid;

            var PayDataGrid = db.PayAmounts.FromSqlRaw("SELECT * FROM PayAmount").ToList();
            PayX.ItemsSource = PayDataGrid;

            var PrivelDataGrid = db.Privileges.FromSqlRaw("SELECT * FROM Privileges").ToList();
            PrivelX.ItemsSource = PrivelDataGrid;

            var SolDataGrid = db.SolutionTypes.FromSqlRaw("SELECT * FROM SolutionType").ToList();
            SolutionX.ItemsSource = SolDataGrid;

            var SolDataGridForAdmin = db.SolutionTypes.FromSqlRaw("SELECT * FROM SolutionType").Take(2).ToList();
            AdminsX.ItemsSource = SolDataGridForAdmin;

        }

        private async void AreaCell(object sender, DataGridCellEditEndingEventArgs e)
        {
            //Считывание строки
            Area? a = e.Row.Item as Area;

            using ExDbContext db = new();

            if (a.Id != 0)
            {
                //Обновление таблицы Район
                await db.Database.ExecuteSqlRawAsync("UPDATE Area SET AreaName = {0} WHERE Id = {1}", a.AreaName, a.Id);
            }

            else if (a.Id == 0)
            {
                // Добавление записи
                if (a.AreaName != null)
                {
                    //Добавить новую запись в таблицу Район
                    await db.Database.ExecuteSqlInterpolatedAsync($"INSERT INTO Area(AreaName) VALUES({a.AreaName})");
                    await Task.Delay(50);
                    StartAdminWin();
                }
                else return;
            }
        }

        private async void LocalCell(object sender, DataGridCellEditEndingEventArgs e)
        {
            //Считывание строки
            Locality? a = e.Row.Item as Locality;

            using ExDbContext db = new();

            if (a.Id != 0)
            {
                //Обновление таблицы Населенный пункт
                await db.Database.ExecuteSqlRawAsync("UPDATE Locality SET LocalName = {0} WHERE Id = {1}", a.LocalName, a.Id);
            }

            else if (a.Id == 0)
            {
                // Добавление записи
                if (a.LocalName != null)
                {
                    //Добавить новую запись в таблицу Населенный пункт
                    await db.Database.ExecuteSqlInterpolatedAsync($"INSERT INTO Locality(LocalName) VALUES({a.LocalName})");
                    await Task.Delay(50);
                    StartAdminWin();
                }
                else return;
            }
        }

        private async void LgotaCell(object sender, DataGridCellEditEndingEventArgs e)
        {
            //Считывание строки
            Privilege? a = e.Row.Item as Privilege;

            using ExDbContext db = new();

            if (a.Id != 0)
            {
                //Обновление таблицы Льгота
                await db.Database.ExecuteSqlRawAsync("UPDATE Privileges SET PrivilegesName = {0} WHERE Id = {1}", a.PrivilegesName, a.Id);
            }

            else if (a.Id == 0)
            {
                // Добавление записи
                if (a.PrivilegesName != null)
                {
                    //Добавить новую запись в таблицу Льгота
                    await db.Database.ExecuteSqlInterpolatedAsync($"INSERT INTO Privileges(PrivilegesName) VALUES({a.PrivilegesName})");
                    await Task.Delay(50);
                    StartAdminWin();
                }
                else return;
            }
        }

        private async void PayCell(object sender, DataGridCellEditEndingEventArgs e)
        {
            //Считывание строки
            PayAmount? a = e.Row.Item as PayAmount;

            using ExDbContext db = new();

            if (a.Id != 0)
            {
                //Обновление таблицы Выплаты
                await db.Database.ExecuteSqlRawAsync("UPDATE PayAmount SET PrivilegesName = {0} WHERE Id = {1}", a.Pay, a.Id);
            }

            else if (a.Id == 0)
            {
                // Добавление записи
                if (a.Pay != null)
                {
                    //Добавить новую запись в таблицу Выплаты
                    await db.Database.ExecuteSqlInterpolatedAsync($"INSERT INTO PayAmount(Pay) VALUES({a.Pay})");
                    await Task.Delay(50);
                    StartAdminWin();
                }
                else return;
            }
        }

        private async void SolutionCell(object sender, DataGridCellEditEndingEventArgs e)
        {
            //Считывание строки
            SolutionType? a = e.Row.Item as SolutionType;

            using ExDbContext db = new();

            if (a.Id != 0)
            {
                //Обновление таблицы Выплаты
                await db.Database.ExecuteSqlRawAsync("UPDATE SolutionType SET SolutionName = {0} WHERE Id = {1}", a.SolutionName, a.Id);
            }

            else if (a.Id == 0)
            {
                // Добавление записи
                if (a.SolutionName != null)
                {
                    //Добавить новую запись в таблицу Выплаты
                    await db.Database.ExecuteSqlInterpolatedAsync($"INSERT INTO SolutionType(SolutionName) VALUES({a.SolutionName})");
                    await Task.Delay(50);
                    StartAdminWin();
                }
                else return;
            }
        }

        private async void AdminCell(object sender, DataGridCellEditEndingEventArgs e)
        {
            //Считывание строки
            SolutionType? a = e.Row.Item as SolutionType;

            using ExDbContext db = new();

            if (a.Id != 0)
            {
                //Обновление таблицы Логинов и паролей
                await db.Database.ExecuteSqlRawAsync("UPDATE SolutionType SET Login = {0}, Passwords = {1} WHERE Id = {2}", a.Login, MD5Hash(a.Passwords), a.Id);
                StartAdminWin();
            }

            //Метод хэширования вводимого пароля
            static string MD5Hash(string input)
            {
                var md5 = MD5.Create();
                var hash = md5.ComputeHash(Encoding.UTF8.GetBytes(input));
                return Convert.ToBase64String(hash);
            }
        }
        #endregion

        [Obsolete]
        private void Button_Click(object sender, RoutedEventArgs e)
        {
            //Интеграция
            OpenFileDialog of = new()
            {
                Filter = "Excel Files|*.xls;*.xlsx;*.xlsm"
            };
            /*
             1 - фамилия
             2 - имя
             3 - отчество
             4 - снилс
             5 - район
             6 - населенный пункт
             7 - адрес
             8 - льгота будет Contains
             9 - серия и номер сертификата
            10 - дата выдачи
            11 - решение
            12 - дата и номер решения по сертификату
            13 - Выплата
            14 - Трек
            15 - Дата отправки почтой
            16 - Коммент
            Предусмотреть NULL
             */
            if (of.ShowDialog() == true)
            {
                using (FileStream fs = new FileStream(of.FileName, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
                {
                    using (SpreadsheetDocument doc = SpreadsheetDocument.Open(fs, false))
                    {
                        WorkbookPart workbookPart = doc.WorkbookPart;
                        SharedStringTablePart sstpart = workbookPart.GetPartsOfType<SharedStringTablePart>().First();
                        SharedStringTable sst = sstpart.SharedStringTable;

                        WorksheetPart worksheetPart = workbookPart.WorksheetParts.First();
                        Worksheet sheet = worksheetPart.Worksheet;

                        var cells = sheet.Descendants<Cell>();
                        int temp = 0;
                        //Второе условие срабатывает на цифры
                        // One way: go through each cell in the sheet
                        foreach (Cell cell in cells)
                        {
                            if ((cell.DataType != null) && (cell.DataType == CellValues.SharedString))
                            {
                                int ssid = int.Parse(cell.CellValue.Text);
                                string str = sst.ChildElements[ssid].InnerText;
                                MessageBox.Show($"{str} ");
                            }
                            else if (cell.CellValue != null)
                            {
                                MessageBox.Show($"числа {cell.CellValue.Text}");
                            }
                            else if (cell.DataType == null)
                            {
                                MessageBox.Show($"NULL");
                            }




                        }
                    }
                }

            }
        }
    }
}