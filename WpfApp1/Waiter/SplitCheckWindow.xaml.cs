using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using WpfApp1.Models;
using WpfApp1.Models.Database;

namespace WpfApp1.Waiter
{
    public partial class SplitCheckWindow : Window
    {
        private List<DishInOrder> orderItems;
        private List<DishInOrder> firstPartItems = new List<DishInOrder>();
        private List<DishInOrder> secondPartItems = new List<DishInOrder>();
        public List<DishInOrder> FirstPartItems { get; private set; }
        public List<DishInOrder> SecondPartItems { get; private set; }

        public SplitCheckWindow(List<DishInOrder> items)
        {
            InitializeComponent();
            orderItems = items;

            // Отобразить список блюд заказа
            foreach (var item in orderItems)
            {
                listBox.Items.Add(item.Dish.Name);
            }
        }

        private void AddToFirstPart_Click(object sender, RoutedEventArgs e)
        {
            // Проверяем, выбрано ли блюдо в списке
            if (listBox.SelectedItem != null)
            {
                // Получаем выбранное блюдо из списка
                string selectedDishName = listBox.SelectedItem.ToString();
                DishInOrder selectedDish = orderItems.FirstOrDefault(x => x.Dish.Name == selectedDishName);

                // Добавляем блюдо в первый чек
                firstPartItems.Add(selectedDish);
                firstPartListBox.Items.Add(selectedDishName);

                // Удаляем блюдо из общего списка, чтобы избежать дублирования
                orderItems.Remove(selectedDish);
                listBox.Items.Remove(selectedDishName);
            }
        }

        private void AddToSecondPart_Click(object sender, RoutedEventArgs e)
        {
            // Проверяем, выбрано ли блюдо в списке
            if (listBox.SelectedItem != null)
            {
                // Получаем выбранное блюдо из списка
                string selectedDishName = listBox.SelectedItem.ToString();
                DishInOrder selectedDish = orderItems.FirstOrDefault(x => x.Dish.Name == selectedDishName);

                // Добавляем блюдо во второй чек
                secondPartItems.Add(selectedDish);
                secondPartListBox.Items.Add(selectedDishName);

                // Удаляем блюдо из общего списка, чтобы избежать дублирования
                orderItems.Remove(selectedDish);
                listBox.Items.Remove(selectedDishName);
            }
        }

        private void Confirm_Click(object sender, RoutedEventArgs e)
        {
            if (firstPartListBox.Items != null)
            {
                foreach (var item in firstPartListBox.Items)
                {
                    string dishName = item.ToString(); // Получаем название блюда из ListBox
                    DishInOrder selectedDish = orderItems.FirstOrDefault(x => x.Dish.Name == dishName); // Находим соответствующее блюдо в списке заказа
                    if (selectedDish != null)
                    {
                        firstPartItems.Add(selectedDish); // Добавляем блюдо в первый чек
                    }
                }
            }

            if (secondPartListBox.Items != null)
            {
                foreach (var item in secondPartListBox.Items)
                {
                    string dishName = item.ToString(); // Получаем название блюда из ListBox
                    DishInOrder selectedDish = orderItems.FirstOrDefault(x => x.Dish.Name == dishName); // Находим соответствующее блюдо в списке заказа
                    if (selectedDish != null)
                    {
                        secondPartItems.Add(selectedDish); // Добавляем блюдо во второй чек
                    }
                }
            }

            this.Close();
        }



    }
}
