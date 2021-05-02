using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace QMS_API.Utils
{
    public class Time
    {
        // TODO: don't forget to add validation
        public int Hours { get; set; }
        public int Minutes { get; set; }
        public int Seconds { get; set; }

        public override string ToString()
        {
            return $"{this.Hours:00}:{this.Minutes:00}:{this.Seconds:00}";
        }
    }
}
