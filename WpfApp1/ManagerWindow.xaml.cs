using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Navigation;
using System.Windows;
using Microsoft.Office.Interop.Excel;
using WpfApp1.Models;
using WpfApp1.Models.Database;
using Application = Microsoft.Office.Interop.Excel.Application;
using Window = System.Windows.Window;

namespace WpfApp1;

public partial class ManagerWindow : Window
{
    private readonly List<CancellationReport> _cancellationReports;
    private readonly DatabaseContext db = new();

    public ManagerWindow()
    {
        InitializeComponent();
        _cancellationReports = db.CancellationReports.ToList();
    }

    private List<Order> _orders => db.Orders.ToList();


    private void GenerateReport_Click(object sender, RoutedEventArgs e)
    {
        // Проверка на выбор дат
        if (!StartDatePicker.SelectedDate.HasValue || !EndDatePicker.SelectedDate.HasValue)
        {
            MessageBox.Show("Пожалуйста, выберите начальную дату и конечную.");
            return;
        }

        var startDate = StartDatePicker.SelectedDate.Value;
        var endDate = EndDatePicker.SelectedDate.Value;

        // Проверка на корректность диапазона дат
        if (endDate < startDate)
        {
            MessageBox.Show("Конечная дата не может быть меньше начальной");
            return;
        }

        // Проверка на выбор типа отчета
        if (ReportTypeComboBox.SelectedIndex == -1)
        {
            MessageBox.Show("Пожалуйста, выберите тип отчета");
            return;
        }

        var dateTimeNowString = DateTime.Now.ToString("yyyyMMddHHmmss");
        var outputPath = "";

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

        MessageBox.Show($"Отчет сохраен по пути: {outputPath}");
    }


    // Функция формирования отчета о продажах за выбранный период времени
    public void GenerateSalesReport(DateTime startDate, DateTime endDate, string basePath)
    {
        var dateTimeNowString = DateTime.Now.ToString("yyyyMMddHHmmss");
        var outputPath =
            $"{basePath}_{dateTimeNowString}.xlsx"; // Формирование конечного пути с учетом текущего времени

        var sales = _orders.Where(order => order.Date >= startDate && order.Date <= endDate && order.IsEnd)
            .GroupBy(order => order.Date.Date)
            .Select(group => new
            {
                Date = group.Key,
                TotalSales = group.Sum(order => order.Result),
                TotalItemsSold = group.Sum(order => order.Count),
                CardPayments = group.Sum(order =>
                    order.Result - (order.Dishes != null
                        ? order.Dishes.Sum(dish => dish.Dish.Price * dish.DishCount)
                        : 0))
            })
            .ToList();

        // Создание отчета в формате Excel
        var excelApp = new Application();
        var workbook = excelApp.Workbooks.Add();
        var worksheet = workbook.Worksheets[1];

        int currentRow = 1;

        // Добавляем заголовок документа
        worksheet.Cells[currentRow, 1] = "Отчет продаж";
        worksheet.Cells[currentRow, 1].Font.Size = 24;
        worksheet.Cells[currentRow, 1].Font.Bold = true;
        worksheet.Cells[currentRow, 1].HorizontalAlignment = XlHAlign.xlHAlignCenter;
        worksheet.Range["A1", "D1"].Merge();
        currentRow++;

        // Добавляем дату составления документа
        worksheet.Cells[currentRow, 1] = $"Дата составления: {DateTime.Now.ToString("dd.MM.yyyy")}";
        worksheet.Cells[currentRow, 1].Font.Size = 12;
        worksheet.Cells[currentRow, 1].HorizontalAlignment = XlHAlign.xlHAlignRight;
        worksheet.Range["A2", "D2"].Merge();
        currentRow += 2;

        // Добавляем заголовки столбцов
        worksheet.Cells[currentRow, 1] = "Дата";
        worksheet.Cells[currentRow, 2] = "Общий объем продаж";
        worksheet.Cells[currentRow, 3] = "Общее количество проданных товаров";
        worksheet.Cells[currentRow, 4] = "Платежи по картам";
        worksheet.Rows[currentRow].Font.Bold = true;
        currentRow++;

        // Заполнение данных
        foreach (var sale in sales)
        {
            worksheet.Cells[currentRow, 1] = sale.Date.ToShortDateString();
            worksheet.Cells[currentRow, 2] = sale.TotalSales;
            worksheet.Cells[currentRow, 3] = sale.TotalItemsSold;
            worksheet.Cells[currentRow, 4] = sale.CardPayments;
            currentRow++;
        }

        // Добавление диаграммы
        var charts = (ChartObjects)worksheet.ChartObjects(Type.Missing);
        var chartObject = charts.Add(100, 80, 300, 250);
        var chart = chartObject.Chart;
        var range = worksheet.Range["A4", $"D{sales.Count + 4}"]; // Выбор диапазона данных для диаграммы
        chart.SetSourceData(range);
        chart.ChartType = XlChartType.xlColumnClustered;

        workbook.SaveAs(outputPath, XlFileFormat.xlWorkbookDefault);
        workbook.Close();
        excelApp.Quit();
    }



