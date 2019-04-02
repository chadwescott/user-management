using Newtonsoft.Json;

namespace UserManager.Api.Config
{
    public class DatabaseSettings
    {
        [JsonProperty("connectionString")]
        public string ConnectionString { get; set; }
    }
}
