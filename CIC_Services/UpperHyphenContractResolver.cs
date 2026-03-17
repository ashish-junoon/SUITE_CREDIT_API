using Newtonsoft.Json.Serialization;
using System.Text.RegularExpressions;

namespace CIC_Services
{
    public class UpperHyphenContractResolver : DefaultContractResolver
    {
        protected override string ResolvePropertyName(string propertyName)
        {
            // Convert PascalCase → Pascal-Case
            var hyphenName = Regex.Replace(propertyName, "([a-z])([A-Z])", "$1-$2");

            // Convert to uppercase
            return hyphenName.ToUpper();
        }
    }
}
