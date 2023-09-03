using Microsoft.EntityFrameworkCore;
using Microsoft.VisualBasic;
using System;
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


        private bool flagfix = true;
        public TableWindow()
        {
            
            InitializeComponent();
            Start();
            ComboboxGO();

        }


        //Получаем измененные данные после редактирования ячейки
        private void dataGrid_CellEditEnding(object sender, DataGridCellEditEndingEventArgs e)
        {
            return;
            //try
            //{
            //    Password? p = e.Row.Item as Password;
            //    if (p.Id != 0)
            //    {
            //        //Редактирование
            //        if (flagfix)
            //        {
            //            var customer = await db.Passwords.Where(c => c.Id == p.Id).FirstOrDefaultAsync();

            //            customer.Login = p.Login;
            //            customer.Pass = p.Pass;
            //            await db.SaveChangesAsync();
            //            flagfix = false;
            //            dataGrid.ItemsSource = await db.Passwords.ToListAsync();
            //            dataGrid.Items.Refresh();
            //            dataGrid.CancelEdit();
            //        }
            //        flagfix = true;
            //    }

            //    else if (p.Id == 0)
            //    {
            //        //Добавление новой записи
            //        if (flagfix)
            //        {
            //            Password password = new();

            //            if (p.Login != null)
            //                password.Login = p.Login;
            //            else
            //                password.Login = "";

            //            if (p.Pass != null)
            //                password.Pass = p.Pass;
            //            else
            //                password.Pass = "";
            //            await db.AddAsync(password);
            //            await db.SaveChangesAsync();
            //            flagfix = false;
            //            dataGrid.ItemsSource = await db.Passwords.ToListAsync();
            //            dataGrid.Items.Refresh();
            //            dataGrid.CancelEdit();
            //        }
            //        flagfix = true;
            //    }
            //}
            //catch (Exception ex)
            //{
            //    MessageBox.Show(ex.Message);
            //}
        }


      
        //Запрос для заполнения таблицы
        //Комментарий чтоб появлялся при наведении
        async void Start()
        {
            using (ExDbContext db = new()) 
            {
                var MyList = from reg in await db.Registries.AsNoTracking().ToListAsync()
                                        join appl in await db.Applicants.AsNoTracking().ToListAsync() on reg.ApplicantFk equals appl.Id
                                        join area in await db.Areas.AsNoTracking().ToListAsync() on appl.AreaFk equals area.Id
                                        join local in await db.Localities.AsNoTracking().ToListAsync() on appl.LocalityFk equals local.Id
                                        join priv in await db.Privileges.AsNoTracking().ToListAsync() on appl.PrivilegesFk equals priv.Id
                                        join pay in await db.PayAmounts.AsNoTracking().ToListAsync() on reg.PayAmountFk equals pay.Id
                                        join sol in await db.SolutionTypes.AsNoTracking().ToListAsync() on reg.SolutionFk equals sol.Id
                                        select new
                                        {
                                            id = reg.Id,
                                            family = appl.Firstname,
                                            name = appl.Middlename,
                                            lastnam = appl.Lastname,
                                            snils = appl.Snils,
                                            area = area.Id - 1,
                                            loc = local.Id - 1,
                                            adres = appl.Adress,
                                            privel = priv.Id - 1,
                                            pays = pay.Id - 1,
                                            sernumb = reg.SerialAndNumberSert,
                                            dategetsert = reg.DateGetSert,
                                            solnam = sol.Id - 1,
                                            datenumsol = reg.DateAndNumbSolutionSert,
                                            com = reg.Comment,
                                            trek = reg.Trek,
                                            mail = reg.MailingDate
                                        };
                dataGrid.ItemsSource = MyList;
            };
        }

        //Заполняем ComboBoxes
       async void ComboboxGO()
        {
            using(ExDbContext db = new())
            {
                AreaCombobox = await db.Areas.AsNoTracking().ToListAsync();
                LocalCombobox = await db.Localities.AsNoTracking().ToListAsync();
                PayCombobox = await db.PayAmounts.AsNoTracking().ToListAsync();
                PrivelCombobox = await db.Privileges.AsNoTracking().ToListAsync();
                SolCombobox = await db.SolutionTypes.AsNoTracking().ToListAsync();
            }
        }

        //Двойной клик, обработка множественного нажатия мыши, чтоб не вылетала программа
        private void Interes(object sender, MouseButtonEventArgs e)
        {
            return;
        }


    }
}