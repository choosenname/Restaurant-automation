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
using WpfApp1.Waiter;

namespace WpfApp1
{
    /// <summary>
    /// Логика взаимодействия для WaiterWindow.xaml
    /// </summary>
    public partial class WaiterWindow : Window
    {
        DatabaseContext db = new DatabaseContext();
        public WaiterWindow()
        {
            InitializeComponent();

            UiAllOrder();
        }

        private void UiAllOrder()
        {
            orderBoard.Children.Clear();
            using (var db = new DatabaseContext())
            {
                var orders = db.Orders.Where(o => o.is_cancel == false && o.IsEnd == false).ToList();
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
            Border customBorder = new Border
            {
                Width = 100,
                Height = 141,
                BorderBrush = Brushes.Black,
                BorderThickness = new Thickness(1),
                Margin = new Thickness(5)
            };
            Label priceLabel = new Label
            {
                Content = $"Цена: {order.Result} руб.",
                Width = 100,
                Height = double.NaN
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
                Content = "Закрыть",
                Background = new SolidColorBrush(Color.FromRgb(44, 62, 80)),
                Foreground = Brushes.White
            };

            btnCard.Click += (sender, e) =>
            {
                EndOrder endOrder = new EndOrder(order);
                endOrder.ShowDialog();
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


        private void Login_Click(object sender, RoutedEventArgs e)
        {
            AddOrder addOrder = new AddOrder();
            addOrder.ShowDialog();
            UiAllOrder();
        }
    }
}
