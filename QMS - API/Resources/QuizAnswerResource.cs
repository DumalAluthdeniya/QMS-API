using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace QMS_API.Resources
{
    public class QuizAnswerResource
    {

        public int Id { get; set; }
        public int QuizAttemptId { get; set; }
        public int QuestionId { get; set; }
        public int TestId { get; set; }
        public string MatchingText { get; set; }
        public string Answer { get; set; }
    }
}
