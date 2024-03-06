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

namespace WpfApp1.Waiter
{
    /// <summary>
    /// Логика взаимодействия для AddOrder.xaml
    /// </summary>
    public partial class AddOrder : Window
    {
        string numberSeat = "";
        int count = 0;
        DatabaseContext db = new DatabaseContext();
        List<OrderDishModel> orderDishes = new List<OrderDishModel>();
        public AddOrder()
        {
            Window window = new Window { Height = 200, Width = 200, WindowStartupLocation = WindowStartupLocation.CenterScreen };
            TextBox textBox1 = new TextBox();
            TextBox textBox2 = new TextBox();
            Button submitButton = new Button();

            textBox1.Margin = new Thickness(10);
            textBox2.Margin = new Thickness(10);
            submitButton.Content = "Далее";
            submitButton.Margin = new Thickness(10);
            submitButton.Click += (sender, e) =>
            {
                if (!int.TryParse(textBox1.Text, out int tableNumber) || tableNumber < 0)
                {
                    MessageBox.Show("Пожалуйста, введите корректный номер стола (положительное целое число).", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }

                if (!int.TryParse(textBox2.Text, out int guestsCount) || guestsCount <= 0)
                {
                    MessageBox.Show("Пожалуйста, введите корректное количество гостей (положительное целое число).", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }

                numberSeat = tableNumber.ToString();
                count = guestsCount;

                window.Close();
                InitializeComponent();  
                categoryComboBox.ItemsSource = db.DishCategories.ToList();
                categoryComboBox.SelectedIndex = 0;
            };

            StackPanel stackPanel = new StackPanel();
            stackPanel.Orientation = Orientation.Vertical;
            stackPanel.Children.Add(new Label { Content = "Номер стола:" });
            stackPanel.Children.Add(textBox1);
            stackPanel.Children.Add(new Label { Content = "Количество гостей:" });
            stackPanel.Children.Add(textBox2);
            stackPanel.Children.Add(submitButton);

            window.Content = stackPanel;
            window.ShowDialog();
        }


        private void GetCategory(DishCategory category)
        {
            foreach (Dish dish in db.Dishes.Include(x => x.Category).Where(x => x.Category.Id == category.Id))
            {
                UIDishes(dish);
            }

        }

        private void UIDishes(Dish dish)
        {
            Border border = new Border
            {
                Width = 100,
                Height = 75,
                BorderBrush = Brushes.Black,
                BorderThickness = new Thickness(1),
                Margin = new Thickness(5),
                Background = Brushes.White
            };

            StackPanel stackPanel = new StackPanel();

            Label label1 = new Label
            {
                Content = dish.Name,
                Width = 100,
                Height = double.NaN  
            };

            Label label2 = new Label
            {
                Content = $"Цена: {dish.Price}",
                Width = 100,
                Height = double.NaN 
            };

            Button addButton = new Button
            {
                Content = "Добавить",
                Background = new SolidColorBrush(Color.FromRgb(44, 62, 80)),
                Foreground = Brushes.White,
                VerticalAlignment = VerticalAlignment.Bottom
            };
            addButton.Click += (sender, e) =>
            {
                OrderDishModel model = orderDishes.FirstOrDefault(x => x.Dish.Id == dish.Id);

                if(model == null)
                {
                    model = new OrderDishModel();
                    model.Dish = dish;
                    model.Count = 1;
                    orderDishes.Add(model);
                }
                else
                {
                    model.Count++;
                }

                UIAllBoard();
            };

            stackPanel.Children.Add(label1);
            stackPanel.Children.Add(label2);
            stackPanel.Children.Add(addButton);

            border.Child = stackPanel;
            dishes.Children.Add(border);
        }

        private void UIAllBoard()
        {
            orderDishesBoard.Children.Clear();
            foreach (OrderDishModel item in orderDishes)
            {
                UIOrderDish(item);
            }
        }

        private void UIOrderDish(OrderDishModel model)
        {
            Border customBorder = new Border
            {
                Width = 100,
                Height = 120,
                BorderBrush = Brushes.Black,
                BorderThickness = new Thickness(1),
                Margin = new Thickness(5)
            };

            StackPanel stackPanel = new StackPanel();

            Label label1 = new Label
            {
                Content = model.Dish.Name,
                Width = 100,
                Height = double.NaN
            };

            Label label2 = new Label
            {
                Content = $"Цена: {model.Dish.Price}",
                Width = 100,
                Height = double.NaN
            };

            Label label3 = new Label
            {
                Content = "Количество",
                HorizontalAlignment = HorizontalAlignment.Center
            };

            StackPanel innerStackPanel = new StackPanel
            {
                Orientation = Orientation.Horizontal
            };

            TextBox quantityTextBox = new TextBox
            {
                Text = model.Count.ToString(),
                Width = 50,
                Margin = new Thickness(5, 0, 5, 0)
            };

            Button minusButton = new Button
            {
                Content = "-",
                Width = 20,
                Height = 20
            };

            minusButton.Click += (sender, e) =>
            {
                model.Count--;
                quantityTextBox.Text = model.Count.ToString();
            };


            Button plusButton = new Button
            {
                Content = "+",
                Width = 20,
                Height = 20
            };

            plusButton.Click += (sender, e) =>
            {
                model.Count++;
                quantityTextBox.Text = model.Count.ToString();
            };

            innerStackPanel.Children.Add(minusButton);
            innerStackPanel.Children.Add(quantityTextBox);
            innerStackPanel.Children.Add(plusButton);

            Button deleteButton = new Button
            {
                Content = "Удалить",
                Background = new SolidColorBrush(Color.FromRgb(44, 62, 80)),
                Foreground = Brushes.White,
                VerticalAlignment = VerticalAlignment.Bottom
            };

            deleteButton.Click += (sender, e) =>
            {
                orderDishes.Remove(model);
                UIAllBoard();
            };

            stackPanel.Children.Add(label1);
            stackPanel.Children.Add(label2);
            stackPanel.Children.Add(label3);
            stackPanel.Children.Add(innerStackPanel);
            stackPanel.Children.Add(deleteButton);

            customBorder.Child = stackPanel;

            orderDishesBoard.Children.Add(customBorder);
        }

        private void categoryComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            dishes.Children.Clear();
            GetCategory((DishCategory)categoryComboBox.SelectedItem);
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            if (orderDishes.Count == 0)
            {
                MessageBox.Show("Нельзя оформить пустой заказ!");
                return;
            }

            Order order = new Order();
            order.Date = DateTime.Now;
            order.NumberSeat = numberSeat;
            order.Count = count;

            db.Orders.Add(order);

            foreach (OrderDishModel item in orderDishes)
            {
                db.DishInOrders.Add(new DishInOrder { Dish = item.Dish, DishCount = item.Count, Order = order });
            }

            db.SaveChanges();

            MessageBox.Show("Заказ успешно добавлен");

            this.Close();
        }

    }

    internal class OrderDishModel
    {
        public Dish Dish { get; set; }
        public int Count { get; set; }
    }

}
