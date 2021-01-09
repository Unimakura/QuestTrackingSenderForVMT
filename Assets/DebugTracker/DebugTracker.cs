using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR;

public class DebugTracker : MonoBehaviour
{
    [SerializeField] Transform trnHMD;
    [SerializeField] Transform trnLHand;
    [SerializeField] Transform trnRHand;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        var pos = InputTracking.GetLocalPosition(XRNode.CenterEye);
        var rot = InputTracking.GetLocalRotation(XRNode.CenterEye);

        if (pos != Vector3.zero)
        {
            trnHMD.localPosition = pos;
            trnHMD.localRotation = rot;
        }

        pos = InputTracking.GetLocalPosition(XRNode.LeftHand);
        rot = InputTracking.GetLocalRotation(XRNode.LeftHand);

        if (pos != Vector3.zero)
        {
            trnLHand.localPosition = pos;
            trnLHand.localRotation = rot;
        }

        pos = InputTracking.GetLocalPosition(XRNode.RightHand);
        rot = InputTracking.GetLocalRotation(XRNode.RightHand);

        if (pos != Vector3.zero)
        {
            trnRHand.localPosition = pos;
            trnRHand.localRotation = rot;
        }
    }
}
