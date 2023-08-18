using exel_for_mfc.Models;
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
            try
            {
                Password? p = e.Row.Item as Password;
                if (p.Id != 0)
                {
                    //Редактирование
                    if (flagfix)
                    {
                        var customer = await db.Passwords.Where(c => c.Id == p.Id).FirstOrDefaultAsync();

                        customer.Login = p.Login;
                        customer.Pass = p.Pass;
                        await db.SaveChangesAsync();
                        flagfix = false;
                        dataGrid.ItemsSource = await db.Passwords.ToListAsync();
                        dataGrid.Items.Refresh();
                        dataGrid.CancelEdit();
                    }
                    flagfix = true;
                }

                else if (p.Id == 0)
                {
                    //Добавление новой записи
                    if (flagfix)
                    {
                        Password password = new();

                        if (p.Login != null)
                            password.Login = p.Login;
                        else
                            password.Login = "";

                        if (p.Pass != null)
                            password.Pass = p.Pass;
                        else
                            password.Pass = "";
                        await db.AddAsync(password);
                        await db.SaveChangesAsync();
                        flagfix = false;
                        dataGrid.ItemsSource = await db.Passwords.ToListAsync();
                        dataGrid.Items.Refresh();
                        dataGrid.CancelEdit();
                    }
                    flagfix = true;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }


        //Двойной клик, обработка множественного нажатия мыши, чтоб не вылетала программа
        private void Interes(object sender, MouseButtonEventArgs e)
        {
            return;
        }
    }
}