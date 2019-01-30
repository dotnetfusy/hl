using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Biblioteka1.Models.ViewModels
{
    public class RegisterVM
    {
        [Required(ErrorMessage = "Username is required")]
        [Display(Name = "Email")]
        [EmailAddress]
        public string UserName { get; set; }
        [Required(ErrorMessage = "Password is required")]
        [Display(Name = "Password")]
        [DataType(DataType.Password)]
        [RegularExpression(@"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[#$^+=!*()@%&]).{6,}$", ErrorMessage = "Minimum 6 characters which contain at least 1 uppercase letter, 1 digit and 1 special characters required")]
        public string Password { get; set; }
        [Required(ErrorMessage = "Confirm Password is required")]
        [Display(Name = "Confirm Password")]
        [DataType(DataType.Password)]
        [Compare("Password")]
        [RegularExpression(@"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[#$^+=!*()@%&]).{6,}$", ErrorMessage = "Minimum 6 characters which contain at least 1 uppercase letter, 1 digit and 1 special characters required")]
        public string ConfirmPassword { get; set; }
    }
}
