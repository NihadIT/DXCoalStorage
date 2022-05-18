using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DXAppWPF.Models
{
     public class StockContext : DbContext
    {
        public StockContext() : base("DBConnection") { }
       
        public DbSet<Warehouse> Warehouses { get; set; }
        public DbSet<Sites> Sites { get; set; }
        public DbSet<History> History { get; set; }

    }
}
