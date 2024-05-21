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
    /// Логика взаимодействия для DeleteDish.xaml
    /// </summary>
    public partial class DeleteDish : Window
    {
        DatabaseContext db = new DatabaseContext();
        public DeleteDish()
        {
            InitializeComponent();
            DataContext = new DeleteViewModel();
        }

        private void Delete_Click(object sender, RoutedEventArgs e)
        {
            var viewModel = DataContext as DeleteViewModel;

            if (viewModel != null)
            {
                List<string> ids = viewModel.DishDeletes
                    .Where(x => x.IsSelected)
                    .Select(x => x.Id)
                    .ToList();

                if (ids.Count == 0)
                {
                    MessageBox.Show("Выберите хотя бы одно блюдо для удаления.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }

                foreach (string id in ids)
                {
                    Dish dish = db.Dishes.First(x => x.Id == Convert.ToInt32(id));
                    db.Dishes.Remove(dish);
                }

                db.SaveChanges();
                MessageBox.Show("Выбранные блюда удалены");
                DataContext = new DeleteViewModel();
            }
        }

    }
}
