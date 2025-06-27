namespace WebAPI.Common.Constants
{
    public static class GeneralStatuses
    {
        public const string Active = "Activo";
        public const string Deleted = "Eliminado";

        public static readonly string[] All = new[]
        {
            Active,
            Deleted
        };
    }
}