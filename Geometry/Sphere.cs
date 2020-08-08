using System;

namespace Geometry
{
    class Sphere : Solid
    {
        public double Radius { get; set; }

        public double Diameter { get { return Radius * 2; } }

        public override double Volume { get { return (4/3) * Math.PI * Math.Pow(Radius, 3); } }

        public Sphere(double radius)
        {
            Radius = radius;
        }
    }
}
