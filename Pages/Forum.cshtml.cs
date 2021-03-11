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
        public IList<Thread> listOfThreads { get; set; }


        Repository rep = new Repository();
        public IActionResult OnGet()
        {
            
            if (Repository.loggedIn == false)
                return OnPostLogIn();
            else
            {
                listOfThreads = rep.GetAllThreads();
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
        public IActionResult OnPostNewTopic()
        {
            string _headder = Request.Form["topicHeadder"];
            string _text = Request.Form["topicText"];
            rep.CreateTopic(_headder, _text);
            return OnGet();
        }
        public IActionResult OnPostDeleteTopic()
        {
            int _topicID = Convert.ToInt32(Request.Form["_topicID"]);
            string _forfatter = Request.Form["_forfatter"];
            rep.DeleteTopic(_topicID, _forfatter);
            return OnGet();
        }
        public IActionResult OnPostNewThread()
        {
            int _topicID = Convert.ToInt32(Request.Form["_threadTopicID"]);
            string _headder = Request.Form["threadHeadder"];
            string _text = Request.Form["threadText"];
            rep.CreateThread(_topicID, _headder, _text);
            return OnGet();
        }
        public IActionResult OnPostDeleteThread()
        {
            int _topicID = Convert.ToInt32(Request.Form["_topicID"]);
            string _forfatter = Request.Form["_forfatter"];
            //rep.DeleteThread(_topicID, _forfatter);
            return OnGet();
        }
    }
}
