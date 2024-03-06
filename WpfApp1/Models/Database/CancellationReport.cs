using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WpfApp1.Models.Database
{
    public class CancellationReport
    {
        public int Id { get; set; }
        public string Reason { get; set; }
        public int OrderId { get; set; } 
    }
}
