using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SysProgProjekat_TreciDeo
{
    public class Business
    {
        public string id {  get; set; }
        public string name { get; set; }
        public bool is_closed { get; set; }
        public int review_count { get; set; }
        public Location location { get; set; }
    }
}
