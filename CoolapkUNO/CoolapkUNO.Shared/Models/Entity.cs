using CoolapkUNO.Networks.Converters;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CoolapkUNO.Models
{
    public class Entity
    {
        [JsonExtensionData]
        public IDictionary<string, JToken> OtherField { get; set; }
        [JsonProperty(PropertyName = "entityId")]
        [JsonConverter(typeof(StringToIntConverter))]
        public uint EntityID { get; set; }
        [JsonConverter(typeof(IntToBoolConverter))]
        public bool EntityFixed { get; set; }
        public string EntityType { get; set; }
        public string EntityTemplate { get; set; }
        virtual public IList<Entity> Entities { get; set; } = new List<Entity>();

        public T Cast<T>() where T : Entity
        {
            // 根据目标类型重新解析实体，OtherField包含了Entity未解析的字段
            var _s = JsonConvert.SerializeObject(this);
            var entity = JsonConvert.DeserializeObject<T>(_s);
            // 遍历Entities然后转换类型 重新解析
            if (entity.Entities != null && entity.Entities.Count > 0)
            {
                var temp = new Entity[entity.Entities.Count()];
                entity.Entities.CopyTo(temp, 0);
                entity.Entities.Clear();
                foreach (var child in temp.OfType<Entity>())
                {
                    entity.Entities.Add(child.AutoCast() as Entity);
                }
            }
            return entity;
        }

        public void Cast<T>(out T entity) where T : Entity => entity = Cast<T>();

        public Entity AutoCast()
        {
            switch (EntityType)
            {
                case "card":
                    switch (EntityTemplate)
                    {
                        case "configCard":
                            return Cast<ConfigCard>();
                    }
                    break;
            }
            return Cast<DefaultCard>();
        }
    }

    public class ConfigCard : Entity
    {
        public string Title { get; set; }
        public string Url { get; set; }
        public string Pic { get; set; }
        [JsonConverter(typeof(StringToIntConverter))]
        public uint Lastupdate { get; set; }
    }

    public class DefaultCard : Entity
    {
    }
}
