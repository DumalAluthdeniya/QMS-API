using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace QMS_API.Models
{
    public class TestQuestion
    {
        public int Id { get; set; }
        public Test Test { get; set; }
        public Question Question { get; set; }

    }
}
