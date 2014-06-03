using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BackgroundUpdateCache.Website.Models
{
    public class CacheDto<T>
    {
        public T Data { get; set; }

        public DateTime UpdateTime { get; set; }
    }

    public class CacheDto: CacheDto<object>
    {
    }
}