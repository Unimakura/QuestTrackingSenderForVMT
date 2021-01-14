using System;
using UnityEngine;
using TMPro;

public class UIEvent : MonoBehaviour
{
    [SerializeField] SendTracker sendTracker = null;
    [SerializeField] TMP_InputField inputIP = null;
    [SerializeField] TMP_InputField inputPort = null;
    [SerializeField] SendingLabelAnimation labelAnimation = null;
    [SerializeField] uOscClientHelper uocHelper = null;

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
            labelAnimation.Start();
        }
        else
        {
            labelAnimation.Stop();
        }

        uocHelper.SetEnabled(status);
        sendTracker.ChangeSendStatus(status);
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
}
