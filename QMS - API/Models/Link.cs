using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace QMS_API.Models
{
    public class Link
    {
        [Key]
        public int Id { get; set; }
        public Guid Code { get; set; }
        public string Name { get; set; }
        public string Url { get; set; }
        public int TimeLimit { get; set; }
        public string Password { get; set; }
        public DateTime CreatedTime { get; set; }
        public ApplicationUser CreatedBy { get; set; }
        public bool IsDeleted { get; set; }
        public virtual Test Test { get; set; }
        public virtual List<QuizAttempt> QuizAttempts { get; set; }

        public Link()
        {
            CreatedTime = DateTime.Now;
        }

    }
}
