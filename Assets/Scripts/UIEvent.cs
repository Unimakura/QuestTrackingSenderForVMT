using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UIEvent : MonoBehaviour
{
    [SerializeField] SendTracker sendTracker = null;
    [SerializeField] TMP_InputField inputIP = null;
    [SerializeField] TMP_InputField inputPort = null;
    [SerializeField] TextMeshProUGUI textFpsButton = null;
    [SerializeField] SendingLabelAnimation labelAnimation = null;
    [SerializeField] uOscClientHelper uocHelper = null;
    [SerializeField] Toggle toggleAdjustAbnormalPosition = null;

    private IList<int> fpsList = new List<int>() {
        72, 36, 18, 9
    };

    private int fpsIndex = 0;

    private void Update()
    {
        if (sendTracker.IsSending)
        {
            labelAnimation.Run();
        }
    }

    /// <summary>
    /// STARTボタン処理
    /// </summary>
    public void OnStartButton()
    {
        bool status = !sendTracker.IsSending;
        ChangeStatus(status);
    }

    private void ChangeStatus(bool status)
    {
        if (status)
        {
            uocHelper.ChangeOSCAddress(inputIP.text, Int32.Parse(inputPort.text));
            UpdateSendTrackerInterval();
            labelAnimation.Start();
        }
        else
        {
            labelAnimation.Stop();
        }

        uocHelper.SetEnabled(status);
        sendTracker.ChangeSendStatus(status);
    }

    private void UpdateSendTrackerInterval()
    {
        int interval = fpsIndex + 1;
        sendTracker.SetInterval(interval);
    }

    /// <summary>
    /// IPのテキスト変更処理
    /// </summary>
    public void OnChangeIP()
    {
        PlayerPrefs.SetString(PlayerPrefsKey.IP, inputIP.text);
        ChangeStatus(false);
    }

    /// <summary>
    /// Portのテキスト変更処理
    /// </summary>
    public void OnChangePort()
    {
        PlayerPrefs.SetString(PlayerPrefsKey.PORT, inputPort.text);
        ChangeStatus(false);
    }

    /// <summary>
    /// FPSの変更処理
    /// </summary>
    public void OnChangeFPS()
    {
        ++fpsIndex;

        if (fpsIndex >= fpsList.Count)
        {
            fpsIndex = 0;
        }
        PlayerPrefs.SetInt(PlayerPrefsKey.FPS_INDEX, fpsIndex);

        textFpsButton.text = fpsList[fpsIndex].ToString();
        UpdateSendTrackerInterval();
    }

    public void OnChangeAdjustAbnormalPosition()
    {
        PlayerPrefs.SetInt(PlayerPrefsKey.ADJUST_ABNORMAL_POSITION,
            (toggleAdjustAbnormalPosition.isOn) ? 1 : 0);
        sendTracker.ChangeAbnormalAdjustPosition(toggleAdjustAbnormalPosition.isOn);
    }

    /// <summary>
    /// IPの値をセットする
    /// </summary>
    /// <param name="text"></param>
    public void SetIP(string text)
    {
        inputIP.text = text;
    }

    /// <summary>
    /// Portの値をセットする
    /// </summary>
    /// <param name="text"></param>
    public void SetPort(string text)
    {
        inputPort.text = text;
    }

    /// <summary>
    /// FPSの値をセットする
    /// </summary>
    /// <param name="text"></param>
    public void SetFpsIndex(int index)
    {
        fpsIndex = index;
        textFpsButton.text = fpsList[fpsIndex].ToString();
    }

    /// <summary>
    /// AdjustAbnormalPositionのフラグセット
    /// </summary>
    /// <param name="value"></param>
    public void SetAdjustAbnormalPosition(int value)
    {
        toggleAdjustAbnormalPosition.isOn = (value == 1) ? true : false;
    }
}
