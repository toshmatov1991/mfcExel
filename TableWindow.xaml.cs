using exel_for_mfc.PassModels;
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
     

            dataGrid.ItemsSource = db.Passwords.ToList();


        }


        //Получаем измененные данные после редактирования ячейки
        private void dataGrid_CellEditEnding(object sender, DataGridCellEditEndingEventArgs e)
        {
            Password p = e.Row.Item as Password;
            if (p.Id != 0)
            {
                if (flagfix)
                {
                    var customer = db.Passwords
                   .Where(c => c.Id == p.Id)
                   .FirstOrDefault();
                    customer.Id = p.Id;
                    customer.Login = p.Login;
                    customer.Pass = p.Pass;
                    db.SaveChanges();
                    flagfix = false;
                    dataGrid.CancelEdit();
                    dataGrid.CancelEdit();
                    flagfix = true;
                    dataGrid.Items.Refresh();
                }
            }

            else if(p.Id == 0)
            {
                 Password password = new();
                    password.Login = p.Login;
                    password.Pass = p.Pass;
                    db.Add(password);
                    db.SaveChanges();
                    flagfix = false;
                    dataGrid.CancelEdit();
                    dataGrid.CancelEdit();
                    flagfix = true;
                    dataGrid.Items.Refresh();
             
              
            }
           
        }

    }
}
