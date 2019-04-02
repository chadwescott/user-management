using System;

using Newtonsoft.Json;

namespace UserManager.Domain.Responses
{
    public class LoginResponse
    {
        [JsonProperty("email")]
        public string Email { get; set; }
    }
}
