using System;
using System.ComponentModel.DataAnnotations;

namespace RemaSoftware.Domain.Models
{
    public class OperationTimeline
    {
        public int OperationTimelineID { get; set; }
        public int BatchOperationID { get; set; }
        public BatchOperations BatchOperations { get; set; }
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}", ApplyFormatInEditMode = true)]
        public DateTime StartDate { get; set; }

        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}", ApplyFormatInEditMode = true)]
        public DateTime EndDate { get; set; }
        [MaxLength(1)]
        public string Status { get; set; }
    }
}
