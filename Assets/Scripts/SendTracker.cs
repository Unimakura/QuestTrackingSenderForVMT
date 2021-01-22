using System;
using UnityEngine;
using TMPro;

[RequireComponent(typeof(uOSC.uOscClient))]
public class SendTracker : MonoBehaviour {
    [SerializeField] Transform tranCenterEye = null;
    [SerializeField] Transform tranLeftHand = null;
    [SerializeField] Transform tranRightHand = null;
    [SerializeField] TextMeshProUGUI textFps = null;

    private uOSC.uOscClient uClient;
    private bool isSending = false;
    public bool IsSending => isSending;

    private int interval = 1;
    private int remainFrame = 0;
    private float currentIntervalTime = 0f;

    private void Awake() {
        uClient = GetComponent<uOSC.uOscClient>();
    }

    void LateUpdate()
    {
        if (CheckSendFrame())
        {
            Send();
        }
    }

    /// <summary>
    /// データ送信する間隔をセットする
    /// </summary>
    /// <param name="value"></param>
    public void SetInterval(int value)
    {
        interval = value;
    }

    /// <summary>
    /// データを送信するフレームかどうかチェックする
    /// </summary>
    /// <returns></returns>
    private bool CheckSendFrame()
    {
        if (!isSending) return false;

        --remainFrame;
        currentIntervalTime += Time.deltaTime;

        if (remainFrame <= 0)
        {
            float fps = 1f / currentIntervalTime;
            textFps.text = "Send/s: " + fps.ToString("f2");

            remainFrame = interval;
            currentIntervalTime = 0f;
            return true;
        }

        return false;
    }
    
    /// <summary>
    /// 送信状態を切り替える
    /// </summary>
    /// <param name="status"></param>
    public void ChangeSendStatus(bool status)
    {
        if (status) {
            remainFrame = interval;
            currentIntervalTime = 0f;
        }

        isSending = status;
    }

    /// <summary>
    /// 送信処理
    /// </summary>
    private void Send() {
        if (uClient == null) return;

        SendTrackerForVMT(TrackerIndex.HIP, tranCenterEye.localPosition, tranCenterEye.localRotation);
        SendTrackerForVMT(TrackerIndex.LEFT_LEG, tranLeftHand.localPosition, tranLeftHand.localRotation);
        SendTrackerForVMT(TrackerIndex.RIGHT_LEG, tranRightHand.localPosition, tranRightHand.localRotation);
    }

    /// <summary>
    /// トラッカーを送信する
    /// </summary>
    /// <param name="index"></param>
    /// <param name="pos"></param>
    /// <param name="rot"></param>
    private void SendTrackerForVMT(int index, Vector3 pos, Quaternion rot)
    {
        uClient.Send("/VMT/Room/Unity",
            index, // 識別番号
            1,     // 有効可否
            0f,    // 補正時間
            RoundOffUnnecessaryNumber(pos.x),
            RoundOffUnnecessaryNumber(pos.y),
            RoundOffUnnecessaryNumber(pos.z),
            RoundOffUnnecessaryNumber(rot.x),
            RoundOffUnnecessaryNumber(rot.y),
            RoundOffUnnecessaryNumber(rot.z),
            RoundOffUnnecessaryNumber(rot.w)
            );
    }

    /// <summary>
    /// 一定値以下の少数を丸める
    /// </summary>
    /// <param name="num"></param>
    /// <returns></returns>
    private float RoundOffUnnecessaryNumber(float num)
    {
        return Mathf.Floor(num * 10000f) / 10000f;
    }
}
