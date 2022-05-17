namespace RemaSoftware.Domain.ContextModels
{
    public class Order_Operation
    {
        public int OrderID { get; set; }

        public Order Orders { get; set; }

        public int OperationID {get; set;}

        public Operation Operations { get; set; }
        public int Ordering { get; set; }

    }
}
