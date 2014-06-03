﻿using Autofac;
using BackgroundUpdateCache.Website.Models;
using BackgroundUpdateCache.Website.Extensions;
using StackExchange.Redis;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Web;
using Newtonsoft.Json;

namespace BackgroundUpdateCache.Website.BackgrounJobs
{
    public class RenewCacheAdapter
    {
        public ILifetimeScope LifetimeScope { get; set; }

        public ConnectionMultiplexer RedisConnection { get; set; }

        public RenewCacheAdapter(ILifetimeScope lifetimeScope, ConnectionMultiplexer connection)
        {
            this.LifetimeScope = lifetimeScope;
            this.RedisConnection = connection;
        }

        public void RenewCache(string key, int expireTime, string typeName, string methodName, string argumentString)
        {
            IDatabase cache = this.RedisConnection.GetDatabase();

            //// 若未過期，不執行更新
            var result = cache.Get<CacheDto>(key);
            if (result.UpdateTime + TimeSpan.FromSeconds(expireTime) > DateTime.Now)
            {
                return;
            }

            //// 取得資料來源Class
            var targetType = Type.GetType(typeName);
            var methodInfo = targetType.GetMethod(methodName);

            var target = this.LifetimeScope.Resolve(targetType);

            //// 將參數轉換回原本的Type (因Json.Net會預設反序列化為Int64，但此處為Int32)
            var arguments = JsonConvert.DeserializeObject<object[]>(argumentString);
            var parameterInfos = methodInfo.GetParameters();
            var changeTypesArguments = new List<object>();
            for (int i = 0; i < parameterInfos.Length; i++)
            {
                var argument = arguments[i];
                var parameterInfo = parameterInfos[i];

                var changedArgument = Convert.ChangeType(argument, parameterInfo.ParameterType);

                changeTypesArguments.Add(changedArgument);
            }

            //// 執行方法，取得更新後的資料
            var returnValue = methodInfo.Invoke(target, changeTypesArguments.ToArray());

            result = new CacheDto
            {
                Data = returnValue,
                UpdateTime = DateTime.Now
            };

            //// 最長Cache時間，一小時 (避免Hangfire過於忙碌時，資料都沒有更新)
            cache.Set(key, result, TimeSpan.FromHours(1));
        }
    }
}
