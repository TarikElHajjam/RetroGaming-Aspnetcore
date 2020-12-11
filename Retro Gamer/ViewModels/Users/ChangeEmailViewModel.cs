using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Retro_Gamer.ViewModels
{
    public class ChangeEmailViewModel
    {
        [Required]
        [Display(Name = "New Email")]
        [DataType(DataType.EmailAddress)]
        public string NewEmail { get; set; }
        public string Token { get; set; }
    }
}
