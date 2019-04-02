using System.ComponentModel.DataAnnotations;

using Newtonsoft.Json;

namespace UserManager.Domain.Requests
{
    public class LoginRequest
    {
        [JsonProperty("email")]
        [MaxLength(255)]
        public string Email { get; set; }

        [JsonProperty("password")]
        [MaxLength(100)]
        public string Password { get; set; }
    }
}
