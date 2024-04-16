using Microsoft.Office.Interop.Word;
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

        private void DisplayEmployees(List<Employee> employees)
        {
            foreach (var employee in employees)
            {
                TextBlock textBlock = new TextBlock();
                textBlock.Text = $"Работник: {employee.Name}, График работы: {employee.WorkSchedule}";
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
            Table table = document.Tables.Add(headerParagraph.Range, Employees.Count + 1, 2, ref missing, ref missing);
            table.Borders.Enable = 1;

            // Добавляем заголовки столбцов
            table.Cell(1, 1).Range.Text = "Работник";
            table.Cell(1, 2).Range.Text = "График работы";

            // Заполняем таблицу данными
            for (int i = 0; i < Employees.Count; i++)
            {
                table.Cell(i + 2, 1).Range.Text = Employees[i].Name;
                table.Cell(i + 2, 2).Range.Text = Employees[i].WorkSchedule;
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
