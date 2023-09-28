using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Spreadsheet;
using DocumentFormat.OpenXml;
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualBasic;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using Newtonsoft.Json;
using Microsoft.Win32;
using System.IO.Packaging;
using System.Globalization;
using Microsoft.Data.SqlClient;
using System.Windows.Controls.Primitives;

namespace exel_for_mfc
{
    public partial class TableWindow : Window
    {
        public static List<Area>? AreaCombobox { get; set; }
        public static List<Locality>? LocalCombobox { get; set; }
        public static List<PayAmount>? PayCombobox { get; set; }
        public static List<Privilege>? PrivelCombobox { get; set; }
        public static List<SolutionType>? SolCombobox { get; set; }
        public static List<SClass>? MyList { get; set; }
        public TableWindow()
        {
            InitializeComponent();
            Start();
        }
        //Запрос для заполнения таблицы
        void Start()
        {
            using (ExDbContext db = new())
            {
                MyList = (from reg in db.Registries
                              join appl in db.Applicants on reg.ApplicantFk equals appl.Id
                              select new SClass
                              {
                                  IdReg = reg.Id,
                                  Family = appl.Firstname,
                                  Name = appl.Middlename,
                                  Lastname = appl.Lastname,
                                  Snils = appl.Snils,
                                  Area = appl.AreaFk - 1,
                                  Local = appl.LocalityFk - 1,
                                  Adress = appl.Adress,
                                  Lgota = appl.PrivilegesFk - 1,
                                  Pay = reg.PayAmountFk - 1,
                                  Sernumb = reg.SerialAndNumberSert,
                                  DateGetSert = reg.DateGetSert,
                                  Solution = reg.SolutionFk - 1,
                                  DateAndNumbSolutionSert = reg.DateAndNumbSolutionSert,
                                  Comment = reg.Comment,
                                  Trek = reg.Trek,
                                  MailingDate = reg.MailingDate,
                                  IdApplicant = appl.Id
                              }).AsNoTracking().ToList();

                dataGrid.ItemsSource = MyList;

                AreaCombobox = db.Areas.FromSqlRaw("SELECT * FROM Area").AsNoTracking().ToList();
                LocalCombobox = db.Localities.FromSqlRaw("SELECT * FROM Locality").AsNoTracking().ToList();
                PayCombobox = db.PayAmounts.FromSqlRaw("SELECT * FROM PayAmount").AsNoTracking().ToList();
                PrivelCombobox = db.Privileges.FromSqlRaw("SELECT * FROM Privileges").AsNoTracking().ToList();
                SolCombobox = db.SolutionTypes.FromSqlRaw("SELECT * FROM SolutionType").AsNoTracking().ToList();
            };
        }
        //Событие редактирования ячейки
        public async void dataGrid_CellEditEnding(object sender, DataGridCellEditEndingEventArgs e)
        {
            //Считывание строки
            SClass? a = e.Row.Item as SClass;

            using ExDbContext db = new();

            if (a.IdReg != 0)
            {
                // Редактирование ячейки (Обновление строки) - Заявитель - Регистр

                //Обновление таблицы Заявитель
                await db.Database.ExecuteSqlRawAsync("UPDATE Applicant SET Firstname = {0}, Middlename = {1}, Lastname = {2}, Adress = {3}, Snils = {4} WHERE Id = {5}", a.Family, a.Name, a.Lastname, a.Adress, a.Snils, a.IdApplicant);

                //Обновление таблицы Регистр
                await db.Database.ExecuteSqlRawAsync("UPDATE Registry SET SerialAndNumberSert = {0}, DateGetSert = {1}, DateAndNumbSolutionSert = {2}, Comment = {3}, Trek = {4}, MailingDate = {5} WHERE Id = {6}", a.Sernumb, a.DateGetSert, a.DateAndNumbSolutionSert, a.Comment, a.Trek, a.MailingDate, a.IdReg);
            }

            else if(a.IdReg == 0)
            {
                
                // Добавление записи
                // Сначала проверка на заполнение всех полей
                // await db.Database.ExecuteSqlRawAsync("INSERT INTO Companies (Name) VALUES ({0})", " ");
                if (a.Family != null 
                    && a.Name!= null
                    && a.Lastname != null
                    && a.Adress != null
                    && a.Area != null
                    && a.Local != null
                    && a.Snils != null)
                { 
                    //Сначала проверяю на наличие такого же человека в БД, если его нету,
                    //то вставляю новую запись в таблицу Заявители,
                    // Иначе просто беру ID того чела который уже есть в базе такой же

                    //Жуткая проверка
                    var myQuery = await db.Applicants.FromSqlRaw("SELECT * FROM Applicant WHERE Firstname LIKE {0} AND Middlename LIKE {1} AND Lastname LIKE {2} AND Adress LIKE {3} AND Snils LIKE {4}", a.Family, a.Name, a.Lastname, a.Adress, a.Snils).AsNoTracking().FirstOrDefaultAsync();

                    if (myQuery != null)
                    {
                        var myQuery123 = await db.Registries.FromSqlRaw("SELECT * FROM Registry WHERE Applicant_FK = {0}", myQuery.Id).AsNoTracking().ToListAsync();

                        //Информировать что такая запись найдена
                        var result = MessageBox.Show($"{a.Family} {a.Name} {a.Lastname}\n в таблице существуют {myQuery123.Count} записи данного заявителя\nДобавить новую запись в таблицу?", "Найдены совпадения!", MessageBoxButton.YesNo, MessageBoxImage.Information);
                        if (result == MessageBoxResult.Yes)
                        {
                            if(a.Pay == null || a.Solution == null || a.Pay == null && a.Solution == null)
                            {
                                //Добавить новую запись в таблицу Регистр
                                await db.Database.ExecuteSqlInterpolatedAsync($"INSERT INTO Registry(Applicant_FK, SerialAndNumberSert, DateGetSert, PayAmount_FK, Solution_FK, DateAndNumbSolutionSert, Comment, Trek, MailingDate) VALUES({await db.Applicants.CountAsync()}, {a.Sernumb}, {a.DateGetSert}, {null}, {null}, {a.DateAndNumbSolutionSert}, {a.Comment}, {a.Trek}, {a.MailingDate})");
                                await Task.Delay(50);
                                Start();
                            }

                            else if(a.Pay != null && a.Solution != null)
                            {
                                //Добавить новую запись в таблицу Регистр
                                await db.Database.ExecuteSqlInterpolatedAsync($"INSERT INTO Registry(Applicant_FK, SerialAndNumberSert, DateGetSert, PayAmount_FK, Solution_FK, DateAndNumbSolutionSert, Comment, Trek, MailingDate) VALUES({await db.Applicants.CountAsync()}, {a.Sernumb}, {a.DateGetSert}, {a.Pay + 1}, {a.Solution + 1}, {a.DateAndNumbSolutionSert}, {a.Comment}, {a.Trek}, {a.MailingDate})");
                                await Task.Delay(50);
                                Start();
                            }
                        }
                        else if (result == MessageBoxResult.No)
                            return;

                    }


                    else if (myQuery == null)
                    {
                        
                        if (a.Lgota == null)
                        {
                            //Добавить новую запись в таблицу заявитель
                            await db.Database.ExecuteSqlInterpolatedAsync($"INSERT INTO Applicant(Firstname, Middlename, Lastname, Area_FK, Locality_FK, Adress, Snils, Privileges_FK) VALUES({a.Family}, {a.Name}, {a.Lastname}, {a.Area + 1}, {a.Local + 1}, {a.Adress}, {a.Snils}, {null})");
                        }

                        else if (a.Area != null && a.Local != null && a.Lgota != null)
                        {
                            //Добавить новую запись в таблицу заявитель
                            await db.Database.ExecuteSqlInterpolatedAsync($"INSERT INTO Applicant(Firstname, Middlename, Lastname, Area_FK, Locality_FK, Adress, Snils, Privileges_FK) VALUES({a.Family}, {a.Name}, {a.Lastname}, {a.Area + 1}, {a.Local + 1}, {a.Adress}, {a.Snils}, {a.Lgota + 1})");
                        }

                        //Добавить новую запись в таблицу Регистр
                        if (a.Pay == null || a.Solution == null || a.Pay == null && a.Solution == null)
                        {
                            //Добавить новую запись в таблицу Регистр
                            await db.Database.ExecuteSqlInterpolatedAsync($"INSERT INTO Registry(Applicant_FK, SerialAndNumberSert, DateGetSert, PayAmount_FK, Solution_FK, DateAndNumbSolutionSert, Comment, Trek, MailingDate) VALUES({await db.Applicants.CountAsync()}, {a.Sernumb}, {a.DateGetSert}, {null}, {null}, {a.DateAndNumbSolutionSert}, {a.Comment}, {a.Trek}, {a.MailingDate})");
                            await Task.Delay(100);
                            Start();
                        }

                        else if (a.Pay != null && a.Solution != null)
                        {
                            //Добавить новую запись в таблицу Регистр
                            await db.Database.ExecuteSqlInterpolatedAsync($"INSERT INTO Registry(Applicant_FK, SerialAndNumberSert, DateGetSert, PayAmount_FK, Solution_FK, DateAndNumbSolutionSert, Comment, Trek, MailingDate) VALUES({await db.Applicants.CountAsync()}, {a.Sernumb}, {a.DateGetSert}, {a.Pay + 1}, {a.Solution + 1}, {a.DateAndNumbSolutionSert}, {a.Comment}, {a.Trek}, {a.MailingDate})");
                            await Task.Delay(100);
                            Start();
                        }
                       
                    }
                }
               
            }
           
        }

