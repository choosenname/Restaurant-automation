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
using Microsoft.Office.Interop.Excel;
using System.Collections.Generic;
using System.Globalization;
using Microsoft.Office.Interop.Word;

namespace WpfApp1
{
    /// <summary>
    /// Логика взаимодействия для SystemAdminWindow.xaml
    /// </summary>
    public partial class SystemAdminWindow : System.Windows.Window
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

            // Добавляем дату составления документа
            var dateParagraph = document.Content.Paragraphs.Add();
            dateParagraph.Range.Text = $"Дата составления: {DateTime.Now.ToString("dd.MM.yyyy")}";
            dateParagraph.Range.Font.Size = 12;
            dateParagraph.Format.Alignment = WdParagraphAlignment.wdAlignParagraphRight;
            dateParagraph.Range.InsertParagraphAfter();

            // Добавляем заголовок документа
            var docTitleParagraph = document.Content.Paragraphs.Add();
            docTitleParagraph.Range.Text = "Меню ресторана";
            docTitleParagraph.Range.Font.Size = 24;
            docTitleParagraph.Range.Font.Bold = 1;
            docTitleParagraph.Format.Alignment = WdParagraphAlignment.wdAlignParagraphCenter;
            docTitleParagraph.Range.InsertParagraphAfter();

            decimal totalMenuCost = 0;
            int totalDishesCount = 0;
            int totalCategoriesCount = categoriesWithDishes.Count;

            foreach (var category in categoriesWithDishes)
            {
                int categoryDishesCount = category.Dishes.Count;
                string categoryDishesText = GetDishCountText(categoryDishesCount);

                // Добавляем заголовок категории
                var categoryParagraph = document.Content.Paragraphs.Add();
                categoryParagraph.Range.Text = $"{category.Name} ({categoryDishesText})";
                categoryParagraph.Range.Font.Size = 18;
                categoryParagraph.Range.Font.Bold = 1;
                categoryParagraph.Format.Alignment = WdParagraphAlignment.wdAlignParagraphLeft;
                categoryParagraph.Range.InsertParagraphAfter();

                decimal categoryCost = 0;

                if (categoryDishesCount > 0)
                {
                    // Добавляем таблицу для блюд этой категории
                    var table = document.Tables.Add(categoryParagraph.Range, categoryDishesCount + 1, 2);
                    table.Borders.Enable = 1; // Включаем границы таблицы
                    table.Cell(1, 1).Range.Text = "Название блюда";
                    table.Cell(1, 2).Range.Text = "Цена";
                    table.Rows[1].Range.Font.Bold = 1; // Заголовки столбцов жирным

                    int row = 2; // Начинаем заполнять таблицу со второй строки
                    foreach (var dish in category.Dishes)
                    {
                        table.Cell(row, 1).Range.Text = dish.Name;
                        table.Cell(row, 2).Range.Text = $"{dish.Price.ToString("N2")} бел.руб";

                        categoryCost += dish.Price;
                        totalMenuCost += dish.Price;
                        row++;
                    }
                }

                totalDishesCount += categoryDishesCount;

                // Добавляем итоговую строку для категории
                var categoryTotalParagraph = document.Content.Paragraphs.Add();
                categoryTotalParagraph.Range.Text = $"Итог по категории '{category.Name}': {categoryCost.ToString("N2")} бел.руб";
                categoryTotalParagraph.Range.Font.Bold = 1;
                categoryTotalParagraph.Range.InsertParagraphAfter();

                // Добавляем пустой абзац после таблицы (или после заголовка, если блюд нет)
                document.Content.Paragraphs.Add().Range.InsertParagraphAfter();
            }

            // Добавляем общий итог по всему меню
            var totalSummaryParagraph = document.Content.Paragraphs.Add();
            totalSummaryParagraph.Range.Text = $"Общее количество категорий: {totalCategoriesCount}\n" +
                                                $"Общее количество блюд: {totalDishesCount}\n" +
                                                $"Общая стоимость меню: {totalMenuCost.ToString("N2")} бел.руб";
            totalSummaryParagraph.Range.Font.Size = 16;
            totalSummaryParagraph.Range.Font.Bold = 1;
            totalSummaryParagraph.Range.InsertParagraphAfter();

