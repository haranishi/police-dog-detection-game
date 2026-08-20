# Police Dog Detection Vertical Slice

警察犬として匂いを読み分け、対象前で座って知らせ、ハンドラーから報酬を受け取るUnity灰箱試作です。商用犬アセット購入前の判断用であり、製品版ではありません。

## 現在地

Unity 6000.3.20f1プロジェクトとコード、EditModeテスト、シーン／macOSビルド生成処理まで用意済みです。2026-08-13のbatchmodeはUnity Licensing Clientの接続タイムアウトで終了し、シーン生成・テスト・ビルド・起動は未検証です。

## 再開コマンド

Unity Hubへサインインし、他のUnity Editorが終了していることを確認してから、必ず次を1本ずつ実行します。

```bash
UNITY='/Applications/Unity/Hub/Editor/6000.3.20f1/Unity.app/Contents/MacOS/Unity'

"$UNITY" -batchmode -nographics -projectPath /Users/hara/Projects/police-dog-detection-game -executeMethod PoliceDog.Editor.PrototypeBuild.CreateVerticalSliceScene -quit -logFile /private/tmp/police-dog-compile.log

"$UNITY" -batchmode -nographics -projectPath /Users/hara/Projects/police-dog-detection-game -runTests -testPlatform EditMode -testResults /Users/hara/Projects/police-dog-detection-game/TestResults/editmode.xml -quit -logFile /private/tmp/police-dog-tests.log

"$UNITY" -batchmode -nographics -projectPath /Users/hara/Projects/police-dog-detection-game -executeMethod PoliceDog.Editor.PrototypeBuild.BuildMac -quit -logFile /private/tmp/police-dog-build.log

open /Users/hara/Projects/police-dog-detection-game/Builds/PoliceDogDetection.app
```

## 操作予定

- WASD：移動、左Ctrl：走る
- Shift長押し：嗅覚集中
- R：鼻の高さを切替
- Space：座って受動通知
- Enter：完了後に再挑戦、Esc：終了

## 実装済みコード

- Unity型に依存しない匂いの距離・鮮度・高さ減衰
- 対象・妨害・残留の通知判定と日本語の誤通知理由
- 仮犬の三人称移動、座る通知、玩具報酬
- 細い連続束・丸い拡散・途切れた線による色以外の匂い文法
- ローカルJSON進捗保存。薬物名、現実の手口、外部通信なし

## 触ってはいけない点

- `Library`、`Temp`、ロックを削除してライセンス問題を回避しない。
- Unity batchmodeを複数同時に起動しない。
- プレイヤーテスト前に商用犬・空港・群衆アセットを購入しない。
- 仮モデルの足滑りや犬らしさを、製品アセットの品質評価へ転用しない。
