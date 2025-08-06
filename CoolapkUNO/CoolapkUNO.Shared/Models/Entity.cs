using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace CoolapkUNO.Models
{
    public class Entity
    {
        [JsonPropertyName("entityId")]
        public int EntityID { get; init; }
        [JsonPropertyName("entityFixed")]
        public int EntityFixed { get; init; }
        [JsonPropertyName("entityType")]
        public string EntityType { get; init; }
        [JsonPropertyName("entityForward")]
        public string EntityForward { get; init; }

        public IEnumerable<Entity> AsEnumerable()
        {
            yield return this;
        }

        public override string ToString() => $"{GetType().Name}: {string.Join(" - ", EntityType, EntityID)}";
    }

    public class NullEntity : Entity;

    public class DataContainer<T>
    {
        [JsonPropertyName("data")]
        public T Data { get; init; }
        [JsonPropertyName("message")]
        public string Message { get; init; }
    }

    public class WebDataContainer<T>
    {
        [JsonPropertyName("title")]
        public string Title { get; init; }
        [JsonPropertyName("dataRow")]
        public T DataRow { get; init; }
    }
}
