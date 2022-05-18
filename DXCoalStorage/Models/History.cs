using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DXAppWPF.Models
{
    public class History
    {
        public int Id { get; set; }
        [DisplayName("Дата и время")]
        public DateTime DateTime { get; set; }
        [DisplayName("Площадь до")]
        public string Site_was { get; set; }
        [DisplayName("Площадь после")]
        public string Site_became { get; set; }
        [DisplayName("Груз до")]
        public int Сargo_was { get; set; }
        [DisplayName("Груз после")]
        public int Сargo_became { get; set; }

    }
}
