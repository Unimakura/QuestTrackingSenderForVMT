# Quest tracking sender for VMT
Oculus Quest の HMD 、左コントローラー、右コントローラーの位置と回転を、 PC で動いている [Virtual Motion Tracker (VMT)](https://github.com/gpsnmeajp/VirtualMotionTracker) に向けて OSC で送信するアプリです。

# 何が嬉しいのか
Oculus Quset を追加のトラッカーとして活用できるので、Oculus Quest が2台あればフルトラッキングができちゃいます、やったね！

![thumbnail](https://pbs.twimg.com/media/Eri1LzsVEAEqZAY?format=jpg&name=small)<br>
↑の画像の **OSC を送信する部分** がこのアプリです。

## デモプレイ
* [VRChat で6点トラッキング](https://twitter.com/e_unimakura/status/1349026414868119552)
* [Beatsaber と VMC & VMCAvatar で6点トラッキング](https://twitter.com/e_unimakura/status/1348546213788860416)
* [Beatsaber 足の精度確認用](https://twitter.com/e_unimakura/status/1351186827038896129)

# ※注意事項※
本来の用途とは異なる使い方になりますので、利用は自己責任でお願いします。

* Vive トラッカーなどと比較すると精度は低く、トラッキングが飛ぶこともあります
* HMD の落下による HMD の破損や、怪我に気を付けてください
* HMD を固定する際、本体に負荷をかけすぎると破損する恐れがあります
* この用途の為だけに Oculus Quest を買うのはオススメしません
(動かなくても責任取れません)

以上の事を理解した上で利用してください。

# セットアップ
[セットアップマニュアル](https://github.com/Unimakura/QuestTrackingSenderForVMT/wiki)


# 開発者向け
## ビルドに必要なもの
* [Unity 2019.4.18.f1](https://unity3d.com/jp/get-unity/download/archive)
* [git](https://git-scm.com/downloads)
    * upmの[uOSC](https://github.com/hecomi/uOSC)のため必要です。
* [Oculus Integration](https://assetstore.unity.com/packages/tools/integration/oculus-integration-82022) <br>
最低限、以下のファイル、フォルダをインポートして`OculusProjectConfig.asset`の`Requires System Keyboard`をチェックします。  
    ```
    Assets/Oculus/VR/
    Assets/Oculus/SampleFramework/Core/DebugUI/
    Assets/Oculus/OculusProjectConfig.asset
    ```
* [Final IK](https://assetstore.unity.com/packages/tools/animation/final-ik-14290)<br>
最低限、以下のファイル、フォルダをインポートします。<br>
<a target="_blank" rel="noopener noreferrer" href="https://github.com/Unimakura/QuestTrackingSenderForVMT/wiki/images/finalik.jpg"><img src="https://github.com/Unimakura/QuestTrackingSenderForVMT/wiki/images/finalik.jpg" height="300px"></a><br>