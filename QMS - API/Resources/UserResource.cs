using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace QMS__API.Resources
{
    public class UserResource
    {
        [Required]
        public string Name { get; set; }
       
        public string Email { get; set; }
        [Required]
        public string Password { get; set; }
    }
}
