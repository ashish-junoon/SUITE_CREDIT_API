using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace CIC.Model.Criff.Request
{
    public class SoftPullRQ
    {
        [JsonPropertyName("last_name")]
        [Required(ErrorMessage = "Last name is required.")]
        [RegularExpression(@"^[a-zA-Z\s]+$", ErrorMessage = "Last name can contain only letters.")]
        public required string last_name { get; set; }
        [JsonPropertyName("first_name")]
        [Required(ErrorMessage = "First name is required.")]
        [RegularExpression(@"^[a-zA-Z\s]+$", ErrorMessage = "First name can contain only letters.")]
        public required string first_name { get; set; }

        [JsonPropertyName("uid_number")]
        [Required]
        [StringLength(12, MinimumLength = 10, ErrorMessage = "PAN must be 10 Char and AADHAAR accept 12 characters only.")]

        [RegularExpression("^[a-zA-Z0-9]*$",ErrorMessage = "Only alphanumeric characters are allowed.")]
        public required string uid_number { get; set; }
        [JsonPropertyName("mobile_number")]
        [Required]
        [RegularExpression(@"^[6-9]\d{9}$", ErrorMessage = "Enter valid 10-digit mobile number")]
        public required string mobile_number { get; set; }
    }
    public class SoftPullRQV1
    {

        [JsonPropertyName("last_name")]
        [Required(ErrorMessage = "Last name is required.")]
        [RegularExpression(@"^[a-zA-Z\s]+$", ErrorMessage = "Last name can contain only letters.")]
        public required string last_name { get; set; }

        [JsonPropertyName("first_name")]
        [Required(ErrorMessage = "First name is required.")]
        [RegularExpression(@"^[a-zA-Z\s]+$", ErrorMessage = "First name can contain only letters.")]
        public required string first_name { get; set; }

        [JsonPropertyName("aadhaar_number")]
        [Required]
        [StringLength(12, MinimumLength = 12, ErrorMessage = "AADHAAR must be 12 Char.")]

        [RegularExpression("^[a-zA-Z0-9]*$", ErrorMessage = "Only alphanumeric characters are allowed.")]
        public required string aadhaar_number { get; set; }

        [JsonPropertyName("pan_number")]
        [Required]
        [StringLength(10, MinimumLength = 10, ErrorMessage = "PAN must be 10 Char only.")]

        [RegularExpression("^[a-zA-Z0-9]*$", ErrorMessage = "Only alphanumeric characters are allowed.")]
        public required string pan_number { get; set; }
        [JsonPropertyName("mobile_number")]
        [Required]
        [RegularExpression(@"^[6-9]\d{9}$", ErrorMessage = "Enter valid 10-digit mobile number")]
        public required string mobile_number { get; set; }
    }
    public class  AuthRQ
    {
        [JsonPropertyName("transaction_id")]
        [Required(ErrorMessage = "Transaction ID is required.")]
        public required string transaction_id { get; set; }

        [JsonPropertyName("orderid")]
        [Required(ErrorMessage = "Order ID is required.")]
        public required string orderid { get; set; }

        [JsonPropertyName("reportId")]
        [Required(ErrorMessage = "Report ID is required.")]
        public required string reportId { get; set; }

        [JsonPropertyName("amsware")]
        [Required(ErrorMessage = "Amsware value is required.")]
        public required string amsware { get; set; }

    }


    public class CrifPrefillRQ
    {
        [JsonPropertyName("customerName")]
        [Required(ErrorMessage = "customerName is required.")]
        public required string customerName { get; set; }

        [JsonPropertyName("mobile")]
        [StringLength(10, MinimumLength = 10, ErrorMessage = "Mobile number must be 10 Char only.")]
        [Required(ErrorMessage = "mobile value is required.")]
        public required string mobile { get; set; }

    }
}
