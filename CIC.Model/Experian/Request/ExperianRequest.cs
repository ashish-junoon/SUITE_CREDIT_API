using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace CIC.Model.Experian.Request
{
   
    public class ExperianRequest
    {
        [JsonPropertyName("Name")]
        [Required(ErrorMessage = "Name is required.")]
        [StringLength(50, MinimumLength = 3, ErrorMessage = "Name must be between 3 and 50 characters.")]
        [RegularExpression(@"^[a-zA-Z\s]+$", ErrorMessage = "Name can contain only letters and spaces.")]
        public string? Name { get; set; }

        [JsonPropertyName("Pan")]
        [Required(ErrorMessage = "PAN is required.")]
        [RegularExpression(@"^[A-Z]{5}[0-9]{4}[A-Z]{1}$",
        ErrorMessage = "Invalid PAN format. Example: ABCDE1234F")]
        public string? Pan { get; set; }

        [JsonPropertyName("Mobile")]
        //[Required(ErrorMessage = "Mobile number is required.")]
        //[RegularExpression(@"^[6-9]\d{9}$",
        //ErrorMessage = "Invalid mobile number. It must be a 10-digit Indian mobile number.")]
        public string? Mobile { get; set; }
    
    }
}
