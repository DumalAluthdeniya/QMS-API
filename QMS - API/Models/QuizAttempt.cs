using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace QMS_API.Models
{
    public class QuizAttempt
    {
        [Key]
        public int Id { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
        public string Enrollment { get; set; }
        public Link Link { get; set; }
        public List<QuizAnswer> QuizAnswers { get; set; }
    }
}
