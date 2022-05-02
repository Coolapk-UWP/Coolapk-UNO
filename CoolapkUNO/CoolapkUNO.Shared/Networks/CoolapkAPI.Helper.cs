using CoolapkUNO.Networks.Models;
using Refit;
using System;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json.Serialization;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;

namespace CoolapkUNO.Networks
{
    public static class CoolapkAPIHelper
    {
        public static ICoolapkAPI CoolapkAPI;

        static CoolapkAPIHelper()
        {
            InitCoolapkAPI();
        }

        public static void InitCoolapkAPI()
        {
            JsonConvert.DefaultSettings = () => new JsonSerializerSettings()
            {
                Converters = { new StringEnumConverter() }
            };

            var httpClient = new HttpClient(new TokenHeaderHandler(new CoolapkHeader { APIVersion = "12" }))
            {
                BaseAddress = new Uri("https://api.coolapk.com"),
            };

            CoolapkAPI = RestService.For<ICoolapkAPI>(httpClient,new RefitSettings
            {
                ContentSerializer = new NewtonsoftJsonContentSerializer(
                    new JsonSerializerSettings
                    {
                        ContractResolver = new CamelCasePropertyNamesContractResolver()
                    }),
            });
        }
    }
}
