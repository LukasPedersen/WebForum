using System;
using WebApplication1;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using WebApplication1.Data;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace WebApplication1.Pages
{
    public class SignUpModel : PageModel
    {
        public void OnGet()
        {
        }
        public IActionResult OnPost()
        {
            Repository rep = new Repository();
            string Iusername = Request.Form["Iusername"];
            string Ipassword = Request.Form["Ipassword"];
            string Ifirstname = Request.Form["Ifirstname"];
            string Ilastname = Request.Form["Ilastname"];
            string Iemail = Request.Form["Iemail"];
            rep.CreateUser(Iusername, Ipassword, Ifirstname, Ilastname, Iemail);
            return RedirectToPage("Index");
        }
    }
}
