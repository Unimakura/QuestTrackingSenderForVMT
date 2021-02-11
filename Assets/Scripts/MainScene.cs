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
        Screen.sleepTimeout = SleepTimeout.NeverSleep;

        uClient = GetComponent<uOSC.uOscClient>();
        laserPointer.laserBeamBehavior = LaserPointer.LaserBeamBehavior.OnWhenHitTarget;

        var ip = DefaultValue.IP;
        var port = DefaultValue.PORT;
        int fpsIndex = DefaultValue.FPS_INDEX;
        int adjustAbnormalPosition = DefaultValue.ADJUST_ABNORMAL_POSITION;
        int smooth = DefaultValue.SMOOTH;
        int coverUp = DefaultValue.COVER_UP;

        if (PlayerPrefs.HasKey(PlayerPrefsKey.IP))
        {
            ip = PlayerPrefs.GetString(PlayerPrefsKey.IP);
        }
        if (PlayerPrefs.HasKey(PlayerPrefsKey.PORT))
        {
            port = PlayerPrefs.GetString(PlayerPrefsKey.PORT);
        }
        if (PlayerPrefs.HasKey(PlayerPrefsKey.FPS_INDEX))
        {
            fpsIndex = PlayerPrefs.GetInt(PlayerPrefsKey.FPS_INDEX);
        }
        if (PlayerPrefs.HasKey(PlayerPrefsKey.ADJUST_ABNORMAL_POSITION))
        {
            adjustAbnormalPosition = PlayerPrefs.GetInt(PlayerPrefsKey.ADJUST_ABNORMAL_POSITION);
        }
        if (PlayerPrefs.HasKey(PlayerPrefsKey.SMOOTH))
        {
            smooth = PlayerPrefs.GetInt(PlayerPrefsKey.SMOOTH);
        }
        if (PlayerPrefs.HasKey(PlayerPrefsKey.COVER_UP))
        {
            coverUp = PlayerPrefs.GetInt(PlayerPrefsKey.COVER_UP);
        }

        uClient.enabled = false;
        
        uiEvent.SetIP(ip);
        uiEvent.SetPort(port);
        uiEvent.SetFpsIndex(fpsIndex);
        uiEvent.SetAdjustAbnormalPosition(adjustAbnormalPosition);
        uiEvent.SetSmooth(smooth);
        uiEvent.SetCoverUp(coverUp);
    }
}
