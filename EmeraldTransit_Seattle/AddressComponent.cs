using System.Collections.Generic;
using Amazon.Lambda.Core;
//using BusInfo;

namespace EmeraldTransit_Seattle {
	public class AddressComponent
    {
        public string long_name { get; set; }
        public string short_name { get; set; }
        public List<string> types { get; set; }
    }


}
