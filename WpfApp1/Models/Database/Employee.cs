using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WpfApp1.Models.Database
{
    public class Employee
    {
        [Key]
        [MaxLength(6)]
        public string Id { get; set; }

        public string Name { get; set; }

        public int TypeId { get; set; }

        public EmployeeType EmployeeType { get; set; }
    }
}
