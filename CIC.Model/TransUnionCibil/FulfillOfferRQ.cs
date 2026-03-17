using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace CIC.Model.TransUnionCibil
{
    public class FulfillOfferRQ
    {
        public Fulfillofferrequest? FulfillOfferRequest { get; set; }
    }

    public class Fulfillofferrequest
    {
        public Customerinfo? CustomerInfo { get; set; }
    }

    public class Customerinfo
    {
        public Name? Name { get; set; }
        public Identificationnumber? IdentificationNumber { get; set; }
        public Address? Address { get; set; }

        [JsonPropertyName("DateOfBirth")]
        [Required(ErrorMessage = "Date of birth is required.")]
        //[RegularExpression(@"^\d{8}$", ErrorMessage = "Date of birth must be in yyyyMMdd format.")]
        public string? DateOfBirth { get; set; }
        public Phonenumber? PhoneNumber { get; set; }

        [JsonPropertyName("Email")]
        [Required(ErrorMessage = "Email is required.")]
        [EmailAddress(ErrorMessage = "Invalid email format.")]
        public string? Email { get; set; }

        [JsonPropertyName("Gender")]
        [Required(ErrorMessage = "Gender is required.")]
        public string? Gender { get; set; }
    }

    public class Name
    {

        [JsonPropertyName("Forename")]
        [Required(ErrorMessage = "Forename is required.")]
        [StringLength(50, MinimumLength = 2, ErrorMessage = "Forename must be between 2 and 50 characters.")]
        [RegularExpression(@"^[a-zA-Z\s]+$", ErrorMessage = "Forename can contain only letters and spaces.")]
        public string? Forename { get; set; }

        [JsonPropertyName("Surname")]
        [Required(ErrorMessage = "Surname is required.")]
        [StringLength(50, MinimumLength = 2, ErrorMessage = "Surname must be between 2 and 50 characters.")]
        [RegularExpression(@"^[a-zA-Z\s]+$", ErrorMessage = "Surname can contain only letters and spaces.")]
        public string? Surname { get; set; }
    }

    public class Identificationnumber
    {
        [JsonPropertyName("IdentifierName")]
        public string? IdentifierName { get; set; }

        [JsonPropertyName("Id")]
        [Required(ErrorMessage = "Id is required.")]
        public string? Id { get; set; }

    }

    public class Address
    {
        [JsonPropertyName("PostalCode")]
        [Required(ErrorMessage = "Postal code is required.")]
        [RegularExpression(@"^\d{6}$", ErrorMessage = "Postal code must be a 6-digit number.")]
        public string? PostalCode { get; set; }

        [JsonPropertyName("City")]
        [Required(ErrorMessage = "City is required.")]
        [StringLength(100, MinimumLength = 2, ErrorMessage = "City must be between 2 and 100 characters.")]
        public string? City { get; set; }

        [JsonPropertyName("Region")]
        [Required(ErrorMessage = "Region/State is required.")]
        [StringLength(100, MinimumLength = 2, ErrorMessage = "Region must be between 2 and 100 characters.")]
        public string? Region { get; set; }

        [JsonPropertyName("StreetAddress")]
        [Required(ErrorMessage = "Street address is required.")]
        [StringLength(250, MinimumLength = 5, ErrorMessage = "Street address must be between 5 and 250 characters.")]
        public string? StreetAddress { get; set; }

        public string? AddressType { get; set; }
        public string? AddressCategoryType { get; set; }
    }

    public class Phonenumber
    {
        [JsonPropertyName("Number")]
        [Required(ErrorMessage = "Mobile number is required.")]
        [RegularExpression(@"^[6-9]\d{9}$", ErrorMessage = "Invalid mobile number. It must be a 10-digit Indian mobile number.")]
        public string? Number { get; set; }

    }

}
