using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(uOSC.uOscClient))]
public class MainScene : MonoBehaviour
{
    [SerializeField] LaserPointer laserPointer = null;
    [SerializeField] UIEvent uiEvent = null;

    private uOSC.uOscClient uClient;

    void Awake()
    {
        Initialize();
    }

    /// <summary>
    /// 初期化
    /// </summary>
    private void Initialize()
    {
        uClient = GetComponent<uOSC.uOscClient>();
        laserPointer.laserBeamBehavior = LaserPointer.LaserBeamBehavior.OnWhenHitTarget;

        var ip = DefaultValue.IP;
        var port = DefaultValue.PORT;

        if (PlayerPrefs.HasKey(PlayerPrefsKey.IP))
        {
            ip = PlayerPrefs.GetString(PlayerPrefsKey.IP);
        }
        if (PlayerPrefs.HasKey(PlayerPrefsKey.PORT))
        {
            port = PlayerPrefs.GetString(PlayerPrefsKey.PORT);
        }

        uClient.enabled = false;
        
        uiEvent.SetIP(ip);
        uiEvent.SetPort(port);
    }
}
