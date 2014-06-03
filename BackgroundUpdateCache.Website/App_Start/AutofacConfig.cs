using Autofac;
using Autofac.Integration.Mvc;
using Autofac.Extras.DynamicProxy2;
using BackgroundUpdateCache.Website.Interceptors;
using BackgroundUpdateCache.Website.Services;
using StackExchange.Redis;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace BackgroundUpdateCache.Website
{
    public class AutofacConfig
    {
        public static void Initialize()
        {
            var builder = new ContainerBuilder();

            builder.RegisterControllers(typeof(MvcApplication).Assembly);

            builder.RegisterType<LongRunningService>()
                   .As<ILongRunningService>()
                   .EnableInterfaceInterceptors();

            builder.RegisterType<CacheInterceptor>()
                   .AsSelf();

            builder.Register(i =>
            {
                var connect = ConnectionMultiplexer.Connect("localhost");

                return connect;
            }).AsSelf()
            .SingleInstance();


            var container = builder.Build();

            DependencyResolver.SetResolver(new AutofacDependencyResolver(container));
        }
    }
}