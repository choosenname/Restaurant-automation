using Microsoft.EntityFrameworkCore;
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
        DatabaseContext db = new DatabaseContext();
        private int discount = 0;
        Order Order { get; set; }
        decimal all = 0;
        public EndOrder(Order order)
        {
            Order = order;
            Window window = new Window { Height = 150, Width = 100, WindowStartupLocation = WindowStartupLocation.CenterScreen };
            TextBox textBox1 = new TextBox();
            Button submitButton = new Button();

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
            example += $"--------------------------------\nЦена без скидки:             {result}\nСкидка:             {discount}%\nСкидка в денежном эквиваленте:          {discontResult}\nИтог со скидкой:             {allResult}\n\nСпасибо!\n================================";
            textBox.Text = example ;
            db.SaveChanges();
        }

        private void End_Click(object sender, RoutedEventArgs e)
        {
            db.Orders.First(x => x.Id == Order.Id).IsEnd = true;

            if (radio1.IsChecked == true)
                db.Kassa.First(x => x.Id == 1).Nalichny += all;
            else
                db.Kassa.First(x => x.Id == 1).Card += all;
            MessageBox.Show("Чек успешно оплачен");
            db.SaveChanges();
            this.Close();

        }

        private void Print_Click(object sender, RoutedEventArgs e)
        {
            Microsoft.Office.Interop.Word.Application wordApp = new Microsoft.Office.Interop.Word.Application();
            wordApp.Visible = true; 
            Document doc = wordApp.Documents.Add();
            doc.Content.Text = textBox.Text;

            doc.SaveAs2("order.docx");

            //wordApp.Quit();
            MessageBox.Show("Чек сохранен");
        }
    }
}
