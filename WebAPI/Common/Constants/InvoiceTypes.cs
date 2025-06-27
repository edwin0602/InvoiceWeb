namespace WebAPI.Common.Constants
{
    public static class InvoiceTypes
    {
        public const string Draft = "Draft";
        public const string Quotation = "Quotation";
        public const string Final = "Final";

        public static readonly string[] All = new[] { Draft, Quotation, Final };
    }
}
