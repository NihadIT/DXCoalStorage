using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DXAppWPF.Models
{
    public class Warehouse
    {
        public int Id { get; set; }
        public int WarehouseNumber { get; set; }
        public int PicketNumber { get; set; }
        public int Site_Id { get; set; }    

    }
}
