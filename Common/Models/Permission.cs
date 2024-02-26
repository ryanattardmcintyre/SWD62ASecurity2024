using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using System.ComponentModel;

namespace Common.Models
{
    public class Permission
    {



        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [Required]
        [ForeignKey("Book")]
        public int BookIdFK { get; set; }
        public virtual Book Book { get; set; }


        [Required]
        [ForeignKey("User")]
        public string UserIdFK { get; set; }
        public virtual IdentityUser User { get; set; }


        [Required]
        [DefaultValue(false)]
        public bool Read { get; set; }
        
        [Required]
        [DefaultValue(false)]
        public bool Write { get; set; }

        public string IpAddress { get; set; }

    }
}
