using NUnit.Framework;
using PoliceDog.Application;
using PoliceDog.Domain;

namespace PoliceDog.Tests
{
    public sealed class DetectionCaseTests
    {
        [Test]
        public void SuccessRequiresRewardBeforeCompletion()
        {
            var detectionCase = new DetectionCase();
            var source = new ScentSource("target", "target", ScentKind.Target, new Point3(0, 0, 0), NoseHeight.Ground, 1, 5);

            var result = detectionCase.SubmitAlert(new[] { new ScentObservation(source, 0.9) });
            Assert.That(result.Outcome, Is.EqualTo(AlertOutcome.Success));
            Assert.That(detectionCase.Phase, Is.EqualTo(CasePhase.Reward));

            detectionCase.AcceptReward();
            Assert.That(detectionCase.Phase, Is.EqualTo(CasePhase.Complete));
            Assert.That(detectionCase.Trust, Is.EqualTo(1));
        }

        [Test]
        public void FalseAlertReturnsToPatrolWithoutPunishmentLoop()
        {
            var detectionCase = new DetectionCase();
            var source = new ScentSource("food", "food", ScentKind.Distractor, new Point3(0, 0, 0), NoseHeight.Ground, 1, 5);

            detectionCase.SubmitAlert(new[] { new ScentObservation(source, 0.9) });
            Assert.That(detectionCase.Phase, Is.EqualTo(CasePhase.Patrol));
            Assert.That(detectionCase.Trust, Is.Zero);
        }
    }
}
