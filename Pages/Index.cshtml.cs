using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;
using razorwebapp.models;

namespace razorwebapp.Pages
{
    public class IndexModel : PageModel
    {
        private readonly ILogger<IndexModel> _logger;
        private readonly MyWebContext myWebContext;

        public IndexModel(ILogger<IndexModel> logger, MyWebContext _context)
        {
            _logger = logger;
            myWebContext = _context;
        }

        public void OnGet()
        {
            var posts = (from a in myWebContext.articles
                        orderby a.Created descending
                        select a).ToList();
                
            ViewData["posts"] = posts;

        }
    }
}
