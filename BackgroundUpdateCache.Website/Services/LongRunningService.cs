using BackgroundUpdateCache.Website.Extensions;
using BackgroundUpdateCache.Website.Models;
using StackExchange.Redis;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Web;

namespace BackgroundUpdateCache.Website.Services
{
    public class LongRunningService : ILongRunningService
    {
        public ConnectionMultiplexer RedisConnection { get; set; }

        public LongRunningService(ConnectionMultiplexer connection)
        {
            this.RedisConnection = connection;
        }

        public CacheDto<IEnumerable<int>> GetData(int min, int max)
        {
            var cache = this.RedisConnection.GetDatabase();

            var key = string.Format("LongRunningService.GetData.{0}.{1}", min, max);
            var cachedData = cache.Get<CacheDto<IEnumerable<int>>>(key);
            if (cachedData == null)
            {
                Thread.Sleep(5000);

                var r = new Random();
                var result = new List<int>();

                for (int i = 0; i < 10; i++)
                {
                    result.Add(r.Next(min, max));
                }

                cachedData = new CacheDto<IEnumerable<int>>
                {
                    Data = result,
                    UpdateTime = DateTime.Now
                };

                cache.Set(key, cachedData, TimeSpan.FromSeconds(30));
            }

            return cachedData;
        }
    }
}