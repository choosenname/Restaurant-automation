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
using WpfApp1.Models;

namespace WpfApp1.SystemAdmin
{
    /// <summary>
    /// Логика взаимодействия для AllEmployees.xaml
    /// </summary>
    public partial class AllEmployees : Window
    {
        DatabaseContext db = new DatabaseContext();
        public AllEmployees()
        {
            InitializeComponent();
            dataGrid.ItemsSource = db.Employees.Include(x => x.EmployeeType).ToList();
        }
    }
}
