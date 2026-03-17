using System.Text.RegularExpressions;

namespace CIC.Helper
{
    public static class ValidationHelper
    {
        public static void ValidateMobile(string mobile)
        {
            if (!Regex.IsMatch(mobile ?? "", "^[6-9]\\d{9}$"))
                throw new ApiException("Invalid mobile number", 400);
        }

        public static void ValidatePan(string pan)
        {
            if (!string.IsNullOrWhiteSpace(pan) &&
                !Regex.IsMatch(pan, "^[A-Z]{5}[0-9]{4}[A-Z]{1}$"))
                throw new ApiException("Invalid PAN number", 400);
        }
    }
}
