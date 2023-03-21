namespace RemaSoftware.WebApp.DTOs;

public class ProductionAnalysisDto
{
    public int OperationTimeLineId { get; set; }
    public int SubBatchId { get; set; }
    public string OperationName { get; set; }
    public string ClientName { get; set; }
    public int Seconds { get; set; }
    public int MachineId { get; set; }
}