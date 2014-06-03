﻿using BackgroundUpdateCache.Website.Attributes;
using Castle.DynamicProxy;
using StackExchange.Redis;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using BackgroundUpdateCache.Website.Extensions;
using BackgroundUpdateCache.Website.Models;
using Newtonsoft.Json;

namespace BackgroundUpdateCache.Website.Interceptors
{
    public class CacheInterceptor : IInterceptor
    {
        public ConnectionMultiplexer RedisConnection { get; set; }

        public CacheInterceptor(ConnectionMultiplexer connection)
        {
            this.RedisConnection = connection;
        }

        public void Intercept(IInvocation invocation)
        {
            //// 判斷目前方法是否有需要啟用Cache，若有加上標記表示需要
            var attributes = invocation.MethodInvocationTarget.GetCustomAttributes(typeof(CacheAttribute), true);
            if (attributes.Count() > 0)
            {
                //// Cache key
                var key = string.Format("{0}.{1}.{2}", invocation.TargetType.FullName,
                                                       invocation.MethodInvocationTarget.Name,
                                                       JsonConvert.SerializeObject(invocation.Arguments));

                //// Expire Time
                var expireTime = (attributes.First() as CacheAttribute).ExpireTime;

                IDatabase cache = this.RedisConnection.GetDatabase();

                //// Is cache over time
                var result = cache.Get(key);
                if (result != null)
                {
                    invocation.ReturnValue = result;
                    return;
                }

                invocation.Proceed();                

                cache.Set(key, invocation.ReturnValue, TimeSpan.FromSeconds(expireTime));
            }
        }
    }
}
