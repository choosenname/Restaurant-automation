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
using WpfApp1.Models;
using WpfApp1.Waiter;
using static System.Net.Mime.MediaTypeNames;
using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Wordprocessing;
using DocumentFormat.OpenXml;
using DocumentFormat.OpenXml.Office.CustomUI;
using ControlzEx.Standard;
using Microsoft.EntityFrameworkCore;
using WpfApp1.SystemAdmin;


namespace WpfApp1
{
    /// <summary>
    /// Логика взаимодействия для RestoranAdminWindow.xaml
    /// </summary>
    public partial class RestoranAdminWindow : Window
    {
        DatabaseContext db = new DatabaseContext();
        private Label seats;
       
        public RestoranAdminWindow()
        {
            InitializeComponent();
            UiAllOrder();
        }
        private void UiAllOrder()
        {

            orderBoard.Children.Clear();
            using (var db = new DatabaseContext())
            {
                var orders = db.Orders.Where(o => o.IsCancel == false && o.IsSplited == false).ToList();
                if (orders.Count == 0)
                {
                    Label noOrdersLabel = new Label
                    {
                        Content = "Нет ни одного заказа",
                        FontWeight = FontWeights.Bold,
                        HorizontalAlignment = HorizontalAlignment.Center,
                        VerticalAlignment = VerticalAlignment.Center,
                        FontSize = 16,
                        Foreground = Brushes.DarkGray
                    };
                    orderBoard.Children.Add(noOrdersLabel);
                }
                else
                {
                    foreach (Order item in orders)
                    {
                        UiOrder(item);
                    }
                }
            }
        }



        private void UiOrder(Order order)
        {
            System.Windows.Controls.Border customBorder = new System.Windows.Controls.Border
            {
                Width = 100,
                Height = 141,
                BorderBrush = Brushes.Black,
                BorderThickness = new Thickness(1),
                Margin = new Thickness(5)
            };
            var orderInDb = db.Orders.FirstOrDefault(o => o.Id == order.Id);
            decimal result = 0;
            foreach (DishInOrder item in db.DishInOrders.Include(x => x.Dish).Where(x => x.Order.Id == order.Id).ToList())
            {
                result += item.Dish.Price * item.DishCount;
            };
            orderInDb.Result = result;
            db.SaveChanges();

            StackPanel stackPanel = new StackPanel();

            Label seatLabel = new Label
            {

                Content = $"Столик: {order.NumberSeat}\nКол-во гостей: {order.Count}",
                Width = 100,
                Height = double.NaN 
            };
            Label priceLabel = new Label
            {
                Content = $"Цена: {result} бел.руб.",
                Width = 100,
                Height = double.NaN
            };
            seats = seatLabel;

            Label dateLabel = new Label
            {
                Content = "Дата: " + order.Date.ToString("dd.MM.yyyy"),
                Width = 100,
                Height = double.NaN
            };

            Label timeLabel = new Label 
            {
                Content = "Время: " + order.Date.ToString("HH:mm"),
                Width = 100,
                Height = double.NaN
            };

            System.Windows.Controls.Button btnCard = new System.Windows.Controls.Button
            {
                Content = "Отменить",
                Background = new SolidColorBrush(System.Windows.Media.Color.FromRgb(44, 62, 80)),
                Foreground = Brushes.White
            };
            string reportFilePath = System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "CancellationReport" + DateTime.Now.ToString("(yyyy.MM.dd.HH.mm.ss)") + ".docx");


            btnCard.Click += (sender, e) =>
            {
                Window window = new Window { Height = 300, Width = 400, WindowStartupLocation = WindowStartupLocation.CenterScreen };
                TextBox textBox1 = new TextBox();
                System.Windows.Controls.Button submitButton = new System.Windows.Controls.Button();

                textBox1.Margin = new Thickness(10);
                textBox1.Height = 160;
                submitButton.Content = "Создать отчет";
                submitButton.Margin = new Thickness(10);
                submitButton.Click += (sender, e) =>
                {
                    MessageBox.Show($"Отчет успешно создан и сохранен по следующему пути:\n{reportFilePath}", "Уведомление", MessageBoxButton.OK, MessageBoxImage.Information);

                    string reason = textBox1.Text;

                    CancellationReport report = new CancellationReport
                    {
                        Reason = reason,
                        OrderId = order.Id
                    };

                    DatabaseContext db = new DatabaseContext();
                    db.CancellationReports.Add(report);
                    db.SaveChanges();

                    var orderInDb = db.Orders.FirstOrDefault(o => o.Id == order.Id);
                    if (orderInDb != null)
                    {
                        orderInDb.IsCancel = true;
                        db.SaveChanges();
                    }

                    Kassa kassa = db.Kassa.First(x => x.Id == 1);
                    kassa.Return += order.Result;
                    db.SaveChanges();
                    UiAllOrder();
                    window.Close();
                    GenerateCancellationReport(reason, reportFilePath);
                };

                StackPanel stackPanel = new StackPanel();
                stackPanel.Orientation = Orientation.Vertical;
                stackPanel.Children.Add(new Label { Content = "Причина отмены:", FontSize = 20 });
                stackPanel.Children.Add(textBox1);
                stackPanel.Children.Add(submitButton);


                window.Content = stackPanel;
                window.ShowDialog();
                UiAllOrder();
            };

            stackPanel.Children.Add(seatLabel);
            stackPanel.Children.Add(dateLabel);
            stackPanel.Children.Add(timeLabel);
            stackPanel.Children.Add(priceLabel);
            stackPanel.Children.Add(btnCard);

            customBorder.Child = stackPanel;

            orderBoard.Children.Add(customBorder);


        }
        private void GenerateCancellationReport(string reason, string filePath)
        {
            using (WordprocessingDocument wordDocument = WordprocessingDocument.Create(filePath, WordprocessingDocumentType.Document))
            {
                MainDocumentPart mainPart = wordDocument.AddMainDocumentPart();
                mainPart.Document = new Document();
                Body body = mainPart.Document.AppendChild(new Body());

                DocumentFormat.OpenXml.Wordprocessing.Paragraph paragraph = body.AppendChild(new DocumentFormat.OpenXml.Wordprocessing.Paragraph());
                DocumentFormat.OpenXml.Wordprocessing.Run run = paragraph.AppendChild(new DocumentFormat.OpenXml.Wordprocessing.Run());
                run.AppendChild(new DocumentFormat.OpenXml.Wordprocessing.Text(seats.Content.ToString() ?? "" ));
                run.AppendChild(new DocumentFormat.OpenXml.Wordprocessing.Text($"\nПричина отмены: {reason}"));

                wordDocument.Save();
            }
        }
    }
}
