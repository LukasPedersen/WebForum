using System;
using System.Collections.Generic;
using WebApplication1.Models;
using System.Linq;
using System.Threading.Tasks;

namespace WebApplication1.Data
{
    public interface IRepository
    {
        public static bool islogin;
        public void CreateUser(string username, string password, string firstName, string lastName, string email);
        public bool UserLogin(string username, string pasword);
        public List<Topic> GetAllTopics();
    }
}
