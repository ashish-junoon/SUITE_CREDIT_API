namespace CIC.Helper
{
    public static class CommonClass
    {
        public static string GetISTTimestamp()
        {
            var ist = TimeZoneInfo.FindSystemTimeZoneById("India Standard Time");
            var now = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, ist);

            return now.ToString("yyyy-MM-dd HH:mm:ss");
        }

        public static (string datetime, string random) GenerateUniqueRequestNo()
        {
            return (
                DateTime.Now.ToString("dd-MM-yyyy HH:mm:ss"),
                DateTime.Now.ToString("yyyyMMddHHmmssfff")
            );
        }
    }
}
