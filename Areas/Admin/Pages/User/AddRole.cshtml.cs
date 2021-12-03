using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using razorwebapp.models;

namespace App.Admin.User
{
    public class AddRoleModel : PageModel
    {
        private readonly UserManager<AppUser> _userManager;
        private readonly SignInManager<AppUser> _signInManager;

        private readonly RoleManager<IdentityRole> _roleManager;

        public AddRoleModel(
            UserManager<AppUser> userManager,
            SignInManager<AppUser> signInManager, 
            RoleManager<IdentityRole> roleManager)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _roleManager = roleManager;
        }

      

        [TempData]
        public string StatusMessage { get; set; }

        [Display(Name = "Roles gan cho User")]
        [BindProperty]
        public string[] RoleNames { get; set; }

        public SelectList allRoles { get; set; }


        public AppUser user { get; set; }
        public async Task<IActionResult> OnGetAsync(string id)
        {
            
            if (string.IsNullOrEmpty(id))
            {
                return NotFound("Unable to load user");
            }

            user = await _userManager.FindByIdAsync(id);

            if (user== null)
            {
                return NotFound($"Unable to load user, id = {id}.");
            }

            RoleNames= (await _userManager.GetRolesAsync(user)).ToArray<string>();

             List<string> roleName = await _roleManager.Roles.Select(r => r.Name).ToListAsync();
            allRoles = new SelectList(roleName);

            return Page();
        }

        public async Task<IActionResult> OnPostAsync(string id)
        {
              if (string.IsNullOrEmpty(id))
             {
                 return NotFound("Unable to load user");
             }

             user = await _userManager.FindByIdAsync(id);

             if (user== null)
             {
                 return NotFound($"Unable to load user, id = {id}.");
             }
             
             var oldRolename = (await _userManager.GetRolesAsync(user)).ToArray();

             var deleteRoles = oldRolename.Where(r => !RoleNames.Contains(r));
             var addRoles= RoleNames.Where(r => !oldRolename.Contains(r));

            List<string> roleName = await _roleManager.Roles.Select(r => r.Name).ToListAsync();
            allRoles = new SelectList(roleName);
            
             var resultDelete = await _userManager.RemoveFromRolesAsync(user, deleteRoles);

             if(!resultDelete.Succeeded)
             {
                    resultDelete.Errors.ToList().ForEach(error => {
                        ModelState.AddModelError(string.Empty, error.Description);
                    });

                    return Page();

             }

              var resultAdd = await _userManager.AddToRolesAsync(user, addRoles);

             if(!resultAdd.Succeeded)
             {
                    resultAdd.Errors.ToList().ForEach(error => {
                        ModelState.AddModelError(string.Empty, error.Description);
                    });

                    return Page();

             }


            StatusMessage = $"Your Roles has been set: {user.UserName}";

             return RedirectToPage("./Index");
        }
    }
}
