using System;
using Amazon.Lambda.Core;
//using BusInfo;

namespace EmeraldTransit_Seattle {
	public class BusInfoResource
    {
        public string Language { get; set; }
        public string SkillName { get; set; }
        public string HelpMessage { get; set; }
        public string HelpReprompt { get; set; }
        public string StopMessage { get; set; }

        public BusInfoResource(string language)
        {
            Language = language ?? throw new ArgumentNullException(nameof(language));
        }
    }


}
