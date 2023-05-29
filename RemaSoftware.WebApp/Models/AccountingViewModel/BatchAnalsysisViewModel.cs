using System;
using System.Collections.Generic;
using RemaSoftware.Domain.Models;

namespace RemaSoftware.WebApp.Models.AccountingViewModel;

public class BatchAnalsysisViewModel
{
    public Batch Batch { get; set; }
    public List<AnalysisOperationChart> AnalysisOperationCharts { get; set; }
    public List<OperationChartSubBatch> AvgTime { get; set; }
}

public class OperationChartSubBatch
{
    public int Id { get; set; }
    public int Time { get; set; }
    public int AvgTime => Time / Pieces;
    public string Date { get; set; }
    public int Pieces { get; set; }
}

public class AnalysisOperationChart
{
    public int Id { get; set; }
    public string OperationName { get; set; }
    public int TotTime { get; set; }
    public int AvgTime => TotTime / Piece;
    public DateTime Date { get; set; }
    public string DateSubBatch { get; set; }
    public int Piece { get; set; }
}