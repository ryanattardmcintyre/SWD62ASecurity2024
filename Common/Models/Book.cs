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

        [Required]
        public string Name { get; set; }
        [Required]
        public string Author { get; set; }
        [Required]
        public string Filename { get; set; }
       
        [Required]
        [ForeignKey("Category")]
        public int CategoryFK { get; set; }

        public virtual Category Category { get; set; }

        [Required]
        public int Year { get; set; }
    }
}
