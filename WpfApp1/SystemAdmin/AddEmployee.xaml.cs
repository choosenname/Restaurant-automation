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
    /// Логика взаимодействия для AddEmployee.xaml
    /// </summary>
    public partial class AddEmployee : Window
    {
        DatabaseContext db = new DatabaseContext();
        
        private int GetUniqueId()
        {
            int id;
            do
            {
                id = new Random().Next(100000, 1000000);
            } while (db.Employees.Any(x => x.Id == id.ToString()));
            return id;
        }

        private int GetUniqueCode()
        {
            int code;
            do
            {
                code = new Random().Next(100000, 1000000);
            } while (db.Employees.Any(x => x.Code == code.ToString()));
            return code;
        }

        public AddEmployee()
        {

            InitializeComponent();
            employeeId.Text = GetUniqueId().ToString();
            employeeCode.Text= GetUniqueCode().ToString();
            typeComboBox.ItemsSource = db.EmployeeTypes.ToList();
            typeComboBox.SelectedIndex = 0;
        }

        private void Add_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(txtName.Text))
                {
                    MessageBox.Show("Пожалуйста, введите имя сотрудника.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }

                if (typeComboBox.SelectedItem == null)
                {
                    MessageBox.Show("Пожалуйста, выберите тип сотрудника.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }

                if (string.IsNullOrWhiteSpace(txtWorkSchedule.Text))
                {
                    MessageBox.Show("Пожалуйста, введите расписание работы.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }

                string id = employeeId.Text;
                string code = employeeCode.Text;
                string name = txtName.Text;
                EmployeeType type = (EmployeeType)typeComboBox.SelectedItem;
                string workSchedule = txtWorkSchedule.Text;

                Employee employee = new Employee
                {
                    Id = id,
                    Name = name,
                    EmployeeType = type,
                    TypeId = type.Id,
                    WorkSchedule = workSchedule,
                    Code = code
                };

                db.Employees.Add(employee);
                db.SaveChanges();

                MessageBox.Show("Сотрудник успешно добавлен", "Успех", MessageBoxButton.OK, MessageBoxImage.Information);

                // Очистка полей ввода после добавления сотрудника
                txtName.Text = "";
                txtWorkSchedule.Text = "";
                typeComboBox.SelectedIndex = 0;
                employeeId.Text = GetUniqueId().ToString();
                employeeCode.Text = GetUniqueCode().ToString();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

    }
}
