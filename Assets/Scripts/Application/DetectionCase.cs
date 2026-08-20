using System;
using PoliceDog.Domain;

namespace PoliceDog.Application
{
    public enum CasePhase
    {
        Patrol,
        Confirming,
        Reward,
        Complete
    }

    public sealed class DetectionCase
    {
        public CasePhase Phase { get; private set; } = CasePhase.Patrol;
        public int Attempts { get; private set; }
        public int SuccessfulFinds { get; private set; }
        public int Trust { get; private set; }
        public AlertResult LastResult { get; private set; }

        public AlertResult SubmitAlert(ScentObservation[] observations)
        {
            if (Phase != CasePhase.Patrol)
            {
                return LastResult;
            }

            Attempts++;
            Phase = CasePhase.Confirming;
            LastResult = AlertEvaluator.Evaluate(observations);
            if (LastResult.Outcome == AlertOutcome.Success)
            {
                SuccessfulFinds++;
                Phase = CasePhase.Reward;
            }
            else
            {
                Phase = CasePhase.Patrol;
            }
            return LastResult;
        }

        public void AcceptReward()
        {
            if (Phase != CasePhase.Reward) return;
            Trust++;
            Phase = CasePhase.Complete;
        }

        public void Restart()
        {
            Phase = CasePhase.Patrol;
            Attempts = 0;
            SuccessfulFinds = 0;
            LastResult = default;
        }
    }
}
