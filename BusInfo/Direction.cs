using System;

namespace BusInfo
{
    public class Direction
    {
        public string Vector { get; }

        public Direction(string v)
        {
            Vector = v ?? throw new ArgumentNullException(nameof(v));
        }
    }
}
