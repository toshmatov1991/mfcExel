using Microsoft.EntityFrameworkCore;
using Microsoft.VisualBasic;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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
    public partial class TableWindow : Window
    {
        public static List<Area>? AreaCombobox { get; set; }
        public static List<Locality>? LocalCombobox { get; set; }
        public static List<PayAmount>? PayCombobox { get; set; }
        public static List<Privilege>? PrivelCombobox { get; set; }
        public static List<SolutionType>? SolCombobox { get; set; }

        bool flagfix = true;


        public TableWindow()
        {
            InitializeComponent();
            Start();
        }

        //Получаем измененные данные после редактирования ячейки
        private async void dataGrid_CellEditEnding(object sender, DataGridCellEditEndingEventArgs e)
        {
            //Считывание строки
            SClass? a = e.Row.Item as SClass;

            //Непосредственно редактирование ячейки (Обновление строки) - Заявитель
            using(ExDbContext db = new())
            {
                //Обновление таблицы Заявитель
                int ApplicantUpdated = await db.Database.ExecuteSqlRawAsync("UPDATE Applicant SET Firstname = {0}, Middlename = {1}, Lastname = {2}, Area_FK = {3}, Locality_FK = {4}, Adress = {5}, Snils = {6}, Privileges_FK = {7} WHERE Id = {8}", a.Family, a.Name, a.Lastname, ReturnIdAreaAsync(a.Area), ReturnIdLocalAsync(a.Local), a.Adress, a.Snils, ReturnIdPrivelAsync(a.Lgota), a.IdApplicant);
            }


            //Вернуть идентификатор Района
            static async Task<int> ReturnIdAreaAsync(string _area)
            {
               using(ExDbContext db = new())
               {
                    var IdArea = await db.Areas.Where(u => u.AreaName == _area).AsNoTracking().FirstOrDefaultAsync();
                    return IdArea.Id;  
               }
            }

            //Вернуть идентификатор Населенного пункта
            static async Task<int> ReturnIdLocalAsync(string _loc)
            {
                using (ExDbContext db = new())
                {
                    var IdLoc = await db.Localities.Where(u => u.LocalName == _loc).AsNoTracking().FirstOrDefaultAsync();
                    return IdLoc.Id;
                }
            }

            //Вернуть идентификатор Льготы
            static async Task<int> ReturnIdPrivelAsync(string _priv)
            {
                using (ExDbContext db = new())
                {
                    var IdPriv = await db.Privileges.Where(u => u.PrivilegesName == _priv).AsNoTracking().FirstOrDefaultAsync();
                    return IdPriv.Id;
                }
            }

            //Непосредственно редактирование ячейки (Обновление строки) - Регистр
            using (ExDbContext db = new())
            {
                //Обновление таблицы Регистр
                int RegistrUpdated = await db.Database.ExecuteSqlRawAsync("UPDATE Registry SET Applicant_FK = {0}, SerialAndNumberSert = {1}, DateGetSert = {2}, PayAmount_FK = {3}, Solution_FK = {4}, DateAndNumbSolutionSert = {5}, Comment = {6}, Trek = {7}, MailingDate = {8} WHERE Id = {9}",
                                                               a.IdApplicant, a.Sernumb, a.DateGetSert, ReturnIdPaylAsync(a.Pay), ReturnIdSolutionAsync(a.Solution), a.DateAndNumbSolutionSert, a.Comment, a.Trek, a.MailingDate, a.IdReg);
            }

            //Вернуть идентификатор Выплаты
            static async Task<decimal> ReturnIdPaylAsync(decimal _pay)
            {
                using (ExDbContext db = new())
                {
                    var IdPay = await db.PayAmounts.Where(u => u.Pay == _pay).AsNoTracking().FirstOrDefaultAsync();
                    return IdPay.Id;
                }
            }

            //Вернуть идентификатор Решения
            static async Task<decimal> ReturnIdSolutionAsync(string _sol)
            {
                using (ExDbContext db = new())
                {
                    var IdSol = await db.SolutionTypes.Where(u => u.SolutionName == _sol).AsNoTracking().FirstOrDefaultAsync();
                    return IdSol.Id;
                }
            }

            //Сделать заполнение комментария отдельным окном? типо реализовать mvvm


            //Password? p = e.Row.Item as Password;
            //if (p.Id != 0)
            //{
            //    //Редактирование
            //    if (flagfix)
            //    {
            //        var customer = await db.Passwords.Where(c => c.Id == p.Id).FirstOrDefaultAsync();

            //        customer.Login = p.Login;
            //        customer.Pass = p.Pass;
            //        await db.SaveChangesAsync();
            //        flagfix = false;
            //        dataGrid.ItemsSource = await db.Passwords.ToListAsync();
            //        dataGrid.Items.Refresh();
            //        dataGrid.CancelEdit();
            //    }
            //    flagfix = true;
            //}

        }

     
      
        //Запрос для заполнения таблицы
        //Комментарий чтоб появлялся при наведении
         void Start()
         {
            using (ExDbContext db = new()) 
            {
                           var MyList =(from reg in db.Registries
                                        join appl in  db.Applicants on reg.ApplicantFk equals appl.Id
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

                AreaCombobox = db.Areas.AsNoTracking().ToList();
                LocalCombobox = db.Localities.AsNoTracking().ToList();
                PayCombobox = db.PayAmounts.AsNoTracking().ToList();
                PrivelCombobox = db.Privileges.AsNoTracking().ToList();
                SolCombobox = db.SolutionTypes.AsNoTracking().ToList();
            };
         }

        //Двойной клик, обработка множественного нажатия мыши, чтоб не вылетала программа

    }
}