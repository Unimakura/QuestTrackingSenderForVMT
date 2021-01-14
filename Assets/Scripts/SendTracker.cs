using UnityEngine;

[RequireComponent(typeof(uOSC.uOscClient))]
public class SendTracker : MonoBehaviour {
    [SerializeField] Transform tranCenterEye = null;
    [SerializeField] Transform tranLeftHand = null;
    [SerializeField] Transform tranRightHand = null;
    
    private uOSC.uOscClient uClient;
    private bool isSending = false;
    public bool IsSending => isSending;

    private void Awake() {
        uClient = GetComponent<uOSC.uOscClient>();
    }

    void LateUpdate()
    {
        if (isSending) Send();
    }
    
    /// <summary>
    /// 送信状態を切り替える
    /// </summary>
    /// <param name="status"></param>
    public void ChangeSendStatus(bool status)
    {
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
