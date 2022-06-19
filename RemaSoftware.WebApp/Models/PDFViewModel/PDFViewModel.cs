using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using RemaSoftware.Domain.Models;

namespace RemaSoftware.WebApp.Models.PDFViewModel
{
    public class PDFViewModel
    {
        public Order Order { get; set; }

        public string Photo { get; set; }
    }
}
