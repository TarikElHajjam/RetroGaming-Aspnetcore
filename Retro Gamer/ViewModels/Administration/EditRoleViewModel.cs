using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Retro_Gamer.Models
{
    public class EditRoleViewModel
    {
        public EditRoleViewModel()
        {
            Users = new List<string>();
        }
      
        public string Id { get; set; }
        [Required(ErrorMessage ="Role Name is required")]
        [Display(Name ="Role Name")]
        public string RoleName { get; set; }
        public List<string> Users { get; set; }
    }
}
