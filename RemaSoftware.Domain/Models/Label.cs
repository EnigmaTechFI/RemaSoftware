namespace RemaSoftware.Domain.Models;

public class Label
{
    public int Id { get; set; }
    public DateTime Date { get; set; }
    public int OkPieces { get; set; }
    public int LostPieces { get; set; }
    public int WastePieces { get; set; }
    public int ZamaPieces { get; set; }
    
    public int ResoScarto { get; set; }
    public int SubBatchCode { get; set; }
    public string SKU { get; set; }
    public string Client { get; set; } 
}