using System.Collections.Generic;
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
    private bool isAdjustAbnormalPosition = false;
    public bool IsAdjustAbnormalPosition => isAdjustAbnormalPosition;

    private int interval = 1;
    private int remainFrame = 0;
    private float currentIntervalTime = 0f;
    private List<Vector3> oldPositions;
    private List<Quaternion> oldRotations;
    private float thresholdMovePos = 10; // 10m/s

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

    public void ChangeAbnormalAdjustPosition(bool value)
    {
        isAdjustAbnormalPosition = value;
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
            Initialize();
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

        oldPositions = new List<Vector3>() {
            tranCenterEye.localPosition,
            tranLeftHand.localPosition,
            tranRightHand.localPosition
        };

        oldRotations = new List<Quaternion>() {
            tranCenterEye.localRotation,
            tranLeftHand.localRotation,
            tranRightHand.localRotation
        };
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
    /// 異常な位置情報を調整する
    /// </summary>
    /// <param name="index"></param>
    /// <param name="pos"></param>
    /// <returns></returns>
    private Vector3 AdjustAbnormalPosition(int index, Vector3 pos)
    {
        float threshold = thresholdMovePos * Time.deltaTime;
        Vector3 oldPos = oldPositions[index];

        if (Mathf.Abs(pos.x - oldPos.x) >= threshold || 
            Mathf.Abs(pos.y - oldPos.y) >= threshold || 
            Mathf.Abs(pos.z - oldPos.z) >= threshold)
        {
            return oldPos;
        }

        return pos;
    }

    /// <summary>
    /// 位置情報を調整する
    /// </summary>
    /// <param name="index"></param>
    /// <param name="pos"></param>
    /// <returns></returns>
    private Vector3 AdjustPosition(int index, Vector3 pos)
    {
        Vector3 returnPos = new Vector3(pos.x, pos.y, pos.z);

        if (isAdjustAbnormalPosition)
        {
            returnPos = AdjustAbnormalPosition(index, returnPos);
        }

        returnPos.x = RoundOffUnnecessaryNumber(returnPos.x);
        returnPos.y = RoundOffUnnecessaryNumber(returnPos.y);
        returnPos.z = RoundOffUnnecessaryNumber(returnPos.z);

        return returnPos;
    }

    /// <summary>
    /// 回転情報を調整する
    /// </summary>
    /// <param name="index"></param>
    /// <param name="rot"></param>
    /// <returns></returns>
    private Quaternion AdjustRotation(int index, Quaternion rot)
    {
        Quaternion returnRot = new Quaternion(rot.x, rot.y, rot.z, rot.w);

        returnRot.x = RoundOffUnnecessaryNumber(returnRot.x);
        returnRot.y = RoundOffUnnecessaryNumber(returnRot.y);
        returnRot.z = RoundOffUnnecessaryNumber(returnRot.z);
        returnRot.w = RoundOffUnnecessaryNumber(returnRot.w);

        return returnRot;
    }

    /// <summary>
    /// トラッカーを送信する
    /// </summary>
    /// <param name="index"></param>
    /// <param name="pos"></param>
    /// <param name="rot"></param>
    private void SendTrackerForVMT(int index, Vector3 pos, Quaternion rot)
    {
        var sendPos = AdjustPosition(index, pos);
        var sendRot = AdjustRotation(index, rot);
        oldPositions[index] = sendPos;
        oldRotations[index] = sendRot;

        uClient.Send("/VMT/Room/Unity",
            index, // 識別番号
            1,     // 有効可否
            0f,    // 補正時間
            sendPos.x,
            sendPos.y,
            sendPos.z,
            sendRot.x,
            sendRot.y,
            sendRot.z,
            sendRot.w
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
