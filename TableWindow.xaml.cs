using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Spreadsheet;
using DocumentFormat.OpenXml;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using Newtonsoft.Json;
using Microsoft.Win32;
using System.Globalization;
using exel_for_mfc.FilterDB;
using DocumentFormat.OpenXml.Vml.Office;
using LinqKit;

namespace exel_for_mfc
{
    public partial class TableWindow : Window
    {
        #region База
        public static string temp1 = "";
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
            FilterStart();
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
                          }).ToList();

                dataGrid.ItemsSource = MyList;

                AreaCombobox = db.Areas.FromSqlRaw("SELECT * FROM Area").ToList();
                LocalCombobox = db.Localities.FromSqlRaw("SELECT * FROM Locality").ToList();
                PayCombobox = db.PayAmounts.FromSqlRaw("SELECT * FROM PayAmount").ToList();
                PrivelCombobox = db.Privileges.FromSqlRaw("SELECT * FROM Privileges").ToList();
                SolCombobox = db.SolutionTypes.FromSqlRaw("SELECT * FROM SolutionType").ToList();
            };
        }
      
        //Событие редактирования ячейки
        public async void dataGrid_CellEditEnding(object sender, DataGridCellEditEndingEventArgs e)
        {
            //Считывание строки
            SClass? a = e.Row.Item as SClass;

            using ExDbContext db = new();

            #region Обновление записи()
            if (a.IdReg != 0)
            {
                // Редактирование ячейки (Обновление строки) - Заявитель - Регистр

                //Обновление таблицы Заявитель
                var upApp = await db.Database.ExecuteSqlRawAsync("UPDATE Applicant SET Firstname = {0}, Middlename = {1}, Lastname = {2}, Adress = {3}, Snils = {4} WHERE Id = {5}", a.Family, a.Name, a.Lastname, a.Adress, a.Snils, a.IdApplicant) ;
                if (upApp == 0)
                    MessageBox.Show("Произошла ошибка при обновлении таблицы(Заявитель)\nПовторите попытку");
                //Обновление таблицы Регистр
                var upReg = await db.Database.ExecuteSqlRawAsync("UPDATE Registry SET SerialAndNumberSert = {0}, DateGetSert = {1}, DateAndNumbSolutionSert = {2}, Comment = {3}, Trek = {4}, MailingDate = {5} WHERE Id = {6}", a.Sernumb, a.DateGetSert, a.DateAndNumbSolutionSert, a.Comment, a.Trek, a.MailingDate, a.IdReg);
                if (upReg == 0)
                    MessageBox.Show("Произошла ошибка при обновлении таблицы(Регистр)\nПовторите попытку");
            }
            #endregion

            #region Добавление записи и проверки всякие)
            else if (a.IdReg == 0)
            {
                // Добавление записи
                // Сначала проверка на заполнение всех полей
                if (   !string.IsNullOrEmpty(a.Family)
                    && !string.IsNullOrEmpty(a.Name)
                    && !string.IsNullOrEmpty(a.Lastname)
                    && !string.IsNullOrEmpty(a.Adress)
                    && a.Area != null
                    && a.Local != null
                    && !string.IsNullOrEmpty(a.Snils))
                {

                    //Проверка-запрос ФИО Адрес Снилс
                    var myQuery = await db.Applicants.FromSqlRaw("SELECT * FROM Applicant WHERE Firstname LIKE {0} AND Middlename LIKE {1} AND Lastname LIKE {2} AND Adress LIKE {3} AND Snils LIKE {4}", a.Family.Replace(" ", ""), a.Name.Replace(" ", ""), a.Lastname.Replace(" ", ""), a.Adress, a.Snils).AsNoTracking().FirstOrDefaultAsync();

                    //Проверка-запрос Снилс
                    var myQuerySnils = await db.Applicants.FromSqlRaw("SELECT * FROM Applicant WHERE Snils LIKE {0}", a.Snils).AsNoTracking().FirstOrDefaultAsync();

                    //Проверка-запрос Адреса
                    var myQueryAdress = await db.Applicants.FromSqlRaw("SELECT * FROM Applicant WHERE Adress LIKE {0}", a.Adress).AsNoTracking().FirstOrDefaultAsync();

                    //Проверка-запрос ФИО
                    var myQueryFIO = await db.Applicants.FromSqlRaw("SELECT * FROM Applicant WHERE Firstname LIKE {0} AND Middlename LIKE {1} AND Lastname LIKE {2}", a.Family, a.Name, a.Lastname).AsNoTracking().FirstOrDefaultAsync();


                    // 1) Если совпали все условия ФИО Адрес Снилс
                    if (myQuery != null)
                    {
                        var myQuery1234 = from r in await db.Registries.AsNoTracking().ToListAsync()
                                          join ap in await db.Applicants.AsNoTracking().ToListAsync() on r.ApplicantFk equals ap.Id
                                          where ap.Snils == myQuery.Snils
                                               && ap.Firstname == myQuery.Firstname
                                               && ap.Middlename == myQuery.Firstname
                                               && ap.Lastname == myQuery.Firstname
                                               && ap.Adress == myQuery.Firstname
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
                                str += $"\nId-{item.Id} {item.Firstname} {item.Middlename[..1]}. {item.Lastname[..1]}. {item.SerialAndNumberSert} {Convert.ToDateTime(item.DateGetSert).ToString("d", new CultureInfo("Ru-ru"))} {ReturnStr(item.sol)}\n";
                            }

                            //Информировать что такая запись найдена
                            var result = MessageBox.Show($"{str}\n в таблице существуют записи данного заявителя\nДобавить новую запись в таблицу?", "Найдены совпадения!", MessageBoxButton.YesNo, MessageBoxImage.Information);

                            if (result == MessageBoxResult.Yes)
                            {
                                if (a.Lgota == null) //
                                {
                                    //Добавить новую запись в таблицу заявитель
                                    var inApp = await db.Database.ExecuteSqlInterpolatedAsync($"INSERT INTO Applicant(Firstname, Middlename, Lastname, Area_FK, Locality_FK, Adress, Snils, Privileges_FK) VALUES({a.Family}, {a.Name}, {a.Lastname}, {a.Area + 1}, {a.Local + 1}, {a.Adress}, {ReturnSnils(a.Snils)}, {null})");
                                    if (inApp == 0)
                                        MessageBox.Show("Произошла ошибка при вставке записи в таблицу Заявитель\nПовторите попытку");
                                }

                                else if (a.Area != null && a.Local != null && a.Lgota != null) //
                                {
                                    //Добавить новую запись в таблицу заявитель
                                    var inApp = await db.Database.ExecuteSqlInterpolatedAsync($"INSERT INTO Applicant(Firstname, Middlename, Lastname, Area_FK, Locality_FK, Adress, Snils, Privileges_FK) VALUES({a.Family}, {a.Name}, {a.Lastname}, {a.Area + 1}, {a.Local + 1}, {a.Adress}, {ReturnSnils(a.Snils)}, {a.Lgota + 1})");
                                    if (inApp == 0)
                                        MessageBox.Show("Произошла ошибка при вставке записи в таблицу Заявитель\nПовторите попытку");
                                }

                                //Запрос на получение Id последнего заявителя в таблице Applicant
                                var getIdLastApp = await db.Applicants.AsNoTracking().OrderBy(u => u.Id).LastOrDefaultAsync();
                                if(getIdLastApp == null)
                                    MessageBox.Show("Произошла ошибка :( (ошибка запроса last id applicant)\nПовторите попытку");

                                else
                                {
                                    if (a.Pay == null || a.Solution == null || a.Pay == null && a.Solution == null)
                                    {
                                        //Добавить новую запись в таблицу Регистр
                                        var inReg = await db.Database.ExecuteSqlInterpolatedAsync($"INSERT INTO Registry(Applicant_FK, SerialAndNumberSert, DateGetSert, PayAmount_FK, Solution_FK, DateAndNumbSolutionSert, Comment, Trek, MailingDate) VALUES({getIdLastApp.Id}, {a.Sernumb}, {a.DateGetSert}, {null}, {null}, {a.DateAndNumbSolutionSert}, {a.Comment}, {a.Trek}, {a.MailingDate})");
                                        await Task.Delay(75);
                                        if(inReg == 0)
                                            MessageBox.Show("Произошла ошибка вставки записи в таблицу Регистр\nПовторите попытку");
                                    }

                                    else if (a.Pay != null && a.Solution != null)
                                    {
                                        //Добавить новую запись в таблицу Регистр
                                        var inReg = await db.Database.ExecuteSqlInterpolatedAsync($"INSERT INTO Registry(Applicant_FK, SerialAndNumberSert, DateGetSert, PayAmount_FK, Solution_FK, DateAndNumbSolutionSert, Comment, Trek, MailingDate) VALUES({getIdLastApp.Id}, {a.Sernumb}, {a.DateGetSert}, {a.Pay + 1}, {a.Solution + 1}, {a.DateAndNumbSolutionSert}, {a.Comment}, {a.Trek}, {a.MailingDate})");
                                        await Task.Delay(75);
                                        if (inReg == 0)
                                            MessageBox.Show("Произошла ошибка вставки записи в таблицу Регистр\nПовторите попытку");
                                    }
                                }
                            }
                            else if (result == MessageBoxResult.No)
                                return;
                        }
                    }

                    // 2) Если совпал снилс
                    else if (myQuerySnils != null)
                    {
                        var myQuery1234 = from r in await db.Registries.AsNoTracking().ToListAsync()
                                          join ap in await db.Applicants.AsNoTracking().ToListAsync() on r.ApplicantFk equals ap.Id
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
                                    var inApp = await db.Database.ExecuteSqlInterpolatedAsync($"INSERT INTO Applicant(Firstname, Middlename, Lastname, Area_FK, Locality_FK, Adress, Snils, Privileges_FK) VALUES({a.Family}, {a.Name}, {a.Lastname}, {a.Area + 1}, {a.Local + 1}, {a.Adress}, {ReturnSnils(a.Snils)}, {null})");
                                    if (inApp == 0)
                                        MessageBox.Show("Произошла ошибка при вставке записи в таблицу Заявитель\nПовторите попытку");

                                }

                                else if (a.Area != null && a.Local != null && a.Lgota != null)
                                {
                                    //Добавить новую запись в таблицу заявитель
                                    var inApp = await db.Database.ExecuteSqlInterpolatedAsync($"INSERT INTO Applicant(Firstname, Middlename, Lastname, Area_FK, Locality_FK, Adress, Snils, Privileges_FK) VALUES({a.Family}, {a.Name}, {a.Lastname}, {a.Area + 1}, {a.Local + 1}, {a.Adress}, {ReturnSnils(a.Snils)}, {a.Lgota + 1})");
                                    if (inApp == 0)
                                        MessageBox.Show("Произошла ошибка при вставке записи в таблицу Заявитель\nПовторите попытку");
                                }


                                //Запрос на получение Id последнего заявителя в таблице Applicant
                                var getIdLastApp = await db.Applicants.AsNoTracking().OrderBy(u => u.Id).LastOrDefaultAsync();
                                if(getIdLastApp == null)
                                    MessageBox.Show("Произошла ошибка при поиске last id applicant\nПовторите попытку");
                                else
                                {
                                    //Добавить новую запись в таблицу Регистр
                                    if (a.Pay == null || a.Solution == null || a.Pay == null && a.Solution == null)
                                    {
                                        //Добавить новую запись в таблицу Регистр
                                        var inReg = await db.Database.ExecuteSqlInterpolatedAsync($"INSERT INTO Registry(Applicant_FK, SerialAndNumberSert, DateGetSert, PayAmount_FK, Solution_FK, DateAndNumbSolutionSert, Comment, Trek, MailingDate) VALUES({getIdLastApp.Id}, {a.Sernumb}, {a.DateGetSert}, {null}, {null}, {a.DateAndNumbSolutionSert}, {a.Comment}, {a.Trek}, {a.MailingDate})");
                                        await Task.Delay(75);
                                        if (inReg == 0)
                                            MessageBox.Show("Произошла ошибка при вставке записи в таблицу Регистр\nПовторите попытку");
                                    }

                                    else if (a.Pay != null && a.Solution != null)
                                    {
                                        //Добавить новую запись в таблицу Регистр
                                        var inReg = await db.Database.ExecuteSqlInterpolatedAsync($"INSERT INTO Registry(Applicant_FK, SerialAndNumberSert, DateGetSert, PayAmount_FK, Solution_FK, DateAndNumbSolutionSert, Comment, Trek, MailingDate) VALUES({getIdLastApp.Id}, {a.Sernumb}, {a.DateGetSert}, {a.Pay + 1}, {a.Solution + 1}, {a.DateAndNumbSolutionSert}, {a.Comment}, {a.Trek}, {a.MailingDate})");
                                        await Task.Delay(75);
                                        if (inReg == 0)
                                            MessageBox.Show("Произошла ошибка при вставке записи в таблицу Регистр\nПовторите попытку");
                                    }
                                }
                            }
                            else if (result == MessageBoxResult.No)
                                return;
                        }
                    }

                    // 3) Если совпал адрес
                    else if (myQueryAdress != null)
                    {
                        var myQuery1234 = from r in await db.Registries.AsNoTracking().ToListAsync()
                                          join ap in await db.Applicants.AsNoTracking().ToListAsync() on r.ApplicantFk equals ap.Id
                                          where ap.Adress == myQuerySnils.Adress
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
                            var result = MessageBox.Show($"{str}\nв таблице существуют записи данного заявителя, по такому же Адресу\nДобавить новую запись в таблицу?", "Найдены совпадения!", MessageBoxButton.YesNo, MessageBoxImage.Information);

                            if (result == MessageBoxResult.Yes)
                            {
                                if (a.Lgota == null)
                                {
                                    //Добавить новую запись в таблицу заявитель
                                    var inApp = await db.Database.ExecuteSqlInterpolatedAsync($"INSERT INTO Applicant(Firstname, Middlename, Lastname, Area_FK, Locality_FK, Adress, Snils, Privileges_FK) VALUES({a.Family}, {a.Name}, {a.Lastname}, {a.Area + 1}, {a.Local + 1}, {a.Adress}, {ReturnSnils(a.Snils)}, {null})");
                                    if (inApp == 0)
                                        MessageBox.Show("Произошла ошибка при вставке записи в таблицу Заявитель\nПовторите попытку");

                                }

                                else if (a.Area != null && a.Local != null && a.Lgota != null)
                                {
                                    //Добавить новую запись в таблицу заявитель
                                    var inApp = await db.Database.ExecuteSqlInterpolatedAsync($"INSERT INTO Applicant(Firstname, Middlename, Lastname, Area_FK, Locality_FK, Adress, Snils, Privileges_FK) VALUES({a.Family}, {a.Name}, {a.Lastname}, {a.Area + 1}, {a.Local + 1}, {a.Adress}, {ReturnSnils(a.Snils)}, {a.Lgota + 1})");
                                    if (inApp == 0)
                                        MessageBox.Show("Произошла ошибка при вставке записи в таблицу Заявитель\nПовторите попытку");
                                }


                                //Запрос на получение Id последнего заявителя в таблице Applicant
                                var getIdLastApp = await db.Applicants.AsNoTracking().OrderBy(u => u.Id).LastOrDefaultAsync();
                                if (getIdLastApp == null)
                                    MessageBox.Show("Произошла ошибка при поиске last id applicant\nПовторите попытку");
                                else
                                {
                                    //Добавить новую запись в таблицу Регистр
                                    if (a.Pay == null || a.Solution == null || a.Pay == null && a.Solution == null)
                                    {
                                        //Добавить новую запись в таблицу Регистр
                                        var inReg = await db.Database.ExecuteSqlInterpolatedAsync($"INSERT INTO Registry(Applicant_FK, SerialAndNumberSert, DateGetSert, PayAmount_FK, Solution_FK, DateAndNumbSolutionSert, Comment, Trek, MailingDate) VALUES({getIdLastApp.Id}, {a.Sernumb}, {a.DateGetSert}, {null}, {null}, {a.DateAndNumbSolutionSert}, {a.Comment}, {a.Trek}, {a.MailingDate})");
                                        await Task.Delay(75);
                                        if (inReg == 0)
                                            MessageBox.Show("Произошла ошибка при вставке записи в таблицу Регистр\nПовторите попытку");
                                    }

                                    else if (a.Pay != null && a.Solution != null)
                                    {
                                        //Добавить новую запись в таблицу Регистр
                                        var inReg = await db.Database.ExecuteSqlInterpolatedAsync($"INSERT INTO Registry(Applicant_FK, SerialAndNumberSert, DateGetSert, PayAmount_FK, Solution_FK, DateAndNumbSolutionSert, Comment, Trek, MailingDate) VALUES({getIdLastApp.Id}, {a.Sernumb}, {a.DateGetSert}, {a.Pay + 1}, {a.Solution + 1}, {a.DateAndNumbSolutionSert}, {a.Comment}, {a.Trek}, {a.MailingDate})");
                                        await Task.Delay(75);
                                        if (inReg == 0)
                                            MessageBox.Show("Произошла ошибка при вставке записи в таблицу Регистр\nПовторите попытку");
                                    }
                                }
                            }
                            else if (result == MessageBoxResult.No)
                                return;
                        }
                    }

                    // 4) Если совпало ФИО
                    else if (myQueryFIO != null)
                    {
                        var myQuery1234 = from r in await db.Registries.AsNoTracking().ToListAsync()
                                          join ap in await db.Applicants.AsNoTracking().ToListAsync() on r.ApplicantFk equals ap.Id
                                          where ap.Firstname == myQueryFIO.Firstname 
                                             && ap.Middlename == myQueryFIO.Middlename
                                             && ap.Lastname == myQueryFIO.Lastname
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
                            var result = MessageBox.Show($"{str}\nв таблице существуют похожие записи, ФИО совпадает полностью!\nДобавить новую запись в таблицу?", "Найдены совпадения!", MessageBoxButton.YesNo, MessageBoxImage.Information);

                            if (result == MessageBoxResult.Yes)
                            {
                                if (a.Lgota == null)
                                {
                                    //Добавить новую запись в таблицу заявитель
                                    var inApp = await db.Database.ExecuteSqlInterpolatedAsync($"INSERT INTO Applicant(Firstname, Middlename, Lastname, Area_FK, Locality_FK, Adress, Snils, Privileges_FK) VALUES({a.Family}, {a.Name}, {a.Lastname}, {a.Area + 1}, {a.Local + 1}, {a.Adress}, {ReturnSnils(a.Snils)}, {null})");
                                    if (inApp == 0)
                                        MessageBox.Show("Произошла ошибка при вставке записи в таблицу Заявитель\nПовторите попытку");

                                }

                                else if (a.Area != null && a.Local != null && a.Lgota != null)
                                {
                                    //Добавить новую запись в таблицу заявитель
                                    var inApp = await db.Database.ExecuteSqlInterpolatedAsync($"INSERT INTO Applicant(Firstname, Middlename, Lastname, Area_FK, Locality_FK, Adress, Snils, Privileges_FK) VALUES({a.Family}, {a.Name}, {a.Lastname}, {a.Area + 1}, {a.Local + 1}, {a.Adress}, {ReturnSnils(a.Snils)}, {a.Lgota + 1})");
                                    if (inApp == 0)
                                        MessageBox.Show("Произошла ошибка при вставке записи в таблицу Заявитель\nПовторите попытку");
                                }


                                //Запрос на получение Id последнего заявителя в таблице Applicant
                                var getIdLastApp = await db.Applicants.AsNoTracking().OrderBy(u => u.Id).LastOrDefaultAsync();
                                if (getIdLastApp == null)
                                    MessageBox.Show("Произошла ошибка при поиске last id applicant\nПовторите попытку");
                                else
                                {
                                    //Добавить новую запись в таблицу Регистр
                                    if (a.Pay == null || a.Solution == null || a.Pay == null && a.Solution == null)
                                    {
                                        //Добавить новую запись в таблицу Регистр
                                        var inReg = await db.Database.ExecuteSqlInterpolatedAsync($"INSERT INTO Registry(Applicant_FK, SerialAndNumberSert, DateGetSert, PayAmount_FK, Solution_FK, DateAndNumbSolutionSert, Comment, Trek, MailingDate) VALUES({getIdLastApp.Id}, {a.Sernumb}, {a.DateGetSert}, {null}, {null}, {a.DateAndNumbSolutionSert}, {a.Comment}, {a.Trek}, {a.MailingDate})");
                                        await Task.Delay(75);
                                        if (inReg == 0)
                                            MessageBox.Show("Произошла ошибка при вставке записи в таблицу Регистр\nПовторите попытку");
                                    }

                                    else if (a.Pay != null && a.Solution != null)
                                    {
                                        //Добавить новую запись в таблицу Регистр
                                        var inReg = await db.Database.ExecuteSqlInterpolatedAsync($"INSERT INTO Registry(Applicant_FK, SerialAndNumberSert, DateGetSert, PayAmount_FK, Solution_FK, DateAndNumbSolutionSert, Comment, Trek, MailingDate) VALUES({getIdLastApp.Id}, {a.Sernumb}, {a.DateGetSert}, {a.Pay + 1}, {a.Solution + 1}, {a.DateAndNumbSolutionSert}, {a.Comment}, {a.Trek}, {a.MailingDate})");
                                        await Task.Delay(75);
                                        if (inReg == 0)
                                            MessageBox.Show("Произошла ошибка при вставке записи в таблицу Регистр\nПовторите попытку");
                                    }
                                }
                            }
                            else if (result == MessageBoxResult.No)
                                return;
                        }
                    }

                    // 5) Если нет такой записи, добавляем новую запись
                    else if (myQuery == null && myQuerySnils == null)
                    {
                        if (a.Lgota == null)
                        {
                            //Добавить новую запись в таблицу заявитель
                            var inApp = await db.Database.ExecuteSqlInterpolatedAsync($"INSERT INTO Applicant(Firstname, Middlename, Lastname, Area_FK, Locality_FK, Adress, Snils, Privileges_FK) VALUES({a.Family}, {a.Name}, {a.Lastname}, {a.Area + 1}, {a.Local + 1}, {a.Adress}, {ReturnSnils(a.Snils)}, {null})");
                            if (inApp == 0)
                                MessageBox.Show("Произошла ошибка при вставке записи в таблицу Заявитель\nПовторите попытку");
                        }

                        else if (a.Area != null && a.Local != null && a.Lgota != null)
                        {
                            //Добавить новую запись в таблицу заявитель
                            var inApp = await db.Database.ExecuteSqlInterpolatedAsync($"INSERT INTO Applicant(Firstname, Middlename, Lastname, Area_FK, Locality_FK, Adress, Snils, Privileges_FK) VALUES({a.Family}, {a.Name}, {a.Lastname}, {a.Area + 1}, {a.Local + 1}, {a.Adress}, {ReturnSnils(a.Snils)}, {a.Lgota + 1})");
                            if (inApp == 0)
                                MessageBox.Show("Произошла ошибка при вставке записи в таблицу Заявитель\nПовторите попытку");
                        }


                        //Запрос на получение Id последнего заявителя в таблице Applicant
                        var getIdLastApp = await db.Applicants.AsNoTracking().OrderBy(u => u.Id).LastOrDefaultAsync();
                        if (getIdLastApp == null)
                            MessageBox.Show("Произошла ошибка при выборке last id applicant\nПовторите попытку");
                        else
                        {
                            //Добавить новую запись в таблицу Регистр
                            if (a.Pay == null || a.Solution == null || a.Pay == null && a.Solution == null)
                            {
                                //Добавить новую запись в таблицу Регистр
                                var inReg = await db.Database.ExecuteSqlInterpolatedAsync($"INSERT INTO Registry(Applicant_FK, SerialAndNumberSert, DateGetSert, PayAmount_FK, Solution_FK, DateAndNumbSolutionSert, Comment, Trek, MailingDate) VALUES({getIdLastApp.Id}, {a.Sernumb}, {a.DateGetSert}, {null}, {null}, {a.DateAndNumbSolutionSert}, {a.Comment}, {a.Trek}, {a.MailingDate})");
                                await Task.Delay(75);
                                if (inReg == 0)
                                    MessageBox.Show("Произошла ошибка при вставке записи в таблицу Регистр\nПовторите попытку");
                            }

                            else if (a.Pay != null && a.Solution != null)
                            {
                                //Добавить новую запись в таблицу Регистр
                                var inReg = await db.Database.ExecuteSqlInterpolatedAsync($"INSERT INTO Registry(Applicant_FK, SerialAndNumberSert, DateGetSert, PayAmount_FK, Solution_FK, DateAndNumbSolutionSert, Comment, Trek, MailingDate) VALUES({getIdLastApp.Id}, {a.Sernumb}, {a.DateGetSert}, {a.Pay + 1}, {a.Solution + 1}, {a.DateAndNumbSolutionSert}, {a.Comment}, {a.Trek}, {a.MailingDate})");
                                await Task.Delay(75);
                                if (inReg == 0)
                                    MessageBox.Show("Произошла ошибка при вставке записи в таблицу Регистр\nПовторите попытку");
                            }
                        }
                     
                    }

                    Start();
                }
            }
            #endregion

            //Возвращаю только Цифры в Снилсе
            static string ReturnSnils(string? t)
            {
                string temp = "";
                for (int i = 0; i < t.Length; i++)
                {
                    if (char.IsDigit(t[i]))
                        temp += t[i];
                }
                return temp;
            }

            //Возвращю тип решения (строку)
            static string ReturnStr(int? t)
            {
                if (t == 1)
                    return "Выдан";
                else if (t == 2)
                    return "Отказ";
                else if (t == 3)
                    return "Аннулир.";
                else if (t == 4)
                    return "Без рассмотрения";
                else return "";

            }
        }

        //Обновить коммент()
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
                    MessageBox.Show(ex.Message, "Произошла ошибка при обновлении комментария\nПовторите попытку");
                }
            }
        }
        #endregion

        #region Поиск()
        //Поиск(Нормально)
        private void Button_Click_3(object sender, RoutedEventArgs e)
        {
            GoSerchNoPainHohuVTgu();
            switch (filterSearch.Text)
            {
                case "По всем полям":
                    dataGrid.ItemsSource = MyList.Where(u => $"{u.IdReg}{u.Family}{u.Name}{u.Lastname}{u.Snils}{u.Adress}{u.Sernumb}".Replace(" ", "").ToLower().Contains(SearchTable.Text.Replace(" ", "").ToLower())).ToList();

                    break;

                case "Фамилия":
                    dataGrid.ItemsSource = MyList.Where(u => u.Family != null && u.Family.Replace(" ", "").ToLower().Contains(SearchTable.Text.Replace(" ", "").ToLower())).ToList();
                    break;

                case "Имя":
                    dataGrid.ItemsSource = MyList.Where(u => u.Name != null && u.Name.Replace(" ", "").ToLower().Contains(SearchTable.Text.Replace(" ", "").ToLower())).ToList();
                    break;

                case "Отчество":
                    dataGrid.ItemsSource = MyList.Where(u => u.Lastname != null && u.Lastname.Replace(" ", "").ToLower().Contains(SearchTable.Text.Replace(" ", "").ToLower())).ToList();
                    break;

                case "ФИО":
                    dataGrid.ItemsSource = MyList.Where(u => $"{u.Family}{u.Name}{u.Lastname}".Replace(" ", "").ToLower().Contains(SearchTable.Text.Replace(" ", "").ToLower())).ToList();
                    break;

                case "Снилс":
                    dataGrid.ItemsSource = MyList.Where(u => u.Snils != null
                                            && u.Snils.Replace(" ", "").Replace("-", "").Contains(SearchTable.Text.Replace(" ", "").Replace("-", ""))).ToList();
                    break;

                case "Адрес":
                    dataGrid.ItemsSource = MyList.Where(u => u.Adress != null
                                                  && u.Adress.Replace(" ", "").ToLower().Contains(SearchTable.Text.Replace(" ", "").ToLower())).ToList();
                    break;

                case "Серия и номер сертификата":
                    dataGrid.ItemsSource = MyList.Where(u => u.Sernumb != null && u.Sernumb.Replace(" ", "").ToLower().Contains(SearchTable.Text.Replace(" ", "").ToLower())).ToList();
                    break;

                case "По ID":
                    dataGrid.ItemsSource = MyList.Where(u => u.IdReg.ToString() != null && u.IdReg.ToString().Replace(" ", "") == SearchTable.Text.Replace(" ", "")).ToList();
                    break;

                default:
                    dataGrid.ItemsSource = MyList.ToList();
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
        static void GoSerchNoPainHohuVTgu()
        {
            using ExDbContext db = new();
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
                      }).ToList();
        }
        #endregion

        #region События изменения значений ComboBox()
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

        #region Выгрузка в Excel()
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

        #region Фильтрация()
        //Заполнение таблиц Фильтров
        public async void FilterStart()
            {
            //Сначала удаляю все из Sqlite
            using (FdbContext db1 = new())
            {
                // удаление при старте проги
                int deleteAreaF = await db1.Database.ExecuteSqlRawAsync("DELETE FROM AreaF");
                int deleteLocalF = await db1.Database.ExecuteSqlRawAsync("DELETE FROM LocalF");
                int deletePayF = await db1.Database.ExecuteSqlRawAsync("DELETE FROM PayF");
                int deletePrivF = await db1.Database.ExecuteSqlRawAsync("DELETE FROM PrivF");
                int deleteSolF = await db1.Database.ExecuteSqlRawAsync("DELETE FROM SolF");
            }

            try
            {
                //Заполнение таблиц Фильтров
                using (ExDbContext db = new())
                {
                    //Район
                    using FdbContext db1 = new();
                    var s = await db.Areas.FromSqlRaw("SELECT * FROM Area").ToListAsync();
                    foreach (var item in s)
                    {
                        await db1.Database.ExecuteSqlRawAsync("INSERT INTO AreaF(id, name, flag) VALUES ({0}, {1}, {2})", item.Id, item.AreaName, 0);
                    }
                    areaFilter.ItemsSource = db1.AreaFs.ToList();


                    //Населенный пункт
                    var local = await db.Localities.FromSqlRaw("SELECT * FROM Locality").ToListAsync();
                    foreach (var item in local)
                    {
                        await db1.Database.ExecuteSqlRawAsync("INSERT INTO LocalF(id, name, flag) VALUES ({0}, {1}, {2})", item.Id, item.LocalName, 0);
                    }
                    locFilter.ItemsSource = db1.Localves.ToList();

                    //Выплата
                    var pay = await db.PayAmounts.FromSqlRaw("SELECT * FROM PayAmount").ToListAsync();
                    foreach (var item in pay)
                    {
                        await db1.Database.ExecuteSqlRawAsync("INSERT INTO PayF(id, name, flag) VALUES ({0}, {1}, {2})", item.Id, item.Pay.ToString(), 0);
                    }
                    payFilter.ItemsSource = db1.PayFs.ToList();

                    //Льготы
                    var priv = await db.Privileges.FromSqlRaw("SELECT * FROM Privileges").ToListAsync();
                    foreach (var item in priv)
                    {
                        if (item.PrivilegesName.Length >= 17)
                            item.PrivilegesName = item.PrivilegesName[..17];
                        await db1.Database.ExecuteSqlRawAsync("INSERT INTO PrivF(id, name, flag) VALUES ({0}, {1}, {2})", item.Id, item.PrivilegesName, 0);
                    }
                    privFilter.ItemsSource = db1.PrivFs.ToList();


                    //Решение
                    var sol = await db.SolutionTypes.FromSqlRaw("SELECT * FROM SolutionType").ToListAsync();
                    foreach (var item in sol)
                    {
                        await db1.Database.ExecuteSqlRawAsync("INSERT INTO SolF(id, name, flag) VALUES ({0}, {1}, {2})", item.Id, item.SolutionName, 0);
                    }
                    solFilter.ItemsSource = db1.Solves.ToList();


                };
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
            
          


        }
        #region CheckBoxes
        //Поставил галочку Район
        private async void AreaCheck(object sender, RoutedEventArgs e)
        {
            using FdbContext db = new();
            await db.Database.ExecuteSqlRawAsync("UPDATE AreaF SET Flag={0} WHERE id={1}", 1, (areaFilter.SelectedItem as AreaF)?.Id);
        }

            //Убрал галочку Район
        private async void AreaUnchecked(object sender, RoutedEventArgs e)
        {
            using FdbContext db = new();
            await db.Database.ExecuteSqlRawAsync("UPDATE AreaF SET Flag={0} WHERE id={1}", 0, (areaFilter.SelectedItem as AreaF)?.Id);
        }

        //Убрал галочку Населенный пункт
        private async void LocalUnchecked(object sender, RoutedEventArgs e)
        {
            using FdbContext db = new();
            await db.Database.ExecuteSqlRawAsync("UPDATE LocalF SET Flag={0} WHERE id={1}", 0, (locFilter.SelectedItem as LocalF)?.Id);
        }
        //Поставил галочку Населенный пункт
        private async void LocalChecked(object sender, RoutedEventArgs e)
        {
            using FdbContext db = new();
            await db.Database.ExecuteSqlRawAsync("UPDATE LocalF SET Flag={0} WHERE id={1}", 1, (locFilter.SelectedItem as LocalF)?.Id);
        }

        //Поставил галочку Выплата
        private async void PayChecked(object sender, RoutedEventArgs e)
        {
            using FdbContext db = new();
            await db.Database.ExecuteSqlRawAsync("UPDATE LocalF SET Flag={0} WHERE id={1}", 1, (payFilter.SelectedItem as PayF)?.Id);
        }

        //Убрал галочку Выплата
        private async void PayUnChecked(object sender, RoutedEventArgs e)
        {
            using FdbContext db = new();
            await db.Database.ExecuteSqlRawAsync("UPDATE LocalF SET Flag={0} WHERE id={1}", 0, (payFilter.SelectedItem as PayF)?.Id);
        }

        //Убрал галочку Льгота
        private async void PrivUnchecked(object sender, RoutedEventArgs e)
        {

            using FdbContext db = new();
            await db.Database.ExecuteSqlRawAsync("UPDATE PrivF SET Flag={0} WHERE id={1}", 0, (privFilter.SelectedItem as PrivF)?.Id);
        }

        //Поставил галочку Льгота
        private async void PrivChecked(object sender, RoutedEventArgs e)
        {
            using FdbContext db = new();
            await db.Database.ExecuteSqlRawAsync("UPDATE PrivF SET Flag={0} WHERE id={1}", 1, (privFilter.SelectedItem as PrivF)?.Id);
        }

        //Поставил галочку Решение
        private async void SolChecked(object sender, RoutedEventArgs e)
        {
            using FdbContext db = new();
            await db.Database.ExecuteSqlRawAsync("UPDATE SolF SET Flag={0} WHERE id={1}", 1, (solFilter.SelectedItem as SolF)?.Id);
        }

        //Убрал галочку Решение
        private async void SolUnChecked(object sender, RoutedEventArgs e)
        {
            using FdbContext db = new();
            await db.Database.ExecuteSqlRawAsync("UPDATE SolF SET Flag={0} WHERE id={1}", 1, (solFilter.SelectedItem as SolF)?.Id);
        }
        #endregion
        //Применить фильтр
        private async void Button_Click_4(object sender, RoutedEventArgs e)
        {
            await GoStartFilter();
        }

       


        //Метод для выборки по фильтрам
        async Task GoStartFilter()
        {
            await Task.Run(() =>
            {
                using FdbContext db1 = new();

                //Район полная обработка
                List<long> areaIdL = new();
                areaIdL = db1.AreaFs.Where(u => u.Flag == 1).Select(c => c.Id - 1).ToList();
                if (areaIdL.Count == 0)
                    areaIdL = db1.AreaFs.Select(c => c.Id - 1).ToList();

                //Населенный пункт полная обработка
                List<long> localIdL = new();
                localIdL = db1.Localves.Where(u => u.Flag == 1).Select(c => c.Id - 1).ToList();
                if (localIdL.Count == 0)
                    localIdL = db1.Localves.Select(c => c.Id - 1).ToList();

               //Льгота полная обработка
               List<long> privIdL = new();
               privIdL = db1.PrivFs.Where(u => u.Flag == 1).Select(c => c.Id - 1).ToList();
               if (privIdL.Count == 0)
                       privIdL = db1.PrivFs.Select(c => c.Id - 1).ToList();


                //Выплата полная обработка
                List<long> payIdL = new();
                payIdL = db1.PayFs.Where(u => u.Flag == 1).Select(c => c.Id - 1).ToList();
                if (payIdL.Count == 0)
                    payIdL = db1.PrivFs.Select(c => c.Id - 1).ToList();

                //Решение полная обработка
                List<long> solIdl = new();
                solIdl = db1.Solves.Where(u => u.Flag == 1).Select(c => c.Id - 1).ToList();
                if (solIdl.Count == 0)
                    solIdl = db1.Solves.Select(c => c.Id - 1).ToList();

                //Предикат попробую юзать
                var predicate = PredicateBuilder.True<Log>();






                try
                {
                    Dispatcher.Invoke(() =>
                    {
                        //Проверка даты
                        // dateStart dateEnd
                        if (string.IsNullOrEmpty(dateStart.Text) && string.IsNullOrEmpty(dateEnd.Text)
                        || string.IsNullOrWhiteSpace(dateStart.Text) && string.IsNullOrWhiteSpace(dateEnd.Text))
                    {
                            dateStart.Text = "10.10.2003";
                            dateEnd.Text = "10.10.2030";
                    }
                    else if (string.IsNullOrEmpty(dateStart.Text) || string.IsNullOrWhiteSpace(dateStart.Text)
                          && !string.IsNullOrEmpty(dateEnd.Text) || !string.IsNullOrWhiteSpace(dateEnd.Text))
                    {
                            dateStart.Text = "10.10.2003";
                    }
                    else if (string.IsNullOrEmpty(dateEnd.Text) || string.IsNullOrWhiteSpace(dateEnd.Text)
                        && !string.IsNullOrEmpty(dateStart.Text) || !string.IsNullOrWhiteSpace(dateStart.Text))
                    {
                            dateEnd.Text = "10.10.2030";
                    }

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
                                      }).Where(u => u.DateGetSert >= Convert.ToDateTime(dateStart.Text)
                                            & u.DateGetSert <= Convert.ToDateTime(dateEnd.Text)
                                            & areaIdL.Contains((long)u.Area)
                                            & localIdL.Contains((long)u.Local)
                                            & privIdL.Contains((long)u.Lgota)
                                            & payIdL.Contains((long)u.Pay)
                                            & solIdl.Contains((long)u.Solution)).ToList();
                        };
                    });




            //        List<Entity> list = new List<Entity>
            //        {
            //            new Entity {ID = 1, Name = "qwerty"},
            //            new Entity {ID = 2, Name = "rewyt"},
            //    new Entity {ID = 4, Name = "asdfg"},
            //};

            //        int compareId = 2;  // ID для сравнения в фильтре
            //        string compareName = "qwerty";  // имя для сравнения в фильтре


            //        bool filterById = true,    // указывает нужно ли фильтровать по полю ID
            //            filterByName = false;   // указывает нужно ли фильтровать по полю Name

            //        Func<Entity, bool> predicateById = x => x.ID == compareId;
            //        Func<Entity, bool> predicateByName = x => x.Name == compareName;

            //        Func<Entity, bool> mainPredicate = x => (!filterById || predicateById(x))
            //                                                && (!filterByName || predicateByName(x));


            //        foreach (var entity in list.Where(mainPredicate))
            //        {
            //            Console.WriteLine(entity);
            //        }






                    if (MyList.Count == 0)
                            MessageBox.Show("По вашему запросу ничего не найдено :(");
                        else
                        {
                            Dispatcher.Invoke(() =>
                            {
                                dataGrid.ItemsSource = MyList.ToList();
                            });
                        }
                    
                }

                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                }

            });
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

            private void Button_Click_1(object sender, RoutedEventArgs e)
            {
                Start();
            }


        //Очистить фильтр
        private void Button_Click_5(object sender, RoutedEventArgs e)
        {

            FilterStart();
            try
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
                              }).ToList();


                    if (MyList.Count == 0)
                        MessageBox.Show("По вашему запросу ничего не найдено :(");
                    else
                    {
                        dataGrid.ItemsSource = MyList.ToList();
                    }
                };
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }

        }

        //Событие перед редактированием ячейки Добавление адреса, если ячейка пустая()
        private void TestBeginningEdit(object sender, DataGridBeginningEditEventArgs e)
        {
            //Получаю название колонки
            if (e.Column.Header.ToString() == "Адрес")
            {
                string content = (e.EditingEventArgs.Source as TextBlock).Text;
                temp1 = "";
                if (string.IsNullOrEmpty(content))
                {
                    //Здесь заполняю Адрес, если пустая строка
                    AdressWindow adres = new(ref temp1);
                    adres.ShowDialog();
                }
                else return;
            }
            //Считывание строки
            SClass? a = e.Row.Item as SClass;
            a.Adress = temp1;
        }

        //Статистические данные()
        private void Button_Click_2(object sender, RoutedEventArgs e)
        {
            StaticWindow staticWindow = new();
            staticWindow.ShowDialog();
        }


        #endregion
    }
}
        