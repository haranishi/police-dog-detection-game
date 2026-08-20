using System.Collections;
using System.Collections.Generic;
using System.Linq;
using PoliceDog.Domain;
using PoliceDog.Infrastructure;
using PoliceDog.Presentation;
using UnityEngine;

namespace PoliceDog.Application
{
    public sealed class PrototypeCaseController : MonoBehaviour
    {
        [SerializeField] private PrototypeDogController dog;
        [SerializeField] private PrototypeHud hud;
        [SerializeField] private List<ScentEmitterView> emitters = new List<ScentEmitterView>();

        private readonly DetectionCase detectionCase = new DetectionCase();
        private LocalProgressStore progressStore;
        private ProgressData progress;
        private bool resolving;
        private bool subscribed;

        public void Configure(
            PrototypeDogController dogController,
            PrototypeHud prototypeHud,
            IEnumerable<ScentEmitterView> scentEmitters)
        {
            dog = dogController;
            hud = prototypeHud;
            emitters = scentEmitters.ToList();
            Subscribe();
        }

        private void Awake()
        {
            progressStore = new LocalProgressStore();
            progress = progressStore.Load();
        }

        private void OnEnable()
        {
            Subscribe();
        }

        private void OnDisable()
        {
            if (dog == null || !subscribed) return;
            dog.AlertRequested -= HandleAlert;
            dog.NoseHeightChanged -= HandleNoseHeight;
            subscribed = false;
        }

        private void Update()
        {
            if (dog == null) return;
            var observations = CurrentObservations();
            foreach (var emitter in emitters)
            {
                var observation = observations.FirstOrDefault(item => item.Source.Id == emitter.ToDomain().Id);
                emitter.SetFocused(dog.IsFocused, (float)observation.Intensity);
            }
        }

        private ScentObservation[] CurrentObservations()
        {
            var position = dog.transform.position;
            var nosePosition = new Point3(position.x, position.y + (dog.NoseHigh ? 1.1 : 0.25), position.z);
            var height = dog.NoseHigh ? NoseHeight.Air : NoseHeight.Ground;
            var sources = emitters.Select(emitter => emitter.ToDomain());
            return ScentFieldQuery.Observe(sources, nosePosition, height).ToArray();
        }

        private void HandleAlert()
        {
            if (resolving || detectionCase.Phase != CasePhase.Patrol) return;
            StartCoroutine(ResolveAlert());
        }

        private IEnumerator ResolveAlert()
        {
            resolving = true;
            hud.SetStatus("ハンドラーが確認中…", "座って静かに知らせました。", "受動通知は逮捕ではなく、追加確認を求める合図です。");
            yield return new WaitForSeconds(1.2f);

            var result = detectionCase.SubmitAlert(CurrentObservations());
            if (result.Outcome == AlertOutcome.Success)
            {
                hud.SetStatus("正しい通知です", result.Reason, "報酬を受け取って訓練を完了しましょう。");
                hud.OfferReward(AcceptReward);
            }
            else
            {
                hud.SetStatus("再確認しましょう", result.Reason, "犬は叱られません。Shiftで形と動きを見比べて続けます。");
            }
            resolving = false;
        }

        private void AcceptReward()
        {
            detectionCase.AcceptReward();
            progress.trust++;
            progress.completions++;
            progressStore.Save(progress);
            hud.SetStatus(
                "Good dog! 訓練完了",
                "ハンドラーと玩具で遊びました。小さな尻尾の動きが信頼の報酬です。",
                "Enterで同じ配置を再挑戦 / Escで終了");
            PrototypeDogFactory.PlayReward(dog.transform);
        }

        private void HandleNoseHeight(bool high)
        {
            hud.SetStatus(
                high ? "鼻を上げた" : "鼻を下げた",
                high ? "空中の新しい匂いを拾いやすくなります。" : "床と荷物の残留臭を拾いやすくなります。",
                "Shiftで嗅覚集中 / Spaceで座って通知");
        }

        private void LateUpdate()
        {
            if (Input.GetKeyDown(KeyCode.Return) && detectionCase.Phase == CasePhase.Complete)
            {
                detectionCase.Restart();
                hud.SetStatus("到着ロビー訓練", "配置は同じです。違う匂いを意識して再挑戦しましょう。", "WASD 移動 / Ctrl 走る / R 鼻 / Shift 嗅覚 / Space 通知");
            }
            if (Input.GetKeyDown(KeyCode.Escape)) UnityEngine.Application.Quit();
        }

        private void Subscribe()
        {
            if (dog == null || subscribed) return;
            dog.AlertRequested += HandleAlert;
            dog.NoseHeightChanged += HandleNoseHeight;
            subscribed = true;
        }
    }
}
