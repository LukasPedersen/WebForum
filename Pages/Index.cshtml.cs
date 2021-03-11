using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using WebApplication1.Data;
using System.Linq;
using System.Threading.Tasks;

namespace WebApplication1.Pages
{
    public class IndexModel : PageModel
    {
        private readonly ILogger<IndexModel> _logger;
        public IndexModel(ILogger<IndexModel> logger)
        {
            _logger = logger;
        }

        public IActionResult OnGet()
        {
            Repository.SetConnectionString();
            if (Repository.loggedIn == false)
                return OnPostLogIn();
            else
                return null;
        }
        public IActionResult OnPostLogIn()
        {
            return RedirectToPage("Login");
        }
        public IActionResult OnPostLogOut()
        {
            Repository.currentLoggedInUsername = null;
            Repository.loggedIn = false;
            return OnGet();
        }
    }
}
