﻿using Microsoft.AspNetCore.Identity;
using System;

namespace RemaSoftware.Domain.Models
{
    public class MyUser : IdentityUser
    {
        public string Name { get; set; }
        public string Surname { get; set; }
        public DateTime Birthday { get; set; }
        public virtual UserClient UserClient { get; set; }
    }
}