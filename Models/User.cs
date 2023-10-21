using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace CDNFreelancer.Models
{
    public class User
    {

        [JsonPropertyName("id")]
        public int Id { get; set; }

        [Required(ErrorMessage = "Username is required.")]
        [JsonPropertyName("username")]
        public string Username { get; set; }

        [Required(ErrorMessage = "Email is required.")]
        [EmailAddress(ErrorMessage = "Invalid email format.")]
        [JsonPropertyName("email")]
        public string Email { get; set; }

        [Required(ErrorMessage = "Phone Number is required.")]
        [JsonPropertyName("phoneNumber")]
        public string PhoneNumber { get; set; }

        [Required(ErrorMessage = "Skillsets are required.")]
        [JsonPropertyName("skillsets")]
        public string Skillsets { get; set; }

        [Required(ErrorMessage = "Hobby is required.")]
        [JsonPropertyName("hobby")]
        public string Hobby { get; set; }
    }
}
