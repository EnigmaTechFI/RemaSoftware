using System.Collections.Generic;
using RemaSoftware.Domain.Models;

namespace RemaSoftware.WebApp.Models.AccountingViewModel
{

    public class ProductionAnalysisLiveViewModel
    {
        public List<ProductionLiveDto> ProductionLiveDtos { get; set; }
        public bool AutomaticMachine { get; set; }
    }
}

public class ProductionLiveDto
{
    public int OperationTimelineId { get; set; }
    public string Client { get; set; }
    public string Operation { get; set; }
    public int Time { get; set; }
    public int SubBatchId { get; set; }
    public int MachineId { get; set; }
}