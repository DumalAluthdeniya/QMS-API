using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using QMS_API.Models;

namespace QMS_API.Resources
{
    public class QuestionResource
    {
        public int Id { get; set; }
        [Required]
        public string Title { get; set; }
        public int QuestionType { get; set; }
        public int DifficultyLevel { get; set; }
        public bool RandomizeAnswers { get; set; }
        public int AnswerMaxLength { get; set; }
        public int Points { get; set; }
        public int GivenAnswerId { get; set; }
        public List<AnswerResource> Answers { get; set; }
    }
}
