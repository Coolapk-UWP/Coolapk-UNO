using CoolapkUNO.Common;
using CoolapkUNO.Helpers;
using CoolapkUNO.Models;
using CoolapkUNO.ViewModels.DataSource;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;

namespace CoolapkUNO.ViewModels.Providers
{
    public class CoolapkListProvider
    {
        private readonly string _idName;
        private string _firstItem, _lastItem;
        private readonly Func<int, string, string, Uri> _getUri;

        public Uri Uri => _getUri(1, string.Empty, string.Empty);
        public Func<JsonElement, IEnumerable<Entity>> GetEntities { get; }

        public CoolapkListProvider(Func<int, string, string, Uri> getUri, Func<JsonElement, IEnumerable<Entity>> getEntities, string idName)
        {
            _getUri = getUri ?? throw new ArgumentNullException(nameof(getUri));
            GetEntities = getEntities ?? throw new ArgumentNullException(nameof(getEntities));
            _idName = string.IsNullOrEmpty(idName) ? throw new ArgumentException($"{nameof(idName)}不能为空") : idName;
        }

        public void Clear() => _lastItem = _firstItem = string.Empty;

        public async Task GetEntityAsync<T>(ICollection<T> models, int p = 1) where T : Entity
        {
            if (p == 1) { Clear(); }
            (bool isSucceed, JsonElement result) result = await RequestHelper.GetDataAsync(_getUri(p, _firstItem, _lastItem), false).ConfigureAwait(false);
            if (result.isSucceed && result.result.ValueKind == JsonValueKind.Array)
            {
                int length = result.result.GetArrayLength();
                for (int i = 0; i < length; i++)
                {
                    JsonElement item = result.result[i];
                    if (i == 0 && string.IsNullOrEmpty(_firstItem))
                    { _firstItem = GetEntityID(item, _idName); }
                    else if (i == length - 1)
                    { _lastItem = GetEntityID(item, _idName); }
                    IEnumerable<Entity> entities = GetEntities(item);
                    if (entities == null) { continue; }
                    models.AddRange(entities.OfType<T>());
                }
            }
        }

        public async Task<IEnumerable<T>> GetEntityAsync<T>(IEnumerable<T> models, int p = 1) where T : Entity
        {
            if (p == 1) { Clear(); }
            (bool isSucceed, JsonElement result) result = await RequestHelper.GetDataAsync(_getUri(p, _firstItem, _lastItem), false).ConfigureAwait(false);
            if (result.isSucceed && result.result.ValueKind == JsonValueKind.Array)
            {
                int length = result.result.GetArrayLength();
                for (int i = 0; i < length; i++)
                {
                    JsonElement item = result.result[i];
                    if (i == 0 && string.IsNullOrEmpty(_firstItem))
                    { _firstItem = GetEntityID(item, _idName); }
                    else if (i == length - 1)
                    { _lastItem = GetEntityID(item, _idName); }
                    IEnumerable<Entity> entities = GetEntities(item);
                    if (entities == null) { continue; }
                    models = models.Concat(entities.OfType<T>());
                }
            }
            return models;
        }

        public async Task<uint> GetEntityAsync<T>(IncrementalLoadingBase<T> models, int p = 1) where T : Entity
        {
            if (p == 1) { Clear(); }
            (bool isSucceed, JsonElement result) result = await RequestHelper.GetDataAsync(_getUri(p, _firstItem, _lastItem), false).ConfigureAwait(false);
            if (result.isSucceed)
            {
                uint count = 0;
                int length = result.result.GetArrayLength();
                for (int i = 0; i < length; i++)
                {
                    JsonElement item = result.result[i];
                    if (i == 0 && string.IsNullOrEmpty(_firstItem))
                    { _firstItem = GetEntityID(item, _idName); }
                    else if (i == length - 1)
                    { _lastItem = GetEntityID(item, _idName); }
                    IEnumerable<Entity> entities = GetEntities(item);
                    if (entities == null) { continue; }
                    foreach (T entity in entities.OfType<T>())
                    {
                        if (await models.AddItemAsync(entity))
                        {
                            count++;
                        }
                    }
                }
                return count;
            }
            return 0;
        }

        private static string GetEntityID(in JsonElement element, string _idName)
        {
            return element.ValueKind != JsonValueKind.Object
                ? string.Empty
                : element.TryGetProperty(_idName, out JsonElement idName)
                    ? GetString(idName)
                    : element.TryGetProperty("entityId", out JsonElement entityId)
                        ? GetString(entityId)
                        : element.TryGetProperty("id", out JsonElement id)
                            ? GetString(id)
                            : string.Empty;
            static string GetString(in JsonElement e) => e.ValueKind == JsonValueKind.String ? e.GetString() : e.ToString();
        }
    }
}
