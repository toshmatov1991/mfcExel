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
using System.ComponentModel;
using System.Diagnostics;
using DocumentFormat.OpenXml.InkML;
using DocumentFormat.OpenXml.Office2016.Drawing.Charts;
using DocumentFormat.OpenXml.Presentation;
using DocumentFormat.OpenXml.Office2010.Excel;
using DocumentFormat.OpenXml.Office2016.Drawing.ChartDrawing;

namespace exel_for_mfc
{
    public partial class TableWindow : Window
    {
        #region База
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
        async void Start()
        {
            await GoStart();
        }
        async Task GoStart()
        {
            await Task.Run(() =>
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
                    Dispatcher.Invoke(() =>
                    {
                        dataGrid.ItemsSource = MyList;
                    });   

                        AreaCombobox = db.Areas.FromSqlRaw("SELECT * FROM Area").AsNoTracking().ToList();
                        LocalCombobox = db.Localities.FromSqlRaw("SELECT * FROM Locality").AsNoTracking().ToList();
                        PayCombobox = db.PayAmounts.FromSqlRaw("SELECT * FROM PayAmount").AsNoTracking().ToList();
                        PrivelCombobox = db.Privileges.FromSqlRaw("SELECT * FROM Privileges").AsNoTracking().ToList();
                        SolCombobox = db.SolutionTypes.FromSqlRaw("SELECT * FROM SolutionType").AsNoTracking().ToList();
                    };
               
            });
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

