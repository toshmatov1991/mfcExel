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
    public partial class AdminWindow : Window
    {
        public static List<Area>? AreaDataGrid { get; set; }
        public static List<Locality>? LocalDataGrid { get; set; }
        public static List<PayAmount>? PayDataGrid { get; set; }
        public static List<Privilege>? PrivelDataGrid { get; set; }
        public static List<SolutionType>? SolDataGrid { get; set; }


        public AdminWindow()
        {
            InitializeComponent();
            StartAdminWin();
        }

        private void StartAdminWin()
        {
            using ExDbContext db = new();

            AreaDataGrid = db.Areas.FromSqlRaw("SELECT * FROM Area").ToList();
            AreaX.ItemsSource = AreaDataGrid;

            LocalDataGrid = db.Localities.FromSqlRaw("SELECT * FROM Locality").ToList();
            LocalX.ItemsSource = LocalDataGrid;

            PayDataGrid = db.PayAmounts.FromSqlRaw("SELECT * FROM PayAmount").ToList();
            PayX.ItemsSource = PayDataGrid;

            PrivelDataGrid = db.Privileges.FromSqlRaw("SELECT * FROM Privileges").ToList();
            PrivelX.ItemsSource = PrivelDataGrid;

            SolDataGrid = db.SolutionTypes.FromSqlRaw("SELECT * FROM SolutionType").ToList();
            SolutionX.ItemsSource = SolDataGrid;
            AdminsX.ItemsSource = SolDataGrid;
        }

        private async void AreaCell(object sender, DataGridCellEditEndingEventArgs e)
        {
            //Считывание строки
            Area? a = e.Row.Item as Area;

            using ExDbContext db = new();

            if (a.Id != 0)
            {
                //Обновление таблицы Район
                await db.Database.ExecuteSqlRawAsync("UPDATE Area SET AreaName = {0} WHERE Id = {1}", a.AreaName, a.Id);
            }

            else if (a.Id == 0)
            {
                // Добавление записи
                if (a.AreaName != null)
                {
                    //Добавить новую запись в таблицу Район
                    await db.Database.ExecuteSqlInterpolatedAsync($"INSERT INTO Area(AreaName) VALUES({a.AreaName})");
                    await Task.Delay(50);
                    StartAdminWin();
                }
                else return;
            }
        }

        private async void LocalCell(object sender, DataGridCellEditEndingEventArgs e)
        {
            //Считывание строки
            Locality? a = e.Row.Item as Locality;

            using ExDbContext db = new();

            if (a.Id != 0)
            {
                //Обновление таблицы Населенный пункт
                await db.Database.ExecuteSqlRawAsync("UPDATE Locality SET LocalName = {0} WHERE Id = {1}", a.LocalName, a.Id);
            }

            else if (a.Id == 0)
            {
                // Добавление записи
                if (a.LocalName != null)
                {
                    //Добавить новую запись в таблицу Населенный пункт
                    await db.Database.ExecuteSqlInterpolatedAsync($"INSERT INTO Locality(LocalName) VALUES({a.LocalName})");
                    await Task.Delay(50);
                    StartAdminWin();
                }
                else return;
            }
        }

        private async void LgotaCell(object sender, DataGridCellEditEndingEventArgs e)
        {
            //Считывание строки
            Privilege? a = e.Row.Item as Privilege;

            using ExDbContext db = new();

            if (a.Id != 0)
            {
                //Обновление таблицы Льгота
                await db.Database.ExecuteSqlRawAsync("UPDATE Privileges SET PrivilegesName = {0} WHERE Id = {1}", a.PrivilegesName, a.Id);
            }

            else if (a.Id == 0)
            {
                // Добавление записи
                if (a.PrivilegesName != null)
                {
                    //Добавить новую запись в таблицу Льгота
                    await db.Database.ExecuteSqlInterpolatedAsync($"INSERT INTO Privileges(PrivilegesName) VALUES({a.PrivilegesName})");
                    await Task.Delay(50);
                    StartAdminWin();
                }
                else return;
            }
        }

        private async void PayCell(object sender, DataGridCellEditEndingEventArgs e)
        {
            //Считывание строки
            PayAmount? a = e.Row.Item as PayAmount;

            using ExDbContext db = new();

            if (a.Id != 0)
            {
                //Обновление таблицы Выплаты
                await db.Database.ExecuteSqlRawAsync("UPDATE PayAmount SET PrivilegesName = {0} WHERE Id = {1}", a.Pay, a.Id);
            }

            else if (a.Id == 0)
            {
                // Добавление записи
                if (a.Pay != null)
                {
                    //Добавить новую запись в таблицу Выплаты
                    await db.Database.ExecuteSqlInterpolatedAsync($"INSERT INTO PayAmount(Pay) VALUES({a.Pay})");
                    await Task.Delay(50);
                    StartAdminWin();
                }
                else return;
            }
        }

        private async void SolutionCell(object sender, DataGridCellEditEndingEventArgs e)
        {
            //Считывание строки
            SolutionType? a = e.Row.Item as SolutionType;

            using ExDbContext db = new();

            if (a.Id != 0)
            {
                //Обновление таблицы Выплаты
                await db.Database.ExecuteSqlRawAsync("UPDATE SolutionType SET SolutionName = {0} WHERE Id = {1}", a.SolutionName, a.Id);
            }

            else if (a.Id == 0)
            {
                // Добавление записи
                if (a.SolutionName != null)
                {
                    //Добавить новую запись в таблицу Выплаты
                    await db.Database.ExecuteSqlInterpolatedAsync($"INSERT INTO SolutionType(SolutionName) VALUES({a.SolutionName})");
                    await Task.Delay(50);
                    StartAdminWin();
                }
                else return;
            }
        }

        private async void AdminCell(object sender, DataGridCellEditEndingEventArgs e)
        {
            //Считывание строки
            SolutionType? a = e.Row.Item as SolutionType;

            using ExDbContext db = new();

            if (a.Id != 0)
            {
                //Обновление таблицы Выплаты
                await db.Database.ExecuteSqlRawAsync("UPDATE SolutionType SET Login = {0}, Passwords = {1} WHERE Id = {2}", a.Login, a.Passwords, a.Id);
            }
          
        }
    }
}