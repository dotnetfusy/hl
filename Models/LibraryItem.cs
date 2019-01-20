using Biblioteka1.Models.Enumerations;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace Biblioteka1.Models
{
    public class LibraryItem
    {
       
        public int Id { get; set; }

        [Required(ErrorMessage = "A title is required.")]
        [StringLength(100, ErrorMessage = "The title is too long.")]
        public string Title { get; set; }

        [Required(ErrorMessage = "An author is required.")]
        [StringLength(100, ErrorMessage = "An author name is too long.")]
        public string Author { get; set; }

        public string Director { get; set; }
        public string Performer { get; set; }

        [Required(ErrorMessage = "A format is required.")]      
        public string Format { get; set; }

        [Required(ErrorMessage = "A year is required.")]
        [StringLength(4, ErrorMessage = "A year is too long.")]
        public string Year { get; set; }

        //element biblioteki wypozyczony?
        public bool CheckedOut { get; set; }

        //element biblioteki wypozyczony do
        public string CheckedOutTo { get; set; }

        //element biblioteki wypozyczony przez
        public string CheckedOutBy { get; set; }

        //element biblioteki wypozyczony - data
        public string CheckedOutDate { get; set; }

        public string CoverString { get; set; }
    }
}
