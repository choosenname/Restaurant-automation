using Microsoft.Office.Interop.Word;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using WpfApp1.Models.Database;
using Paragraph = Microsoft.Office.Interop.Word.Paragraph;
using Table = Microsoft.Office.Interop.Word.Table;
using Window = System.Windows.Window;

namespace WpfApp1
{
    public partial class EmployeeSchedulePage : Window
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
            var translationDict = new Dictionary<string, string>
            {
                {"Monday", "Понедельник"},
                {"Tuesday", "Вторник"},
                {"Wednesday", "Среда"},
                {"Thursday", "Четверг"},
                {"Friday", "Пятница"},
                {"Saturday", "Суббота"},
                {"Sunday", "Воскресенье"}
            };

            return translationDict.TryGetValue(englishDayOfWeek, out var translatedDay)
                ? translatedDay
                : englishDayOfWeek;
        }

        private int GetWorkingDaysCount(Employee employee, DateTime startDate, DateTime endDate)
        {
            int workingDaysCount = 0;
            DateTime currentDate = startDate;

            while (currentDate <= endDate)
            {
                if (employee.WorkDays.Contains(currentDate.DayOfWeek))
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

            var employeeData = employees.Select(employee => new
            {
                employee.Name,
                employee.StartWork,
                employee.EndWork,
                WorkDaysFormatted = string.Join(", ", employee.WorkDays.Select(day => TranslateDayOfWeek(day.ToString()))),
                WorkingDaysThisMonth = GetWorkingDaysCount(employee, currentDate.AddDays(1 - currentDate.Day), currentDate.AddMonths(1).AddDays(-currentDate.Day)),
                RemainingWorkingDaysThisMonth = GetWorkingDaysCount(employee, currentDate, currentDate.AddMonths(1).AddDays(-currentDate.Day))
            }).ToList();

            EmployeesDataGrid.ItemsSource = employeeData;
        }

        private void ExportToWord_Click(object sender, RoutedEventArgs e)
        {
            var winword = new Microsoft.Office.Interop.Word.Application { ShowAnimation = false, Visible = true };
            object missing = System.Reflection.Missing.Value;
            Document document = winword.Documents.Add(ref missing, ref missing, ref missing, ref missing);

            var docTitleParagraph = document.Content.Paragraphs.Add(ref missing);
            docTitleParagraph.Range.Text = "График работы";
            docTitleParagraph.Range.Font.Size = 24;
            docTitleParagraph.Range.Font.Bold = 1;
            docTitleParagraph.Format.Alignment = WdParagraphAlignment.wdAlignParagraphCenter;
            docTitleParagraph.Format.SpaceAfter = 10;

            var beforeTableParagraph = document.Content.Paragraphs.Add(ref missing);
            beforeTableParagraph.Range.Text = "График работы сотрудников";
            beforeTableParagraph.Range.Font.Size = 20;
            beforeTableParagraph.Range.Font.Bold = 1;
            beforeTableParagraph.Format.Alignment = WdParagraphAlignment.wdAlignParagraphCenter;
            beforeTableParagraph.Format.SpaceAfter = 10;

            Table table = document.Tables.Add(beforeTableParagraph.Range, Employees.Count + 1, 6, ref missing, ref missing);
            table.Borders.Enable = 1;

            table.Cell(1, 1).Range.Text = "Работник";
            table.Cell(1, 2).Range.Text = "Начало работы";
            table.Cell(1, 3).Range.Text = "Конец работы";
            table.Cell(1, 4).Range.Text = "Рабочие дни";
            table.Cell(1, 5).Range.Text = "Смен в этом месяце";
            table.Cell(1, 6).Range.Text = "Оставшихся смен в этом месяце";

            DateTime currentDate = DateTime.Now;

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

            string currentTime = DateTime.Now.ToString("yyyyMMddHHmmss");
            object filename = $"EmployeeSchedule_{currentTime}.docx";
            object filePath = System.IO.Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), (string)filename);
            document.SaveAs2(ref filePath);
        }

        private void ExportToExcel_Click(object sender, RoutedEventArgs e)
        {
            var excelApp = new Microsoft.Office.Interop.Excel.Application();
            var workbook = excelApp.Workbooks.Add();
            var worksheet = (Microsoft.Office.Interop.Excel.Worksheet)workbook.Worksheets[1];

            int currentRow = 1;

            worksheet.Cells[currentRow, 1] = "График работы сотрудников";
            worksheet.Cells[currentRow, 1].Font.Size = 24;
            worksheet.Cells[currentRow, 1].Font.Bold = true;
            worksheet.Cells[currentRow, 1].HorizontalAlignment = Microsoft.Office.Interop.Excel.XlHAlign.xlHAlignCenter;
            worksheet.Range["A1", "F1"].Merge();
            currentRow += 2;

            worksheet.Cells[currentRow, 1] = "Работник";
            worksheet.Cells[currentRow, 2] = "Начало работы";
            worksheet.Cells[currentRow, 3] = "Конец работы";
            worksheet.Cells[currentRow, 4] = "Рабочие дни";
            worksheet.Cells[currentRow, 5] = "Смен в этом месяце";
            worksheet.Cells[currentRow, 6] = "Оставшихся смен в этом месяце";

            worksheet.Rows[currentRow].Font.Bold = true;
            currentRow++;

            DateTime currentDate = DateTime.Now;

            for (int i = 0; i < Employees.Count; i++)
            {
                string translatedWorkDays = string.Join(", ", Employees[i].WorkDays.Select(day => TranslateDayOfWeek(day.ToString())));

                int workingDaysThisMonth = GetWorkingDaysCount(Employees[i], currentDate.AddDays(1 - currentDate.Day), currentDate.AddMonths(1).AddDays(-currentDate.Day));
                int remainingWorkingDaysThisMonth = GetWorkingDaysCount(Employees[i], currentDate, currentDate.AddMonths(1).AddDays(-currentDate.Day));

                worksheet.Cells[currentRow, 1] = Employees[i].Name;
                worksheet.Cells[currentRow, 2] = Employees[i].StartWork;
                worksheet.Cells[currentRow, 3] = Employees[i].EndWork;
                worksheet.Cells[currentRow, 4] = translatedWorkDays;
                worksheet.Cells[currentRow, 5] = workingDaysThisMonth.ToString();
                worksheet.Cells[currentRow, 6] = remainingWorkingDaysThisMonth.ToString();
                currentRow++;
            }

            worksheet.Columns.AutoFit();
            worksheet.Rows.AutoFit();

            excelApp.Visible = true;

            string currentTime = DateTime.Now.ToString("yyyyMMddHHmmss");
            string filename = $"EmployeeSchedule_{currentTime}.xlsx";
            string filePath = System.IO.Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), filename);
            workbook.SaveAs(filePath);
        }
    }
}
