using Microsoft.EntityFrameworkCore;
using Microsoft.Office.Interop.Word;
using System;
using System.Collections.Generic;
using System.ComponentModel;
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
using Window = System.Windows.Window;

namespace WpfApp1.Waiter
{
    /// <summary>
    /// Логика взаимодействия для EndOrder.xaml
    /// </summary>
    public partial class EndOrder : Window
    {
        private string _selectedOption;
        private List<DishInOrder> orderItems;
        DatabaseContext db = new DatabaseContext();
        private int discount = 0;
        Order Order { get; set; }
        decimal all = 0;

        public string SelectedOption
        {
            get { return _selectedOption; }
            set
            {
                _selectedOption = value;
                OnPropertyChanged("SelectedOption");
                GetCheck(Order);
            }
        }

        public EndOrder(Order order)
        {
            _selectedOption = "Наличные";
            Order = order;
            Window window = new Window { Height = 150, Width = 100, WindowStartupLocation = WindowStartupLocation.CenterScreen };
            TextBox textBox1 = new TextBox();
            Button submitButton = new Button();
            orderItems = db.DishInOrders.Include(x => x.Dish).Include(x => x.Order).Where(x => x.Order.Id == order.Id).ToList();

            textBox1.Margin = new Thickness(10);
            textBox1.Text = "0";
            submitButton.Content = "Далее";
            submitButton.Margin = new Thickness(10);
            submitButton.Click += (sender, e) =>
            {
                discount = Convert.ToInt32(textBox1.Text);
                window.Close();
                InitializeComponent();
                GetCheck(order);
                SplitBtn.Visibility = order.Count > 1 ? Visibility.Visible : Visibility.Collapsed;
            };

            StackPanel stackPanel = new StackPanel();
            stackPanel.Orientation = Orientation.Vertical;
            stackPanel.Children.Add(new Label { Content = "Размер скидки:" });
            stackPanel.Children.Add(textBox1);
            stackPanel.Children.Add(submitButton);

            window.Content = stackPanel;
            window.ShowDialog();
        }

        private void GetCheck(Order order)
        {
            string example = $"================================\nРесторан\n\nДата и время: {order.Date.ToString("dd.MM.yyyy HH:mm")} PM\n--------------------------------\nБлюда:\n";
            int count = 0;
            decimal result = 0;
            foreach (DishInOrder item in db.DishInOrders.Include(x => x.Dish).Include(x => x.Order).Where(x => x.Order.Id == order.Id).ToList())
            {
                count++;
                example += $"{count}. {item.Dish.Name}         {item.Dish.Price} * {item.DishCount}\n";
                result += item.Dish.Price * item.DishCount;
            }
            decimal discontResult = result * Convert.ToDecimal(discount / 100.0);
            decimal allResult = result - discontResult;
            all = allResult;
            order.Result = all;
            example += $"--------------------------------\nЦена без скидки:             {result}\nСкидка:             {discount}%\nСкидка в денежном эквиваленте:          {discontResult}\nИтог со скидкой:             {allResult}\nМетод оплаты:                 {SelectedOption}\nСпасибо!\n================================";
            textBox.Text = example;
            db.SaveChanges();
        }


        private void End_Click(object sender, RoutedEventArgs e)
        {
            Order orderToRemove = db.Orders.FirstOrDefault(x => x.Id == Order.Id);
            if (orderToRemove != null)
            {
                orderToRemove.IsEnd = true;
            }

            // Обновляем данные в кассе в зависимости от выбранного метода оплаты
            if (radio1.IsChecked == true)
            {
                db.Kassa.First(x => x.Id == 1).Nalichny += all;
            }
            else
            {
                db.Kassa.First(x => x.Id == 1).Card += all;
            }

            // Сохраняем изменения в базе данных
            db.SaveChanges();

            // Выводим сообщение о том, что чек успешно оплачен
            MessageBox.Show("Чек успешно оплачен");

            // Печатаем чек
            PrintCheck();

            // Закрываем текущее окно
            this.Close();
        }

        private void Login_Click(object sender, RoutedEventArgs e)
        {
            AddOrder addOrder = new AddOrder(Order);
            var dialogResult = addOrder.ShowDialog();

            if (dialogResult ?? false)
            {
                Order orderToSplit = db.Orders.FirstOrDefault(x => x.Id == Order.Id);
                if (orderToSplit != null)
                {
                    orderToSplit.IsSplited = true;
                }

                // Сохраняем изменения в базе данных
                db.SaveChanges();
            }

            this.Close();
        }

        private void SplitCheck_Click(object sender, RoutedEventArgs e)
        {
            SplitCheckWindow splitCheckWindow = new SplitCheckWindow(Order, orderItems);
            splitCheckWindow.ShowDialog();

            // Получаем выбранные блюда для каждого чека из словаря
            Dictionary<ListBox, List<DishInOrder>> listBoxItemsMap = splitCheckWindow.ListBoxItemsMap;

            // Проходим по словарю и создаем заказы на основе выбранных блюд для каждого ListBox
            foreach (var listBox in listBoxItemsMap.Keys)
            {
                if (listBoxItemsMap[listBox].Count == 0)
                    continue;
                // Создаем новый заказ
                Order newOrder = new Order
                {
                    Date = Order.Date,
                    NumberSeat = Order.NumberSeat,
                    Count = Order.Count,
                    IsEnd = Order.IsEnd,
                    IsCancel = Order.IsCancel,
                    Dishes = new List<DishInOrder>()
                };

                // Получаем выбранные блюда для текущего ListBox
                List<DishInOrder> selectedItems = listBoxItemsMap[listBox];

                // Добавляем выбранные блюда в новый заказ
                decimal result = 0;
                foreach (var dish in selectedItems)
                {
                    newOrder.Dishes.Add(new DishInOrder { Dish = dish.Dish, DishCount = dish.DishCount });
                    result += dish.Dish.Price * dish.DishCount;
                }

                newOrder.Result = result;

                // Сохраняем новый заказ в базе данных
                db.Orders.Add(newOrder);
            }

            // Отмечаем исходный заказ как разделенный
            Order orderToSplit = db.Orders.FirstOrDefault(x => x.Id == Order.Id);
            if (orderToSplit != null)
            {
                orderToSplit.IsSplited = true;
            }

            // Сохраняем изменения в базе данных
            db.SaveChanges();

            // Закрываем текущее окно
            this.Close();
        }

        private void Print_Click(object sender, RoutedEventArgs e)
        {
            PrintCheck();

            //wordApp.Quit();
            MessageBox.Show("Чек сохранен");
        }

        public void PrintCheck()
        {
            string dateTimeNowString = DateTime.Now.ToString("yyyyMMddHHmmss");
            Microsoft.Office.Interop.Word.Application wordApp = new Microsoft.Office.Interop.Word.Application();
            wordApp.Visible = true;
            Document doc = wordApp.Documents.Add();
            doc.Content.Text = textBox.Text;

            doc.SaveAs2($"order_{dateTimeNowString}.docx");
        }

        private void RadioButton_Checked(object sender, RoutedEventArgs e)
        {
            if (sender is RadioButton radioButton)
            {
                SelectedOption = radioButton.Content.ToString();
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}