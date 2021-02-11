using System.Collections.Generic;
using UnityEngine;
using System;

/// <summary>
/// トラッキングの情報を提供する
/// </summary>
public class TrackingPresenter : MonoBehaviour
{
    [SerializeField] Transform tranCenterEye = null;
    [SerializeField] Transform tranLeftHand = null;
    [SerializeField] Transform tranRightHand = null;
    [SerializeField] CoverUpController coverUpController = null;

    private bool isAdjustAbnormalPosition = false;
    private bool isSmooth = false;
    private bool isCoverUp = false;
    private List<OldPositions> oldPositions;
    private List<OldRotations> oldRotations;
    private List<int> adjustAbnormalPosCount;
    private List<int> skipAdjustAbnormalPosRemainCount;
    private List<int> maybeTrackingLostCount;
    private List<bool> maybeTrackingLost;
    private List<Vector3> currentPositions;
    private List<Quaternion> currentRotations;
    private int[] trackerIndexs = new int[] { TrackerIndex.HIP, TrackerIndex.LEFT_LEG, TrackerIndex.RIGHT_LEG };

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
        maybeTrackingLostCount = new List<int>() { 0,0,0 };
        maybeTrackingLost = new List<bool>() { false,false,false };
        
        currentPositions = new List<Vector3>() {
            Vector3.zero, Vector3.zero, Vector3.zero
        };

        currentRotations = new List<Quaternion>() {
            Quaternion.identity, Quaternion.identity, Quaternion.identity
        };
    }

    /// <summary>
    /// 現在の位置を返す
    /// </summary>
    /// <param name="index"></param>
    /// <returns></returns>
    public Vector3 GetCurrentPositionByIndex(int index)
    {
        return currentPositions[index];
    }

    /// <summary>
    /// 現在の回転を返す
    /// </summary>
    /// <param name="index"></param>
    /// <returns></returns>
    public Quaternion GetCurrentRotationByIndex(int index)
    {
        return currentRotations[index];
    }

    /// <summary>
    /// トラッキングの位置を計算する
    /// </summary>
    public void CalculateTracking()
    {
        for (var i=0; i<trackerIndexs.Length; ++i)
        {
            int index = trackerIndexs[i];
            
            // MEMO 前と前々を使うためAdjustPositionより先に行う
            if (isCoverUp)
            {
                CalculateMaybeTrackingLost(index);
            }

            // ロストしていても計算は行う
            currentPositions[index] = AdjustPosition(index, GetTrackingPositionByIndex(index));
            currentRotations[index] = AdjustRotation(index, GetTrackingRotationByIndex(index));

            if (isCoverUp && maybeTrackingLost[index]) {
                // トラッキングがロストしていそうなので、FinalIKの位置に書き換える
                currentPositions[index] = coverUpController.GetCoverUpPosition(index);
            }
        }
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
    
    private Quaternion GetTrackingRotationByIndex(int index)
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

        oldPositions[index].UpdateOriginalBefore(currentPos);

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
                TrackingConst.LERP_RATE);
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
                TrackingConst.LERP_RATE);
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

        float threshold = TrackingConst.THRESHOLD_MOVE_POS * Time.deltaTime;
        var estimatePos = GetEstimatePosition(index);

        if (Vector3.Distance(pos, estimatePos) >= threshold)
        {
            ++adjustAbnormalPosCount[index];

            if (adjustAbnormalPosCount[index] >= TrackingConst.MAX_ADJUST_ABNORMAL_POS)
            {
                adjustAbnormalPosCount[index] = 0;

                // 異常値調整が規定回数を超えた場合、一定期間調整をスキップするようにする
                skipAdjustAbnormalPosRemainCount[index] = TrackingConst.SKIP_ADJUST_ABNORMAL_POS;
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
    /// ロストしているかもしれないチェック
    /// </summary>
    /// <param name="index"></param>
    /// <returns></returns>
    private void CalculateMaybeTrackingLost(int index)
    {
        // HMDはロストしない
        if (index == TrackerIndex.HIP) return;

        var oldPoss = oldPositions[index];
        
        // 前回の値と前々回の値で計算する
        // 閾値よりも移動している場合はロストしていないとする
        float observedDistance = Vector3.Distance(oldPoss.OriginalPositionBefore, oldPoss.OriginalPositionBeforeLast);
        if (observedDistance > TrackingConst.THRESHOLD_TRACKING_LOST)
        {
            maybeTrackingLostCount[index] = Math.Max(maybeTrackingLostCount[index] - 1, 0);
            if (maybeTrackingLostCount[index] == 0) SetTrackingLost(index, false);
            return;
        }

        Vector3 acceleration = (index == TrackerIndex.RIGHT_LEG)
            ? OVRInput.GetLocalControllerAcceleration(OVRInput.Controller.RTouch)
            : OVRInput.GetLocalControllerAcceleration(OVRInput.Controller.LTouch);

        Vector3 calcPos = oldPoss.OriginalPositionBeforeLast + acceleration * Time.deltaTime;

        // 観測した移動値と、計測した移動値が閾値以内の場合はロストしていないとする
        float calcDistance = Vector3.Distance(calcPos, oldPoss.OriginalPositionBeforeLast);
        if (Math.Abs(observedDistance - calcDistance) < TrackingConst.THRESHOLD_TRACKING_LOST_DISTANCE)
        {
            maybeTrackingLostCount[index] = Math.Max(maybeTrackingLostCount[index] - 1, 0);
            if (maybeTrackingLostCount[index] == 0) SetTrackingLost(index, false);
            return;
        }
        
        maybeTrackingLostCount[index] = Math.Min(maybeTrackingLostCount[index] + 1, TrackingConst.TRACKING_LOST_CONTINUES_COUNT);
        
        // ロストしていそう
        if (maybeTrackingLostCount[index] == TrackingConst.TRACKING_LOST_CONTINUES_COUNT)
        {
            SetTrackingLost(index, true);
        }
    }

    private void SetTrackingLost(int index, bool isLost)
    {
        maybeTrackingLost[index] = isLost;
        coverUpController.SetTrackingLost(index, isLost);
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

    public void ChangeCoverUp(bool value)
    {
        isCoverUp = value;
    }

    /// <summary>
    /// 過去のPosition情報
    /// </summary>
    private class OldPositions
    {
        private Vector3 originalPositionBefore;
        private Vector3 originalPositionBeforeLast;
        private Vector3 positionBefore;
        private Vector3 positionBeforeLast;
        private float oldDeltaTime = 0f;

        public Vector3 OriginalPositionBefore => originalPositionBefore;
        public Vector3 OriginalPositionBeforeLast => originalPositionBeforeLast;
        public Vector3 PositionBefore => positionBefore;
        public Vector3 PositionBeforeLast => positionBeforeLast;
        public float OldDeltaTime => oldDeltaTime;

        public OldPositions(Vector3 before, Vector3 beforeLst)
        {
            positionBefore = before;
            positionBeforeLast = beforeLst;
            originalPositionBefore = before;
            originalPositionBeforeLast = before;
        }

        public void UpdateBefore(Vector3 before)
        {
            positionBeforeLast = positionBefore;
            positionBefore = before;
            oldDeltaTime = Time.deltaTime;
        }

        public void UpdateOriginalBefore(Vector3 before)
        {
            originalPositionBeforeLast = originalPositionBefore;
            originalPositionBefore = before;
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
