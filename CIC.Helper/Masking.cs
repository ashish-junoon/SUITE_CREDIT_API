namespace CIC.Helper
{
    public static class Masking
    {
        private static string Mask(string value)
        {
            if (string.IsNullOrEmpty(value) || value.Length < 4) return "****";
            return value[..2] + "******" + value[^2..];
        }
    }
}
