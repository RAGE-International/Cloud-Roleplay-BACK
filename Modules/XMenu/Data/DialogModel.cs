using System;
using System.Collections.Generic;
using System.Text;

namespace Backend.Modules.XMenu.Data
{
    public class DialogModel
    {
        public string title { get; set; }
        public string discription { get; set; }
        public List<DialogButtonObject> buttons { get; set; }
    }
}
