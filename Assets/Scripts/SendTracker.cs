using UnityEngine;

[RequireComponent(typeof(uOSC.uOscClient))]
public class SendTracker : MonoBehaviour {
    [SerializeField] private uOSC.uOscClient uClient = null;
    [SerializeField] private Transform tranCenterEye = null;
    [SerializeField] private Transform tranLeftHand = null;
    [SerializeField] private Transform tranRightHand = null;

    void LateUpdate()
    {
        SendBone();
    }

    void SendBone() {
        if (uClient == null) return;

        // 0: 腰
        SendBoneTransformForVMT(0, tranCenterEye.localPosition, tranCenterEye.localRotation);

        // 1: 左足
        SendBoneTransformForVMT(1, tranLeftHand.localPosition, tranLeftHand.localRotation);

        // 2: 右足
        SendBoneTransformForVMT(2, tranRightHand.localPosition, tranRightHand.localRotation);
    }

    void SendBoneTransformForVMT(int index, Vector3 pos, Quaternion rot)
    {
        uClient.Send("/VMT/Room/Unity",
            index, // 識別番号
            (int)1, // 有効可否
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
