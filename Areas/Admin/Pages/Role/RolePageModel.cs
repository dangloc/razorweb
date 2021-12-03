using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using razorwebapp.models;

namespace App.Admin.Role
{
    public class RolePageModel : PageModel
{
    protected readonly RoleManager<IdentityRole> _roleManager;
    protected readonly MyWebContext _context;
    [TempData]
    public  string StatusMessage { get; set; }

    public RolePageModel(RoleManager<IdentityRole> roleManager, MyWebContext myWebContext)
    {
        _roleManager = roleManager;
        _context = myWebContext;
        
    }
}
}