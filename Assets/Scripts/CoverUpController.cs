using UnityEngine;
using RootMotion.FinalIK;

public class CoverUpController : MonoBehaviour
{
    [SerializeField] private Transform tranTrackingSpace;
    [SerializeField] private Transform tranHMD;
    [SerializeField] private Transform tranCoverUpTarget;
    [SerializeField] private Transform tranLeftLeg;
    [SerializeField] private Transform tranRightLeg;
    [SerializeField] private VRIK vrik;

    // Update is called once per frame
    void Update()
    {
        tranCoverUpTarget.position = tranTrackingSpace.InverseTransformPoint(tranHMD.position);
        tranCoverUpTarget.rotation = tranHMD.rotation;
    }

    public Vector3 GetCoverUpPosition(int index)
    {
        Vector3 pos = (index == TrackerIndex.LEFT_LEG)
            ? tranLeftLeg.position
            : tranRightLeg.position;
        

        return tranTrackingSpace.InverseTransformPoint(pos);
    }

    public void SetTrackingLost(int index, bool isLost)
    {
        if (index == TrackerIndex.RIGHT_LEG)
        {
            vrik.solver.rightLeg.positionWeight = (isLost) ? 0 : 1;
        }
        else
        {
            vrik.solver.leftLeg.positionWeight = (isLost) ? 0 : 1;
        }
    }
}
