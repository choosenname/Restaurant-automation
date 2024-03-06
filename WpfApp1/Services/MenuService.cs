    using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WpfApp1.Models.Database;
using WpfApp1.Models;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace WpfApp1.Services
{
    internal class MenuService
    {
        private readonly DatabaseContext _context;

        public MenuService(DatabaseContext context)
        {
            _context = context;
        }

        public async Task<List<DishCategory>> GetMenuDataAsync()
        {
            var categoriesWithDishes = await _context.DishCategories
                                                     .Include(c => c.Dishes)
                                                     .ToListAsync();
            return categoriesWithDishes;
        }
    }
}