        #region События изменения значений ComboBox
        private async void AreaComboEvent(object sender, EventArgs e)
        {
            //Меняем район Заявителю
            using ExDbContext db = new();
            await db.Database.ExecuteSqlRawAsync("UPDATE Applicant SET Area_FK = {0} WHERE Id = {1}", (sender as ComboBox)?.SelectedIndex + 1, (dataGrid.SelectedItem as SClass)?.IdApplicant);
        }
        private async void LocalComboEvent(object sender, EventArgs e)
        {
            //Меняем Населенный пункт Заявителю
            using ExDbContext db = new();
            await db.Database.ExecuteSqlRawAsync("UPDATE Applicant SET Locality_FK = {0} WHERE Id = {1}", (sender as ComboBox)?.SelectedIndex + 1, (dataGrid.SelectedItem as SClass)?.IdApplicant);
        }
        private async void PrivilegesComboEvent(object sender, EventArgs e)
        {
            using ExDbContext db = new();
            await db.Database.ExecuteSqlRawAsync("UPDATE Applicant SET Privileges_FK = {0} WHERE Id = {1}", (sender as ComboBox)?.SelectedIndex + 1, (dataGrid.SelectedItem as SClass)?.IdApplicant);
        }
        private async void PayComboEvent(object sender, EventArgs e)
        {
            using ExDbContext db = new();
            await db.Database.ExecuteSqlRawAsync("UPDATE Registry SET PayAmount_FK = {0} WHERE Id = {1}", (sender as ComboBox)?.SelectedIndex + 1, (dataGrid.SelectedItem as SClass)?.IdReg);
        }
        private async void SolutionComboEvent(object sender, EventArgs e)
        {
            using ExDbContext db = new();
            await db.Database.ExecuteSqlRawAsync("UPDATE Registry SET Solution_FK = {0} WHERE Id = {1}", (sender as ComboBox)?.SelectedIndex + 1, (dataGrid.SelectedItem as SClass)?.IdReg);
        }
        #endregion

