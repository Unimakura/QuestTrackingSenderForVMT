# Quest tracking sender for VMT
Oculus Quest の HMD 、左コントローラー、右コントローラーの位置と回転を、 PC で動いている [Virtual Motion Tracker (VMT)](https://github.com/gpsnmeajp/VirtualMotionTracker) に向けて OSC で送信するアプリです。

# 何が嬉しいのか
Oculus Quset を追加のトラッカーとして活用できるので、Oculus Quest が2台あればフルトラッキングができちゃいます、やったね！

![thumbnail](https://pbs.twimg.com/media/Eri1LzsVEAEqZAY?format=jpg&name=small)<br>
↑の画像の **OSC を送信する部分** がこのアプリです。

## デモプレイ
* [VRChat で6点トラッキング](https://twitter.com/e_unimakura/status/1349026414868119552)
* [Beatsaber と VMC & VMCAvatar で6点トラッキング](https://twitter.com/e_unimakura/status/1348546213788860416)

# ※注意事項※
本来の用途とは異なる使い方になりますので、利用は自己責任でお願いします。

# セットアップ
[セットアップマニュアル](https://github.com/Unimakura/QuestTrackingSenderForVMT/wiki)


# 開発者向け
## ビルドに必要なもの
* [Unity 2019.3.6.f1](https://unity3d.com/jp/get-unity/download/archive)
* [uOSC](https://github.com/hecomi/uOSC/releases/tag/v0.0.2)
* [Oculus Integration](https://assetstore.unity.com/packages/tools/integration/oculus-integration-82022)  
最低限、以下のファイル、フォルダをインポートして`OculusProjectConfig.asset`の`Requires System Keyboard`をチェックします。  
    ```
    Assets/Oculus/VR/
    Assets/Oculus/SampleFramework/Core/DebugUI/
    Assets/Oculus/OculusProjectConfig.asset
    ```
