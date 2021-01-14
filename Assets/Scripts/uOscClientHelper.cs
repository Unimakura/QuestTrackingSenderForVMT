using System.Reflection;
using UnityEngine;

[RequireComponent(typeof(uOSC.uOscClient))]
public class uOscClientHelper : MonoBehaviour
{
    private uOSC.uOscClient uClient;

    private void Awake() {
        uClient = GetComponent<uOSC.uOscClient>();
    }

    /// <summary>
    /// 有効無効化
    /// </summary>
    /// <param name="status"></param>
    public void SetEnabled(bool status)
    {
        uClient.enabled = status;
    }

    /// <summary>
    /// uOscClientのOSC送信先アドレスを変更する
    /// </summary>
    /// <param name="address"></param>
    /// <param name="port"></param>
    public void ChangeOSCAddress(string address, int port)
    {
        var fieldInfo = typeof(uOSC.uOscClient).GetField("address", BindingFlags.NonPublic | BindingFlags.Instance);
        fieldInfo.SetValue(uClient, address);
        
        fieldInfo = typeof(uOSC.uOscClient).GetField("port", BindingFlags.NonPublic | BindingFlags.Instance);
        fieldInfo.SetValue(uClient, port);
    }
}
