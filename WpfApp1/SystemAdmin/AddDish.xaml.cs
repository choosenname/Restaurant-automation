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

namespace WpfApp1.SystemAdmin
{
    /// <summary>
    /// Логика взаимодействия для AddDish.xaml
    /// </summary>
    public partial class AddDish : Window
    {
        DatabaseContext db = new DatabaseContext();
        public AddDish()
        {
            InitializeComponent();
            typeComboBox.ItemsSource = db.DishCategories.ToList();
            typeComboBox.SelectedIndex = 0;
        }

        private void Add_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                string name = txtName.Text;
                string priceTxt = txtPrice.Text;
                DishCategory category = (DishCategory)typeComboBox.SelectedItem;

                // Проверка на ввод имени блюда
                if (string.IsNullOrWhiteSpace(name))
                {
                    MessageBox.Show("Введите название блюда.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }

                // Проверка на ввод цены
                if (string.IsNullOrEmpty(priceTxt))
                {
                    MessageBox.Show("Введите цену.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }

                // Проверка на валидность и отрицательность цены
                if (!decimal.TryParse(priceTxt, out decimal price) || price < 0)
                {
                    MessageBox.Show("Введите корректную положительную цену.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }

                // Создание нового блюда и добавление его в базу данных
                Dish dish = new Dish { Name = name, Price = price, Category = category };
                db.Dishes.Add(dish);
                db.SaveChanges();

                MessageBox.Show("Блюдо добавлено");

                // Очистка полей после успешного добавления
                txtName.Text = "";
                txtPrice.Text = "";
                typeComboBox.ItemsSource = db.DishCategories.ToList();
                typeComboBox.SelectedIndex = 0;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }


        private void Button_Click(object sender, RoutedEventArgs e)
        {
            AddCategory addCategory = new AddCategory();
            addCategory.ShowDialog();
            typeComboBox.ItemsSource = db.DishCategories.ToList();
            typeComboBox.SelectedIndex = 0;
        }
    }
}
