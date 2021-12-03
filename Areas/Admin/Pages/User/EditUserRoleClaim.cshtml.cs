using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using App.Models;

namespace App.Admin.User
{
    public class EditUserRoleClaimModel : PageModel
    {
        private readonly AppDbContext _context;
        private readonly UserManager<AppUser> _userManager;
        public EditUserRoleClaimModel(AppDbContext myWebContext, UserManager<AppUser> userManager)
        {
            _context = myWebContext;
            _userManager = userManager;

        }

        [TempData]
        public string StatusMessage { get; set; }
        public NotFoundObjectResult OnGet() => NotFound("Khong duoc truy cap");

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

        public AppUser user { get; set; }

        public async Task<IActionResult> OnGetAddClaimAsync(string userid)
        {
            user = await _userManager.FindByIdAsync(userid);
            if(user== null) return NotFound("Khong tim thay user");
            return Page();
        }

         public async Task<IActionResult> OnPostAddClaimAsync(string userid)
        {
            user = await _userManager.FindByIdAsync(userid);
            if(user == null) return NotFound("Khong tim thay user");

            if(!ModelState.IsValid) return Page();

            var claims = _context.UserClaims.Where(c => c.UserId == user.Id);

            if(claims.Any(c => c.ClaimType == Input.ClaimType && c.ClaimValue==Input.ClaimValue))
            {
                ModelState.AddModelError(string.Empty, "Dac tinh nay da co");
                return Page();
            }

            await _userManager.AddClaimAsync(user, new Claim(Input.ClaimType, Input.ClaimValue));
            StatusMessage = "Da them dac tinh cho user";
            return RedirectToPage("./AddRole", new {id = user.Id});
        }

        public IdentityUserClaim<string> userclaim { get; set;}
    
        
        public async Task<IActionResult> OnGetEditClaimAsync(int? claimid)
        {

            if (claimid == null) return NotFound("khong tim thay user");

            userclaim = _context.UserClaims.Where(c => c.Id == claimid).FirstOrDefault();
            user = await _userManager.FindByIdAsync(userclaim.UserId);
            if(user== null) return NotFound("Khong tim thay user");

            Input = new InputModel()
            {
                ClaimType = userclaim.ClaimType,
                ClaimValue = userclaim.ClaimValue
            };
            return Page();
        }

        public async Task<IActionResult> OnPostEditClaimAsync(int? claimid)
        {

            if (claimid == null) return NotFound("khong tim thay user");

            userclaim = _context.UserClaims.Where(c => c.Id == claimid).FirstOrDefault();
            user = await _userManager.FindByIdAsync(userclaim.UserId);
            if(user== null) return NotFound("Khong tim thay user");

            if(ModelState.IsValid) return Page();

           if( _context.UserClaims.Any(c => c.UserId == user.Id 
            && c.ClaimType == Input.ClaimType 
            && c.ClaimValue == Input.ClaimValue 
            && c.Id != userclaim.Id)) 
            {
                ModelState.AddModelError(string.Empty, " Claim nay da co");
                return Page();
            }

            userclaim.ClaimType = Input.ClaimType;
            userclaim.ClaimValue = Input.ClaimValue;

            await _context.SaveChangesAsync();
            StatusMessage = "Ban vua cap nhat";

            return RedirectToPage("./AddRole", new {id = user.Id});
        }

          public async Task<IActionResult> OnPostDeleteAsync(int? claimid)
        {

            if (claimid == null) return NotFound("khong tim thay user");

            userclaim = _context.UserClaims.Where(c => c.Id == claimid).FirstOrDefault();
            user = await _userManager.FindByIdAsync(userclaim.UserId);
            if(user== null) return NotFound("Khong tim thay user");

            await _userManager.RemoveClaimAsync(user, new Claim(userclaim.ClaimType, userclaim.ClaimValue));
            StatusMessage = "Ban vua xoa claim";

            return RedirectToPage("./AddRole", new {id = user.Id});
        }
    }

}
