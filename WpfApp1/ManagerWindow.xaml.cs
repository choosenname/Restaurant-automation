using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows;
using Microsoft.Office.Interop.Excel;
using WpfApp1.Models;
using WpfApp1.Models.Database;
using Application = Microsoft.Office.Interop.Excel.Application;

namespace WpfApp1
{
    public partial class ManagerWindow : System.Windows.Window
    {
        private List<Order> _orders
        {
            get
            {
                return db.Orders.ToList();
            }
        }
        private readonly List<CancellationReport> _cancellationReports;
        DatabaseContext db = new DatabaseContext();

        public ManagerWindow()
        {
            InitializeComponent();
            _cancellationReports = db.CancellationReports.ToList();
        }


        private void GenerateReport_Click(object sender, RoutedEventArgs e)
        {
            // Проверка на выбор дат
            if (!StartDatePicker.SelectedDate.HasValue || !EndDatePicker.SelectedDate.HasValue)
            {
                MessageBox.Show("Please select both start and end dates.");
                return;
            }

            var startDate = StartDatePicker.SelectedDate.Value;
            var endDate = EndDatePicker.SelectedDate.Value;

            // Проверка на корректность диапазона дат
            if (endDate < startDate)
            {
                MessageBox.Show("End date must be greater than or equal to start date.");
                return;
            }

            // Проверка на выбор типа отчета
            if (ReportTypeComboBox.SelectedIndex == -1)
            {
                MessageBox.Show("Please select a report type.");
                return;
            }

            string dateTimeNowString = DateTime.Now.ToString("yyyyMMddHHmmss");
            string outputPath = "";

            // Выбор типа отчета
            switch (ReportTypeComboBox.SelectedIndex)
            {
                case 0: // Sales Report
                    outputPath = $"D:\\SalesReport_{dateTimeNowString}.xlsx";
                    GenerateSalesReport(startDate, endDate, outputPath);
                    break;
                case 1: // Cash Report
                    outputPath = $"D:\\CashReport_{dateTimeNowString}.xlsx";
                    GenerateCashReport(startDate, endDate, outputPath);
                    break;
                case 2: // Cancellation Report
                    outputPath = $"D:\\CancellationReport_{dateTimeNowString}.xlsx";
                    GenerateCancellationReport(outputPath);
                    break;
                default:
                    // Так как мы уже проверили выбор типа отчета выше, этот блок не должен выполняться
                    return;
            }

            MessageBox.Show($"Report generated successfully at {outputPath}");
        }


        // Функция формирования отчета о продажах за выбранный период времени
        public void GenerateSalesReport(DateTime startDate, DateTime endDate, string basePath)
        {
            string dateTimeNowString = DateTime.Now.ToString("yyyyMMddHHmmss");
            string outputPath = $"{basePath}_{dateTimeNowString}.xlsx"; // Формирование конечного пути с учетом текущего времени

            var sales = _orders.Where(order => order.Date >= startDate && order.Date <= endDate && order.IsEnd == true)
                               .GroupBy(order => order.Date.Date)
                               .Select(group => new
                               {
                                   Date = group.Key,
                                   TotalSales = group.Sum(order => order.Result),
                                   TotalItemsSold = group.Sum(order => order.Count),
                                   CardPayments = group.Sum(order => order.Result - (order.Dishes != null ? order.Dishes.Sum(dish => dish.Dish.Price * dish.DishCount) : 0))
                               })
                               .ToList();

            // Создание отчета в формате Excel
            var excelApp = new Application();
            var workbook = excelApp.Workbooks.Add();
            var worksheet = workbook.Worksheets[1];

            worksheet.Cells[1, 1] = "Date";
            worksheet.Cells[1, 2] = "Total Sales";
            worksheet.Cells[1, 3] = "Total Items Sold";
            worksheet.Cells[1, 4] = "Card Payments";

            int row = 2;
            foreach (var sale in sales)
            {
                worksheet.Cells[row, 1] = sale.Date.ToShortDateString();
                worksheet.Cells[row, 2] = sale.TotalSales;
                worksheet.Cells[row, 3] = sale.TotalItemsSold;
                worksheet.Cells[row, 4] = sale.CardPayments;
                row++;
            }

            workbook.SaveAs(outputPath, XlFileFormat.xlWorkbookDefault);
            workbook.Close();
            excelApp.Quit();
        }


        // Функция формирования отчета о кассовых операциях
        public void GenerateCashReport(DateTime startDate, DateTime endDate, string basePath)
        {
            string dateTimeNowString = DateTime.Now.ToString("yyyyMMddHHmmss");
            string outputPath = $"{basePath}_{dateTimeNowString}.xlsx"; // Формирование конечного пути с учетом текущего времени

            var cashTransactions = _orders.Where(order => order.Date >= startDate && order.Date <= endDate && order.IsEnd == true)
                                          .Select(order => new
                                          {
                                              Date = order.Date,
                                              Nalichny = order.Result - (order.Dishes != null ? order.Dishes.Sum(dish => dish.Dish.Price * dish.DishCount) : 0)
                                          })
                                          .ToList();

            // Создание отчета в формате Excel
            var excelApp = new Application();
            var workbook = excelApp.Workbooks.Add();
            var worksheet = workbook.Worksheets[1];

            worksheet.Cells[1, 1] = "Date";
            worksheet.Cells[1, 2] = "Cash Transactions";

            int row = 2;
            foreach (var transaction in cashTransactions)
            {
                worksheet.Cells[row, 1] = transaction.Date.ToShortDateString();
                worksheet.Cells[row, 2] = transaction.Nalichny;
                row++;
            }

            workbook.SaveAs(outputPath, XlFileFormat.xlWorkbookDefault);
            workbook.Close();
            excelApp.Quit();
        }

        public void GenerateCancellationReport(string basePath)
        {
            string dateTimeNowString = DateTime.Now.ToString("yyyyMMddHHmmss");
            string outputPath = $"{basePath}_{dateTimeNowString}.xlsx"; // Формирование конечного пути с учетом текущего времени

            // Получение данных об отмененных заказах
            var cancelledOrders = _orders.Where(order => order.IsCancel)
                                          .Join(_cancellationReports,
                                                order => order.Id,
                                                report => report.OrderId,
                                                (order, report) => new
                                                {
                                                    OrderId = order.Id,
                                                    Date = order.Date,
                                                    Reason = report.Reason
                                                })
                                          .ToList();

            // Создание отчета в формате Excel
            var excelApp = new Application();
            var workbook = excelApp.Workbooks.Add();
            var worksheet = workbook.Worksheets[1];

            worksheet.Cells[1, 1] = "Order ID";
            worksheet.Cells[1, 2] = "Date";
            worksheet.Cells[1, 3] = "Cancellation Reason";

            int row = 2;
            foreach (var cancelledOrder in cancelledOrders)
            {
                worksheet.Cells[row, 1] = cancelledOrder.OrderId;
                worksheet.Cells[row, 2] = cancelledOrder.Date.ToShortDateString();
                worksheet.Cells[row, 3] = cancelledOrder.Reason;
                row++;
            }

            workbook.SaveAs(outputPath, XlFileFormat.xlWorkbookDefault);
            workbook.Close();
            excelApp.Quit();
        }

    }
}
