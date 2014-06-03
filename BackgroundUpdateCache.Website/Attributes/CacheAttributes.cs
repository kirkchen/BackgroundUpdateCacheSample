﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BackgroundUpdateCache.Website.Attributes
{
    [AttributeUsage(AttributeTargets.Method, Inherited = false, AllowMultiple = false)]
    sealed class CacheAttribute : Attribute
    {
        public CacheAttribute()
        {
            this.ExpireTime = 20 * 60;
        }

        public string CacheKey { get; set; }

        public int ExpireTime { get; set; }
    }
}
