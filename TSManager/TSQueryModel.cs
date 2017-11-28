using System;
using System.Collections.Generic;
using System.Text;
using Newtonsoft.Json;

namespace TSManager
{
    public enum TimeStampUnit
    {
        ms,
        s,
        mi,
        h,
        d,
        w,
        mm,
        y
    }
    public class Tag
    {
        
        public string Name { get; set; }
        public int limit { get; set; }
        public object Aggregations { get; set; }
        public object Filters { get; set; }
        public object Group { get; set; }
        public string Start { get; set; }
        public string End { get; set; }
    }

    public class TSQueryModel
    {
        public string Start { get; set; }
        public  string End { get; set; }
        public int limit { get; set; }
        public Tag Tags { get; set; }
        
    }
}
