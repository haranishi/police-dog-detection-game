using System.Collections.Generic;

namespace PoliceDog.Domain
{
    public enum AlertOutcome
    {
        Success,
        FalseAlert,
        InsufficientEvidence,
        NoNearbySource
    }

    public readonly struct AlertResult
    {
        public readonly AlertOutcome Outcome;
        public readonly string SourceLabel;
        public readonly string Reason;

        public AlertResult(AlertOutcome outcome, string sourceLabel, string reason)
        {
            Outcome = outcome;
            SourceLabel = sourceLabel;
            Reason = reason;
        }
    }

    public static class AlertEvaluator
    {
        public static AlertResult Evaluate(
            IReadOnlyList<ScentObservation> observations,
            double minimumEvidence = 0.28,
            double ambiguityMargin = 0.08)
        {
            if (observations == null || observations.Count == 0)
            {
                return new AlertResult(
                    AlertOutcome.NoNearbySource,
                    string.Empty,
                    "通知できる匂いが近くにありません。鼻の高さと距離を確認しましょう。");
            }

            var strongest = observations[0];
            if (strongest.Intensity < minimumEvidence)
            {
                return new AlertResult(
                    AlertOutcome.InsufficientEvidence,
                    strongest.Source.Label,
                    "匂いが弱く、発生源を確定できません。もう少し近くで確認しましょう。");
            }

            if (observations.Count > 1 && strongest.Intensity - observations[1].Intensity < ambiguityMargin)
            {
                return new AlertResult(
                    AlertOutcome.InsufficientEvidence,
                    strongest.Source.Label,
                    "複数の匂いが重なっています。形と途切れ方を見て再確認しましょう。");
            }

            switch (strongest.Source.Kind)
            {
                case ScentKind.Target:
                    return new AlertResult(
                        AlertOutcome.Success,
                        strongest.Source.Label,
                        "細い連続した束が発生源へ強まりました。正しい受動通知です。");
                case ScentKind.Residual:
                    return new AlertResult(
                        AlertOutcome.FalseAlert,
                        strongest.Source.Label,
                        "途切れた古い痕跡へ通知しました。残留臭は現在の発生源ではありません。");
                default:
                    return new AlertResult(
                        AlertOutcome.FalseAlert,
                        strongest.Source.Label,
                        "広がる丸い粒子へ通知しました。これは無害な周囲臭です。");
            }
        }
    }
}
