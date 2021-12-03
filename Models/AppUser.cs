using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Identity;

namespace App.Models
{
    public class AppUser : IdentityUser
    {

        [StringLength(256)]
        [Column(TypeName = "nvarchar")]
        public string HomeAndress { get; set; }
        
        [DataType(DataType.Date)]
        public DateTime? BirthDay { get; set; }
        
    }
}