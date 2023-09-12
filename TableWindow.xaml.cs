using Microsoft.EntityFrameworkCore;
using Microsoft.VisualBasic;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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

namespace exel_for_mfc
{
    public partial class TableWindow : Window
    {
        public static List<Area>? AreaCombobox { get; set; }
        public static List<Locality>? LocalCombobox { get; set; }
        public static List<PayAmount>? PayCombobox { get; set; }
        public static List<Privilege>? PrivelCombobox { get; set; }
        public static List<SolutionType>? SolCombobox { get; set; }

        //Thread myThread = new Thread(Print);
        //myThread.Start();

        public TableWindow()
        {
            InitializeComponent();
            Start();
        }

        //Получаем измененные данные после редактирования ячейки
        private async void dataGrid_CellEditEnding(object sender, DataGridCellEditEndingEventArgs e)
        {
            //Сделать заполнение комментария отдельным окном? типо реализовать mvvm
            try
            {
              //Непосредственно редактирование ячейки (Обновление строки) - Заявитель - Регистр
                using (ExDbContext db = new())
                {
                    //Считывание строки
                    SClass? a = e.Row.Item as SClass;

                    //Обновление таблицы Заявитель
                    await db.Database.ExecuteSqlRawAsync("UPDATE Applicant SET Firstname = {0}, Middlename = {1}, Lastname = {2}, Adress = {3}, Snils = {4} WHERE Id = {5}", a.Family, a.Name, a.Lastname, a.Adress, a.Snils, a.IdApplicant);

                    //Обновление таблицы Регистр
                    await db.Database.ExecuteSqlRawAsync("UPDATE Registry SET SerialAndNumberSert = {0}, DateGetSert = {1}, DateAndNumbSolutionSert = {2}, Comment = {3}, Trek = {4}, MailingDate = {5} WHERE Id = {6}", a.Sernumb, a.DateGetSert, a.DateAndNumbSolutionSert, a.Comment, a.Trek, a.MailingDate, a.IdReg);
                    
                }
                dataGrid.CancelEdit();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
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

                AreaCombobox = db.Areas.FromSqlRaw("SELECT * FROM Area").AsNoTracking().ToList();
                LocalCombobox = db.Localities.FromSqlRaw("SELECT * FROM Locality").AsNoTracking().ToList();
                PayCombobox = db.PayAmounts.FromSqlRaw("SELECT * FROM PayAmount").AsNoTracking().ToList();
                PrivelCombobox = db.Privileges.FromSqlRaw("SELECT * FROM Privileges").AsNoTracking().ToList();
                SolCombobox = db.SolutionTypes.FromSqlRaw("SELECT * FROM SolutionType").AsNoTracking().ToList();
            };
         }

        private async void AreaComboEvent(object sender, EventArgs e)
        {
            //Меняем район Заявителю
            using (ExDbContext db = new())
            {
                await db.Database.ExecuteSqlRawAsync("UPDATE Applicant SET Area_FK = {0} WHERE Id = {1}", (sender as ComboBox)?.SelectedIndex + 1, (dataGrid.SelectedItem as SClass)?.IdApplicant);
            }    
        }

        private async void LocalComboEvent(object sender, EventArgs e)
        {
            //Меняем Населенный пункт Заявителю
            using (ExDbContext db = new())
            {
                await db.Database.ExecuteSqlRawAsync("UPDATE Applicant SET Locality_FK = {0} WHERE Id = {1}", (sender as ComboBox)?.SelectedIndex + 1, (dataGrid.SelectedItem as SClass)?.IdApplicant);
            }
        }

        private async void PrivilegesComboEvent(object sender, EventArgs e)
        {
            using (ExDbContext db = new())
            {
                await db.Database.ExecuteSqlRawAsync("UPDATE Applicant SET Privileges_FK = {0} WHERE Id = {1}", (sender as ComboBox)?.SelectedIndex + 1, (dataGrid.SelectedItem as SClass)?.IdApplicant);
            }
        }

        private async void PayComboEvent(object sender, EventArgs e)
        {
            using (ExDbContext db = new())
            {
                await db.Database.ExecuteSqlRawAsync("UPDATE Registry SET PayAmount_FK = {0} WHERE Id = {1}", (sender as ComboBox)?.SelectedIndex + 1, (dataGrid.SelectedItem as SClass)?.IdReg);
            }
        }

        private async void SolutionComboEvent(object sender, EventArgs e)
        {
            using (ExDbContext db = new())
            {
                await db.Database.ExecuteSqlRawAsync("UPDATE Registry SET Solution_FK = {0} WHERE Id = {1}", (sender as ComboBox)?.SelectedIndex + 1, (dataGrid.SelectedItem as SClass)?.IdReg);
            }
        }
    }




    //Двойной клик, обработка множественного нажатия мыши, чтоб не вылетала программа

}