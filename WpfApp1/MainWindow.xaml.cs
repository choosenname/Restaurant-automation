using MahApps.Metro.Controls;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Scaffolding.Metadata;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using WpfApp1.Models;
using WpfApp1.Models.Database;

namespace WpfApp1
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        DatabaseContext db = new DatabaseContext();
        public MainWindow()
        {
            InitializeComponent();
        }
        private void TxtId_LostFocus(object sender, RoutedEventArgs e)
        {
            TextBox textBox = (TextBox)sender;
            if (string.IsNullOrWhiteSpace(textBox.Text))
            {
                textBox.Text = "Введите ваш ID";
                textBox.Foreground = Brushes.LightGray;
            }
        }

        private void TxtId_GotFocus(object sender, RoutedEventArgs e)
        {
            TextBox textBox = (TextBox)sender;
            if (textBox.Text == "Введите ваш ID")
            {
                textBox.Text = "";
                textBox.Foreground = Brushes.Black;
            }
        }

        private void Window_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            if (!txtId.IsFocused && !IsMouseOverTextBox(txtId))
            {
                if (string.IsNullOrWhiteSpace(txtId.Text))
                {
                    txtId.Text = "Введите ваш ID";
                    txtId.Foreground = Brushes.LightGray;
                }
            }
        }

        private bool IsMouseOverTextBox(TextBox textBox)
        {
            Point position = Mouse.GetPosition(textBox);
            return position.X >= 0 && position.X <= textBox.ActualWidth &&
                   position.Y >= 0 && position.Y <= textBox.ActualHeight;
        }


        private void Login_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                string id = txtId.Text;
                if (id == null) throw new Exception("Введите id");
                if (!db.Employees.Any(x => x.Id == id)) throw new Exception("Проверьте id");

                Employee employee = db.Employees.Include(x => x.EmployeeType).First(x => x.Id == id);
                switch (employee.EmployeeType.Id)
                {
                    case 1:
                        SystemAdminWindow systemAdminWindow = new SystemAdminWindow();
                        systemAdminWindow.Show();
                        break;
                    case 2:
                        RestoranAdminWindow restoranAdminWindow = new RestoranAdminWindow();
                        restoranAdminWindow.Show();
                        break;
                    case 3:
                        WaiterWindow waiterWindow = new WaiterWindow();
                        waiterWindow.Show();
                        break;
                    case 4:
                        ManagerWindow managerWindow = new ManagerWindow();
                        managerWindow.Show();
                        break;
                    default:
                        break;
                }

                // Закрыть текущее окно авторизации
                this.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

    }
}