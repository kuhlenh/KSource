using Amazon.Lambda.Core;
//using BusInfo;

namespace EmeraldTransit_Seattle {
	public class Geometry
    {
        public Location location { get; set; }
        public string location_type { get; set; }
        public Viewport viewport { get; set; }
    }


}
