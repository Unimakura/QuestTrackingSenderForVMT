# Quest tracking sender for VMT
Oculus Quest の HMD 、左コントローラー、右コントローラーの位置と回転を、 PC で動いている Virtual Motion Tracker (VMC) に向けて OSC で送信するアプリです。

# 何が嬉しいのか
Oculus Quset を追加のトラッカーとして活用できるので、Oculus Quest が2台あればフルトラッキングができちゃいます、やったね！

![thumbnail](https://pbs.twimg.com/media/Eri1LzsVEAEqZAY?format=jpg&name=small)<br>
↑の画像の **OSC を送信する部分** がこのアプリです。

## デモプレイ
* [VRChat で6点トラッキング](https://twitter.com/e_unimakura/status/1349026414868119552)
* [Beatsaber と VMC & VMCAvatar で6点トラッキング](https://twitter.com/e_unimakura/status/1348546213788860416)

# ※注意事項※
本来の用途とは異なる使い方になりますので、利用は自己責任でお願いします。

* HMD の落下による HMD の破損や、怪我に気を付けてください
* HMD を固定する際、本体に負荷をかけすぎると破損する恐れがあります
* Vive トラッカーなどと比較すると精度は低く、トラッキングが飛ぶこともあります
* この用途の為だけに Oculus Quest を買うのはオススメしません
(動かなくても責任取れません)

以上の事を理解した上で利用してください。

# セットアップ
## PC 側
* Steam と SteamVR のインストール
* [Virtual Motion Tracker](https://github.com/gpsnmeajp/VirtualMotionTracker) のインストール<br>
( インストール方法は [こちら](https://github.com/gpsnmeajp/VirtualMotionTracker/blob/master/docs/howto.md) )
* SteamVR の設定 → デバイス → Viveトラッカーを管理 → Viveトラッカーの管理 より、トラッカーの役割を設定<br>
/devices/vmt/VMT_0 : 腰<br>
/devices/vmt/VMT_1 : 左足<br>
/devices/vmt/VMT_2 : 右足
* ファイアウォールの設定で UDP の 39570 ポートを開放

## Oculus Quest 側
* [ダウンロードページ](https://github.com/Unimakura/QuestTrackingSenderForVMT/releases) より apk ファイルをダウンロード
* ダウンロードした **QuestTrackingSenderForVMT.apk** を Oculus Quest 本体にインストール<br>(インストールには [SideQuest](https://sidequestvr.com/) を使うと便利です。参考サイトは [こちら](https://vracademy.jp/development/install_sidequest/) )
* 提供元不明のアプリより **QuestTrackingSenderForVMT** を起動<br>
![thumbnail](https://pbs.twimg.com/media/ErsvaBnVgAEI1Km?format=jpg&name=small)
* IP の入力欄に PC の **ローカル IP** を入力<br>
![thumbnail](https://pbs.twimg.com/media/ErsvY_0VgAMCI3d?format=jpg&name=small)<br>
※入力する際に表示されるキーボードは、右コントローラーの Oculus ボタンで閉じることができます
* **START** ボタンを押す

# 動作確認
![thumbnail](https://pbs.twimg.com/media/ErS8ZYpVoAMTpAZ?format=jpg&name=360x360)<br>
このように **VMT** トラッカーが表示されれば動いています。

# Quest と Quest の位置調整
両方の右コントローラーを同時に持って、同時に Oculus ボタンを長押しする<br>
![thumbnail](https://pbs.twimg.com/media/ErgteiZUYAAns0p?format=jpg&name=small)



# 開発者向け
## ビルドに必要なもの
* [Unity 2019.3.6.f1](https://unity3d.com/jp/get-unity/download/archive)
* [uOSC](https://github.com/hecomi/uOSC/releases/tag/v0.0.2)
* [Oculus Integration](https://assetstore.unity.com/packages/tools/integration/oculus-integration-82022)