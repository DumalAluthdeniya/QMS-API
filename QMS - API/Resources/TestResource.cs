using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using QMS_API.Models;

namespace QMS_API.Resources
{
    public class TestResource
    {

        public int Id { get; set; }
        public string Name { get; set; }
        public string Introduction { get; set; }
        public List<int> Questions { get; set; }
        public List<LinkResource> Links { get; set; }
        public IEnumerable<QuestionResource> QuestionsList { get; set; }
        public string User { get; set; }
    }
}
