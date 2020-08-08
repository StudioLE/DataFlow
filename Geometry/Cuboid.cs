using System;

namespace Geometry
{
    class Cuboid : Solid
    {
        public double Width { get; set; }

        public double Height { get; set; }

        public double Length { get; set; }

        public override double Volume { get { return Width * Height * Length; } }

        public Cuboid(double width, double length, double height)
        {
            Width = width;
            Length = length;
            Height = height;
        }
    }
}
