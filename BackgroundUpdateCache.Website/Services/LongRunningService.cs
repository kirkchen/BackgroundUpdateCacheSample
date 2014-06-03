using BackgroundUpdateCache.Website.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Web;

namespace BackgroundUpdateCache.Website.Services
{
    public class LongRunningService : ILongRunningService
    {
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