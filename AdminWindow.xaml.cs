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
        }



    }
}
