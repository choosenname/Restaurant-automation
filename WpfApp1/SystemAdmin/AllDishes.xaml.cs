using Microsoft.EntityFrameworkCore;
using Microsoft.Office.Interop.Word;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
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
using Paragraph = Microsoft.Office.Interop.Word.Paragraph;
using Table = Microsoft.Office.Interop.Word.Table;
using Window = System.Windows.Window;

namespace WpfApp1.SystemAdmin
{
    /// <summary>
    /// Логика взаимодействия для AllDishes.xaml
    /// </summary>
    public partial class AllDishes : Window
    {
        DatabaseContext db = new DatabaseContext();
        ICollectionView collectionView; 

        public AllDishes()
        {
            InitializeComponent();

            searchBox.TextChanged += SearchBox_TextChanged;

            collectionView = CollectionViewSource.GetDefaultView(db.Dishes.Include(x => x.Category).ToList());
            dataGrid.ItemsSource = collectionView;
        }

        private void Add_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                db.SaveChanges();

                MessageBox.Show("Данные обновлены");
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void SearchBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            string searchText = searchBox.Text.Trim().ToLower();

            if (collectionView != null)
            {
                if (string.IsNullOrEmpty(searchText))
                {
                    collectionView.Filter = null; 
                }
                else
                {
                    collectionView.Filter = item =>
                    {
                        var dish = item as Dish;
                        return dish.Name.ToLower().Contains(searchText);
                    };
                }
            }
        }



        private void searchBox_GotFocus(object sender, RoutedEventArgs e)
        {
            if (searchBox.Text == "Поиск по названию блюда")
            {
                searchBox.Text = "";
                searchBox.Foreground = Brushes.Black;
            }
        }

        private void searchBox_LostFocus(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrEmpty(searchBox.Text))
            {
                searchBox.Text = "Поиск по названию блюда";
                searchBox.Foreground = Brushes.Gray;
            }
        }
    }
}
