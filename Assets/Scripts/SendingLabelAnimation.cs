using UnityEngine;
using TMPro;

public class SendingLabelAnimation : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI textStartButton = null;

    private int sendingLabelAnimationStep = 0;

    private void Awake() {
        textStartButton.text = Label.START;
    }

    /// <summary>
    /// NOW SENDINGアニメーション
    /// </summary>
    public void Run()
    {
        ++sendingLabelAnimationStep;

        if (sendingLabelAnimationStep == 25)
        {
            sendingLabelAnimationStep = 0;
            textStartButton.text = Label.SENDING;
        }
        else if (sendingLabelAnimationStep % 5 == 0)
        {
            textStartButton.text = "<" + textStartButton.text + ">";
        }
    }

    /// <summary>
    /// NOW SENDING 表示開始
    /// </summary>
    public void StartAnimation()
    {
        sendingLabelAnimationStep = 0;
        textStartButton.text = Label.SENDING;
    }

    /// <summary>
    /// NOW SENDING 表示停止
    /// </summary>
    public void StopAnimation()
    {
        textStartButton.text = Label.START;
    }
}
