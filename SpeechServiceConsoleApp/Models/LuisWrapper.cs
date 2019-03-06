using System;
using System.Collections.Generic;
using System.Text;

namespace SpeechServiceConsoleApp.Models
{
    class LuisWrapper
    {
        public string Query { get; set; }
        public TopIntent TopScoringIntent { get; set; }
        public EntityWrapper[] Entities { get; set; }
        public CompositeEntityWrapper[] CompositeEntities { get; set; }
    }
}
