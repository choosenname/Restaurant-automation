using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WpfApp1.Models;
using WpfApp1.Models.Database;

namespace WpfApp1.SystemAdmin
{
    class DeleteViewModel
    {
        DatabaseContext db = new DatabaseContext();

        public ObservableCollection<DeleteItem> EmplDeletes { get; set; }
        public ObservableCollection<DeleteItem> DishDeletes { get; set; }

        private void GetEmplDeletes()
        {
            ObservableCollection<DeleteItem> Deletes = new ObservableCollection<DeleteItem>();
            foreach (Employee employee in db.Employees.Include(x => x.EmployeeType))
            {
                bool isDeletable = employee.Id != "100000"; // Системный администратор не может быть удален
                Deletes.Add(new DeleteItem { Id = employee.Id, Info = $"{employee.Name} {employee.EmployeeType.Name}", IsDeletable = isDeletable });
            }
            EmplDeletes = Deletes;
        }

        private void GetDishDeletes() 
        {
            ObservableCollection<DeleteItem> Deletes = new ObservableCollection<DeleteItem>();
            foreach (Dish dish in db.Dishes.Include(x => x.Category))
            {
                Deletes.Add(new DeleteItem { Id = dish.Id.ToString(), Info = $"{dish.Name} {dish.Category.Name}" });
            }
            DishDeletes = Deletes;
        }

        public DeleteViewModel()
        {
            GetEmplDeletes();
            GetDishDeletes();
        }
    }
}
