﻿using RemaSoftware.Domain.Models;
using System.Collections.Generic;

namespace RemaSoftware.WebApp.Models.ProductViewModel
{
    public class UpdateProductViewModel
    {
        public Product Product { get; set; }
        public List<Client> Clients { get; set; }
        public string Photo { get; set; }
    }
}