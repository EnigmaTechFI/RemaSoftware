using Microsoft.AspNetCore.Identity;
using System;

namespace RemaSoftware.Domain.ContextModels
{
    public class MyUser : IdentityUser
    {
        public string Name { get; set; }
        public string Surname { get; set; }
        public DateTime Birthday { get; set; }
    }
}
