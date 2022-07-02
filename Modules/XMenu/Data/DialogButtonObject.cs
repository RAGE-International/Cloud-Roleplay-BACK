using System;
using System.Collections.Generic;
using System.Text;

namespace Backend.Modules.XMenu.Data
{
    public class DialogButtonObject
    {
        public string name { get; set; }
        public string eventname { get; set; }
        public object[] arguments { get; set; }
    }
}
