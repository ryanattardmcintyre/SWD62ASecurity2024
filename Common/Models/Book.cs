using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Common.Models
{
    public class Book
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [Required(AllowEmptyStrings =false, ErrorMessage="Fill in the name")]
        [RegularExpression("^[A-Za-z ]+$", ErrorMessage ="Name must contain only letters")]
        public string Name { get; set; }

        [Required(AllowEmptyStrings = false, ErrorMessage = "Fill in the author name")]
        public string Author { get; set; }


        [Required(AllowEmptyStrings =true)]
        public string Filename { get; set; }
       
      
        [ForeignKey("Category")]
        public int CategoryFK { get; set; }

        public virtual Category Category { get; set; }

        [Required(ErrorMessage = "Fill in the year")]
        public int Year { get; set; }
 
    }
}
