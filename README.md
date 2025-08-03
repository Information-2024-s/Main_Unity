# 2I研究所
2I研究所のゲーム部分のプロリザルト表示
## 使い方
git cloneしてきて、unity hubからadd ->add project from disk でcloneしてきたやつ選択して、起動したら下のとこからscenes ->で好きなシーン選ぶ。(sampleSceneでいいと思うよ) 
## 各スクリプトの説明
* [wii_cursor](/Assets/scripts/WiiCursor.cs):wiiリモコンを使ってカーソル移動、弾の射出をする。各look_onにアタッチ
* [Syouzyun](/Assets/scripts/Syouzyun.cs):マウスで弾の射出をする。main_cameraにアタッチ
* [ScoreManager](/Assets/scripts/ScoreManager.cs):スコア関連の関数、変数。Managerにアタッチ。
* [Moving_1](/Assets/scripts/Moving_1.cs):多分の敵の移動系のやつじゃね？敵にアタッチ。
* [MausuCursor](/Assets/scripts/MausuCursor.cs):マウスカーソルに追従させるやつ。だと思う。Managerにアタッチ。
* [Goal](/Assets/scripts/Goal.cs):ゴールした時のリザルト表示。Panelにアタッチ。
* [Enemy](/Assets/scripts/Enemy.cs):弾に当たった時の関数がある。スコア加算とかエフェクトとか。敵にアタッチ。
* [camera_move](/Assets/scripts/camera_move.cs):謎。敵にアタッチ。
* [Shot](/Assets/Items/Shot.cs):敵に当たったことを検知してEnemy.csのやつ呼ぶ。直接アタッチはしない。

## 開発関係のメモ
### プレイヤーの番号
0:黒
1:青
2:赤
3:緑
