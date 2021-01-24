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
    private bool isSmooth = false;

    private int interval = 1;
    private int remainFrame = 0;
    private float currentIntervalTime = 0f;
    private List<OldPositions> oldPositions;
    private List<OldRotations> oldRotations;
    private List<int> oldPosCount;
    private float thresholdMovePos = 5;

    private void Awake() {
        uClient = GetComponent<uOSC.uOscClient>();
    }

    void LateUpdate()
    {
        if (isSending)
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

    public void ChangeSmooth(bool value)
    {
        isSmooth = value;
    }

    public void SetThresholdMovePos(int value)
    {
        thresholdMovePos = value;
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

        oldPositions = new List<OldPositions>() {
            new OldPositions(tranCenterEye.localPosition, tranCenterEye.localPosition),
            new OldPositions(tranLeftHand.localPosition, tranLeftHand.localPosition),
            new OldPositions(tranRightHand.localPosition, tranRightHand.localPosition)
        };

        oldRotations = new List<OldRotations>() {
            new OldRotations(tranCenterEye.localRotation, tranCenterEye.localRotation),
            new OldRotations(tranLeftHand.localRotation, tranLeftHand.localRotation),
            new OldRotations(tranRightHand.localRotation, tranRightHand.localRotation)
        };

        oldPosCount = new List<int>() { 0,0,0 };
    }

    /// <summary>
    /// 送信処理
    /// </summary>
    private void Send() {
        if (uClient == null) return;

        AdjustAndSendTrackerForVMT(TrackerIndex.HIP, tranCenterEye.localPosition, tranCenterEye.localRotation);
        AdjustAndSendTrackerForVMT(TrackerIndex.LEFT_LEG, tranLeftHand.localPosition, tranLeftHand.localRotation);
        AdjustAndSendTrackerForVMT(TrackerIndex.RIGHT_LEG, tranRightHand.localPosition, tranRightHand.localRotation);
    }

    /// <summary>
    /// トラッカーを送信する
    /// </summary>
    /// <param name="index"></param>
    /// <param name="pos"></param>
    /// <param name="rot"></param>
    private void AdjustAndSendTrackerForVMT(int index, Vector3 pos, Quaternion rot)
    {
        var sendPos = AdjustPosition(index, pos);
        var sendRot = AdjustRotation(index, rot);

        if (CheckSendFrame())
        {
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
    }

    /// <summary>
    /// 位置情報を調整する
    /// </summary>
    /// <param name="index"></param>
    /// <param name="pos"></param>
    /// <returns></returns>
    private Vector3 AdjustPosition(int index, Vector3 pos)
    {
        Vector3 currentPos = new Vector3(pos.x, pos.y, pos.z);
        Vector3 returnPos;

        if (isAdjustAbnormalPosition)
        {
            currentPos = AdjustAbnormalPosition(index, currentPos);
        }

        // 均す処理は今回と前々回の値を均した値を送信するため、最後に実行する
        if (isSmooth)
        {
            returnPos = SmoothBeforePosition(index, currentPos);
        }
        else {
            returnPos = currentPos;
        }
        
        oldPositions[index].UpdateBefore(currentPos);

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
        Quaternion currentRot = new Quaternion(rot.x, rot.y, rot.z, rot.w);
        Quaternion returnRot;
        
        // 均す処理は今回と前々回の値を均した値を送信するため、最後に実行する
        if (isSmooth)
        {
            returnRot = SmoothBeforeRotation(index, currentRot);
        }
        else
        {
            returnRot = currentRot;
        }
        
        oldRotations[index].UpdateBefore(currentRot);

        returnRot.x = RoundOffUnnecessaryNumber(returnRot.x);
        returnRot.y = RoundOffUnnecessaryNumber(returnRot.y);
        returnRot.z = RoundOffUnnecessaryNumber(returnRot.z);
        returnRot.w = RoundOffUnnecessaryNumber(returnRot.w);

        return returnRot;
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
        Vector3 oldPos = oldPositions[index].PositionBefore;

        if (oldPosCount[index] < SendTrackerValue.THRESHOLD_LOCK_POS &&
            Vector3.Distance(pos, oldPos) >= threshold)
        {
            ++oldPosCount[index];
            Debug.Log("Change Old Pos");
            return oldPos;
        }

        oldPosCount[index] = 0;
        return pos;
    }

    /// <summary>
    /// 位置を均す
    /// </summary>
    /// <param name="index"></param>
    /// <param name="pos"></param>
    /// <returns></returns>
    private Vector3 SmoothBeforePosition(int index, Vector3 pos)
    {
        var pos2 = oldPositions[index].PositionBeforeLast;
        var smoothedPos = new Vector3(
            CenterValue(pos.x, pos2.x),
            CenterValue(pos.y, pos2.y),
            CenterValue(pos.z, pos2.z)
        );

        oldPositions[index].OverwriteBefore(smoothedPos);

        return smoothedPos;
    }

    /// <summary>
    /// 回転を均す
    /// </summary>
    /// <param name="index"></param>
    /// <param name="rot"></param>
    /// <returns></returns>
    private Quaternion SmoothBeforeRotation(int index, Quaternion rot)
    {
        var rot2 = oldRotations[index].RotationBeforeLast;
        var smoothedRot = new Quaternion(
            CenterValue(rot.x, rot2.x),
            CenterValue(rot.y, rot2.y),
            CenterValue(rot.z, rot2.z),
            CenterValue(rot.w, rot2.w)
        );
        
        oldRotations[index].OverwriteBefore(smoothedRot);

        return smoothedRot;
    }

    /// <summary>
    /// 中間値を計算する
    /// </summary>
    /// <param name="val1"></param>
    /// <param name="val2"></param>
    /// <returns></returns>
    private float CenterValue(float val1, float val2)
    {
        return (val1 + val2) / 2f;
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

    private class OldPositions
    {
        private Vector3 positionBefore;
        private Vector3 positionBeforeLast;

        public Vector3 PositionBefore => positionBefore;
        public Vector3 PositionBeforeLast => positionBeforeLast;

        public OldPositions(Vector3 before, Vector3 beforeLst)
        {
            positionBefore = before;
            positionBeforeLast = beforeLst;
        }

        public void UpdateBefore(Vector3 before)
        {
            positionBeforeLast = positionBefore;
            positionBefore = before;
        }

        public void OverwriteBefore(Vector3 before)
        {
            positionBefore = before;
        }
    }
    
    private class OldRotations
    {
        private Quaternion rotationBefore;
        private Quaternion rotationBeforeLast;

        public Quaternion RotationBefore => rotationBefore;
        public Quaternion RotationBeforeLast => rotationBeforeLast;

        public OldRotations(Quaternion before, Quaternion beforeLst)
        {
            rotationBefore = before;
            rotationBeforeLast = beforeLst;
        }

        public void UpdateBefore(Quaternion before)
        {
            rotationBeforeLast = rotationBefore;
            rotationBefore = before;
        }

        public void OverwriteBefore(Quaternion before)
        {
            rotationBefore = before;
        }
    }
}
