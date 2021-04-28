using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace QMS_API.Models
{
    public class Answer
    {
        [Key]
        public int Id { get; set; }
        [Required]
        public string Name { get; set; }
        public bool IsCorrectAnswer { get; set; }
        public string MatchingText { get; set; }
        public Question Question { get; set; }
    }
}
