using BackgroundUpdateCache.Website.Models;
using System;
using System.Collections.Generic;
namespace BackgroundUpdateCache.Website.Services
{
    public interface ILongRunningService
    {
        DataCollection GetData();
    }
}
