using RemaSoftware.Domain.Models;
using System.Collections.Generic;

namespace RemaSoftware.WebApp.Models.ClientViewModel
{
    public class InfoClientViewModel
    {
        public Client Client { get; set; }
        
        public List<MyUser> MyUsers { get; set; }
     }
}
