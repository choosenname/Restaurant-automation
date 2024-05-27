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
        public Dictionary<ListBox, List<DishInOrder>> ListBoxItemsMap { get; set; } = new Dictionary<ListBox, List<DishInOrder>>();
        private List<StackPanel> stackPanels = new List<StackPanel>();

        public SplitCheckWindow(Order order, List<DishInOrder> items)
        {
            InitializeComponent();
            orderItems = new List<DishInOrder>(items); // Use a copy of the list to avoid side effects

            // Отобразить список блюд заказа
            foreach (var item in orderItems)
            {
                listBox.Items.Add(item.Dish.Name);
            }

            // Генерация ListBox и кнопок для каждого
            for (int i = 0; i < order.Count; i++)
            {
                // Создаем новый StackPanel
                StackPanel stackPanel = new StackPanel();
                stackPanel.Orientation = Orientation.Horizontal;
                stackPanel.Margin = new Thickness(10);

                // Создаем ListBox
                ListBox listBox = new ListBox();
                listBox.Width = 140;
                listBox.Height = 120; // Adjust height for visibility
                listBox.Margin = new Thickness(0);
                listBox.VerticalAlignment = VerticalAlignment.Stretch;
                listBox.HorizontalAlignment = HorizontalAlignment.Stretch;
                stackPanel.Children.Add(listBox);

                // Создаем кнопку
                Button button = new Button();
                button.Content = "+";
                button.HorizontalAlignment = HorizontalAlignment.Right;
                button.Margin = new Thickness(10);
                button.VerticalAlignment = VerticalAlignment.Top;
                button.Width = 30;
                button.Click += (sender, e) => Part_Click(sender, e, listBox);
                stackPanel.Children.Add(button);

                // Добавляем StackPanel в Grid
                stack.Children.Add(stackPanel);
                Grid.SetRow(stackPanel, i);

                // Добавляем ListBox и пустой список блюд в словарь
                ListBoxItemsMap[listBox] = new List<DishInOrder>();

                // Добавляем StackPanel в список для дальнейшей работы с ними
                stackPanels.Add(stackPanel);
            }
        }

        private void Part_Click(object sender, RoutedEventArgs e, ListBox targetListBox)
        {
            // Проверяем, выбрано ли блюдо в списке
            if (listBox.SelectedItem != null)
            {
                // Получаем выбранное блюдо из списка
                string selectedDishName = listBox.SelectedItem.ToString();
                DishInOrder selectedDish = orderItems.FirstOrDefault(x => x.Dish.Name == selectedDishName);

                if (selectedDish != null)
                {
                    // Получаем список блюд для текущего ListBox
                    List<DishInOrder> currentListBoxItems = ListBoxItemsMap[targetListBox];

                    // Добавляем блюдо в ListBox, если оно еще не добавлено
                    if (!currentListBoxItems.Contains(selectedDish))
                    {
                        targetListBox.Items.Add(selectedDishName);

                        // Добавляем блюдо в список для текущего ListBox
                        currentListBoxItems.Add(selectedDish);

                        // Удаляем блюдо из общего списка, чтобы избежать дублирования
                        orderItems.Remove(selectedDish);
                        listBox.Items.Remove(selectedDishName);
                    }
                    else
                    {
                        MessageBox.Show("Это блюдо уже добавлено в данный список.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                    }
                }
                else
                {
                    MessageBox.Show("Выбранное блюдо не найдено.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
            else
            {
                MessageBox.Show("Пожалуйста, выберите блюдо из списка.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }

        private void Confirm_Click(object sender, RoutedEventArgs e)
        {
            // Add any additional validation logic if necessary
            this.Close();
        }
    }
}
