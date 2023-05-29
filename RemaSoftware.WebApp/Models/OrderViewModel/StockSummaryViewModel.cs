using System.Collections.Generic;
using RemaSoftware.Domain.Models;

namespace RemaSoftware.WebApp.Models.OrderViewModel;

public class StockSummaryViewModel
{
    public List<SubBatch> SubBatches { get; set; }
}