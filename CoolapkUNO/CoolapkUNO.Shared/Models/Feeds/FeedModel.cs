﻿using Newtonsoft.Json.Linq;

namespace CoolapkUNO.Models.Feeds
{
    internal enum FeedDisplayMode
    {
        Normal = 0,
        NotShowDyhName = 0x02,
        IsFirstPageFeed = 0x01,
        NotShowMessageTitle = 0x04
    }

    internal class FeedModel : FeedModelBase
    {
        public bool IsStickTop { get; private set; }
        public bool ShowLikes { get; private set; } = true;
        public bool ShowDateline { get; private set; } = true;

        public FeedModel(JObject token, FeedDisplayMode mode = FeedDisplayMode.Normal) : base(token)
        {
            ShowLikes = !(EntityType == "forwardFeed");
            IsStickTop = token.TryGetValue("isStickTop", out JToken j) && int.Parse(j.ToString()) == 1;
        }
    }
}
