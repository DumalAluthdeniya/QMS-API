using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Security.AccessControl;
using System.Threading.Tasks;

namespace QMS_API.Models
{
    public class QuizAnswer
    {
        [Key]
        public int Id { get; set; }
        public QuizAttempt QuizAttempt { get; set; }
        public Question Question { get; set; }
        public Test Test { get; set; }
        public string MatchingText { get; set; }
        public string Answer { get; set; }
    }
}
