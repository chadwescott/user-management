using Newtonsoft.Json;

namespace UserManager.Domain.Responses
{
    public class LinkResponse
    {
        [JsonProperty("href")]
        public string Href { get; set; }

        [JsonProperty("rel")]
        public string Rel { get; set; }

        [JsonProperty("method")]
        public string Method { get; set; }

        [JsonProperty("mediaType")]
        public string MediaType { get; set; }
    }
}
