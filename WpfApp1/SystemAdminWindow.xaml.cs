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
using WpfApp1.Models.Database;
using WpfApp1.Services;
using WpfApp1.SystemAdmin;

namespace WpfApp1
{
    /// <summary>
    /// Логика взаимодействия для SystemAdminWindow.xaml
    /// </summary>
    public partial class SystemAdminWindow : Window
    {
        private MenuService _menuService;
        public DatabaseContext _dbContext;

        public SystemAdminWindow()
        {
            InitializeComponent();
            string userName = GetCurrentUserName();
            var dbContext = new DatabaseContext();
            _menuService = new MenuService(dbContext);
            _dbContext = dbContext;
            LoadMenuDataAsync();
        }
        private async void ExportButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                // Предполагая, что _menuService уже инициализирован и содержит метод GetMenuDataAsync
                var menuData = await _menuService.GetMenuDataAsync();
                ExportMenuToWord(menuData);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при экспорте: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void ExportMenuToWord(List<DishCategory> categoriesWithDishes)
        {
            var wordApp = new Microsoft.Office.Interop.Word.Application();
            var document = wordApp.Documents.Add();

            foreach (var category in categoriesWithDishes)
            {
                // Добавляем заголовок категории
                var paragraph = document.Content.Paragraphs.Add();
                paragraph.Range.Text = category.Name;
                paragraph.Range.Font.Bold = 1; // Делаем заголовок категории жирным
                paragraph.Range.InsertParagraphAfter();

                if (category.Dishes.Any())
                {
                    // Добавляем таблицу для блюд этой категории
                    var table = document.Tables.Add(paragraph.Range, category.Dishes.Count + 1, 2);
                    table.Borders.Enable = 1; // Включаем границы таблицы
                    table.Cell(1, 1).Range.Text = "Название блюда";
                    table.Cell(1, 2).Range.Text = "Цена";
                    table.Rows[1].Range.Font.Bold = 1; // Заголовки столбцов жирным

                    int row = 2; // Начинаем заполнять таблицу со второй строки
                    foreach (var dish in category.Dishes)
                    {
                        table.Cell(row, 1).Range.Text = dish.Name;
                        table.Cell(row, 2).Range.Text = $"{dish.Price:C}";
                        row++;
                    }
                }

                // Добавляем пустой абзац после таблицы (или после заголовка, если блюд нет)
                document.Content.Paragraphs.Add().Range.InsertParagraphAfter();
            }

            wordApp.Visible = true; // Показываем документ пользователю
        }


        private async void LoadMenuDataAsync()
        {
            try
            {
                var menuData = await _menuService.GetMenuDataAsync();   
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при загрузке данных: {ex.Message}");
            }
        }
        private void Logout_Click(object sender, RoutedEventArgs e)
        {
            this.Close();

            MainWindow loginWindow = new MainWindow();
            loginWindow.Show();
        }
        private string GetCurrentUserName()
        {
            string username = string.Empty;

            try
            {
                using (DatabaseContext dbContext = new DatabaseContext()) 
                {
                    var currentUser = dbContext.Employees.FirstOrDefault();

                    if (currentUser != null)
                    {
                        username = currentUser.Name;
                    }
                    else
                    {
                        MessageBox.Show("Ошибка: Не удалось найти текущего пользователя.");
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Ошибка при получении имени текущего пользователя: " + ex.Message);
            }

            return username;
        }


        private void All_Employee_Click(object sender, RoutedEventArgs e)
        {
            AllEmployees allEmployees = new AllEmployees();
            allEmployees.ShowDialog();
        }

        private void Add_Employee_Click(object sender, RoutedEventArgs e)
        {
            AddEmployee addEmployee = new AddEmployee();
            addEmployee.ShowDialog();
        }

        private void Delete_Employee_Click(object sender, RoutedEventArgs e)
        {
            DeleteEmployee deleteEmployee = new DeleteEmployee();
            deleteEmployee.ShowDialog();
        }

        private void Delete_Dish_Click(object sender, RoutedEventArgs e)
        {
            DeleteDish deleteDish = new DeleteDish();
            deleteDish.ShowDialog();
        }

        private void Add_Dish_Click(object sender, RoutedEventArgs e)
        {
            AddDish addDish = new AddDish();
            addDish.ShowDialog();   
        }

        private void All_Dish_Click(object sender, RoutedEventArgs e)
        {
            AllDishes allDishes = new AllDishes();
            allDishes.ShowDialog();
        }
        private void ShowEmployeeSchedule_Click(object sender, RoutedEventArgs e)
        {

            List<Employee> allEmployees = _dbContext.Employees.ToList();

            // Создаем экземпляр EmployeeSchedulePage, передавая список всех сотрудников в конструктор
            EmployeeSchedulePage employeeSchedulePage = new EmployeeSchedulePage(allEmployees);

            // Отображаем страницу
            employeeSchedulePage.Show();
        }
    }
}
