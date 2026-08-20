namespace PoliceDog.Domain
{
    public readonly struct Point3
    {
        public readonly double X;
        public readonly double Y;
        public readonly double Z;

        public Point3(double x, double y, double z)
        {
            X = x;
            Y = y;
            Z = z;
        }

        public double DistanceTo(Point3 other)
        {
            var dx = X - other.X;
            var dy = Y - other.Y;
            var dz = Z - other.Z;
            return System.Math.Sqrt(dx * dx + dy * dy + dz * dz);
        }
    }
}
