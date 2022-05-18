using DXAppWPF.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DXCoalStorage.Data
{
    public class StockTable
    {
        [DisplayName("Номер склада")]
        public int WarehouseNumber { get; set; }
        [DisplayName("Номер пикета")]
        public int PicketNumber { get; set; }
        [DisplayName("Номер площади")]
        public string SiteNumber { get; set; }
        [DisplayName("Груз на площадке")]
        public int Cargo { get; set; }

    }
}
