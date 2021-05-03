using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;
using QMS_API.Utils;

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
        public string Duration { get; set; }
        [Column(TypeName = "decimal(10, 2)")]
        public decimal Score { get; set; }
        public int Percentage { get; set; }
        public int CorrectQuestions { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime FinishDate { get; set; }
        public bool Submitted { get; set; }
        public List<QuizAnswer> QuizAnswers { get; set; }

        public QuizAttempt()
        {
            StartDate = DateTime.Now;
        }
    }
}
