using UnityEngine;
using TMPro;

public class MissionTaskLabel : MonoBehaviour
{
    [SerializeField] private GameObject checkMark;
    [SerializeField] private GameObject strikethrough;
    [SerializeField] private LocalizationBase taskText;
    [SerializeField] private TextMeshProUGUI counterText;
    
    private bool isCounterCheck;
    
    public void Initialized(string file, string taskNumber)
    {
        taskText.UpdatePhrase(file, taskNumber);
        gameObject.SetActive(true);
    }

    public void EnableCounter(int targetCounter)
    {
        isCounterCheck = targetCounter > 1;
        
        if (isCounterCheck)
        {
            counterText.gameObject.SetActive(true);
            SetCounterText(0, targetCounter);
        }
    }

    public void SetCounterText(int currentCounter, int targetCounter)
    {
        if (isCounterCheck)
        {
            counterText.text = $"[{currentCounter}/{targetCounter}]";
        }
    }
    
    public void SetCheckDone(bool value)
    {
        checkMark.SetActive(value);
        strikethrough.SetActive(value);
    }
}