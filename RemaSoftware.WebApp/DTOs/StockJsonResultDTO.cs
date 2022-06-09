namespace RemaSoftware.WebApp.DTOs;

public class StockJsonResultDTO : JsonResultBaseDTO
{
    public int? Number_Piece { get; set; }
    public decimal? Price_Tot { get; set; }
    public StockJsonResultDTO() { }
        
    public StockJsonResultDTO(bool result, string toastMessage) : base(result, toastMessage) { }
}