    // Функция формирования отчета о кассовых операциях
    public void GenerateCashReport(DateTime startDate, DateTime endDate, string basePath)
    {
        var dateTimeNowString = DateTime.Now.ToString("yyyyMMddHHmmss");
        var outputPath = $"{basePath}_{dateTimeNowString}.xlsx"; // Формирование конечного пути с учетом текущего времени

        var cashTransactions = _orders.Where(order => order.Date >= startDate && order.Date <= endDate && order.IsEnd)
            .Select(order => new
            {
                order.Date,
                Nalichny = order.Result -
                           (order.Dishes != null ? order.Dishes.Sum(dish => dish.Dish.Price * dish.DishCount) : 0)
            })
            .ToList();

        // Создание отчета в формате Excel
        var excelApp = new Application();
        var workbook = excelApp.Workbooks.Add();
        var worksheet = workbook.Worksheets[1];

        int currentRow = 1;

        // Добавляем заголовок документа
        worksheet.Cells[currentRow, 1] = "Кассовый протокол";
        worksheet.Cells[currentRow, 1].Font.Size = 24;
        worksheet.Cells[currentRow, 1].Font.Bold = true;
        worksheet.Cells[currentRow, 1].HorizontalAlignment = XlHAlign.xlHAlignCenter;
        worksheet.Range["A1", "B1"].Merge();
        currentRow++;

        // Добавляем дату составления документа
        worksheet.Cells[currentRow, 1] = $"Дата составления: {DateTime.Now.ToString("dd.MM.yyyy")}";
        worksheet.Cells[currentRow, 1].Font.Size = 12;
        worksheet.Cells[currentRow, 1].HorizontalAlignment = XlHAlign.xlHAlignRight;
        worksheet.Range["A2", "B2"].Merge();
        currentRow += 2;

        // Добавляем заголовки столбцов
        worksheet.Cells[currentRow, 1] = "Дата";
        worksheet.Cells[currentRow, 2] = "Операции с наличными деньгами";
        worksheet.Rows[currentRow].Font.Bold = true;
        currentRow++;

        // Заполнение данных
        foreach (var transaction in cashTransactions)
        {
            worksheet.Cells[currentRow, 1] = transaction.Date.ToShortDateString();
            worksheet.Cells[currentRow, 2] = transaction.Nalichny;
            currentRow++;
        }

        // Автоматическое изменение размера столбцов по содержимому
        worksheet.Columns.AutoFit();

        // Автоматическое изменение размера строк по содержимому
        worksheet.Rows.AutoFit();

        workbook.SaveAs(outputPath, XlFileFormat.xlWorkbookDefault);
        workbook.Close();
        excelApp.Quit();
    }



    public void GenerateCancellationReport(string basePath)
    {
        var dateTimeNowString = DateTime.Now.ToString("yyyyMMddHHmmss");
        var outputPath = $"{basePath}_{dateTimeNowString}.xlsx"; // Формирование конечного пути с учетом текущего времени

        // Получение данных об отмененных заказах
        var cancelledOrders = _orders.Where(order => order.IsCancel)
            .Join(_cancellationReports,
                order => order.Id,
                report => report.OrderId,
                (order, report) => new
                {
                    OrderId = order.Id,
                    order.Date,
                    report.Reason
                })
            .ToList();

        // Создание отчета в формате Excel
        var excelApp = new Application();
        var workbook = excelApp.Workbooks.Add();
        var worksheet = workbook.Worksheets[1];

        int currentRow = 1;

        // Добавляем заголовок документа
        worksheet.Cells[currentRow, 1] = "Отчет об отменах";
        worksheet.Cells[currentRow, 1].Font.Size = 24;
        worksheet.Cells[currentRow, 1].Font.Bold = true;
        worksheet.Cells[currentRow, 1].HorizontalAlignment = XlHAlign.xlHAlignCenter;
        worksheet.Range["A1", "C1"].Merge();
        currentRow++;

        // Добавляем дату составления документа
        worksheet.Cells[currentRow, 1] = $"Дата составления: {DateTime.Now.ToString("dd.MM.yyyy")}";
        worksheet.Cells[currentRow, 1].Font.Size = 12;
        worksheet.Cells[currentRow, 1].HorizontalAlignment = XlHAlign.xlHAlignRight;
        worksheet.Range["A2", "C2"].Merge();
        currentRow += 2;

        // Добавляем заголовки столбцов
        worksheet.Cells[currentRow, 1] = "Номер заказа";
        worksheet.Cells[currentRow, 2] = "Дата";
        worksheet.Cells[currentRow, 3] = "Причина отмены";
        worksheet.Rows[currentRow].Font.Bold = true;
        currentRow++;

        // Заполнение данных
        foreach (var cancelledOrder in cancelledOrders)
        {
            worksheet.Cells[currentRow, 1] = cancelledOrder.OrderId;
            worksheet.Cells[currentRow, 2] = cancelledOrder.Date.ToShortDateString();
            worksheet.Cells[currentRow, 3] = cancelledOrder.Reason;
            currentRow++;
        }

        workbook.SaveAs(outputPath, XlFileFormat.xlWorkbookDefault);
        workbook.Close();
        excelApp.Quit();
    }

}