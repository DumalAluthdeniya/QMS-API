using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using static QMS_API.Enums.Enums;

namespace QMS_API.Models
{
    public class Question
    {
        [Key]
        public int Id { get; set; }
        [Required]
        public string Title { get; set; }
        [Required]
        public QuestionTypes QuestionType { get; set; }
        [Required]
        public DifficultyLevels DifficultyLevel { get; set; }
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
            DifficultyLevel = DifficultyLevels.Easy;
            QuestionType = QuestionTypes.MultipleChoice;
            AnswerMaxLength = 1000;
            IsDeleted = false;
            Points = 0;
            RandomizeAnswers = false;
        }
    }
}
