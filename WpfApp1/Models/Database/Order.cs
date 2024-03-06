using Microsoft.Identity.Client;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WpfApp1.Models.Database
{
    public class Order
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        public DateTime Date { get; set; }

        public string NumberSeat { get; set; }

        public int Count { get; set; }
        public decimal Result { get; set; }
        public bool IsEnd { get; set; } = false;

        public bool is_cancel { get; set; } = false;

        public List<DishInOrder> Dishes { get; set; }

    }

}
    