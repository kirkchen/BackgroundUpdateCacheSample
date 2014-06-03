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
using HangFire;
using BackgroundUpdateCache.Website.BackgrounJobs;

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

                //// 完全無資料時，直接產生
                var result = cache.Get<CacheDto>(key);
                if (result == null)
                {
                    invocation.Proceed();
                    
                    var dataToCache = new CacheDto{
                        Data = invocation.ReturnValue,
                        UpdateTime = DateTime.Now
                    };

                    cache.Set(key, dataToCache, TimeSpan.FromHours(1));

                    return;
                }

                //// 快取過期時，產生一個背景更新的工作
                //// 因背景後續才執行，因此此處可以馬上回應
                if (result.UpdateTime + TimeSpan.FromSeconds(expireTime) < DateTime.Now)
                {
                    BackgroundJob.Enqueue<RenewCacheAdapter>(
                        i => i.RenewCache(key,
                                          expireTime,
                                          invocation.TargetType.FullName,
                                          invocation.MethodInvocationTarget.Name,
                                          JsonConvert.SerializeObject(invocation.Arguments)));
                }

                //// 直接回傳舊資料
                invocation.ReturnValue = result.Data;
            }
        }
    }
}
