using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using razorwebapp.models;

namespace App.Admin.Role
{
    [Authorize(Roles = "Admin")]
    public class AddRoleClaimModel : RolePageModel
    {
        public AddRoleClaimModel(RoleManager<IdentityRole> roleManager, MyWebContext myWebContext) : base(roleManager, myWebContext)
        {
        }

        public class InputModel
        {
        [Display(Name="ClainName")]
        [Required(ErrorMessage="Nhap {0}")]
        [StringLength(256, MinimumLength =3, ErrorMessage = "{0} phai dai tu {2} den {1} ki tu")]
        public string ClaimType { get; set; }

        [Display(Name="ClaimValue")]
        [Required(ErrorMessage="Nhap {0}")]
        [StringLength(256, MinimumLength =3, ErrorMessage = "{0} phai dai tu {2} den {1} ki tu")]
        public string ClaimValue { get; set; }
        }
    

        [BindProperty]

        public InputModel Input { get; set; }

        public IdentityRole role { get; set; }

        public async Task<IActionResult> OnGet(string roleid)
        {
           role = await _roleManager.FindByIdAsync(roleid);
           if(role == null) return NotFound("Khong tim thay role");
           return Page();
        }

        public async Task<IActionResult> OnPostAsync(string roleid)
        {

            role = await _roleManager.FindByIdAsync(roleid);
           if(role == null) return NotFound("Khong tim thay role");

            if(!ModelState.IsValid)
            {
                return Page();
            }

            if((await _roleManager.GetClaimsAsync(role)).Any(c=> c.Type == Input.ClaimType && c.Value == Input.ClaimValue))
            {
                ModelState.AddModelError(string.Empty, "Claim nay da co");
                return Page();
            }

            var newClaim = new Claim(Input.ClaimType, Input.ClaimValue);
            var result =  await _roleManager.AddClaimAsync(role, newClaim); 

            if(!result.Succeeded)
            {
                result.Errors.ToList().ForEach(e => {
                     ModelState.AddModelError(string.Empty, e.Description);

                });
                return Page();
            }


            StatusMessage = "vua them Claim moi";
            

            return RedirectToPage("./Edit", new {roleid = role.Id});

        }
    }
}
