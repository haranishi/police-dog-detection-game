using System.Linq;
using NUnit.Framework;
using PoliceDog.Domain;

namespace PoliceDog.Tests
{
    public sealed class ScentFieldQueryTests
    {
        [Test]
        public void IntensityFallsWithDistanceAndFreshness()
        {
            var fresh = Source("fresh", ScentKind.Target, NoseHeight.Ground, 1, 10);
            var old = Source("old", ScentKind.Residual, NoseHeight.Ground, 0.5, 10);

            Assert.That(ScentFieldQuery.IntensityAt(fresh, new Point3(0, 0, 0), NoseHeight.Ground), Is.EqualTo(1).Within(0.001));
            Assert.That(ScentFieldQuery.IntensityAt(fresh, new Point3(5, 0, 0), NoseHeight.Ground), Is.EqualTo(0.5).Within(0.001));
            Assert.That(ScentFieldQuery.IntensityAt(old, new Point3(0, 0, 0), NoseHeight.Ground), Is.EqualTo(0.25).Within(0.001));
        }

        [Test]
        public void NoseHeightChangesObservationWithoutHidingOtherLayerCompletely()
        {
            var source = Source("air", ScentKind.Distractor, NoseHeight.Air, 1, 10);
            var matching = ScentFieldQuery.IntensityAt(source, new Point3(0, 0, 0), NoseHeight.Air);
            var otherLayer = ScentFieldQuery.IntensityAt(source, new Point3(0, 0, 0), NoseHeight.Ground);

            Assert.That(matching, Is.EqualTo(1).Within(0.001));
            Assert.That(otherLayer, Is.EqualTo(0.35).Within(0.001));
        }

        [Test]
        public void ObservationsAreSortedStrongestFirst()
        {
            var sources = new[]
            {
                Source("far", ScentKind.Distractor, NoseHeight.Ground, 1, 10, 8),
                Source("near", ScentKind.Target, NoseHeight.Ground, 1, 10, 2)
            };

            var result = ScentFieldQuery.Observe(sources, new Point3(0, 0, 0), NoseHeight.Ground);
            Assert.That(result.Select(item => item.Source.Id), Is.EqualTo(new[] { "near", "far" }));
        }

        private static ScentSource Source(
            string id,
            ScentKind kind,
            NoseHeight height,
            double freshness,
            double range,
            double x = 0)
        {
            return new ScentSource(id, id, kind, new Point3(x, 0, 0), height, freshness, range);
        }
    }
}
