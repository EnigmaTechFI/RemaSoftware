using System.Collections.Generic;
using RemaSoftware.Domain.Models;

namespace RemaSoftware.WebApp.Models.OrderViewModel;

public class QualityControlViewModel
{
    public int subBatchId { get; set; }
    public List<OperationTimeline> OperationTimeLine { get; set; }
}