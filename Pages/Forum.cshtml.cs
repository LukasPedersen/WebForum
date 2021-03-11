using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WebApplication1.Models;
using WebApplication1.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace WebApplication1.Pages
{
    public class ForumModel : PageModel
    {
        private readonly IRepository _context;

        public ForumModel(IRepository context)
        {
            _context = context;
        }
        public IList<Topic> listOfTopics { get; set; }


        
        public IActionResult OnGet()
        {
            Repository rep = new Repository();
            if (Repository.loggedIn == false)
                return OnPostLogIn();
            else
            {
                listOfTopics = rep.GetAllTopics();
                return null;
            }
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
