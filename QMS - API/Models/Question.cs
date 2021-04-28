using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace QMS_API.Models
{
    public class Question
    {
        [Key]
        public int Id { get; set; }
        [Required]
        public string Title { get; set; }
        [Required]
        public int QuestionType { get; set; }
        [Required]
        public int DifficultyLevel { get; set; }
        public bool RandomizeAnswers { get; set; }
        public int AnswerMaxLength { get; set; } 
        public bool IsDeleted { get; set; }
        public int Points { get; set; }
        public DateTime CreatedTime { get; set; }
        public virtual List<Answer> Answers { get; set; }
        public virtual IEnumerable<TestQuestion> TestQuestions { get; set; }


        public Question()
        {
            CreatedTime = DateTime.Now;
            DifficultyLevel = 1;
            QuestionType = 1;
            AnswerMaxLength = 1000;
            IsDeleted = false;
            Points = 0;
            RandomizeAnswers = false;
        }
    }
}
