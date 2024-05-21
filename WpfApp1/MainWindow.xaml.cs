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
                textBox.Text = "Введите ваш код";
                textBox.Foreground = Brushes.LightGray;
            }
        }

        private void TxtId_GotFocus(object sender, RoutedEventArgs e)
        {
            TextBox textBox = (TextBox)sender;
            if (textBox.Text == "Введите ваш код")
            {
                textBox.Text = "";
                textBox.Foreground = Brushes.Black;
            }
        }

        private void Window_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            if (!txtCode.IsFocused && !IsMouseOverTextBox(txtCode))
            {
                if (string.IsNullOrWhiteSpace(txtCode.Text))
                {
                    txtCode.Text = "Введите ваш код";
                    txtCode.Foreground = Brushes.LightGray;
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
                string code = txtCode.Text;
                if (code == null) throw new Exception("Введите код");
                if (!db.Employees.Any(x => x.Code == code)) throw new Exception("Проверьте код");

                Employee employee = db.Employees.Include(x => x.EmployeeType).First(x => x.Code == code);
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
                //this.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

    }
}