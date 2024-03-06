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

namespace WpfApp1.SystemAdmin
{
    /// <summary>
    /// Логика взаимодействия для AddCategory.xaml
    /// </summary>
    public partial class AddCategory : Window
    {
        DatabaseContext db = new DatabaseContext();
        public AddCategory()
        {
            InitializeComponent();
        }

        private void Add_Click(object sender, RoutedEventArgs e)
        {
            db.DishCategories.Add(new Models.Database.DishCategory { Name = txtName.Text });
            db.SaveChanges();
            MessageBox.Show("Категория успешно добавлена");
            this.Close();
        }
    }
}
