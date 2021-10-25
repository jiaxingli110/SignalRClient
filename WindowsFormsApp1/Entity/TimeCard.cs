using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WindowsFormsApp1.Entity
{
    public class TimeCard
    {
        public string ID { get; set; }
        public string UserName { get; set; }
        public string PrimeID { get; set; }
        public string LeaderPrimeID { get; set; }
        public DateTime StartTime { get; set; }
        public DateTime EndTime { get; set; }
    }
}
