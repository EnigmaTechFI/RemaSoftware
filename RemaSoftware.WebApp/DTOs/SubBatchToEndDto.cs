namespace RemaSoftware.WebApp.DTOs;

public class SubBatchToEndDto
{
    public int SubBatchId { get; set; }
    public int OkPieces { get; set; }
    public int LostPieces { get; set; }
    public int WastePieces { get; set; }
    
    public int ZamaPieces { get; set; }
    public int OperationTimeLineId { get; set; }
}