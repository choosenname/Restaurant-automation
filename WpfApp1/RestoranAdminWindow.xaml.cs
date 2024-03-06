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

namespace WpfApp1
{
    /// <summary>
    /// Логика взаимодействия для RestoranAdminWindow.xaml
    /// </summary>
    public partial class RestoranAdminWindow : Window
    {
        public RestoranAdminWindow()
        {
            InitializeComponent();

            UiAllOrder();
        }

        private void UiAllOrder()
        {
            orderBoard.Children.Clear();
            DatabaseContext tmp = new DatabaseContext();
            foreach (Order item in tmp.Orders)
            {
                UiOrder(item);
            }
        }

        private void UiOrder(Order order)
        {
            Border customBorder = new Border
            {
                Width = 100,
                Height = 115,
                BorderBrush = Brushes.Black,
                BorderThickness = new Thickness(1),
                Margin = new Thickness(5)
            };

            StackPanel stackPanel = new StackPanel();

            Label seatLabel = new Label
            {

                Content = $"Столик: {order.NumberSeat}\nКол-во гостей: {order.Count}",
                Width = 100,
                Height = double.NaN
            };

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

            Button btnCard = new Button
            {
                Content = "Отменить",
                Background = new SolidColorBrush(Color.FromRgb(44, 62, 80)),
                Foreground = Brushes.White
            };

            btnCard.Click += (sender, e) =>
            {
                Window window = new Window { Height = 300, Width = 400, WindowStartupLocation = WindowStartupLocation.CenterScreen };
                TextBox textBox1 = new TextBox();
                Button submitButton = new Button();

                textBox1.Margin = new Thickness(10);
                textBox1.Height = 160;
                submitButton.Content = "Создать очет";
                submitButton.Margin = new Thickness(10);
                submitButton.Click += (sender, e) =>
                {
                    DatabaseContext db = new DatabaseContext();
                    Kassa kassa = db.Kassa.First(x => x.Id == 1);
                    kassa.Return += order.Result;
                    db.Orders.Remove(order);
                    db.SaveChanges();
                    UiAllOrder();
                    window.Close();

                };

                StackPanel stackPanel = new StackPanel();
                stackPanel.Orientation = Orientation.Vertical;
                stackPanel.Children.Add(new Label { Content = "Причина отмены:", FontSize = 20});
                stackPanel.Children.Add(textBox1);
                stackPanel.Children.Add(submitButton);


                window.Content = stackPanel;
                window.ShowDialog();
                UiAllOrder();
            };

            stackPanel.Children.Add(seatLabel);
            stackPanel.Children.Add(dateLabel);
            stackPanel.Children.Add(timeLabel);
            stackPanel.Children.Add(btnCard);

            customBorder.Child = stackPanel;

            orderBoard.Children.Add(customBorder);
        }
    }
}
