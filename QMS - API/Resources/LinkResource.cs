using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace QMS_API.Resources
{
    public class LinkResource
    { 
        public int Id { get; set; }
        public Guid Code { get; set; }
        public string Name { get; set; }
        public string Url { get; set; }
        public int TimeLimit { get; set; }
        public string Password { get; set; }
        public string User { get; set; }
        public int TestId { get; set; }
        public TestResource Test { get; set; }
        public List<QuizAttemptResource> QuizAttempts { get; set; }
    }
}
