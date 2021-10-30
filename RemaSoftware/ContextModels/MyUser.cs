using Microsoft.AspNetCore.Identity;
using System;

namespace RemaSoftware.ContextModels
{
    public class MyUser : IdentityUser
    {
        public string Name { get; set; }
        public string Surname { get; set; }
        public DateTime Birthday { get; set; }
    }
}
