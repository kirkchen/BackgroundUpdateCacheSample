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
        public DataCollection GetData()
        {
            Thread.Sleep(5000);

            var r = new Random();
            var result = new List<int>();

            for (int i = 0; i < 10; i++)
            {
                result.Add(r.Next(0, 100));
            }

            return new DataCollection
            {
                Datas = result,
                UpdateTime = DateTime.Now
            };
        }
    }
}