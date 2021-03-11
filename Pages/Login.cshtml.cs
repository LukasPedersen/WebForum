using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace WebApplication1.Pages.Shared
{
    public class LoginModel : PageModel
    {
        public void OnGet()
        {
        }
        public IActionResult OnPost()
        {
            Repository rep = new Repository();
            string Iusername = Request.Form["Iusername"];
            string Ipassword = Request.Form["Ipassword"];
            bool exists = rep.UserLogin(Iusername, Ipassword);
            if (exists == true)
            {
                return RedirectToPage("Index");
            }
            else
            return RedirectToPage("Login");
        }
    }
}
