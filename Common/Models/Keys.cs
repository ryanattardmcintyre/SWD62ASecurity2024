using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Common.Models
{
    /*
    public class Keys
    {

        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        public string PublicKey { get; set; }
        public string PrivateKey { get; set; }

        [Required]
        [ForeignKey("User")]
        public string UserIdFK { get; set; }
        public virtual IdentityUser User { get; set; }

    }

    public class Logs
    {
        public int Id{get;set;}

        public DateTime Timestamp {get;set;}
        public string Message {get;set;}
        public string? User{get;set;} //it must not be referenced
    }

    */
}
