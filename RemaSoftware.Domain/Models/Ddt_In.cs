using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace RemaSoftware.Domain.Models
{
    public class Ddt_In
    {
        public int Ddt_In_ID { get; set; }
        public int? Ddt_Out_ID { get; set; }
        public int ProductID { get; set; }
        public Product Product { get; set; }
        public int BatchID { get; set; }
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}", ApplyFormatInEditMode = true)]
        public DateTime DataIn { get; set; }

        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}", ApplyFormatInEditMode = true)]
        public DateTime DataOut { get; set; }
        [Required(ErrorMessage = "Questo campo è obbligatorio!")]
        [MaxLength(50)]
        public string Code { get; set; }
        [MaxLength(30)]
        public string FC_Ddt_In_ID { get; set; }
        [Required(ErrorMessage = "Questo campo è obbligatorio!")]
        public int Number_Piece { get; set; }
        public int Priority { get; set;}
        [MaxLength(1)]
        public string Status { get; set; }
        public bool IsReso { get; set; }
        public string Note { get; set; }
        public string Description { get; set; }
        public Batch Batch { get; set; }
    }
}
