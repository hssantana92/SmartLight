using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SQLite;

namespace SmartLight.Models
{
    // Database Table for SQLite created to store the TimeOn and TimeOff auto timer values.

    [Table("devices")]
    public class DeviceStorage
    {
        [PrimaryKey, AutoIncrement]
        public int Id { get; set; }

        [MaxLength(20), Unique]
        public string IpAddress { get; set; }


        public TimeSpan TimeOn { get; set; }

        public TimeSpan TimeOff { get; set; }
        
        public bool Power { get; set; }
    }
}