        //Сохранить таблицу в Excel
        static async Task SaveDataInExel()
        {
            await Task.Run(async () =>
            {
                SaveFileDialog dialog = new SaveFileDialog();
                dialog.Filter = "Execl files (*.xlsx)|*.xlsx";

                if (dialog.ShowDialog() == true)
                {
                    // Lets converts our object data to Datatable for a simplified logic.
                    // Datatable is most easy way to deal with complex datatypes for easy reading and formatting. 
                    DataTable table = (DataTable)JsonConvert.DeserializeObject(JsonConvert.SerializeObject(MyList), typeof(DataTable));

                    using (SpreadsheetDocument document = SpreadsheetDocument.Create(dialog.FileName, SpreadsheetDocumentType.Workbook))
                    {
                        WorkbookPart workbookPart = document.AddWorkbookPart();
                        workbookPart.Workbook = new Workbook();

                        WorksheetPart worksheetPart = workbookPart.AddNewPart<WorksheetPart>();

                        var sheetData = new SheetData();

                        worksheetPart.Worksheet = new Worksheet(sheetData);

                        // Create custom widths for columns - Здесь задаем ширину колонок
                        Columns lstColumns = worksheetPart.Worksheet.GetFirstChild<Columns>();
                        bool needToInsertColumns = false;
                        if (lstColumns == null)
                        {
                            lstColumns = new Columns();
                            needToInsertColumns = true;
                        }
                        // Min = 1, Max = 1 ==> Apply this to column 1 (A)
                        // Min = 2, Max = 2 ==> Apply this to column 2 (B)
                        // Width = 25 ==> Set the width to 25
                        // CustomWidth = true ==> Tell Excel to use the custom width
                        lstColumns.Append(new Column() { Min = 1, Max = 1, Width = 7, CustomWidth = true });  // id
                        lstColumns.Append(new Column() { Min = 2, Max = 2, Width = 20, CustomWidth = true }); // f
                        lstColumns.Append(new Column() { Min = 3, Max = 3, Width = 20, CustomWidth = true }); // n
                        lstColumns.Append(new Column() { Min = 4, Max = 4, Width = 20, CustomWidth = true }); // l
                        lstColumns.Append(new Column() { Min = 5, Max = 5, Width = 15, CustomWidth = true }); // snils
                        lstColumns.Append(new Column() { Min = 6, Max = 6, Width = 25, CustomWidth = true }); // район
                        lstColumns.Append(new Column() { Min = 7, Max = 7, Width = 25, CustomWidth = true }); // населенный
                        lstColumns.Append(new Column() { Min = 8, Max = 8, Width = 30, CustomWidth = true }); // адрес
                        lstColumns.Append(new Column() { Min = 9, Max = 9, Width = 35, CustomWidth = true }); // льгота
                        lstColumns.Append(new Column() { Min = 10, Max = 10, Width = 18, CustomWidth = true }); // размер выплаты
                        lstColumns.Append(new Column() { Min = 11, Max = 11, Width = 30, CustomWidth = true }); // серия и номер серта
                        lstColumns.Append(new Column() { Min = 12, Max = 12, Width = 20, CustomWidth = true }); // дата выдачи серта
                        lstColumns.Append(new Column() { Min = 13, Max = 13, Width = 15, CustomWidth = true }); // решение
                        lstColumns.Append(new Column() { Min = 14, Max = 14, Width = 25, CustomWidth = true }); // дата и номер решения
                        lstColumns.Append(new Column() { Min = 15, Max = 15, Width = 25, CustomWidth = true }); // трек
                        lstColumns.Append(new Column() { Min = 16, Max = 16, Width = 25, CustomWidth = true }); // дата отправки почтой
                        lstColumns.Append(new Column() { Min = 17, Max = 17, Width = 15, CustomWidth = true }); // дата отправки почтой
                        // Only insert the columns if we had to create a new columns element
                        if (needToInsertColumns)
                            worksheetPart.Worksheet.InsertAt(lstColumns, 0);




                        Sheets sheets = workbookPart.Workbook.AppendChild(new Sheets());
                        Sheet sheet = new() { Id = workbookPart.GetIdOfPart(worksheetPart), SheetId = 1, Name = "Sheet1" };

                        sheets.Append(sheet);

                        Row headerRow = new();

                        //Здесь постройка и название колонок
                        List<string> columns = new();
                        foreach (DataColumn column in table.Columns)
                        {
                            columns.Add(column.ColumnName);
                            Cell cell = new()
                            {
                                DataType = CellValues.String,
                                CellValue = new CellValue(DoOperation(column.ColumnName))
                            };
                            headerRow.AppendChild(cell);

                        }

                        sheetData.AppendChild(headerRow);

                        //Данные заносятся здесь
                        foreach (DataRow dsrow in table.Rows)
                        {
                            Row newRow = new Row();

                            foreach (string col in columns)
                            {
                                if (col == "Area")
                                {
                                    Cell cell = new()
                                    {
                                        DataType = CellValues.String,
                                        CellValue = new CellValue(dsrow[col].ToString())//Тут значение Id
                                    };
                                    using (ExDbContext db = new())
                                    {
                                        var GetNameOfArea = await db.Areas.Where(u => u.Id == Convert.ToInt32(cell.CellValue.Text) + 1).FirstOrDefaultAsync();
                                        cell.CellValue = new CellValue(GetNameOfArea.AreaName);
                                        newRow.AppendChild(cell);
                                    }
                                }

                                else if (col == "Local")
                                {
                                    Cell cell = new Cell();
                                    cell.DataType = CellValues.String;
                                    cell.CellValue = new CellValue(dsrow[col].ToString());//Тут значение Id
                                    using (ExDbContext db = new())
                                    {
                                        var GetNameOfLocal = await db.Localities.Where(u => u.Id == Convert.ToInt32(cell.CellValue.Text) + 1).FirstOrDefaultAsync();
                                        cell.CellValue = new CellValue(GetNameOfLocal.LocalName);
                                        newRow.AppendChild(cell);
                                    }
                                }

                                else if (col == "Lgota")
                                {
                                    Cell cell = new()
                                    {
                                        DataType = CellValues.String,
                                        CellValue = new CellValue(dsrow[col].ToString())//Тут значение Id
                                    };
                                    using (ExDbContext db = new())
                                    {
                                        var GetNameOfLocal = await db.Privileges.Where(u => u.Id == Convert.ToInt32(cell.CellValue.Text) + 1).FirstOrDefaultAsync();
                                        cell.CellValue = new CellValue(GetNameOfLocal.PrivilegesName);
                                        newRow.AppendChild(cell);
                                    }
                                }

                                else if (col == "Pay")
                                {
                                    Cell cell = new Cell();
                                    cell.DataType = CellValues.String;
                                    cell.CellValue = new CellValue(dsrow[col].ToString());//Тут значение Id
                                    using (ExDbContext db = new())
                                    {
                                        var GetNameOfLocal = await db.PayAmounts.Where(u => u.Id == Convert.ToInt32(cell.CellValue.Text) + 1).FirstOrDefaultAsync();
                                        cell.CellValue = new CellValue((decimal)GetNameOfLocal.Pay);
                                        newRow.AppendChild(cell);
                                    }
                                }

                                else if (col == "Solution")
                                {
                                    Cell cell = new Cell();
                                    cell.DataType = CellValues.String;
                                    cell.CellValue = new CellValue(dsrow[col].ToString());//Тут значение Id
                                    using (ExDbContext db = new())
                                    {
                                        var GetNameOfLocal = await db.SolutionTypes.Where(u => u.Id == Convert.ToInt32(cell.CellValue.Text) + 1).FirstOrDefaultAsync();
                                        cell.CellValue = new CellValue(GetNameOfLocal.SolutionName);
                                        newRow.AppendChild(cell);
                                    }
                                }

                                else if (col == "DateGetSert" || col == "MailingDate")
                                {
                                    Cell cell = new()
                                    {
                                        DataType = CellValues.String,
                                        CellValue = new CellValue(Convert.ToDateTime(dsrow[col].ToString()).ToString("d", new CultureInfo("Ru-ru")))//Тут значение Id
                                    };
                                    newRow.AppendChild(cell);
                                }

                                else if (col == "IdApplicant")
                                    continue;

                                else
                                {
                                    Cell cell = new()
                                    {
                                        DataType = CellValues.String,
                                        CellValue = new CellValue(dsrow[col].ToString())//Тут значение Id
                                    };
                                    newRow.AppendChild(cell);
                                }
                            }
                            sheetData.AppendChild(newRow);
                        }

                        workbookPart.Workbook.Save();
                    }


                }

                static string DoOperation(string str)
                {
                    switch (str)
                    {
                        case "IdReg": return "№ п/п";
                        case "Family": return "Фамилия";
                        case "Name": return "Имя";
                        case "Lastname": return "Отчество";
                        case "Snils": return "Снилс";
                        case "Area": return "Район";
                        case "Local": return "Населенный пункт";
                        case "Adress": return "Адрес";
                        case "Lgota": return "Льгота";
                        case "Sernumb": return "Серия и номер сертификата";
                        case "DateGetSert": return "Дата выдачи сертификата";
                        case "Solution": return "Решение";
                        case "DateAndNumbSolutionSert": return "Дата и номер решения";
                        case "Trek": return "Трек";
                        case "Pay": return "Размер выплаты";
                        case "MailingDate": return "Дата отправки";
                        case "Comment": return "Комментарий";
                        default: return "";
                    }
                }
            });
        }

