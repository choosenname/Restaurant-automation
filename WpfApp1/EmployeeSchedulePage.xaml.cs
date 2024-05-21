using Microsoft.Office.Interop.Word;
using System;
using System.Collections.Generic;
using System.Globalization;
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
using WpfApp1.Models.Database;
using Paragraph = Microsoft.Office.Interop.Word.Paragraph;
using Table = Microsoft.Office.Interop.Word.Table;

namespace WpfApp1
{
    /// <summary>
    /// Логика взаимодействия для EmployeeSchedulePage.xaml
    /// </summary>
    public partial class EmployeeSchedulePage : System.Windows.Window
    {
        public List<Employee> Employees { get; set; }

        public EmployeeSchedulePage(List<Employee> employees)
        {
            InitializeComponent();
            Employees = employees;
            DisplayEmployees(employees);
        }

        private string TranslateDayOfWeek(string englishDayOfWeek)
        {
            Dictionary<string, string> translationDict = new Dictionary<string, string>
            {
                {"Monday", "Понедельник"},
                {"Tuesday", "Вторник"},
                {"Wednesday", "Среда"},
                {"Thursday", "Четверг"},
                {"Friday", "Пятница"},
                {"Saturday", "Суббота"},
                {"Sunday", "Воскресенье"}
            };

            // Проверяем, есть ли в словаре английское название дня недели
            if (translationDict.ContainsKey(englishDayOfWeek))
            {
                // Если есть, возвращаем русское название
                return translationDict[englishDayOfWeek];
            }
            else
            {
                // Если нет, возвращаем исходное английское название
                return englishDayOfWeek;
            }
        }

        private int GetWorkingDaysCount(Employee employees, DateTime startDate, DateTime endDate)
        {
            int workingDaysCount = 0;
            DateTime currentDate = startDate;

            while (currentDate <= endDate)
            {
                if (employees.WorkDays.Contains(currentDate.DayOfWeek))
                {
                    workingDaysCount++;
                }
                currentDate = currentDate.AddDays(1);
            }

            return workingDaysCount;
        }

        private void DisplayEmployees(List<Employee> employees)
        {
            DateTime currentDate = DateTime.Now;

            foreach (var employee in employees)
            {
                string translatedWorkDays = string.Join(", ", employee.WorkDays.Select(day => TranslateDayOfWeek(day.ToString())));

                int workingDaysThisMonth = GetWorkingDaysCount(employee, currentDate.AddDays(1 - currentDate.Day), currentDate.AddMonths(1).AddDays(-currentDate.Day));
                int remainingWorkingDaysThisMonth = GetWorkingDaysCount(employee, currentDate, currentDate.AddMonths(1).AddDays(-currentDate.Day));

                TextBlock textBlock = new TextBlock();
                textBlock.Text = $"Работник: {employee.Name}, Начало работы: {employee.StartWork} конец работы: {employee.EndWork}" +
                    $" рабочие дни: {translatedWorkDays}, смен в этом месяце: {workingDaysThisMonth}, " +
                    $"оставшихся смен в этом месяце: {remainingWorkingDaysThisMonth}";
                EmployeesStackPanel.Children.Add(textBlock);
            }
        }
        private void ExportToWord_Click(object sender, RoutedEventArgs e)
        {
            // Создаем экземпляр приложения Word
            Microsoft.Office.Interop.Word.Application winword = new Microsoft.Office.Interop.Word.Application();
            winword.ShowAnimation = false;
            winword.Visible = true;
            object missing = System.Reflection.Missing.Value;

            // Создаем новый документ Word
            Document document = winword.Documents.Add(ref missing, ref missing, ref missing, ref missing);

            // Создаем строку заголовка
            Paragraph headerParagraph = document.Content.Paragraphs.Add(ref missing);
            headerParagraph.Range.Text = "График работы сотрудников";
            headerParagraph.Range.Font.Size = 20;
            headerParagraph.Range.Font.Bold = 1;
            headerParagraph.Format.Alignment = WdParagraphAlignment.wdAlignParagraphCenter;
            headerParagraph.Format.SpaceAfter = 10;

            // Создаем таблицу с двумя столбцами и заголовками
            Table table = document.Tables.Add(headerParagraph.Range, Employees.Count + 1, 6, ref missing, ref missing);
            table.Borders.Enable = 1;

            // Добавляем заголовки столбцов
            table.Cell(1, 1).Range.Text = "Работник";
            table.Cell(1, 2).Range.Text = "Начало работы";
            table.Cell(1, 3).Range.Text = "Конец работы";
            table.Cell(1, 4).Range.Text = "Рабочие дни";
            table.Cell(1, 5).Range.Text = "Смен в этом месяце";
            table.Cell(1, 6).Range.Text = "Оставшихся смен в этом месяце";

            DateTime currentDate = DateTime.Now;

            // Заполняем таблицу данными
            for (int i = 0; i < Employees.Count; i++)
            {
                string translatedWorkDays = string.Join(", ", Employees[i].WorkDays.Select(day => TranslateDayOfWeek(day.ToString())));

                int workingDaysThisMonth = GetWorkingDaysCount(Employees[i], currentDate.AddDays(1 - currentDate.Day), currentDate.AddMonths(1).AddDays(-currentDate.Day));
                int remainingWorkingDaysThisMonth = GetWorkingDaysCount(Employees[i], currentDate, currentDate.AddMonths(1).AddDays(-currentDate.Day));

                table.Cell(i + 2, 1).Range.Text = Employees[i].Name;
                table.Cell(i + 2, 2).Range.Text = Employees[i].StartWork;
                table.Cell(i + 2, 3).Range.Text = Employees[i].EndWork;
                table.Cell(i + 2, 4).Range.Text = translatedWorkDays;
                table.Cell(i + 2, 5).Range.Text = workingDaysThisMonth.ToString();
                table.Cell(i + 2, 6).Range.Text = remainingWorkingDaysThisMonth.ToString();
            }

            // Формируем имя файла с текущим временем
            string currentTime = DateTime.Now.ToString("yyyyMMddHHmmss");
            object filename = $"EmployeeSchedule_{currentTime}.docx";

            // Сохраняем документ
            object filePath = System.IO.Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), (string)filename);
            document.SaveAs2(ref filePath);
        }

    }
}
