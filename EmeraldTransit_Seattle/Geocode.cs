using System.Collections.Generic;
using Amazon.Lambda.Core;
//using BusInfo;

namespace EmeraldTransit_Seattle {
	public class Geocode
    {
        public List<Result> results { get; set; }
        public string status { get; set; }
    }


}
