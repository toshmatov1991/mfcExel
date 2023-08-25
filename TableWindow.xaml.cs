using exel_for_mfc.PassModels;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
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
        PassContext db = new();
        private bool flagfix = true;
        public TableWindow()
        {
            InitializeComponent();
            //ExelDbContext exelDb = new();
            //var str = exelDb.Registries.ToList();
            //foreach (var item in str)
            //{
            //    MessageBox.Show(item.Applicant + " " + item.Trek);
            //}
           
            //dataGrid.ItemsSource = db.Passwords.ToList();
        }


        //Получаем измененные данные после редактирования ячейки
        private async void dataGrid_CellEditEnding(object sender, DataGridCellEditEndingEventArgs e)
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


        //Двойной клик, обработка множественного нажатия мыши, чтоб не вылетала программа
        private void Interes(object sender, MouseButtonEventArgs e)
        {
            return;
        }

        //Запрос для заполнения таблицы
        //Комментарий чтоб появлялся при наведении
        async Task Start()
        {
            using (ExDbContext db = new()) 
            {
                var getlist = from reg in await db.Registries.AsNoTracking().ToListAsync()
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
                                area = area.AreaName,
                                loc = local.LocalName,
                                adres = appl.Adress,
                                privel = priv.PrivilegesName,
                                pays = pay.Pay,
                                sernumb = reg.SerialAndNumberSert,
                                dategetsert = reg.DateGetSert,
                                solnam = sol.SolutionName,
                                datenumsol = reg.DateAndNumbSolutionSert,
                                com = reg.Comment,
                                trek = reg.Trek,
                                mail = reg.MailingDate
                              };

                dataGrid.ItemsSource = getlist;
            };
        }

        //Заполнить Comboboxы
        async Task StartToComboBox()
        {
            using (ExDbContext db = new())
            {
                // Размер выплат
                var razvip = await db.PayAmounts.ToListAsync();
                foreach (var item in collection)
                {

                }
                // Тип решения
                var typresh = await db.SolutionTypes.ToListAsync();

                // Район
                var areas = await db.Areas.ToListAsync();

                // Населенный пункт
                var localy = await db.Localities.ToListAsync();

                // Льготы
                var privi = await db.Privileges.ToListAsync();

            }
        }
    }
}