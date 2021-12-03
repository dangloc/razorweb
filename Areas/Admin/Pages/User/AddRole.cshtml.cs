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
using App.Models;

namespace App.Admin.User
{
    public class AddRoleModel : PageModel
    {
        private readonly UserManager<AppUser> _userManager;
        private readonly SignInManager<AppUser> _signInManager;

        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly AppDbContext _context;

        public AddRoleModel(
            UserManager<AppUser> userManager,
            SignInManager<AppUser> signInManager, 
            RoleManager<IdentityRole> roleManager,
            AppDbContext context)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _roleManager = roleManager;
            _context = context;
        }

      

        [TempData]
        public string StatusMessage { get; set; }

        [Display(Name = "Roles gan cho User")]
        [BindProperty]
        public string[] RoleNames { get; set; }

        public SelectList allRoles { get; set; }


        public AppUser user { get; set; }

        public List<IdentityRoleClaim<string>> claimInRole { get; set; }
        public List<IdentityUserClaim<string>> claimInUserClaim { get; set; }
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

            await GetClaims(id);

            return Page();
        }

        async Task GetClaims(string id)
        {
            var listRoles = from r in _context.Roles
                            join ur in _context.UserRoles on r.Id equals ur.RoleId
                            where ur.UserId == id
                            select r;

            var _claimInRole = from c in _context.RoleClaims
                                join r in listRoles on c.RoleId equals r.Id
                                select c;

            claimInRole = await _claimInRole.ToListAsync();

            claimInUserClaim =  await (from c in _context.UserClaims
            where c.UserId == id select c).ToListAsync();
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
             
             await GetClaims(id);


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
