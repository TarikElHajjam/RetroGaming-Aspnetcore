using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Retro_Gamer.Models
{
    public class Memories
    {
        [Key]
        public int Id { get; set; }
        public AppUser User { get; set; }  
        [MinLength(50), MaxLength(600),Required]
        public string Memory { get; set; }
        public int gameId { get; set; }
        public string UserId { get; set; }
        public string UserName { get; set; }
        public DateTime DatePosted { get; set; }
    }
}
