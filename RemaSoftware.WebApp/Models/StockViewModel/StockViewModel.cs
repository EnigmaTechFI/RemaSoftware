using System.ComponentModel.DataAnnotations;

namespace RemaSoftware.WebApp.Models.StockViewModel
{
    public class StockViewModel
    {
        public int StockArticleId { get; set; }
        
        [Required(ErrorMessage = "Questo campo è obbligatorio!")]
        [MaxLength(100)]
        public string Name { get; set; }

        [MaxLength(100)]
        public string Brand { get; set; }

        [Required(ErrorMessage = "Questo campo è obbligatorio!")]
        public int Number_Piece { get; set; }

        [Required(ErrorMessage = "Questo campo è obbligatorio!")]
        public decimal Price_Uni { get; set; }

        [MaxLength(20)]
        public string Size { get; set; }        
        public decimal Price_Tot => this.Price_Uni * this.Number_Piece;
    }
}
