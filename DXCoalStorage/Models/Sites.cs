using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DXAppWPF.Models
{
    public class Sites
    {
        [Key]
        public int Id { get; set; }
        public string SiteNumber { get; set; }
        public int Cargo { get; set; }
    }
}
