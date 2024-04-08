using Common.Models;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using Presentation.Validators;

namespace Presentation.Models
{
    public class BookViewModel //the pages do not interact directly with the classes forming the database
    {
        public string EncryptedId { get; set; }
        public int Id { get; set; }

        [Required(AllowEmptyStrings = false, ErrorMessage = "Fill in the name")]
        [RegularExpression("^[A-Za-z ]+$", ErrorMessage = "Name must contain only letters")]
        public string Name { get; set; }

        [Required(AllowEmptyStrings = false, ErrorMessage = "Fill in the author name")]
        public string Author { get; set; }


        [Required(AllowEmptyStrings = true)]
        public string Filename { get; set; }


        public IFormFile File { get; set; }



        [CategoryValidation]
        public int CategoryFK { get; set; }

        [Required(ErrorMessage = "Fill in the year")]
        [YearValidation()]
        public int Year { get; set; }
    }
}
