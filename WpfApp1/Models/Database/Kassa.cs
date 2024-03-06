using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WpfApp1.Models.Database
{
    public class Kassa
    {
        [Key]
        public int Id { get; set; }
        public decimal Nalichny { get; set; }
        public decimal Card { get; set; }
        public decimal Return { get; set; }
    }
}
