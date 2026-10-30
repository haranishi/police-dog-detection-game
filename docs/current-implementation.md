# 匂い判定・訓練状態・保存の現状設計

2026-10-30。READMEの架空の受動通知訓練を引き継ぎ、実装済みコードの契約を補う。実際の探知性能を表すモデルではない。

## 構成と責務
| 層 | 正本と責務 |
| --- | --- |
| Domain | [ScentTypes.cs](../Assets/Scripts/Domain/ScentTypes.cs)、[ScentFieldQuery.cs](../Assets/Scripts/Domain/ScentFieldQuery.cs)、[AlertEvaluator.cs](../Assets/Scripts/Domain/AlertEvaluator.cs)で匂いと通知判定 |
| Application | [DetectionCase.cs](../Assets/Scripts/Application/DetectionCase.cs)で訓練状態、[PrototypeCaseController.cs](../Assets/Scripts/Application/PrototypeCaseController.cs)でUnity入力・表示・保存を接続 |
| Presentation/Delivery | [PrototypeBootstrap.cs](../Assets/Scripts/Delivery/PrototypeBootstrap.cs)で場面を組み、[PrototypeDogController.cs](../Assets/Scripts/Presentation/PrototypeDogController.cs)で移動と通知入力 |
| Infrastructure | [LocalProgressStore.cs](../Assets/Scripts/Infrastructure/LocalProgressStore.cs)で進捗JSONを読み書き |

[Domain asmdef](../Assets/Scripts/Domain/PoliceDog.Domain.asmdef)は`noEngineReferences=true`。Unity型を使わない距離・強度・判定と、Unity側の入力/演出を分けている。

## 匂いから判定まで
- 匂い源は`Target / Distractor / Residual`、鼻の高さは`Ground / Air`。鮮度は0〜1、範囲は最低0.1へ補正する。
- 範囲外の強度は0。範囲内では`(1-距離/範囲) × 鮮度補正 × 高さ補正`を0〜1へ収める。
- 高さ一致の補正は1、不一致は0.35。残留臭だけ鮮度を2乗し、それ以外は鮮度をそのまま使う。
- `Observe`は強度0.03以上だけを強い順に返す。この順序の配列を`AlertEvaluator`へ渡す。
- 観測なしは`NoNearbySource`、最強が0.28未満か上位2件の差が0.08未満なら`InsufficientEvidence`。
- 十分な根拠がある最強の匂いがTargetなら`Success`、Distractor/Residualなら`FalseAlert`。結果に理由とラベルを含む。

## 訓練の状態と入力
- `DetectionCase`は`Patrol → Confirming → Reward/Patrol`へ同期処理で遷移する。成功しても報酬を受け取るまで完了しない。
- `Reward`で`AcceptReward`を受けるとTrustを加算して`Complete`へ進む。Patrol以外での通知提出は直前結果を返す。
- Unity側は`resolving`で二重通知を防ぎ、確認演出の1.2秒後に現在の観測を提出する。Confirmingを1.2秒保持する実装ではない。
- 完了後のEnterは再挑戦。`Restart`は試行数・発見数・直前結果を初期化し、モデル内のTrustは保持する。
- 毎フレームの仮想犬位置/鼻の高さと匂い源から観測を作り、表示側の形/動きを更新する。実カメラや実際の匂い情報は取得しない。

## 保存するデータ
- `UnityEngine.Application.persistentDataPath`内の`progress.json`に、`schemaVersion=1`、`trust`、`completions`を保存する。
- 新規・JSON不正・読み込み例外・未知のschemaは初期進捗へ戻す。保存値の追加の範囲検証は実装されていない。
- Controllerは報酬時に保存用の信頼/完了数を増やして保存する。モデル内のTrustと保存用進捗は別のオブジェクト。
- Saveは一時ファイルへ書いた後、旧ファイルを削除して移動する。保存例外をこのクラス内で捕捉する処理はない。
- 保存対象は進捗の整数だけ。仮想位置、匂い観測、実プレイヤーの個人情報を外部へ送る処理はない。

## 検証条件
- [強度テスト](../Assets/Tests/EditMode/ScentFieldQueryTests.cs)の距離/鮮度/高さ/降順、[通知テスト](../Assets/Tests/EditMode/AlertEvaluatorTests.cs)の強弱/曖昧さ/分類を維持する。
- [状態テスト](../Assets/Tests/EditMode/DetectionCaseTests.cs)の「報酬前は未完了」「誤通知はPatrolへ戻る」を維持する。
- 追加変更では閾値の直前/一致/直後、再挑戦時の保持/初期化、JSON不正/未知schema/保存失敗を合成データで確認する。
- Unityでの入力、色以外の匂い表現、報酬演出、保存/再起動は、READMEのライセンス制約が解消した後に確認する。
- `Library`・`Temp`・ロックを削除して回避せず、Unity batchmodeを同時起動しない。仮モデルの挙動を製品アセットの品質と混同しない。

## 今回の記録
- コード、匂い源、保存データ、Scene、依存は変更0。ソースと文書を確認しただけで、Unity起動・テスト・ビルド・実操作は行っていない。
- READMEのUnityライセンス接続による未検証状態を、今回の文書追加で成功へ変更していない。
