# 2I研究所
2I研究所のゲーム部分のプロリザルト表示
## 使い方
#### 開発
git cloneしてきて、unity hubからadd ->add project from disk でcloneしてきたやつ選択して、起動したら下のとこからscenes ->で好きなシーン選ぶ。(Main1でいいと思うよ) 
#### プレイ
installerからインストール（いつか作る、と思う）<br>
niAI laboratory_Data\StreamingAssets\config.jsonから、URL、ステージなどを設定<br>
普通にexeを起動
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
* [timeline_player](/Assets/Items/timeline_player.cs):timelineを再生するやつ。使い方は[ここ](https://d.kuku.lu/bjhngj7pc)を見てね。


## 開発関係のメモ
### プレイヤーの番号
~~0:黒 1:青 2:赤 3:緑~~<br>
0:黒 1:青 2:赤 3:緑(9/8修正)
### config.jsonのキー
* stage:どこのステージか。0～2(ゼロインデックス)の数字で入力。
* scene_name:QRどを読んだ後どのシーンに遷移するか。文字列で入力。
* camera_num:QR読むのに使うカメラが内部カメラか外部カメラかとか、カメラのインデックスを数字で指定。
* local_server_URL:顔写真、バッテリー、ログのデータを管理してるローカルサーバーのURL。http://xxx.xxx.xxx.xxx:xxxx/まで文字列で入力。
* DB＿URL:スコアを管理してるDBサーバーのURL。~~/api/scoresまで文字列で入力。(になってるけど多分後で変える)
* api_key:DB鯖にデータを送るときに使う。文字列。
## 進捗報告
stage2進捗(9/15)https://d.kuku.lu/yjnyx4jy7<br>
バッテリーログ送信(9/15)https://d.kuku.lu/huprmzkca


