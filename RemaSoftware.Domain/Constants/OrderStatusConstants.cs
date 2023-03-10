namespace RemaSoftware.Domain.Constants
{
    public static class OrderStatusConstants
    {
        public const string STATUS_ARRIVED = "A";
        public static readonly string STATUS_ARRIVED_DESC = "In magazzino";
        
        public const string STATUS_WORKING = "B";
        public static readonly string STATUS_WORKING_DESC = "In lavorazione";
        
        public const string STATUS_COMPLETED = "C";
        public static readonly string STATUS_COMPLETED_DESC = "Completato";
        
        public const string STATUS_PARTIALLY_COMPLETED = "D";
        public static readonly string STATUS_PARTIALLY_COMPLETED_DESC = "Completato parzialmente";

        public const string STATUS_DELIVERED = "E";
        public static readonly string STATUS_DELIVERED_DESC = "Consegnato";

        public static readonly Dictionary<string, StatusDto> OrderStatuses = new Dictionary<string, StatusDto>
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
                    StatusCssClass = "orange"
                }
            },
            {
                STATUS_PARTIALLY_COMPLETED,
                new StatusDto
                {
                    Status = STATUS_PARTIALLY_COMPLETED,
                    StatusDescription = STATUS_PARTIALLY_COMPLETED_DESC,
                    StatusCssClass = "orange"
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
            {
                STATUS_DELIVERED,
                new StatusDto
                {
                    Status = STATUS_DELIVERED,
                    StatusDescription = STATUS_DELIVERED_DESC,
                    StatusCssClass = "green"
                }
            }
        };

        public static StatusDto GetNewOrderStatus(string currentStatus)
        {
            switch (currentStatus)
            {
                case STATUS_ARRIVED:
                    return OrderStatuses[STATUS_WORKING];
                case STATUS_WORKING:
                    return  OrderStatuses[STATUS_COMPLETED];
                default:
                    throw new Exception("Passaggio di status non previsto.");
                
            }
        }
    }

    public class StatusDto
    {
        public string Status { get; set; }
        public string StatusDescription { get; set; }
        public string StatusCssClass { get; set; }
    }
    
}