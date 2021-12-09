using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using razorwebapp.models;

namespace App.Admin.Role
{
    [Authorize(Roles = "Admin")]
    public class EditModel : RolePageModel
    {
        public EditModel(RoleManager<IdentityRole> roleManager, MyWebContext myWebContext) : base(roleManager, myWebContext)
        {
        }

        public class InputModel
        {
        [Display(Name="RoleName")]
        [Required(ErrorMessage="Nhap {0}")]
        [StringLength(256, MinimumLength =3, ErrorMessage = "{0} phai dai tu {2} den {1} ki tu")]
        public string Name { get; set; }
        }
    

        [BindProperty]

        public InputModel Input { get; set; }

        public IdentityRole role { get; set; }

        public async Task<IActionResult> OnGet(string roleid)
        {
            if(roleid == null) return NotFound("Khong tim thay role");


            role = await _roleManager.FindByIdAsync(roleid);

            if(role != null)
            {
                Input = new InputModel()
                {
                    Name = role.Name
                };
                return Page();
            }
            return NotFound("Khong tim thay role");
        }

        public async Task<IActionResult> OnPostAsync(string roleid)
        {
             if(roleid == null) return NotFound("Khong tim thay role");
             role = await _roleManager.FindByIdAsync(roleid);
                if(role == null)  return NotFound("Khong tim thay role");


            if(!ModelState.IsValid)
            {
                return Page();
            }

            role.Name = Input.Name;
            var result = await _roleManager.UpdateAsync(role);
            if (result.Succeeded)
            {
                StatusMessage = $"Ban vua chinh sua role {Input.Name}";
                return RedirectToPage("./Index");
            }
            else
            {
                result.Errors.ToList().ForEach(error =>{
                    ModelState.AddModelError(string.Empty, error.Description);
                });
            }


            return Page();

        }
    }
}