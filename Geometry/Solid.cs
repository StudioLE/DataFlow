using System;
using System.Drawing;

namespace Geometry
{
    class Solid
    {
        public Color Color { get; set; } = Color.Black;

        public double Density { get; set; } = 1;

        public virtual double Volume { get; set; }

        public double Mass { get { return Volume * Density; } }
    }
}
