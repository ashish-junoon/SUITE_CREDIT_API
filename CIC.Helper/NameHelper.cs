using System.Security;

namespace CIC.Helper
{
    public static class NameHelper
    {
        public static (string FirstName, string LastName) SplitName(string fullName)
        {
            if (string.IsNullOrWhiteSpace(fullName))
                return ("", "");

            var parts = fullName.Trim()
                                .Split(' ', StringSplitOptions.RemoveEmptyEntries);

            var firstName = parts[0];
            var lastName = parts.Length > 1
                ? string.Join(" ", parts.Skip(1))
                : parts[0];

            // XML escape for safety
            return (SecurityElement.Escape(firstName),
                    SecurityElement.Escape(lastName));
        }
    }
}
