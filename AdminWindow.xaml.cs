using ClosedXML.Excel;
using DocumentFormat.OpenXml;
using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Spreadsheet;
using DocumentFormat.OpenXml.Wordprocessing;
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
                        var app = new Applicant();
                        var reg = new Registry();
                        int temp = 0;

                        //Второе условие срабатывает на цифры
                        //Просто адский цикл
                        foreach (Cell cell in cells)
                        {

                            switch (temp)
                            {
                                case 0: break;
                                case 1: //Фамилия //Проверка на NULL каждой строки
                                    if ((cell.DataType != null) && (cell.DataType == CellValues.SharedString))
                                    {
                                        int s1 = int.Parse(cell.CellValue.Text);
                                        string str1 = sst.ChildElements[s1].InnerText;
                                        app.Firstname = str1;
                                    }
                                    //Числа
                                    else if (cell.CellValue != null)
                                    {
                                        app.Firstname = cell.CellValue.Text;
                                    }

                                    //NULL
                                    else if (cell.DataType == null)
                                    {
                                        app.Firstname = null;
                                    }
                                    break;

                                case 2: //Имя
                                    if ((cell.DataType != null) && (cell.DataType == CellValues.SharedString))
                                    {
                                        int s2 = int.Parse(cell.CellValue.Text);
                                        string str2 = sst.ChildElements[s2].InnerText;
                                        app.Middlename = str2;
                                    }
                                    //Числа
                                    else if (cell.CellValue != null)
                                    {
                                        app.Middlename = cell.CellValue.Text;
                                    }
                                    //NULL
                                    else if (cell.DataType == null)
                                    {
                                        app.Middlename = null;
                                    }
                                    break;

                                case 3: //Отчество
                                    if ((cell.DataType != null) && (cell.DataType == CellValues.SharedString))
                                    {
                                        int s3 = int.Parse(cell.CellValue.Text);
                                        string str3 = sst.ChildElements[s3].InnerText;
                                        app.Lastname = str3;
                                    }
                                    //Числа
                                    else if (cell.CellValue != null)
                                    {
                                        app.Lastname = cell.CellValue.Text;
                                    }

                                    //NULL
                                    else if (cell.DataType == null)
                                    {
                                        app.Lastname = null;
                                    }
                                    break;

                                case 4: //Снилс
                                    if ((cell.DataType != null) && (cell.DataType == CellValues.SharedString))
                                    {
                                        int s3 = int.Parse(cell.CellValue.Text);
                                        string str3 = sst.ChildElements[s3].InnerText;
                                        app.Snils = str3;
                                    }
                                    //Числа
                                    else if (cell.CellValue != null)
                                    {
                                        app.Snils = cell.CellValue.Text;
                                    }

                                    //NULL
                                    else if (cell.DataType == null)
                                    {
                                        app.Snils = null;
                                    }
                                    break;

                                case 5: //Район //Нужно получить значение иначе просто нулл
                                    if ((cell.DataType != null) && (cell.DataType == CellValues.SharedString))
                                    {
                                        int s3 = int.Parse(cell.CellValue.Text);
                                        string str3 = sst.ChildElements[s3].InnerText;
                                        app.AreaFk = ReturnIdArea(str3);
                                    }

                                    //Числа
                                    else if (cell.CellValue != null)
                                    {
                                        app.AreaFk = null;
                                    }

                                    //NULL
                                    else if (cell.DataType == null)
                                    {
                                        app.AreaFk = null;
                                    }
                                    break;

                                case 6:  //Населенный пункт
                                    Console.WriteLine("Ваше имя - Sam");
                                    break;
                                case 7:
                                    Console.WriteLine("Ваше имя - Sam");
                                    break;
                                case 8:
                                    Console.WriteLine("Ваше имя - Sam");
                                    break;
                                case 9:
                                    Console.WriteLine("Ваше имя - Sam");
                                    break;
                                case 10:
                                    Console.WriteLine("Ваше имя - Sam");
                                    break;
                                case 11:
                                    Console.WriteLine("Ваше имя - Sam");
                                    break;
                                case 12:
                                    Console.WriteLine("Ваше имя - Sam");
                                    break;
                                case 13:
                                    Console.WriteLine("Ваше имя - Sam");
                                    break;
                                case 14:
                                    Console.WriteLine("Ваше имя - Sam");
                                    break;
                                case 15: //Контрольное условие и вставка
                                    Console.WriteLine("Ваше имя - Sam");
                                    break;
                            }
                        }
                    }
                }

            }

            //Функция возврата Района
            static int ReturnIdArea(string str)
            {
                int idArea = 0;
                using (ExDbContext db = new())
                {
                    var equalArea = db.Areas.AsNoTracking().Where(u => u.AreaName == str).FirstOrDefault();
                    if (equalArea != null)
                        idArea = equalArea.Id;
                    
                    else if(equalArea == null)
                    {
                        Area area = new();
                        area.AreaName = str;
                        db.Areas.Add(area);
                        db.SaveChanges();
                        // И ветнуть id нового
                        var getIdLast = db.Areas.AsNoTracking().LastOrDefaultAsync();
                        if (getIdLast != null)
                            idArea = getIdLast.Id;
                        else
                            MessageBox.Show("Произошла непредвиденная ошибка", "Это не конец");
                    }
                }
                return idArea;
            }

            //Функция возврата Населенного пункта
            static int ReturnIdLocal(string str)
            {
                int idLocal = 0;
                using (ExDbContext db = new())
                {
                    var equalLoc = db.Localities.AsNoTracking().Where(u => u.LocalName == str).FirstOrDefault();
                    if (equalLoc != null)
                        idLocal = equalLoc.Id;

                    else if (equalLoc == null)
                    {
                        Locality loc = new();
                        loc.LocalName = str;
                        db.Localities.Add(loc);
                        db.SaveChanges();
                        // И ветнуть id нового
                        var getIdLast = db.Localities.AsNoTracking().LastOrDefaultAsync();
                        if (getIdLast != null)
                            idLocal = getIdLast.Id;
                        else
                            MessageBox.Show("Произошла непредвиденная ошибка", "Это не конец");
                    }
                }
                return idLocal;
            }

            //Функция возврата Льгота
            static int ReturnIdPriv(string str)
            {
                int idPriv = 0;
                using (ExDbContext db = new())
                {
                    var equalPriv = db.Privileges.AsNoTracking().Where(u => u.PrivilegesName == str).FirstOrDefault();
                    if (equalPriv != null)
                        idPriv = equalPriv.Id;

                    else if (equalPriv == null)
                    {
                        Privilege privilege = new();
                        privilege.PrivilegesName = str;
                        db.Privileges.Add(privilege);
                        db.SaveChanges();
                        // И ветнуть id нового
                        var getIdLast = db.Privileges.AsNoTracking().LastOrDefaultAsync();
                        if (getIdLast != null)
                            idPriv = getIdLast.Id;
                        else
                            MessageBox.Show("Произошла непредвиденная ошибка", "Это не конец");
                    }
                }
                return idPriv;
            }

            //Функция возврата Решение
            static int ReturnIdSol(string str)
            {
                return 0;
            }

            //Функция возврата Выплата
            static int ReturnIdPay(string str)
            {
                return 0;
            }
        }
    }
}