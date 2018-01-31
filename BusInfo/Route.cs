namespace BusInfo
{
    public class Route
    {
        public Route(string agencyId, string color, string description, string id, string longName, string shortName, string textColor, int type, string url)
        {
            AgencyId = agencyId;
            Color = color;
            Description = description;
            Id = id;
            LongName = longName;
            ShortName = shortName;
            TextColor = textColor;
            Type = type;
            Url = url;
        }

        public string AgencyId { get; set; }
        public string Color { get; set; }
        public string Description { get; set; }
        public string Id { get; set; }
        public string LongName { get; set; }
        public string ShortName { get; set; }
        public string TextColor { get; set; }
        public int Type { get; set; }
        public string Url { get; set; }
    }
}
