namespace RemaSoftware.Domain.Models;

public class PriceOperation
{
    public Price Price { get; set; }
    
    public Operation Operation { get; set; }
    
    public int PriceID { get; set; }
    public int OperationID { get; set; }
}