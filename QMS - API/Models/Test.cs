using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace QMS_API.Models
{
    public class Test
    {
        [Key]
        public int Id { get; set; }
        [Required]
        public string Name { get; set; }
        public string Introduction { get; set; }
        public IEnumerable<TestQuestion> TestQuestions { get; set; }
        public DateTime CreatedTime { get; set; }
        public ApplicationUser CreatedBy { get; set; }
        public bool IsDeleted { get; set; }
        public virtual List<Link> Links { get; set; }

        public Test()
        {
            CreatedTime = DateTime.Now;
            IsDeleted = false;
        }

    }
}
