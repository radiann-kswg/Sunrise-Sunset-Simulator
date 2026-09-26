# AGENTS.md — Sunrise Sunset Simulator

> 本リポジトリで作業する AI エージェント（Claude（Cowork / Claude Code）/ GitHub Copilot / Codex 等）向け指示書の正本（SSOT）。`CLAUDE.md` は本ファイルを指す薄いポインタ。運用ルールの追記は本ファイルにのみ行う。
> ロールプレイ指定は本リポジトリに置かない（Cowork プロジェクト「Unity周り」側の既定＝錦野歌嫁に従う）。
> 2026-09-26 作成（Unity 2021.3.18f1 → 6000.3.23f1 版上げ・Claude バイブコーディング対応と同時）。同日に太陽位置の計算を標準の式へ書き直し、UI を TMP 化、フォントをサブモジュール化、テストを追加。

## 1. 概要

現在時刻と緯度から太陽の向き（Directional Light の回転）を求めるリアルタイム日照シミュレータ（`Assets/RadianN/Products/Realtime SunSim/`）。

| ファイル | 内容 |
| --- | --- |
| `Scripts/SunRotationGenerator.cs` | 静的クラス。地方時・緯度・経度・時差 → 太陽の高度・方位（NOAA Solar Calculator の簡略式＝Meeus。大気差なし）。`SunLightRotation()` が Directional Light 用の回転 |
| `Scripts/SimulatorTester.cs` | 観測地と時計（実時間 or `simulatedStart` から `timeScale` 倍速）を持ち、Directional Light を太陽の向きに回し薄明（±`twilightDeg`）で光量をフェード。`CurrentTime` / `Elevation` / `Azimuth` を公開 |
| `Scripts/ClockDirecter.cs` | 時計表示（TMP）。`clock` に SimulatorTester を繋ぐとその時刻、未指定なら PC 時刻 |
| `Scripts/SunPositionHUD.cs` | 高度・方位・8 方位の表示 `El:  +45.2  Az: 178.3 S` |
| `Tests/Editor/SunRotationGeneratorTests.cs` | EditMode テスト。参照値は Python `astral` 3.2 で出した東京の四季（許容 0.5°）。asmdef 無し＝Editor アセンブリ |
| `Scenes/SampleScene.unity` | 動作確認シーン（`Assets/Realtime GI Lighting.lighting` を使用）。中央の球は `Materials/Earth.mat`（`Textures/Earth_BlueMarble.png` = NASA Blue Marble NG を 2048×1024 に縮小・パブリックドメイン） |
| `Assets/RadianN/Other Productions/Fonts/PenchantManufacture/` | サブモジュール `PenchantManufacture_ImageAssets/assets/fonts/` からのコピー＋TMP SDF（Dynamic・ASCII 事前登録）。数字系（時計・太陽位置）に使う。**CJK 未収録** |
| `Assets/RadianN/Other Productions/Fonts/x0y0pxFreeFont/` | サブモジュール `hicchicc.github.io/00ff/` からのコピー＋TMP SDF。**かなのみで漢字は無い**（`太陽高度方位` は欠字）。2026-09-26 に User が見出しも Penchant に変えたので同梱のみ（シーンでは未使用） |
| `PenchantManufacture_ImageAssets/` / `hicchicc.github.io/` | **git サブモジュール**（フォントの正本。ライセンスは各リポジトリ／配布サイト。clone 後 `git submodule update --init`） |
| `Assets/TextMesh Pro/` | TMP Essential Resources |
| `Assets/Settings/` | URP アセット・レンダラー・Global Settings・DefaultVolumeProfile |
| `Assets/Editor/GitTools.cs` | `Tools > Git Commit All` / `GitTools.RunGit(...)`（他リポジトリと同一ファイル） |

## 2. 技術スタック

- Unity **6000.3.23f1**（Unity 6.3 LTS）・**URP 17.3.0**（2026-09-26 に Built-in から移行。`Assets/Settings/URP_Asset` を Graphics と全 Quality に割当・Soft Shadows 有効。PPv2・ベイク・プローブは元から無し。Procedural Skybox と Enlighten Realtime GI の設定はそのまま）・uGUI 2.0（TMP 同梱。シーンの Text は 2026-09-26 に TMP へ置換済み）
- D-DIN（旧時計のフォント・SIL OFL）は 2026-09-26 に削除。
- 追加パッケージ（User 導入）: `com.unity.ai.assistant`（Unity 純正 MCP リレー）、`com.unity.ai.inference`、`com.unity.recorder` 5.1.7（日照の早回し動画用。Editor クラッシュの前例あり）
- `com.unity.pipeline` 0.6.0-exp.1（Unity CLI `unity command …` 用）
- ブランチ `main`・リモート `ous-radian-n/Sunrise-Sunset-Simulator`（他リポジトリと違い `radiann-kswg` ではない）

## 3. コーディング規則

- 回答は日本語。既存の命名と `/// <summary>` の日本語コメントを維持する。
- 太陽位置の計算式・定数を変えるときは出典を残し、`Tests/Editor/` の参照値（astral）で確認する。座標系は **+Z = 北・+X = 東・+Y = 上**、方位は北 0°・東 90°。
- ロジックは `public static` 純関数（`SunPosition` / `SunDirection` / `Format` / `Compass`）に置いてテストで固定する。日出・日没の確認は `useSystemClock=false`・`timeScale=900` で Play し、`ScreenCapture.CaptureScreenshot` で撮る（`unity command capture_game_view` は Overlay Canvas が写らず `Assets/Temp/` を作る）。
- 100 行を超える変更は先に計画を提示する。

## 4. 運用ルール

1. シーン・GameObject・Prefab の操作は Unity CLI（`unity status` → `unity command …`）または Unity MCP 経由。`.unity` / `.prefab` の直接編集は最後の手段。
2. 完了前に Console のエラー（`error CS` を含む）を確認する（`unity command console --level error` / `recompile_status`）。
3. `Library/` `Temp/` `Logs/` `obj/` `UserSettings/` `.vs/` と `.meta` は手で触らない。
4. **サンドボックスから git を書かない**（読むだけなら `GIT_OPTIONAL_LOCKS=0`）。commit は `GitTools.RunGit("add -A")` → `GitTools.RunGit("commit -F Temp/evals/msg.txt")` か User。push は指示があるときだけ。
5. `ProjectSettings.asset` の `organizationId` / `cloudProjectId` は Unity が勝手に書き換える。コミットに含めるかは User 判断。
6. `eval_file` は Play 突入直後・パッケージ導入中は「Network error」「timed out」で返る。コマンド自体は実行済みのことが多いので、状態を読み直してから再実行する。
