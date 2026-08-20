using System.Collections.Generic;

namespace PoliceDog.Domain
{
    public static class ScentFieldQuery
    {
        public static double IntensityAt(ScentSource source, Point3 nosePosition, NoseHeight noseHeight)
        {
            var distance = source.Position.DistanceTo(nosePosition);
            if (distance >= source.Range) return 0;

            var heightFactor = source.Height == noseHeight ? 1.0 : 0.35;
            var distanceFactor = 1.0 - distance / source.Range;
            var freshnessFactor = source.Kind == ScentKind.Residual
                ? source.Freshness * source.Freshness
                : source.Freshness;
            return Clamp01(distanceFactor * freshnessFactor * heightFactor);
        }

        public static IReadOnlyList<ScentObservation> Observe(
            IEnumerable<ScentSource> sources,
            Point3 nosePosition,
            NoseHeight noseHeight,
            double minimumIntensity = 0.03)
        {
            var observations = new List<ScentObservation>();
            foreach (var source in sources)
            {
                var intensity = IntensityAt(source, nosePosition, noseHeight);
                if (intensity >= minimumIntensity)
                {
                    observations.Add(new ScentObservation(source, intensity));
                }
            }
            observations.Sort((left, right) => right.Intensity.CompareTo(left.Intensity));
            return observations;
        }

        private static double Clamp01(double value)
        {
            if (value < 0) return 0;
            return value > 1 ? 1 : value;
        }
    }
}
