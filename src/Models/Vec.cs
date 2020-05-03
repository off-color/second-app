using System;


namespace covidSim.Models
{
    public class Vec
    {
        public Vec(int x, int y)
        {
            X = x;
            Y = y;
        }

        public readonly int X;
        public readonly int Y;

        public double DistanceToOther(Vec other)
        {
            var dx = X - other.X;
            var dy = Y - other.Y;
            return Math.Sqrt(dx * dx + dy * dy);
        }
    }
}