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

    private float interval = 0.1f;
    private float remainTime = 0f;

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
    public void SetInterval(float value)
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

        remainTime -= Time.deltaTime;

        if (remainTime <= 0f)
        {
            float fps = 1f / (interval - remainTime);
            textFps.text = "Send/s: " + fps.ToString("f2");

            remainTime = interval;
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
            remainTime = interval;
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
            index,     // 識別番号
            (int)1,    // 有効可否
            (float)0f, // 補正時間
            (float)pos.x,
            (float)pos.y,
            (float)pos.z,
            (float)rot.x,
            (float)rot.y,
            (float)rot.z,
            (float)rot.w);
    }
}
