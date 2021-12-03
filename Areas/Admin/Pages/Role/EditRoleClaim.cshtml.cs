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
using App.Models;

namespace App.Admin.Role
{
    [Authorize(Roles = "Admin")]
    public class EditRoleClaimModel : RolePageModel
    {
        public EditRoleClaimModel(RoleManager<IdentityRole> roleManager, AppDbContext myWebContext) : base(roleManager, myWebContext)
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

        IdentityRoleClaim<string> claim { get; set; }

        public async Task<IActionResult> OnGet(int? claimid)
        {
            if(claimid ==null) return NotFound("Khong tim thay role");

           claim =  _context.RoleClaims.Where(c => c.Id == claimid).FirstOrDefault();

           if(claim == null) return NotFound("Khong tim thay role");

           role = await _roleManager.FindByIdAsync(claim.RoleId);
           if(role == null) return NotFound("Khong tim thay role");

           Input = new InputModel()
           {
               ClaimType = claim.ClaimType, 
               ClaimValue = claim.ClaimValue
           };
           return Page();
        }

        public async Task<IActionResult> OnPostAsync(int? claimid)
        {

             if(claimid==null) return NotFound("Khong tim thay role");

           claim =  _context.RoleClaims.Where(c => c.Id == claimid).FirstOrDefault();

           if(claim== null) return NotFound("Khong tim thay role");

           role = await _roleManager.FindByIdAsync(claim.RoleId);
           if(role == null) return NotFound("Khong tim thay role");


            if(!ModelState.IsValid)
            {
                return Page();
            }

            if( _context.RoleClaims.Any(c=> c.RoleId== role.Id && c.ClaimType == Input.ClaimType && c.ClaimValue == Input.ClaimValue && c.Id != claim.Id))
            {
                ModelState.AddModelError(string.Empty, "Claim nay da co");
                return Page();
            }

            claim.ClaimType = Input.ClaimType;
            claim.ClaimValue = Input.ClaimValue;

            await _context.SaveChangesAsync();

            

            StatusMessage = "vua cap nhat Claim ";
            

            return RedirectToPage("./Edit", new {roleid = role.Id});

        }

           public async Task<IActionResult> OnPostDeleteAsync(int? claimid)
        {

             if(claimid==null) return NotFound("Khong tim thay role");

           claim =  _context.RoleClaims.Where(c => c.Id == claimid).FirstOrDefault();

           if(claim== null) return NotFound("Khong tim thay role");

           role = await _roleManager.FindByIdAsync(claim.RoleId);
           if(role == null) return NotFound("Khong tim thay role");

            await _roleManager.RemoveClaimAsync(role, new Claim(claim.ClaimType,claim.ClaimValue));
            

            StatusMessage = "vua xoa Claim ";
            

            return RedirectToPage("./Edit", new {roleid = role.Id});

        }
    }
    }
    

