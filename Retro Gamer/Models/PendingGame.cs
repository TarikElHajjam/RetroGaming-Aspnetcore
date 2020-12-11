using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Retro_Gamer.Models
{
    /// <summary>
    ///  Users will use this model to be able to add new game but they will need to await
    ///  Admin approval before making the added games visable for everyone.
    /// </summary>
    public class PendingGame
    {
        [Key]
        public int Id { get; set; }
        [Required, MaxLength(50)]
        public string Name { get; set; }
        [Required]
        [DataType(DataType.Date)]
        [Display(Name = "Release Date")]
        public DateTime RelaseDate { get; set; }
        [Required]
        public Genre Genres { get; set; }
        [Required, Range(1, 5)]
        public int Rating { get; set; }
        [Required, MaxLength(220), MinLength(50)]
        public string Description { get; set; }
        public string PhotoUrl { get; set; }
       
        public bool IsSelected { get; set; }
        public string UserId { get; set; }
    }
}
