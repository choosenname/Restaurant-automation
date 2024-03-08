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
        private readonly List<Order> _orders;
        private readonly List<CancellationReport> _cancellationReports;
        DatabaseContext db = new DatabaseContext();

        public ManagerWindow()
        {
            InitializeComponent();

            _orders = db.Orders.ToList();
            _cancellationReports = db.CancellationReports.ToList();
        }

        private void GenerateReport_Click(object sender, RoutedEventArgs e)
        {
            var startDate = StartDatePicker.SelectedDate ?? DateTime.MinValue;
            var endDate = EndDatePicker.SelectedDate ?? DateTime.MaxValue;


            // Выбор типа отчета
            switch (ReportTypeComboBox.SelectedIndex)
            {
                case 0: // Sales Report
                    GenerateSalesReport(startDate, endDate, "SalesReport.xlsx");
                    break;
                case 1: // Cash Report
                    GenerateCashReport(startDate, endDate, "CashReport.xlsx");
                    break;
                case 2: // Cancellation Report
                    GenerateCancellationReport("CancellationReport.xlsx");
                    break;
                default:
                    MessageBox.Show("Please select a report type.");
                    break;
            }

            MessageBox.Show("Report generated successfully.");
        }

        // Функция формирования отчета о продажах за выбранный период времени
        public void GenerateSalesReport(DateTime startDate, DateTime endDate, string outputPath)
        {
            var sales = _orders.Where(order => order.Date >= startDate && order.Date <= endDate && order.IsEnd)
                               .GroupBy(order => order.Date.Date)
                               .Select(group => new
                               {
                                   Date = group.Key,
                                   TotalSales = group.Sum(order => order.Result),
                                   TotalItemsSold = group.Sum(order => order.Count),
                                   CardPayments = group.Sum(order => order.Dishes.Sum(dish => dish.Dish.Price * dish.DishCount))
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
        public void GenerateCashReport(DateTime startDate, DateTime endDate, string outputPath)
        {
            var cashTransactions = _orders.Where(order => order.Date >= startDate && order.Date <= endDate && order.IsEnd)
                                          .Select(order => new
                                          {
                                              Date = order.Date,
                                              Nalichny = order.Result
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

        // Функция формирования отчета о кассовых операциях
        public void GenerateCancellationReport(string outputPath)
        {
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
