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
        public static List<Area> srt { get; set; }

        private bool flagfix = true;
        public TableWindow()
        {
            
            InitializeComponent();
            ERT();
            Start();
            

            //ExelDbContext exelDb = new();
            //var str = exelDb.Registries.ToList();
            //foreach (var item in str)
            //{
            //    MessageBox.Show(item.Applicant + " " + item.Trek);
            //}

            //dataGrid.ItemsSource = db.Passwords.ToList();
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


        //Двойной клик, обработка множественного нажатия мыши, чтоб не вылетала программа
        private void Interes(object sender, MouseButtonEventArgs e)
        {
            return;
        }



      


        //Запрос для заполнения таблицы
        //Комментарий чтоб появлялся при наведении
        async void Start()
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
                                area = area.Id,
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
               
                Dispatcher.Invoke(() =>
                {
                   
                    dataGrid.ItemsSource = getlist.ToList();
                    
                });
               
            };
        }


        void ERT()
        {
            using(ExDbContext db = new())
            {
                srt = db.Areas.ToList();
            }
           
        }


        //Заполнить Comboboxы
        async Task StartToComboBox()
        {
            using (ExDbContext db = new())
            {
                
                List<decimal> str1 = new();
                List<string> str2 = new();
                List<string> str3 = new();
                List<string> str4 = new();
                List<string> str5 = new();

                var razvip = await db.PayAmounts.AsNoTracking().ToListAsync();
                var typresh = await db.SolutionTypes.AsNoTracking().ToListAsync();
                var areas = await db.Areas.AsNoTracking().ToListAsync();
                var localy = await db.Localities.AsNoTracking().ToListAsync();
                var privi = await db.Privileges.AsNoTracking().ToListAsync();
                await Task.Run(() =>
                {
                    // Размер выплат
                    foreach (var item in razvip)
                    {
                        str1.Add((decimal)item.Pay);
                    }

                    // Тип решения
                    foreach (var item in typresh)
                    {
                        str2.Add(item.SolutionName);
                    }

                    // Район
                    foreach (var item in areas)
                    {
                        str3.Add(item.AreaName);
                    }

                    // Населенный пункт
                    foreach (var item in localy)
                    {
                        str4.Add(item.LocalName);
                    }

                    // Льготы
                    foreach (var item in privi)
                    {
                        str5.Add(item.PrivilegesName);
                    }

                    Dispatcher.Invoke(() =>
                    {
                        //pay_Xaml.ItemsSource = str1;
                        //sol_Xaml.ItemsSource = str2;
                        //area_Xaml.ItemsSource = str3;
                        //loc_Xaml.ItemsSource = str4;
                        //lgota_Xaml.ItemsSource = str5;
                    });
                });
            }
        }
    }
}