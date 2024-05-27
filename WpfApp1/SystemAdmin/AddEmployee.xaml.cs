using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using WpfApp1.Models;
using WpfApp1.Models.Database;

namespace WpfApp1.SystemAdmin
{
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
            employeeCode.Text = GetUniqueCode().ToString();
            typeComboBox.ItemsSource = db.EmployeeTypes.ToList();
            typeComboBox.SelectedIndex = 0;
        }

        private DayOfWeek[] GetSelectedDaysOfWeek()
        {
            List<DayOfWeek> selectedDays = new List<DayOfWeek>();

            if (chkMonday.IsChecked == true)
                selectedDays.Add(DayOfWeek.Monday);
            if (chkTuesday.IsChecked == true)
                selectedDays.Add(DayOfWeek.Tuesday);
            if (chkWednesday.IsChecked == true)
                selectedDays.Add(DayOfWeek.Wednesday);
            if (chkThursday.IsChecked == true)
                selectedDays.Add(DayOfWeek.Thursday);
            if (chkFriday.IsChecked == true)
                selectedDays.Add(DayOfWeek.Friday);
            if (chkSaturday.IsChecked == true)
                selectedDays.Add(DayOfWeek.Saturday);
            if (chkSunday.IsChecked == true)
                selectedDays.Add(DayOfWeek.Sunday);

            return selectedDays.ToArray();
        }

        private bool IsTimeValid(string time)
        {
            return TimeSpan.TryParse(time, out _);
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

                if (string.IsNullOrWhiteSpace(timePickerStartWork.Text) || string.IsNullOrWhiteSpace(timePickerEndWork.Text))
                {
                    MessageBox.Show("Пожалуйста, введите расписание работы.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }

                if (!IsTimeValid(timePickerStartWork.Text) || !IsTimeValid(timePickerEndWork.Text))
                {
                    MessageBox.Show("Пожалуйста, введите корректное время в формате HH:mm.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }

                var selectedDays = GetSelectedDaysOfWeek();
                if (selectedDays.Length == 0)
                {
                    MessageBox.Show("Пожалуйста, выберите хотя бы один рабочий день.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }

                string id = employeeId.Text;
                string code = employeeCode.Text;
                string name = txtName.Text;
                EmployeeType type = (EmployeeType)typeComboBox.SelectedItem;
                var startWork = timePickerStartWork.Text;
                var endWork = timePickerEndWork.Text;

                Employee employee = new Employee
                {
                    Id = id,
                    Name = name,
                    EmployeeType = type,
                    TypeId = type.Id,
                    StartWork = startWork,
                    EndWork = endWork,
                    Code = code,
                    WorkDays = selectedDays,
                };

                db.Employees.Add(employee); // Don't forget to add the employee to the context
                db.SaveChanges();

                MessageBox.Show("Сотрудник успешно добавлен", "Успех", MessageBoxButton.OK, MessageBoxImage.Information);

                // Очистка полей ввода после добавления сотрудника
                txtName.Text = "";
                timePickerStartWork.Text = "";
                timePickerEndWork.Text = "";
                typeComboBox.SelectedIndex = 0;
                employeeId.Text = GetUniqueId().ToString();
                employeeCode.Text = GetUniqueCode().ToString();

                // Сбросить CheckBox-ы
                chkMonday.IsChecked = false;
                chkTuesday.IsChecked = false;
                chkWednesday.IsChecked = false;
                chkThursday.IsChecked = false;
                chkFriday.IsChecked = false;
                chkSaturday.IsChecked = false;
                chkSunday.IsChecked = false;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }
}
