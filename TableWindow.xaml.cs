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
            ComboboxGO();




        }

        //Получаем измененные данные после редактирования ячейки
        private void dataGrid_CellEditEnding(object sender, DataGridCellEditEndingEventArgs e)
        {
            SClass? a = e.Row.Item as SClass;
            MessageBox.Show(a.Adress + "  " + a.Name + " " + "id заявителя --> " + a.IdApplicant);

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
                                        join area in  db.Areas on appl.AreaFk equals area.Id
                                        join local in  db.Localities on appl.LocalityFk equals local.Id
                                        join priv in  db.Privileges on appl.PrivilegesFk equals priv.Id
                                        join pay in  db.PayAmounts on reg.PayAmountFk equals pay.Id
                                        join sol in  db.SolutionTypes on reg.SolutionFk equals sol.Id
                                        select new SClass
                                        {
                                            IdReg = reg.Id,
                                            Family = appl.Firstname,
                                            Name = appl.Middlename,
                                            Lastname = appl.Lastname,
                                            Snils = appl.Snils,
                                            Area = area.Id - 1,
                                            Local = local.Id - 1,
                                            Adress = appl.Adress,
                                            Lgota = priv.Id - 1,
                                            Pay = reg.PayAmountFk - 1,
                                            Sernumb = reg.SerialAndNumberSert,
                                            DateGetSert = reg.DateGetSert,
                                            Solution = sol.Id - 1,
                                            DateAndNumbSolutionSert = reg.DateAndNumbSolutionSert,
                                            Comment = reg.Comment,
                                            Trek = reg.Trek,
                                            MailingDate = reg.MailingDate,
                                            IdApplicant = appl.Id
                                        }).AsNoTracking().ToList();

              dataGrid.ItemsSource = MyList;
            };
        }

        //Заполняем ComboBoxes
        void ComboboxGO()
        {
            using(ExDbContext db = new())
            {
                AreaCombobox =  db.Areas.AsNoTracking().ToList();
                LocalCombobox =  db.Localities.AsNoTracking().ToList();
                PayCombobox = db.PayAmounts.AsNoTracking().ToList();
                PrivelCombobox = db.Privileges.AsNoTracking().ToList();
                SolCombobox =  db.SolutionTypes.AsNoTracking().ToList();
            }
        }

        //Двойной клик, обработка множественного нажатия мыши, чтоб не вылетала программа

    }
}