        //Выгрузить в Excel
        private async void Button_Click(object sender, RoutedEventArgs e)
        {
            await SaveDataInExel();
        }

        //Обновить коммент
        private async void CommentUpdate(object sender, TextChangedEventArgs e)
        {
            string a = "";
            if (e.Source.ToString().Length == 31)
                a = null;

            else
                a = e.OriginalSource.ToString().Substring(33);

            using ExDbContext db = new();
            await db.Database.ExecuteSqlRawAsync("UPDATE Registry SET Comment = {0} WHERE Id = {1}", a, (dataGrid.SelectedItem as SClass)?.IdReg);
        }

        //Поиск
        private void GoSearchToTable(object sender, KeyEventArgs e)
        {
            //SearchTable
            using (ExDbContext db = new())
            {
                MyList = (from reg in db.Registries
                          join appl in db.Applicants on reg.ApplicantFk equals appl.Id
                          select new SClass
                          {
                              IdReg = reg.Id,
                              Family = appl.Firstname,
                              Name = appl.Middlename,
                              Lastname = appl.Lastname,
                              Snils = appl.Snils,
                              Area = appl.AreaFk - 1,
                              Local = appl.LocalityFk - 1,
                              Adress = appl.Adress,
                              Lgota = appl.PrivilegesFk - 1,
                              Pay = reg.PayAmountFk - 1,
                              Sernumb = reg.SerialAndNumberSert,
                              DateGetSert = reg.DateGetSert,
                              Solution = reg.SolutionFk - 1,
                              DateAndNumbSolutionSert = reg.DateAndNumbSolutionSert,
                              Comment = reg.Comment,
                              Trek = reg.Trek,
                              MailingDate = reg.MailingDate,
                              IdApplicant = appl.Id
                          }).Where(u => u.Family.Contains(SearchTable.Text)
                                     || u.Name.Contains(SearchTable.Text)
                                     || u.Lastname.Contains(SearchTable.Text)
                                     || u.Snils.Contains(SearchTable.Text)
                                     || u.Adress.Contains(SearchTable.Text)
                                     || u.Sernumb.Contains(SearchTable.Text)).AsNoTracking().ToList();

                dataGrid.ItemsSource = MyList;
            }
            
        }
    }
}