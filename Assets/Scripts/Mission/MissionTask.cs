public class MissionTask
{
    public bool IsDone { get; private set; }
    public bool IsCounterZero => currentCounter == 0;

    private readonly MissionTaskLabel missionTaskLabel;
    private readonly int targetCounter;
    
    private int currentCounter;

    public MissionTask(int target, MissionTaskLabel label)
    {
        targetCounter = target;
        missionTaskLabel = label;
        missionTaskLabel.EnableCounter(targetCounter);
    }

    public void IncreaseCounter()
    {
        if (IsDone) return;
        
        currentCounter++;
        UpdateCounterLabel();

        if (currentCounter == targetCounter)
        {
            IsDone = true;
            missionTaskLabel.SetCheckDone(true);
        }
    }

    public void DecreaseCounter()
    {
        if (IsCounterZero) return;
        
        currentCounter--;
        UpdateCounterLabel();
        
        if (IsDone)
        {
            missionTaskLabel.SetCheckDone(false);
        }

        IsDone = false;
    }

    public void OnReset()
    {
        IsDone = false;
        currentCounter = 0;
        
        UpdateCounterLabel();
        missionTaskLabel.SetCheckDone(false);
    }

    private void UpdateCounterLabel()
    {
        missionTaskLabel.SetCounterText(currentCounter, targetCounter);
    }
}