using CoolapkUNO.Models;
using Refit;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace CoolapkUNO.Networks
{
    public partial interface ICoolapkAPI
    {
        [Get("/v6/main/init")]
        Task<CollectionResp<Entity>> GetMainInit();
    }
}
