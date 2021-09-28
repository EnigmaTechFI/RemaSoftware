using Microsoft.AspNetCore.Mvc.ViewFeatures;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;

namespace RemaSoftware.ContextModels
{
    public class Order
    {

        public int OrderID { get; set; }

        [Required(ErrorMessage = "Questo campo è obbligatorio!")]
        public string Name { get; set; }

        public int ClientID { get; set; }
        public Client Client { get; set; }

        [Required(ErrorMessage = "Questo campo è obbligatorio!")]
        public int Number_Piece { get; set; }

        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{dd/MM/yyyy}", ApplyFormatInEditMode = true)]
        public DateTime DataIn { get; set; }

        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{dd/MM/yyyy}", ApplyFormatInEditMode = true)]
        public DateTime DataOut { get; set; }

        [Required(ErrorMessage = "Questo campo è obbligatorio!")]
        public string SKU { get; set; }

        [Required(ErrorMessage = "Questo campo è obbligatorio!")]
        public string Image_URL { get; set; }

        public string Pdf_URL { get; set; }

        public string Description { get; set; }

        public bool Flag_Fattureincloud { get; set; }

        public double Price_Tot { get; set; }

        public double Price_Uni { get; set; }

        public virtual ICollection<Order_Operation> Order_Operation { get; set; }
    }
}
