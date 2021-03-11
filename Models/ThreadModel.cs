using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WebApplication1.Models
{
    public class Thread
    {
        public int userID { get; set; }
        public int topicsID { get; set; }
        public string headder { get; set; }
        public string content { get; set; }
        public string threadForfatter { get; set; }
    }
}