            wordApp.Visible = true; // Показываем документ пользователю
        }


        private void ExportMenuToExcel(List<DishCategory> categoriesWithDishes)
        {
            var excelApp = new Microsoft.Office.Interop.Excel.Application();
            var workbook = excelApp.Workbooks.Add();
            var worksheet = (Microsoft.Office.Interop.Excel.Worksheet)workbook.Worksheets[1];

            int currentRow = 1;
            decimal totalMenuCost = 0;
            int totalDishesCount = 0;
            int totalCategoriesCount = categoriesWithDishes.Count;

            // Добавляем дату составления документа
            worksheet.Cells[currentRow, 1] = $"Дата составления: {DateTime.Now.ToString("dd.MM.yyyy")}";
            worksheet.Cells[currentRow, 1].Font.Size = 12;
            worksheet.Cells[currentRow, 1].HorizontalAlignment = Microsoft.Office.Interop.Excel.XlHAlign.xlHAlignRight;
            worksheet.Range["A1", "B1"].Merge();
            currentRow += 2;

            // Добавляем заголовок документа
            worksheet.Cells[currentRow, 1] = "Меню ресторана";
            worksheet.Cells[currentRow, 1].Font.Size = 24;
            worksheet.Cells[currentRow, 1].Font.Bold = true;
            worksheet.Cells[currentRow, 1].HorizontalAlignment = Microsoft.Office.Interop.Excel.XlHAlign.xlHAlignCenter;
            worksheet.Range[$"A{currentRow}", $"B{currentRow}"].Merge();
            currentRow += 2;

            foreach (var category in categoriesWithDishes)
            {
                int categoryDishesCount = category.Dishes.Count;
                string categoryDishesText = GetDishCountText(categoryDishesCount);

                // Добавляем заголовок категории
                worksheet.Cells[currentRow, 1] = $"{category.Name} ({categoryDishesText})";
                worksheet.Cells[currentRow, 1].Font.Size = 18;
                worksheet.Cells[currentRow, 1].Font.Bold = true;
                currentRow++;

                decimal categoryCost = 0;

                if (categoryDishesCount > 0)
                {
                    // Добавляем заголовки столбцов
                    worksheet.Cells[currentRow, 1] = "Название блюда";
                    worksheet.Cells[currentRow, 2] = "Цена";
                    worksheet.Rows[currentRow].Font.Bold = true;
                    currentRow++;

                    // Заполняем таблицу блюдами
                    foreach (var dish in category.Dishes)
                    {
                        worksheet.Cells[currentRow, 1] = dish.Name;
                        worksheet.Cells[currentRow, 2].Value = $"{dish.Price.ToString("N2")} бел.руб";

                        categoryCost += dish.Price;
                        totalMenuCost += dish.Price;
                        currentRow++;
                    }
                }

                totalDishesCount += categoryDishesCount;

                // Добавляем итоговую строку для категории
                worksheet.Cells[currentRow, 1] = $"Итог по категории '{category.Name}':";
                worksheet.Cells[currentRow, 2] = $"{categoryCost.ToString("N2")} бел.руб";
                worksheet.Rows[currentRow].Font.Bold = true;
                currentRow++;

                // Добавляем пустую строку после категории
                currentRow++;
            }

            // Добавляем общий итог по всему меню
            worksheet.Cells[currentRow, 1] = "Общее количество категорий:";
            worksheet.Cells[currentRow, 2] = totalCategoriesCount;
            worksheet.Rows[currentRow].Font.Bold = true;
            currentRow++;

            worksheet.Cells[currentRow, 1] = "Общее количество блюд:";
            worksheet.Cells[currentRow, 2] = totalDishesCount;
            worksheet.Rows[currentRow].Font.Bold = true;
            currentRow++;

            worksheet.Cells[currentRow, 1] = "Общая стоимость меню:";
            worksheet.Cells[currentRow, 2] = $"{totalMenuCost.ToString("N2")} бел.руб";
            worksheet.Rows[currentRow].Font.Bold = true;
            currentRow++;

            // Автоматическое изменение размера столбцов по содержимому
            worksheet.Columns.AutoFit();

            // Автоматическое изменение размера строк по содержимому
            worksheet.Rows.AutoFit();

            excelApp.Visible = true;
        }


        // Функция для получения текста с правильным склонением для числа блюд
        private string GetDishCountText(int count)
        {
            if (count % 10 == 1 && count % 100 != 11)
                return $"{count} блюдо";
            else if (count % 10 >= 2 && count % 10 <= 4 && (count % 100 < 10 || count % 100 >= 20))
                return $"{count} блюда";
            else
                return $"{count} блюд";
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

        private async void ExportExcelButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                // Предполагая, что _menuService уже инициализирован и содержит метод GetMenuDataAsync
                var menuData = await _menuService.GetMenuDataAsync();
                ExportMenuToExcel(menuData);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при экспорте: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }
}
