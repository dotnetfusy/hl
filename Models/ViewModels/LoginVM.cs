using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Biblioteka1.Models.ViewModels
{
    public class LoginVM
    {
        [Required]
        [Display(Name = "Email")]
        [EmailAddress]
        public string UserName { get; set; }
        [Required]
        [Display(Name = "Password")]
        [DataType(DataType.Password)]
        public string Password { get; set; }
    }
}
