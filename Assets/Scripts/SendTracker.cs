using UnityEngine;
using TMPro;

[RequireComponent(typeof(uOSC.uOscClient))]
[RequireComponent(typeof(TrackingPresenter))]
public class SendTracker : MonoBehaviour {
    [SerializeField] TextMeshProUGUI textFps = null;

    private uOSC.uOscClient uClient;
    private TrackingPresenter trackingPresenter;
    private bool isSending = false;
    public bool IsSending => isSending;
    private int interval = 1;
    private int remainFrame = 0;
    private float currentIntervalTime = 0f;

    private void Awake() {
        uClient = GetComponent<uOSC.uOscClient>();
        trackingPresenter = GetComponent<TrackingPresenter>();
    }

    void LateUpdate()
    {
        if (isSending)
        {
            trackingPresenter.CalculateTracking();

            if (CheckSendFrame())
            {
                Send();
            }
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
            Initialize();
            trackingPresenter.Initialize();
        }

        isSending = status;
    }

    /// <summary>
    /// 初期化
    /// </summary>
    private void Initialize()
    {
        remainFrame = interval;
        currentIntervalTime = 0f;
    }

    /// <summary>
    /// 送信処理
    /// </summary>
    private void Send()
    {
        SendWithIndex(TrackerIndex.HIP);
        SendWithIndex(TrackerIndex.LEFT_LEG);
        SendWithIndex(TrackerIndex.RIGHT_LEG);
    }

    private void SendWithIndex(int index)
    {
        SendTrackerForVMT(
            index,
            trackingPresenter.GetCurrentPositionByIndex(index),
            trackingPresenter.GetCurrentRotationByIndex(index));
    }

    
    /// <summary>
    /// トラッカーを送信する
    /// </summary>
    /// <param name="index"></param>
    /// <param name="pos"></param>
    /// <param name="rot"></param>
    private void SendTrackerForVMT(int index, Vector3 pos, Quaternion rot)
    {
        if (uClient == null) return;

        uClient.Send("/VMT/Room/Unity",
            index, // 識別番号
            1,     // 有効可否
            0f,    // 補正時間
            pos.x,
            pos.y,
            pos.z,
            rot.x,
            rot.y,
            rot.z,
            rot.w
            );
    }
}
