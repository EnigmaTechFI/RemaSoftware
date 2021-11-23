using System.Collections.Generic;

namespace RemaSoftware.Constants
{
    public static class OrderStatusConstants
    {
        public static readonly char STATUS_ARRIVED = 'A';
        public static readonly string STATUS_ARRIVED_DESC = "Arrivato in magazzino";
        
        public static readonly char STATUS_WORKING = 'B';
        public static readonly string STATUS_WORKING_DESC = "In lavorazione";
        
        public static readonly char STATUS_COMPLETED = 'C';
        public static readonly string STATUS_COMPLETED_DESC = "Completato e uscito dal magazzino";
        
        public static readonly Dictionary<char, StatusDto> OrderStatuses = new Dictionary<char, StatusDto>
        {
            {
                STATUS_ARRIVED,
                new StatusDto
                {
                    Status = STATUS_ARRIVED,
                    StatusDescription = STATUS_ARRIVED_DESC,
                    StatusCssClass = "red"
                }
            },
            {
                STATUS_WORKING,
                new StatusDto
                {
                    Status = STATUS_WORKING,
                    StatusDescription = STATUS_WORKING_DESC,
                    StatusCssClass = "blue"
                }
            },
            {
                STATUS_COMPLETED,
                new StatusDto
                {
                    Status = STATUS_COMPLETED,
                    StatusDescription = STATUS_COMPLETED_DESC,
                    StatusCssClass = "green"
                }
            },
        };
    }

    public class StatusDto
    {
        public char Status { get; set; }
        public string StatusDescription { get; set; }
        public string StatusCssClass { get; set; }
    }
    
}