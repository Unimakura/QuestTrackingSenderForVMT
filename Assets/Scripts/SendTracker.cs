using UnityEngine;
using UnityEngine.XR;

[RequireComponent(typeof(uOSC.uOscClient))]
public class SendTracker : MonoBehaviour {
    [SerializeField] uOSC.uOscClient uClient = null;

    void LateUpdate()
    {
        SendBone();
    }

    void SendBone() {
        if (uClient == null) return;

        var pos = InputTracking.GetLocalPosition(XRNode.CenterEye);
        var rot = InputTracking.GetLocalRotation(XRNode.CenterEye);

        // 0: 腰
        if (pos != Vector3.zero) SendBoneTransformForVMT(0, pos, rot);

        pos = InputTracking.GetLocalPosition(XRNode.LeftHand);
        rot = InputTracking.GetLocalRotation(XRNode.LeftHand);

        // 1: 左足
        if (pos != Vector3.zero) SendBoneTransformForVMT(1, pos, rot);

        pos = InputTracking.GetLocalPosition(XRNode.RightHand);
        rot = InputTracking.GetLocalRotation(XRNode.RightHand);

        // 1: 右足
        if (pos != Vector3.zero) SendBoneTransformForVMT(2, pos, rot);
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

// /*
//  * SampleBonesSend
//  * gpsnmeajp
//  * https://sh-akira.github.io/VirtualMotionCaptureProtocol/
//  *
//  * These codes are licensed under CC0.
//  * http://creativecommons.org/publicdomain/zero/1.0/deed.ja
//  */
// using System;
// using System.Collections;
// using System.Collections.Generic;
// using UnityEngine;
// using UnityEngine.UI;
// using VRM;

// [RequireComponent(typeof(uOSC.uOscClient))]
// public class SampleBonesSend : MonoBehaviour
// {
//     [SerializeField] Text textHeadPos;
//     [SerializeField] Text textHeadRot;
//     [SerializeField] Text textSpinePos;
//     [SerializeField] Text textSpineRot;
//     uOSC.uOscClient uClient = null;

//     public GameObject Model = null;
//     public bool isBoneSend = true;
//     public bool isTrackerSend = true;
//     public bool isBlendSend = true;
//     public bool isVMTSend = true;
//     public bool isHeadSend = true;
//     public bool isSpineSend = true;
//     public bool isHipsSend = true;
//     public bool isLeftHandSend = true;
//     public bool isRightHandSend = true;
//     public bool isLeftFootSend = true;
//     public bool isRightFootSend = true;
//     public bool isLeftLowerArmSend = true;
//     public bool isRightLowerArmSend = true;
//     public bool isLeftLowerLegSend = true;
//     public bool isRightLowerLegSend = true;

//     private GameObject OldModel = null;

//     Animator animator = null;
//     VRMBlendShapeProxy blendShapeProxy = null;

//     private Vector3 basePosition;
//     private float height;
//     private const int ADJUST_HEIGHT = 15;
//     private float scale = 1;

//     public enum VirtualDevice
//     {
//         HMD = 0,
//         Controller = 1,
//         Tracker = 2,
//     }

//     void Awake() {
//         Application.targetFrameRate = 60; //60FPSに設定
//     }

//     void Start()
//     {
//         uClient = GetComponent<uOSC.uOscClient>();
//     }

//     void LateUpdate()
//     {
//         //モデルが更新されたときのみ読み込み
//         if (Model != null && OldModel != Model)
//         {
//             animator = Model.GetComponent<Animator>();
//             blendShapeProxy = Model.GetComponent<VRMBlendShapeProxy>();
//             OldModel = Model;
//         }

//         if (Model != null && animator != null && uClient != null)
//         {
//             //Root
//             var RootTransform = Model.transform;
//             if (isBoneSend && RootTransform != null)
//             {
//                 uClient.Send("/VMC/Ext/Root/Pos",
//                     "root",
//                     RootTransform.position.x, RootTransform.position.y, RootTransform.position.z,
//                     RootTransform.rotation.x, RootTransform.rotation.y, RootTransform.rotation.z, RootTransform.rotation.w);
                
//                 // SendTransformWithBasePosition("/VMC/Ext/Root/Pos", "root", RootTransform);
//             }

//             //Bones
//             if (isBoneSend)
//             {
//                 foreach (HumanBodyBones bone in Enum.GetValues(typeof(HumanBodyBones)))
//                 {
//                     if (bone != HumanBodyBones.LastBone)
//                     {
//                         var Transform = animator.GetBoneTransform(bone);
//                         if (Transform != null)
//                         {
//                             uClient.Send("/VMC/Ext/Bone/Pos",
//                                 bone.ToString(),
//                                 Transform.localPosition.x, Transform.localPosition.y, Transform.localPosition.z,
//                                 Transform.localRotation.x, Transform.localRotation.y, Transform.localRotation.z, Transform.localRotation.w);
//                         }
//                     }
//                 }
//             }

//             if (isTrackerSend)
//             {
//                 //ボーン位置を仮想トラッカーとして送信
//                 if (isHeadSend)          SendBoneTransformForTracker(HumanBodyBones.Head, "Head");
//                 if (isSpineSend)         SendBoneTransformForTracker(HumanBodyBones.Spine, "Spine");
//                 if (isHipsSend)          SendBoneTransformForTracker(HumanBodyBones.Hips, "Hips");
//                 if (isLeftHandSend)      SendBoneTransformForTracker(HumanBodyBones.LeftHand, "LeftHand");
//                 if (isRightHandSend)     SendBoneTransformForTracker(HumanBodyBones.RightHand, "RightHand");
//                 if (isLeftFootSend)      SendBoneTransformForTracker(HumanBodyBones.LeftFoot, "LeftFoot");
//                 if (isRightFootSend)     SendBoneTransformForTracker(HumanBodyBones.RightFoot, "RightFoot");
//                 if (isLeftLowerArmSend)  SendBoneTransformForTracker(HumanBodyBones.LeftLowerArm, "LeftLowerArm");
//                 if (isRightLowerArmSend) SendBoneTransformForTracker(HumanBodyBones.RightLowerArm, "RightLowerArm");
//                 if (isLeftLowerLegSend)  SendBoneTransformForTracker(HumanBodyBones.LeftLowerLeg, "LeftLowerLeg");
//                 if (isRightLowerLegSend) SendBoneTransformForTracker(HumanBodyBones.RightLowerLeg, "RightLowerLeg");
//             }

//             if (isVMTSend)
//             {
//                 //ボーン位置を仮想トラッカー(VMT)として送信
//                 if (isHeadSend)          SendBoneTransformForVMT(HumanBodyBones.Head, 1);
//                 if (isSpineSend)         SendBoneTransformForVMT(HumanBodyBones.Spine, 2);
//                 if (isHipsSend)          SendBoneTransformForVMT(HumanBodyBones.Hips, 3);
//                 if (isLeftHandSend)      SendBoneTransformForVMT(HumanBodyBones.LeftHand, 4);
//                 if (isRightHandSend)     SendBoneTransformForVMT(HumanBodyBones.RightHand, 5);
//                 if (isLeftFootSend)      SendBoneTransformForVMT(HumanBodyBones.LeftFoot, 6);
//                 if (isRightFootSend)     SendBoneTransformForVMT(HumanBodyBones.RightFoot, 7);
//                 if (isLeftLowerArmSend)  SendBoneTransformForVMT(HumanBodyBones.LeftLowerArm, 8);
//                 if (isRightLowerArmSend) SendBoneTransformForVMT(HumanBodyBones.RightLowerArm, 9);
//                 if (isLeftLowerLegSend)  SendBoneTransformForVMT(HumanBodyBones.LeftLowerLeg, 10);
//                 if (isRightLowerLegSend) SendBoneTransformForVMT(HumanBodyBones.RightLowerLeg, 11);
//             }

//             //BlendShape
//             if (isBlendSend && blendShapeProxy != null)
//             {
//                 foreach (var b in blendShapeProxy.GetValues())
//                 {
//                     uClient.Send("/VMC/Ext/Blend/Val",
//                         b.Key.ToString(),
//                         (float)b.Value
//                         );
//                 }
//                 uClient.Send("/VMC/Ext/Blend/Apply");
//             }

//             //Available
//             uClient.Send("/VMC/Ext/OK", 1);
//         }
//         else
//         {
//             uClient.Send("/VMC/Ext/OK", 0);
//         }
//         uClient.Send("/VMC/Ext/T", Time.time);
//     }

//     void SendBoneTransformForTracker(HumanBodyBones bone, string DeviceSerial)
//     {
//         var DeviceTransform = animator.GetBoneTransform(bone);
//         if (DeviceTransform != null) {
//             // uClient.Send("/VMC/Ext/Tra/Pos",
//             // (string)DeviceSerial,
//             // (float)DeviceTransform.position.x,
//             // (float)DeviceTransform.position.y,
//             // (float)DeviceTransform.position.z,
//             // (float)DeviceTransform.rotation.x,
//             // (float)DeviceTransform.rotation.y,
//             // (float)DeviceTransform.rotation.z,
//             // (float)DeviceTransform.rotation.w);
//             // SendBoneTransformWithBasePosition("/VMC/Ext/Tra/Pos", (string)DeviceSerial, DeviceTransform);

//             var tmpPosition = (DeviceTransform.position - basePosition) * scale;

//             uClient.Send("/VMC/Ext/Tra/Pos",
//                 (string)DeviceSerial,
//                 (float)tmpPosition.x,
//                 (float)tmpPosition.y,
//                 (float)tmpPosition.z,
//                 (float)DeviceTransform.rotation.x,
//                 (float)DeviceTransform.rotation.y,
//                 (float)DeviceTransform.rotation.z,
//                 (float)DeviceTransform.rotation.w);

//             // if (DeviceSerial == "Head") {
//             //     textHeadPos.text = tmpPosition.ToString();
//             //     textHeadRot.text = DeviceTransform.rotation.eulerAngles.ToString();
//             // }
//             // else if (DeviceSerial == "Spine") {
//             //     textSpinePos.text = tmpPosition.ToString();
//             //     textSpineRot.text = DeviceTransform.rotation.eulerAngles.ToString();
//             // }
//         }
//     }

//     void SendBoneTransformForVMT(HumanBodyBones bone, int index)
//     {
//         var DeviceTransform = animator.GetBoneTransform(bone);
//         if (DeviceTransform != null) {
//             var tmpPosition = (DeviceTransform.position - basePosition) * scale;

//             uClient.Send("/VMT/Room/Unity",
//                 index, // 識別番号
//                 (int)1, // 有効可否
//                 (float)0f, // 補正時間
//                 (float)tmpPosition.x,
//                 (float)tmpPosition.y,
//                 (float)tmpPosition.z,
//                 (float)DeviceTransform.rotation.x,
//                 (float)DeviceTransform.rotation.y,
//                 (float)DeviceTransform.rotation.z,
//                 (float)DeviceTransform.rotation.w);
//         }
//     }

//     // void SendBoneTransformWithBasePosition(string address, string key, Transform _transform) {
//     //     var tmpPosition = (_transform.position - basePosition) * scale;

//     //     uClient.Send(address,
//     //         key,
//     //         (float)tmpPosition.x,
//     //         (float)tmpPosition.y,
//     //         (float)tmpPosition.z,
//     //         (float)_transform.rotation.x,
//     //         (float)_transform.rotation.y,
//     //         (float)_transform.rotation.z,
//     //         (float)_transform.rotation.w);
//     // }

//     public void ResetBasePosition() {
//         if (animator == null) {
//             return;
//         }

//         var leftFoot = animator.GetBoneTransform(HumanBodyBones.LeftFoot);
//         var rightFoot = animator.GetBoneTransform(HumanBodyBones.RightFoot);
//         if (leftFoot != null && rightFoot != null) {
//             basePosition = (leftFoot.position + rightFoot.position) * 0.5f;

//             var head = animator.GetBoneTransform(HumanBodyBones.Head);
//             scale = (float)(((height - ADJUST_HEIGHT) / 100f) / (head.position.y - basePosition.y));
//         }
//     }

//     public void SetHeight(float _height) {
//         height = _height;
//     }
// }
