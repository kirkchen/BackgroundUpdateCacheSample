using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BackgroundUpdateCache.Website.Models
{
    [Serializable]
    public class CacheDto<T>
    {
        public T Data { get; set; }

        public DateTime UpdateTime { get; set; }
    }

    [Serializable]
    public class CacheDto: CacheDto<object>
    {
    }
}