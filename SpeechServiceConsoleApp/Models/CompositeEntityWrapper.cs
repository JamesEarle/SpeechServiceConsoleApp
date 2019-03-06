using System;
using System.Collections.Generic;
using System.Text;

namespace SpeechServiceConsoleApp.Models
{
    class CompositeEntityWrapper
    {
        public string ParentType { get; set; }
        public string Value { get; set; }
        public CompositeEntityChild[] Children { get; set; }
        public int EndIndex { get; set; }
        public float Score { get; set; }
    }
}
