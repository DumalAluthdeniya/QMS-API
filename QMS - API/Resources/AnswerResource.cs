using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace QMS_API.Resources
{
    public class AnswerResource
    {
        
        public int Id { get; set; }
        public string Name { get; set; }
        public bool IsCorrectAnswer { get; set; }
        public string MatchingText { get; set; }
        public string GivenMatchingText { get; set; }
        public string Duration { get; set; }

    }
}
