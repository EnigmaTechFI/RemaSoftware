using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace RemaSoftware.ContextModels
{
    public class Order
    {
        public int OrderID { get; set; }

        [MaxLength(30)]
        public string ID_FattureInCloud { get; set; }

        [Required(ErrorMessage = "Questo campo è obbligatorio!")]
        [MaxLength(100)]
        public string Name { get; set; }

        public int ClientID { get; set; }
        public Client Client { get; set; }

        [Required(ErrorMessage = "Questo campo è obbligatorio!")]
        public int Number_Piece { get; set; }

        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}", ApplyFormatInEditMode = true)]
        public DateTime DataIn { get; set; }

        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}", ApplyFormatInEditMode = true)]
        public DateTime DataOut { get; set; }

        [Required(ErrorMessage = "Questo campo è obbligatorio!")]
        [MaxLength(20)]
        public string SKU { get; set; }

        [Required(ErrorMessage = "Questo campo è obbligatorio!")]
        [MaxLength(30)]
        public string DDT { get; set; }

        [Required(ErrorMessage = "Questo campo è obbligatorio!")]
        [MaxLength(70)]
        public string Image_URL { get; set; }

        public string Description { get; set; }

        public decimal Price_Uni { get; set; }
        public decimal Price_Tot => this.Price_Uni * this.Number_Piece;
        public string Note { get; set; }
        public char Status { get; set; }
        public virtual ICollection<Order_Operation> Order_Operation { get; set; }
    }
}
