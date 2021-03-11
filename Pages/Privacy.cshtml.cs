using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WebApplication1.Pages
{
    public class PrivacyModel : PageModel
    {
        private readonly ILogger<PrivacyModel> _logger;

        public PrivacyModel(ILogger<PrivacyModel> logger)
        {
            _logger = logger;
        }

        public IActionResult OnGet()
        {
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
