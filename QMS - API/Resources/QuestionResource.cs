using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using QMS_API.Models;
using static QMS_API.Enums.Enums;

namespace QMS_API.Resources
{
    public class QuestionResource
    {
        public int Id { get; set; }
        [Required]
        public string Title { get; set; }
        public QuestionTypes QuestionType { get; set; }
        public string Topic { get; set; }
        public DifficultyLevels DifficultyLevel { get; set; }
        public bool RandomizeAnswers { get; set; }
        public int AnswerMaxLength { get; set; }
        public int Points { get; set; }
        public int GivenAnswerId { get; set; }
        public List<AnswerResource> Answers { get; set; }
        public string Duration { get; set; }
        public string User { get; set; }
    }
}
