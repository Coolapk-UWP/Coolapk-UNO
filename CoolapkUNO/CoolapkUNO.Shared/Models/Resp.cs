using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace CoolapkUNO.Models
{
    public class RespBase<T>
    {
        public int Status { get; set; }
        public string Error { get; set; }
        public string Message { get; set; }
        public string ForwardUrl { get; set; }
        public virtual T Data { get; set; }
    }

    public class Resp<T> : RespBase<T> { }

    public class CollectionResp<T> : RespBase<IList<T>> { }
}
