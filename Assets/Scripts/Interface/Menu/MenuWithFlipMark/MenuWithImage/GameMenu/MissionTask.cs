using UnityEngine;
using TMPro;

public class MissionTask : MonoBehaviour
{
    public bool IsDone => currentCounter == targetCounter;
    public bool IsCounterZero => currentCounter == 0;
    
    [SerializeField] private GameObject checkMark;
    [SerializeField] private GameObject strikethrough;
    [SerializeField] private LocalizationBase taskText;
    [SerializeField] private TextMeshProUGUI counterText;
    [SerializeField] private int targetCounter = 1;

    private int currentCounter;
    private bool isCounterCheck;
    
    public void Initialized(string file, string taskNumber)
    {
        taskText.UpdatePhrase(file, taskNumber);
        isCounterCheck = targetCounter > 1;

        if (isCounterCheck)
        {
            counterText.gameObject.SetActive(true);
            counterText.text = $"[{currentCounter}/{targetCounter}]";
        }
        
        gameObject.SetActive(true);
    }

    public void AddCounter()
    {
        if (IsDone) return;
        
        currentCounter++;

        if (isCounterCheck)
        {
            counterText.text = $"[{currentCounter}/{targetCounter}]";
        }

        if (currentCounter == targetCounter)
        {
            SetCheckDone(true);
        }
    }

    public void SubtractCounter()
    {
        if (IsCounterZero) return;
        
        if (IsDone)
        {
            SetCheckDone(false);
        }

        currentCounter--;
        
        if (isCounterCheck)
        {
            counterText.text = $"[{currentCounter}/{targetCounter}]";
        }
    }

    public void OnReset()
    {
        currentCounter = 0;
        
        if (isCounterCheck)
        {
            counterText.text = $"[{currentCounter}/{targetCounter}]";
        }
        
        SetCheckDone(false);
    }

    private void SetCheckDone(bool value)
    {
        checkMark.SetActive(value);
        strikethrough.SetActive(value);
    }
}