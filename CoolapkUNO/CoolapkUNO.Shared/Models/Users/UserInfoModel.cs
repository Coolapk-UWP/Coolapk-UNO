using CoolapkUNO.Helpers;
using System.Text.Json.Serialization;

namespace CoolapkUNO.Models.Users
{
    public class UserInfoModel : Entity
    {
        [JsonPropertyName("uid")]
        public int UID { get; init; }
        [JsonPropertyName("level")]
        public int Level { get; init; }
        [JsonPropertyName("regdate")]
        public int RegDate { get; init; }
        [JsonPropertyName("logintime")]
        public int LoginTime { get; init; }
        [JsonPropertyName("block_status")]
        public int BlockStatus { get; init; }

        [JsonPropertyName("regip")]
        public string RegIP { get; init; }
        [JsonPropertyName("email")]
        public string Email { get; init; }
        [JsonPropertyName("mobile")]
        public string Mobile { get; init; }
        [JsonPropertyName("loginip")]
        public string LoginIP { get; init; }
        [JsonPropertyName("username")]
        public string UserName { get; init; }
        [JsonIgnore]
        public string LoginText => $"{DateTimeHelper.ConvertUnixTimeStampToReadable(LoginTime)}活跃";
        [JsonPropertyName("verify_title")]
        public string VerifyTitle { get; init; }

        [JsonPropertyName("cover")]
        public string Cover { get; init; }
        [JsonPropertyName("userAvatar")]
        public string UserAvatar { get; init; }
        [JsonPropertyName("next_level_percentage")]
        public string NextLevelPercentage { get; init; }

        [JsonPropertyName("experience")]
        public long Experience { get; init; }
        [JsonPropertyName("next_level_experience")]
        public long NextLevelExperience { get; init; }

        [JsonIgnore]
        public string Url => $"/u/{UID}";
    }
}
