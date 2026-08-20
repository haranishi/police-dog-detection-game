using NUnit.Framework;
using PoliceDog.Domain;

namespace PoliceDog.Tests
{
    public sealed class AlertEvaluatorTests
    {
        [TestCase(ScentKind.Target, AlertOutcome.Success)]
        [TestCase(ScentKind.Distractor, AlertOutcome.FalseAlert)]
        [TestCase(ScentKind.Residual, AlertOutcome.FalseAlert)]
        public void StrongestScentDeterminesExplainableOutcome(ScentKind kind, AlertOutcome expected)
        {
            var observation = Observation("source", kind, 0.8);
            var result = AlertEvaluator.Evaluate(new[] { observation });

            Assert.That(result.Outcome, Is.EqualTo(expected));
            Assert.That(result.Reason, Is.Not.Empty);
        }

        [Test]
        public void WeakEvidenceDoesNotBecomeFalseAlert()
        {
            var result = AlertEvaluator.Evaluate(new[] { Observation("weak", ScentKind.Target, 0.2) });
            Assert.That(result.Outcome, Is.EqualTo(AlertOutcome.InsufficientEvidence));
        }

        [Test]
        public void SimilarSignalsAreReportedAsAmbiguous()
        {
            var result = AlertEvaluator.Evaluate(new[]
            {
                Observation("target", ScentKind.Target, 0.7),
                Observation("food", ScentKind.Distractor, 0.65)
            });
            Assert.That(result.Outcome, Is.EqualTo(AlertOutcome.InsufficientEvidence));
            StringAssert.Contains("重な", result.Reason);
        }

        private static ScentObservation Observation(string id, ScentKind kind, double intensity)
        {
            var source = new ScentSource(id, id, kind, new Point3(0, 0, 0), NoseHeight.Ground, 1, 5);
            return new ScentObservation(source, intensity);
        }
    }
}