            else if (a.IdReg == 0)
            {
                // Добавление записи
                // Сначала проверка на заполнение всех полей
                // await db.Database.ExecuteSqlRawAsync("INSERT INTO Companies (Name) VALUES ({0})", " ");
                if (a.Family != null
                    && a.Name != null
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

                        //Отдельная проверка Снилса
                        var myQuerySnils = await db.Applicants.FromSqlRaw("SELECT * FROM Applicant WHERE Snils LIKE {0}", a.Snils).AsNoTracking().FirstOrDefaultAsync();

                        if (myQuery != null)
                        {
                            var myQuery1234 = from r in db.Registries.AsNoTracking()
                                              join ap in db.Applicants.AsNoTracking() on r.ApplicantFk equals ap.Id
                                              where ap.Snils == myQuery.Snils
                                              select new
                                              {
                                                  r.Id,
                                                  ap.Firstname,
                                                  ap.Middlename,
                                                  ap.Lastname,
                                                  r.SerialAndNumberSert,
                                                  r.DateGetSert,
                                                  sol = r.SolutionFk
                                              };



                            string str = "";

                            if (myQuery1234 == null)
                                return;
                            else
                            {
                                foreach (var item in myQuery1234)
                                {
                                    str += $"\nId-{item.Id} {item.Firstname} {item.Middlename.Substring(0, 1)}. {item.Lastname.Substring(0, 1)}. {item.SerialAndNumberSert} {Convert.ToDateTime(item.DateGetSert).ToString("d", new CultureInfo("Ru-ru"))} {ReturnStr(item.sol)}\n";
                                }

                                //Информировать что такая запись найдена
                                var result = MessageBox.Show($"{str}\n в таблице существуют записи данного заявителя\nДобавить новую запись в таблицу?", "Найдены совпадения!", MessageBoxButton.YesNo, MessageBoxImage.Information);

                                if (result == MessageBoxResult.Yes)
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

                                    //Запрос на получение Id последнего заявителя в таблице Applicant
                                    var getIdLastApp = await db.Applicants.AsNoTracking().OrderBy(u => u.Id).LastOrDefaultAsync();

                                    if (a.Pay == null || a.Solution == null || a.Pay == null && a.Solution == null)
                                    {
                                        //Добавить новую запись в таблицу Регистр
                                        await db.Database.ExecuteSqlInterpolatedAsync($"INSERT INTO Registry(Applicant_FK, SerialAndNumberSert, DateGetSert, PayAmount_FK, Solution_FK, DateAndNumbSolutionSert, Comment, Trek, MailingDate) VALUES({getIdLastApp.Id}, {a.Sernumb}, {a.DateGetSert}, {null}, {null}, {a.DateAndNumbSolutionSert}, {a.Comment}, {a.Trek}, {a.MailingDate})");
                                        await Task.Delay(100);
                                    }

                                    else if (a.Pay != null && a.Solution != null)
                                    {
                                        //Добавить новую запись в таблицу Регистр
                                        await db.Database.ExecuteSqlInterpolatedAsync($"INSERT INTO Registry(Applicant_FK, SerialAndNumberSert, DateGetSert, PayAmount_FK, Solution_FK, DateAndNumbSolutionSert, Comment, Trek, MailingDate) VALUES({getIdLastApp.Id}, {a.Sernumb}, {a.DateGetSert}, {a.Pay + 1}, {a.Solution + 1}, {a.DateAndNumbSolutionSert}, {a.Comment}, {a.Trek}, {a.MailingDate})");
                                        await Task.Delay(100);
                                    }
                                    
                            }
                                else if (result == MessageBoxResult.No)
                                    return;
                            }




                        }

                        //Обработка снилса
                        else if (myQuerySnils != null)
                        {

                            var myQuery1234 = from r in db.Registries.AsNoTracking()
                                              join ap in db.Applicants.AsNoTracking() on r.ApplicantFk equals ap.Id
                                              where ap.Snils == myQuerySnils.Snils
                                              select new
                                              {
                                                  r.Id,
                                                  ap.Firstname,
                                                  ap.Middlename,
                                                  ap.Lastname,
                                                  r.SerialAndNumberSert,
                                                  r.DateGetSert,
                                                  sol = r.SolutionFk
                                              };



                            string str = "";

                            if (myQuery1234 == null)
                                return;
                            else
                            {
                                foreach (var item in myQuery1234)
                                {
                                    str += $"\nId-{item.Id} {item.Firstname} {item.Middlename.Substring(0, 1)}. {item.Lastname.Substring(0, 1)}. {item.SerialAndNumberSert} {Convert.ToDateTime(item.DateGetSert).ToString("d", new CultureInfo("Ru-ru"))} {ReturnStr(item.sol)}\n";
                                }

                                //Информировать что такая запись найдена
                                var result = MessageBox.Show($"{str}\n в таблице существуют записи данного заявителя\nДобавить новую запись в таблицу?", "Найдены совпадения!", MessageBoxButton.YesNo, MessageBoxImage.Information);

                                if (result == MessageBoxResult.Yes)
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


                                    //Запрос на получение Id последнего заявителя в таблице Applicant
                                    var getIdLastApp = await db.Applicants.AsNoTracking().OrderBy(u => u.Id).LastOrDefaultAsync();

                                    //Добавить новую запись в таблицу Регистр
                                    if (a.Pay == null || a.Solution == null || a.Pay == null && a.Solution == null)
                                    {
                                        //Добавить новую запись в таблицу Регистр
                                        await db.Database.ExecuteSqlInterpolatedAsync($"INSERT INTO Registry(Applicant_FK, SerialAndNumberSert, DateGetSert, PayAmount_FK, Solution_FK, DateAndNumbSolutionSert, Comment, Trek, MailingDate) VALUES({getIdLastApp.Id}, {a.Sernumb}, {a.DateGetSert}, {null}, {null}, {a.DateAndNumbSolutionSert}, {a.Comment}, {a.Trek}, {a.MailingDate})");
                                        await Task.Delay(100);
                                    }

                                    else if (a.Pay != null && a.Solution != null)
                                    {
                                        //Добавить новую запись в таблицу Регистр
                                        await db.Database.ExecuteSqlInterpolatedAsync($"INSERT INTO Registry(Applicant_FK, SerialAndNumberSert, DateGetSert, PayAmount_FK, Solution_FK, DateAndNumbSolutionSert, Comment, Trek, MailingDate) VALUES({getIdLastApp.Id}, {a.Sernumb}, {a.DateGetSert}, {a.Pay + 1}, {a.Solution + 1}, {a.DateAndNumbSolutionSert}, {a.Comment}, {a.Trek}, {a.MailingDate})");
                                        await Task.Delay(100);
                                        
                                    }
                                  
                            }
                                else if (result == MessageBoxResult.No)
                                    return;
                            }
                        }

                        else if (myQuery == null && myQuerySnils == null)
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


                            //Запрос на получение Id последнего заявителя в таблице Applicant
                            var getIdLastApp = await db.Applicants.AsNoTracking().OrderBy(u => u.Id).LastOrDefaultAsync();


