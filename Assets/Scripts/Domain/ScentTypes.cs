namespace PoliceDog.Domain
{
    public enum ScentKind
    {
        Target,
        Distractor,
        Residual
    }

    public enum NoseHeight
    {
        Ground,
        Air
    }

    public sealed class ScentSource
    {
        public string Id { get; }
        public string Label { get; }
        public ScentKind Kind { get; }
        public Point3 Position { get; }
        public NoseHeight Height { get; }
        public double Freshness { get; }
        public double Range { get; }

        public ScentSource(
            string id,
            string label,
            ScentKind kind,
            Point3 position,
            NoseHeight height,
            double freshness,
            double range)
        {
            Id = id;
            Label = label;
            Kind = kind;
            Position = position;
            Height = height;
            Freshness = Clamp01(freshness);
            Range = range < 0.1 ? 0.1 : range;
        }

        private static double Clamp01(double value)
        {
            if (value < 0) return 0;
            return value > 1 ? 1 : value;
        }
    }

    public readonly struct ScentObservation
    {
        public readonly ScentSource Source;
        public readonly double Intensity;

        public ScentObservation(ScentSource source, double intensity)
        {
            Source = source;
            Intensity = intensity;
        }
    }
}
