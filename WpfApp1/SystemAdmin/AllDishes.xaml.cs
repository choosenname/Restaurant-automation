using Microsoft.EntityFrameworkCore;
using Microsoft.Office.Interop.Word;
using System;
using System.Collections.Generic;
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
        public AllDishes()
        {
            InitializeComponent();
            dataGrid.ItemsSource = db.Dishes.Include(x => x.Category).ToList();
        }

        private void Add_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                IEnumerable<Dish> allDishes = dataGrid.ItemsSource as IEnumerable<Dish>;

                if (allDishes == null) throw new Exception("Нет блюд");

                db.SaveChanges();

                MessageBox.Show("Данные обновлены");
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
            
        }
    }
}
