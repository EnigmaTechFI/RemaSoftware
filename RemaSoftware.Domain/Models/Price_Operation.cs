namespace RemaSoftware.Domain.Models;

public class Price_Operation
{
    public Price Price { get; set; }
    
    public Operation Operation { get; set; }
    public int Price_ID { get; set; }
    public int Operation_ID { get; set; }
}