                            //Добавить новую запись в таблицу Регистр
                            if (a.Pay == null || a.Solution == null || a.Pay == null && a.Solution == null)
                            {
                                //Добавить новую запись в таблицу Регистр
                                await db.Database.ExecuteSqlInterpolatedAsync($"INSERT INTO Registry(Applicant_FK, SerialAndNumberSert, DateGetSert, PayAmount_FK, Solution_FK, DateAndNumbSolutionSert, Comment, Trek, MailingDate) VALUES({getIdLastApp.Id}, {a.Sernumb}, {a.DateGetSert}, {null}, {null}, {a.DateAndNumbSolutionSert}, {a.Comment}, {a.Trek}, {a.MailingDate})");
                                await Task.Delay(100);
                            }

                            else if (a.Pay != null && a.Solution != null)
                            {
                                //Добавить новую запись в таблицу Регистр
                                await db.Database.ExecuteSqlInterpolatedAsync($"INSERT INTO Registry(Applicant_FK, SerialAndNumberSert, DateGetSert, PayAmount_FK, Solution_FK, DateAndNumbSolutionSert, Comment, Trek, MailingDate) VALUES({getIdLastApp.Id}, {a.Sernumb}, {a.DateGetSert}, {a.Pay + 1}, {a.Solution + 1}, {a.DateAndNumbSolutionSert}, {a.Comment}, {a.Trek}, {a.MailingDate})");
                                await Task.Delay(100);
                            }
                        }
                    
                        Start();
                }
            }

            //Возвращю тип решения (строку)
            static string ReturnStr(int? t)
            {
               if(t == 1)
                   return "Выдан";
               else if(t == 2)
                    return "Отказ";
                else if (t == 3)
                    return "Аннулир.";
                else return "";

            }
        }

        //Обновить коммент(Нормально)
        private async void CommentUpdate(object sender, TextChangedEventArgs e)
        {
            if ((dataGrid.SelectedItem as SClass)?.IdReg == 0 || (dataGrid.SelectedItem as SClass)?.IdReg == null)
                return;
            else
            {
                try
                {
                    string a = "";
                    if (e.Source.ToString().Length == 31)
                        a = null;

                    else
                        a = e.OriginalSource.ToString().Substring(33);

                    using ExDbContext db = new();
                    await db.Database.ExecuteSqlRawAsync("UPDATE Registry SET Comment = {0} WHERE Id = {1}", a, (dataGrid.SelectedItem as SClass)?.IdReg);
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message, "Произошла ошибка при обновлении комментария");
                }
            }  
        }

        //Поиск
        private async void Button_Click_3(object sender, RoutedEventArgs e)
        {
            switch (filterSearch.Text)
            {
                case "По всем полям":
                    //По всем полям
                    await GoSerchNoPainHohuVTgu();
                    break;

                case "Фамилия":
                    await Task.Run(() =>
                    {
                        //Надо еще убрать пробелы
                        var filtered = MyList.Where(u => u.Family != null && u.Family.Replace(" ", "").ToLower().Contains(SearchTable.Text.Replace(" ", "").ToLower()));

                        Dispatcher.Invoke(() =>
                        {
                            dataGrid.ItemsSource = filtered;
                        });
                    });
                    break;

                case "Имя":
                    await Task.Run(() =>
                    {
                        //Надо еще убрать пробелы
                        var filtered = MyList.Where(u => u.Name != null && u.Name.Replace(" ", "").ToLower().Contains(SearchTable.Text.Replace(" ", "").ToLower()));

                        Dispatcher.Invoke(() =>
                        {
                            dataGrid.ItemsSource = filtered;
                        });
                    });
                    break;

                case "Отчество":
                    await Task.Run(() =>
                    {
                        //Надо еще убрать пробелы
                        var filtered = MyList.Where(u => u.Lastname != null && u.Lastname.Replace(" ", "").ToLower().Contains(SearchTable.Text.Replace(" ", "").ToLower()));

                        Dispatcher.Invoke(() =>
                        {
                            dataGrid.ItemsSource = filtered;
                        });
                    });
                    break;

                case "ФИО":
                    await Task.Run(() =>
                    {
                        //Надо еще убрать пробелы
                        var filtered = MyList.Where(u => $"{u.Family}{u.Name}{u.Lastname}".Replace(" ", "").ToLower().Contains(SearchTable.Text.Replace(" ", "").ToLower()));


                        Dispatcher.Invoke(() =>
                        {
                            dataGrid.ItemsSource = filtered;
                        });
                    });
                    break;

                case "Снилс":
                    await Task.Run(() =>
                    {
                        //Надо еще убрать пробелы
                        var filtered = MyList.Where(u => u.Snils != null  
                                                        && u.Snils.Replace(" ", "").Replace("-", "").Contains(SearchTable.Text.Replace(" ", "").Replace("-", "")));
                        if (filtered == null)
                            return;
                        else
                        {
                            Dispatcher.Invoke(() =>
                            {
                                dataGrid.ItemsSource = filtered;
                            });
                        }
                    });
                    break;

                case "Адрес":
                    await Task.Run(() =>
                    {
                        //Надо еще убрать пробелы
                        Dispatcher.Invoke(() =>
                        {
                            var filtered = MyList.Where(u => u.Adress != null 
                                                          && u.Adress.Replace(" ", "").ToLower().Contains(SearchTable.Text.Replace(" ", "").ToLower()));
                            dataGrid.ItemsSource = filtered;
                        });
                    });
                    break;

                case "Серия и номер сертификата":
                    await Task.Run(() =>
                    {
                        //Надо еще убрать пробелы
                        var filtered = MyList.Where(u => u.Sernumb != null && u.Sernumb.Replace(" ", "").ToLower().Contains(SearchTable.Text.Replace(" ", "").ToLower()));

                        Dispatcher.Invoke(() =>
                        {
                            dataGrid.ItemsSource = filtered;
                        });
                    });
                    break;

                case "По ID":
                    await Task.Run(() =>
                    {
                        //Надо еще убрать пробелы
                        var filtered = MyList.Where(u => u.IdReg.ToString() != null && u.IdReg.ToString().Replace(" ", "") == SearchTable.Text.Replace(" ", ""));

                        Dispatcher.Invoke(() =>
                        {
                            dataGrid.ItemsSource = filtered;
                        });
                    });
                    break;

                default:
                    await GoSerchNoPainHohuVTgu();
                    break;
            }
        }

        //Событие срабатывает когда поле очищается, и возвращает весь список в таблицу(нормально)
        private void ClearSearc(object sender, KeyEventArgs e)
        {
            if (string.IsNullOrEmpty(SearchTable.Text) || string.IsNullOrWhiteSpace(SearchTable.Text))
                dataGrid.ItemsSource = MyList;
        }

        //Задача поиска(нормально)
        async Task GoSerchNoPainHohuVTgu()
        {
            await Task.Run(() =>
            {
                //Надо еще убрать пробелы
                var filtered = MyList.Where(u => $"{u.IdReg}{u.Family}{u.Name}{u.Lastname}{u.Snils}{u.Adress}{u.Sernumb}".Replace(" ","").ToLower().Contains(SearchTable.Text.Replace(" ","").ToLower()));

                Dispatcher.Invoke(() =>
                {
                    dataGrid.ItemsSource = filtered;
                });
            });
        }
        #endregion

        #region События изменения значений ComboBox(Решено)
        private async void AreaComboEvent(object sender, EventArgs e)
        {

            if ((dataGrid.SelectedItem as SClass)?.IdReg == 0 || (dataGrid.SelectedItem as SClass)?.IdReg == null)
                return;
            else
            {
                try
                {
                    //Меняем район Заявителю
                    using ExDbContext db = new();
                    var GetId = await db.Areas.AsNoTracking().Where(u => u.AreaName == (sender as ComboBox).Text).FirstOrDefaultAsync();
                    if (GetId != null)
                        await db.Database.ExecuteSqlRawAsync("UPDATE Applicant SET Area_FK = {0} WHERE Id = {1}", GetId.Id, (dataGrid.SelectedItem as SClass)?.IdApplicant);
                    else
                        MessageBox.Show("Произошла ошибка при обновлении данных");
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Произошла ошибка, повторите попытку", ex.Message);
                }
            }
        }

        private async void LocalComboEvent(object sender, EventArgs e)
        {

            if ((dataGrid.SelectedItem as SClass)?.IdReg == 0 || (dataGrid.SelectedItem as SClass)?.IdReg == null)
                return;
            else
            {
                try
                {
                    //Меняем Населенный пункт Заявителю
                    using ExDbContext db = new();
                    var GetId = await db.Localities.AsNoTracking().Where(u => u.LocalName == (sender as ComboBox).Text).FirstOrDefaultAsync();
                    if (GetId != null)
                        await db.Database.ExecuteSqlRawAsync("UPDATE Applicant SET Locality_FK = {0} WHERE Id = {1}", GetId.Id, (dataGrid.SelectedItem as SClass)?.IdApplicant);
                    else
                        MessageBox.Show("Произошла ошибка при обновлении данных");
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Произошла ошибка, повторите попытку", ex.Message);
                }
            }
        }
        private async void PrivilegesComboEvent(object sender, EventArgs e)
        {

            if ((dataGrid.SelectedItem as SClass)?.IdReg == 0 || (dataGrid.SelectedItem as SClass)?.IdReg == null)
                return;
            else
            {
                try
                {
                    await Task.Delay(350);
                    using ExDbContext db = new();
                    var GetId = await db.Privileges.AsNoTracking().Where(u => u.PrivilegesName == (sender as ComboBox).Text).FirstOrDefaultAsync();
                    if (GetId != null)
                        await db.Database.ExecuteSqlRawAsync("UPDATE Applicant SET Privileges_FK = {0} WHERE Id = {1}", GetId.Id, (dataGrid.SelectedItem as SClass)?.IdApplicant);
                    else
                        MessageBox.Show("Произошла ошибка при обновлении данных\n Повторите попытку");
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Произошла ошибка, повторите попытку", ex.Message);
                }
            }
        }
        private async void PayComboEvent(object sender, EventArgs e)
        {

            if ((dataGrid.SelectedItem as SClass)?.IdReg == 0 || (dataGrid.SelectedItem as SClass)?.IdReg == null)
                return;
            else
            {
                try
                {
                    //Надо еще проверить является ли это существующей строкой или это новое
                    decimal dec = ReturnChislo((sender as ComboBox).Text.Replace(" ", ""));
                    using ExDbContext db = new();
                    var GetId = await db.PayAmounts.AsNoTracking().Where(u => u.Pay == dec).FirstOrDefaultAsync();
                    if (GetId != null)
                        await db.Database.ExecuteSqlRawAsync("UPDATE Registry SET PayAmount_FK = {0} WHERE Id = {1}", GetId.Id, (dataGrid.SelectedItem as SClass)?.IdReg);
                    else
                        MessageBox.Show("Произошла ошибка при обновлении данных");

                    static decimal ReturnChislo(string str)
                    {
                        string temp = "";
                        for (int i = 0; i < str.Length; i++)
                        {
                            if (char.IsDigit(str[i]))
                                temp += str[i];
                        }
                        return Convert.ToDecimal(temp);
                    }

                }
                catch (Exception ex)
                {
                    MessageBox.Show("Произошла ошибка, повторите попытку", ex.Message);
                }

            }

        }
        private async void SolutionComboEvent(object sender, EventArgs e)
        {

            if ((dataGrid.SelectedItem as SClass)?.IdReg == 0 || (dataGrid.SelectedItem as SClass)?.IdReg == null)
                return;
            else
            {
                try
                {
                    using ExDbContext db = new();
                    var GetId = await db.SolutionTypes.AsNoTracking().Where(u => u.SolutionName == (sender as ComboBox).Text).FirstOrDefaultAsync();
                    if (GetId != null)
                        await db.Database.ExecuteSqlRawAsync("UPDATE Registry SET Solution_FK = {0} WHERE Id = {1}", GetId.Id, (dataGrid.SelectedItem as SClass)?.IdReg);
                    else
                        MessageBox.Show("Произошла ошибка при обновлении данных");
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Произошла ошибка, повторите попытку", ex.Message);
                }
            }
        }
        #endregion

        #region Выгрузка в Excel(Решено)
        //Сохранить таблицу в Excel
        static async Task SaveDataInExel()
        {
            try
            {
                await Task.Run(async () =>
                {
                    SaveFileDialog dialog = new();
                    dialog.Filter = "Execl files (*.xlsx)|*.xlsx";

                    if (dialog.ShowDialog() == true)
                    {
                        // Lets converts our object data to Datatable for a simplified logic.
                        // Datatable is most easy way to deal with complex datatypes for easy reading and formatting. 
                        DataTable table = (DataTable)JsonConvert.DeserializeObject(JsonConvert.SerializeObject(MyList), typeof(DataTable));

                        using SpreadsheetDocument document = SpreadsheetDocument.Create(dialog.FileName, SpreadsheetDocumentType.Workbook);
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

                                    if (cell.InnerText == "")
                                    {
                                        newRow.AppendChild(cell);
                                    }
                                    else
                                    {
                                        using ExDbContext db = new();
                                        var GetNameOfArea = await db.Areas.Where(u => u.Id == Convert.ToInt32(cell.CellValue.Text) + 1).FirstOrDefaultAsync();
                                        cell.CellValue = new CellValue(GetNameOfArea.AreaName);
                                        newRow.AppendChild(cell);
                                    }

                                }

                                else if (col == "Local")
                                {
                                    Cell cell = new Cell
                                    {
                                        DataType = CellValues.String,
                                        CellValue = new CellValue(dsrow[col].ToString())//Тут значение Id
                                    };
                                    if (cell.InnerText == "")
                                    {
                                        newRow.AppendChild(cell);
                                    }
                                    else
                                    {
                                        using ExDbContext db = new();
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
                                    if (cell.InnerText == "")
                                    {
                                        newRow.AppendChild(cell);
                                    }
                                    else
                                    {
                                        using ExDbContext db = new();
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
                                    if (cell.InnerText == "")
                                    {
                                        newRow.AppendChild(cell);
                                    }
                                    else
                                    {
                                        using ExDbContext db = new();
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
                                    if (cell.InnerText == "")
                                    {
                                        newRow.AppendChild(cell);
                                    }
                                    else
                                    {
                                        using ExDbContext db = new();
                                        var GetNameOfLocal = await db.SolutionTypes.Where(u => u.Id == Convert.ToInt32(cell.CellValue.Text) + 1).FirstOrDefaultAsync();
                                        cell.CellValue = new CellValue(GetNameOfLocal.SolutionName);
                                        newRow.AppendChild(cell);
                                    }
                                }

                                else if (col == "DateGetSert" || col == "MailingDate")
                                {
                                    Cell cell = new Cell();
                                    cell.DataType = CellValues.String;
                                    cell.CellValue = new CellValue(dsrow[col].ToString());//Тут значение Id
                                    if (cell.InnerText == "")
                                    {
                                        newRow.AppendChild(cell);
                                    }

                                    else
                                    {
                                        cell.DataType = CellValues.String;
                                        cell.CellValue = new CellValue(Convert.ToDateTime(dsrow[col].ToString()).ToString("d"));//Тут значение Id

                                        newRow.AppendChild(cell);
                                    }

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

                    static string DoOperation(string str)
                    {
                        return str switch
                        {
                            "IdReg" => "№ п/п",
                            "Family" => "Фамилия",
                            "Name" => "Имя",
                            "Lastname" => "Отчество",
                            "Snils" => "Снилс",
                            "Area" => "Район",
                            "Local" => "Населенный пункт",
                            "Adress" => "Адрес",
                            "Lgota" => "Льгота",
                            "Sernumb" => "Серия и номер сертификата",
                            "DateGetSert" => "Дата выдачи сертификата",
                            "Solution" => "Решение",
                            "DateAndNumbSolutionSert" => "Дата и номер решения",
                            "Trek" => "Трек",
                            "Pay" => "Размер выплаты",
                            "MailingDate" => "Дата отправки",
                            "Comment" => "Комментарий",
                            _ => "",
                        };
                    }
                });
            }
            catch (Exception ex)
            {

                MessageBox.Show(ex.Message);
            }
         
        }

        //Выгрузить в Excel
        private async void Button_Click(object sender, RoutedEventArgs e)
        {
            await SaveDataInExel();
        }
        #endregion

        #region Фильтрация(в процессе)
        //Использовать для данных datagrid и включить autogeneratecolumns


        //Обновить фильтры
        private void Button_Click_2(object sender, RoutedEventArgs e)
        {

        }
        #endregion

        #region Обработка возможных исключений и другие мелочи
        private void AreaExeption(object sender, MouseButtonEventArgs e)
        {
            return;
        }
        private void AreaKeyDown(object sender, KeyEventArgs e)
        {
            return;
        }
        private void AreaKeyUp(object sender, KeyEventArgs e)
        {
            return;
        }

        private async void Button_Click_1(object sender, RoutedEventArgs e)
        {
            await GoStart();
        }

       


        #endregion
    }
}