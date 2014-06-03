using Autofac.Extras.DynamicProxy2;
using BackgroundUpdateCache.Website.Attributes;
using BackgroundUpdateCache.Website.Interceptors;
using BackgroundUpdateCache.Website.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Web;

namespace BackgroundUpdateCache.Website.Services
{
    [Intercept(typeof(CacheInterceptor))]
    public class LongRunningService : ILongRunningService
    {
        [Cache(ExpireTime = 30)]
        public CacheDto<IEnumerable<int>> GetData(int min, int max)
        {
            Thread.Sleep(5000);

            var r = new Random();
            var result = new List<int>();

            for (int i = 0; i < 10; i++)
            {
                result.Add(r.Next(min, max));
            }

            return new CacheDto<IEnumerable<int>>
            {
                Data = result,
                UpdateTime = DateTime.Now
            };
        }
    }
}