using System;
using System.Collections.Generic;
using System.Text;

namespace Backend.Models.NativeMenuModel.Data
{
    public class NativeItem
    {
        public string Name { get; set; }
        public string EventName { get; set; }
        public object[] Arguments { get; set; }
        public NativeItem(string name, string eventname, object[] arguments)
        {
            this.Name = name;
            this.EventName = eventname;
            this.Arguments = arguments;
        }
    }
}
