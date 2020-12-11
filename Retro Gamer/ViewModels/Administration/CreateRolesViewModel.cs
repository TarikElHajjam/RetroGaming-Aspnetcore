using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Retro_Gamer.Models
{
    public class CreateRoleViewModel
    {
        [Required]
        [Remote(action:"IsRoleInUse", controller:"Administration")]
        public string Name { get; set; }
    }
}
