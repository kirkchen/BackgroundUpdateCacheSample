using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BackgroundUpdateCache.Website.Models
{
    public class DataCollection
    {
        public IEnumerable<int> Datas { get; set; }

        public DateTime UpdateTime { get; set; }
    }
}