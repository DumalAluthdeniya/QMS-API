using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace QMS_API.Resources
{
    public class QuizSubmitResource
    {
        public DateTime FinishTime { get; set; }
        public int QuizAttemptId { get; set; }
    }
}
