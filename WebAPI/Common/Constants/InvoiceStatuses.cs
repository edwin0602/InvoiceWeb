namespace WebAPI.Common.Constants
{
    public static class InvoiceStatuses
    {
        public const string Created = "Created";
        public const string Cancelled = "Cancelled";
        public const string Paid = "Paid";
        public const string Pending = "Pending";
        public const string PartiallyPaid = "PartiallyPaid";
        public const string Overdue = "Overdue";

        public static readonly string[] All = new[]
        {
            Created,
            Cancelled,
            Paid,
            Pending,
            PartiallyPaid,
            Overdue
        };
    }
}
