﻿using System.ComponentModel.DataAnnotations;

namespace RemaSoftware.Domain.Models
{
    public class Product
    {
        public int ProductID { get; set; }
        public int ClientID {get; set; }
        public Client Client { get; set; }

        [Required(ErrorMessage = "Questo campo è obbligatorio!")]
        [MaxLength(100)]
        public string Name { get; set; }
        [MaxLength(30)]
        public string RemaCode { get; set; }
        [Required(ErrorMessage = "Questo campo è obbligatorio!")]
        [MaxLength(50)]
        public string SKU { get; set; }
        [MaxLength(70)]
        public string FileName { get; set; }
        public virtual List<Ddt_In> Ddts_In { get; set; }

        public virtual List<Price> Prices { get; set; }
    }
}
