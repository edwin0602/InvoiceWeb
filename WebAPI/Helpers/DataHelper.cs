namespace WebAPI.Helpers
{
    public class DataHelper
    {
        public static string GenerateConsecutive(string root = "FAC")
        {
            var datePart = DateTime.UtcNow.ToString("yyMMdd");
            var randomPart = Guid.NewGuid().ToString("N").Substring(0, 6).ToUpper();
            return $"{root}-{datePart}-{randomPart}";
        }
    }
}
