using System;
using System.ComponentModel.DataAnnotations;

namespace RemaSoftware.Domain.Models
{
    public class OperationTimeline
    {
        public int OperationTimelineID { get; set; }
        public int ProductOperationID { get; set; }
        public ProductOperation ProductOperation { get; set; }
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
