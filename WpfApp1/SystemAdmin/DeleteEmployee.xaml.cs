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
using WpfApp1.Models.Database;

namespace WpfApp1.SystemAdmin
{
    /// <summary>
    /// Логика взаимодействия для DeleteEmployee.xaml
    /// </summary>
    public partial class DeleteEmployee : Window
    {
        DatabaseContext db = new DatabaseContext();
        public DeleteEmployee()
        {
            InitializeComponent();
            DataContext = new DeleteViewModel();
        }

        private void Delete_Click(object sender, RoutedEventArgs e)
        {
            var viewModel = DataContext as DeleteViewModel;

            if(viewModel != null)
            {
                List<string> ids = viewModel.EmplDeletes
                    .Where(x => x.IsSelected)
                    .Select(x => x.Id)
                    .ToList();
                foreach (string id in ids)
                {
                    Employee employee = db.Employees.First(x => x.Id == id);
                    db.Employees.Remove(employee);
                }
                MessageBox.Show("Выбранные сотрудники удалены");
                db.SaveChanges();
                DataContext = new DeleteViewModel();
            }
        }
    }
}
