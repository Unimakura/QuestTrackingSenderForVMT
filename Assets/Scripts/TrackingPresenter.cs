using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// トラッキングの情報を提供する
/// </summary>
public class TrackingPresenter : MonoBehaviour
{
    [SerializeField] Transform tranCenterEye = null;
    [SerializeField] Transform tranLeftHand = null;
    [SerializeField] Transform tranRightHand = null;

    private bool isAdjustAbnormalPosition = false;
    private bool isSmooth = false;
    private List<OldPositions> oldPositions;
    private List<OldRotations> oldRotations;
    private List<int> adjustAbnormalPosCount;
    private List<int> skipAdjustAbnormalPosRemainCount;

    /// <summary>
    /// 初期化
    /// </summary>
    public void Initialize()
    {
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

        adjustAbnormalPosCount = new List<int>() { 0,0,0 };
        skipAdjustAbnormalPosRemainCount = new List<int>() { 0,0,0 };
    }

    /// <summary>
    /// 指定のPositionを計算して返す
    /// </summary>
    /// <param name="index"></param>
    /// <returns></returns>
    public Vector3 CalculateTrackingPositionByIndex(int index)
    {
        return AdjustPosition(index, GetTrackingPositionByIndex(index));
    }

    /// <summary>
    /// 指定のRotationを計算して返す
    /// </summary>
    /// <param name="index"></param>
    /// <returns></returns>

    public Quaternion CalculateTrackingRotationByIndex(int index) {
        return AdjustRotation(index, GetTrackingRotationByIndex(index));
    }

    private Vector3 GetTrackingPositionByIndex(int index)
    {
        if (index == TrackerIndex.LEFT_LEG) {
            return tranLeftHand.localPosition;
        }
        else if (index == TrackerIndex.RIGHT_LEG) {
            return tranRightHand.localPosition;
        }
        else {
            return tranCenterEye.localPosition;
        }
    }
    
    public Quaternion GetTrackingRotationByIndex(int index)
    {
        if (index == TrackerIndex.LEFT_LEG) {
            return tranLeftHand.localRotation;
        }
        else if (index == TrackerIndex.RIGHT_LEG) {
            return tranRightHand.localRotation;
        }
        else {
            return tranCenterEye.localRotation;
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

        if (isSmooth)
        {
            returnPos = AdjustSmoothPosition(index, currentPos);
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
    /// ポジションをスムージング
    /// </summary>
    /// <param name="index"></param>
    /// <param name="pos"></param>
    /// <returns></returns>
    private Vector3 AdjustSmoothPosition(int index, Vector3 pos)
    {
        var estimatePos = GetEstimatePosition(index);

        return Vector3.Lerp(
                pos,
                estimatePos,
                SendTrackerValue.LERP_RATE);
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
        
        if (isSmooth)
        {
            returnRot = Quaternion.Lerp(
                currentRot,
                oldRotations[index].RotationBefore,
                SendTrackerValue.LERP_RATE);
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
        if (skipAdjustAbnormalPosRemainCount[index] > 0)
        {
            --skipAdjustAbnormalPosRemainCount[index];
            return pos;
        }

        float threshold = SendTrackerValue.THRESHOLD_MOVE_POS * Time.deltaTime;
        var estimatePos = GetEstimatePosition(index);

        if (Vector3.Distance(pos, estimatePos) >= threshold)
        {
            ++adjustAbnormalPosCount[index];
            Debug.Log("Adjust Abnormal Pos");

            if (adjustAbnormalPosCount[index] >= SendTrackerValue.MAX_ADJUST_ABNORMAL_POS)
            {
                adjustAbnormalPosCount[index] = 0;

                // 異常値調整が規定回数を超えた場合、一定期間調整をスキップするようにする
                skipAdjustAbnormalPosRemainCount[index] = SendTrackerValue.SKIP_ADJUST_ABNORMAL_POS;
            }
            return estimatePos;
        }

        return pos;
    }

    /// <summary>
    /// 推測値を求める
    /// </summary>
    /// <param name="index"></param>
    /// <returns></returns>
    private Vector3 GetEstimatePosition(int index)
    {
        var oldPoss = oldPositions[index];
        Vector3 velocity = (oldPoss.PositionBefore - oldPoss.PositionBeforeLast) / oldPoss.OldDeltaTime;
        return oldPoss.PositionBefore + (velocity * Time.deltaTime);
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

    public void ChangeAbnormalAdjustPosition(bool value)
    {
        isAdjustAbnormalPosition = value;
    }

    public void ChangeSmooth(bool value)
    {
        isSmooth = value;
    }

    /// <summary>
    /// 過去のPosition情報
    /// </summary>
    private class OldPositions
    {
        private Vector3 positionBefore;
        private Vector3 positionBeforeLast;
        private float oldDeltaTime = 0f;

        public Vector3 PositionBefore => positionBefore;
        public Vector3 PositionBeforeLast => positionBeforeLast;
        public float OldDeltaTime => oldDeltaTime;

        public OldPositions(Vector3 before, Vector3 beforeLst)
        {
            positionBefore = before;
            positionBeforeLast = beforeLst;
        }

        public void UpdateBefore(Vector3 before)
        {
            positionBeforeLast = positionBefore;
            positionBefore = before;
            oldDeltaTime = Time.deltaTime;
        }

        public void OverwriteBefore(Vector3 before)
        {
            positionBefore = before;
        }
    }
    
    /// <summary>
    /// 過去のRotation情報
    /// </summary>
    private class OldRotations
    {
        private Quaternion rotationBefore;
        private Quaternion rotationBeforeLast;
        private float oldDeltaTime = 0f;

        public Quaternion RotationBefore => rotationBefore;
        public Quaternion RotationBeforeLast => rotationBeforeLast;
        public float OldDeltaTime => oldDeltaTime;

        public OldRotations(Quaternion before, Quaternion beforeLst)
        {
            rotationBefore = before;
            rotationBeforeLast = beforeLst;
        }

        public void UpdateBefore(Quaternion before)
        {
            rotationBeforeLast = rotationBefore;
            rotationBefore = before;
            oldDeltaTime = Time.deltaTime;
        }

        public void OverwriteBefore(Quaternion before)
        {
            rotationBefore = before;
        }
    }
}
