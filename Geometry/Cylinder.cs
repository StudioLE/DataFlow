using System;

namespace Geometry
{
    class Cylinder : Sphere
    {
        public double Height { get; set; }

        public override double Volume { get { return Math.PI * Radius * Height; } }

        public Cylinder(double radius, double height) : base(radius)
        {
            Radius = radius;
            Height = height;
        }
    }
}
