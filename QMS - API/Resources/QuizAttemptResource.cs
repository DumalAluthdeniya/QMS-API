using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using QMS_API.Models;

namespace QMS_API.Resources
{
    public class QuizAttemptResource
    {

        public int Id { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
        public string Enrollment { get; set; }
        public int LinkId { get; set; }
        public string Duration { get; set; }
        public decimal Score { get; set; }
        public int Percentage { get; set; }
        public int CorrectQuestions { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime FinishDate { get; set; }
        public bool Submitted { get; set; }
        public List<QuizAnswerResource> QuizAnswers { get; set; }
    }
}
