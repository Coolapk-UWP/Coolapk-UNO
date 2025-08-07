using System.Text.Json.Serialization;

namespace CoolapkUNO.Models.Feeds
{
    public class SourceFeedModel : Entity
    {
        [JsonPropertyName("id")]
        public int ID { get; init; }

        [JsonPropertyName("url")]
        public string Url { get; init; }
        [JsonPropertyName("message")]
        public string Message { get; init; }
        [JsonPropertyName("shareUrl")]
        public string ShareUrl { get; init; }
        [JsonPropertyName("message_title")]
        public string MessageTitle { get; init; }
        [JsonPropertyName("feedType")]
        public string FeedType { get; init; } = "feed";
    }
}
