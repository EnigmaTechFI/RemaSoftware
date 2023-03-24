using System.Collections.Generic;
using RemaSoftware.Domain.Models;

namespace RemaSoftware.WebApp.Models.AccountingViewModel;

public class BatchAnalsysisViewModel
{
    public Batch Batch { get; set; }
    public List<OperationChart> OperationCharts { get; set; }
    public List<OperationChartSubBatch> AvgTime { get; set; }
}

public class OperationChart
{
    
    public string Operation { get; set; }
    public List<OperationChartSubBatch> OperationChartSubBatch { get; set; }
}

public class OperationChartSubBatch
{
    public int Id { get; set; }
    public int Time { get; set; }
}