using UnityEngine;
using UnityEngine.UI;

public class MissionSubmenu : MonoBehaviour
{
    [Header("Mission")]
    [SerializeField] private Sprite missionSprite;
    [SerializeField] private Image missionImage;
    [SerializeField] private Vector2 missionImageSize;
    [SerializeField] private bool isFlipImage;
    
    [SerializeField] private MissionTask[] missionTasks;
    private int tasksDoneTarget;
    private int tasksDoneAmount;
    
    [Header("Descriptions")]
    [SerializeField] private LocalizationBase pauseDescription;
    [SerializeField] private LocalizationBase failedDescription;
    [SerializeField] private LocalizationBase passedDescription;

    private void Start()
    {
        missionImage.sprite = missionSprite;
        missionImage.rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, missionImageSize.x);
        missionImage.rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, missionImageSize.y);
        missionImage.rectTransform.localScale = isFlipImage 
            ? new Vector3(-1, 1, 1) 
            : new Vector3(1, 1, 1);
        
        var level = SceneManagerAdapter.GetActiveScene().ToString();
        var missionFile = $"Languages/{LocalizationManager.CurrentLanguage}/{level}/Mission";
        tasksDoneTarget = JsonReader<string, string>.LoadFile(missionFile).Count;

        for (var i = 0; i < tasksDoneTarget; i++)
        {
            missionTasks[i].Initialized($"{level}/Mission", i.ToString());
        }
        
        var infoFile = $"{level}/Info";
        pauseDescription.UpdatePhrase(infoFile, "description");
        failedDescription.UpdatePhrase(infoFile, "failedDescription");
        passedDescription.UpdatePhrase(infoFile, "passedDescription");
    }

    public void OnReset()
    {
        tasksDoneAmount = 0;
        
        foreach (var missionTask in missionTasks)
        {
            missionTask.OnReset();
        }
    }
    
    public void AddTaskCounter(int taskIndex)
    {
        if (missionTasks[taskIndex].IsDone) return;
        
        missionTasks[taskIndex].AddCounter();

        if (missionTasks[taskIndex].IsDone)
        {
            tasksDoneAmount++;
        }
        
        if (tasksDoneAmount == tasksDoneTarget)
        {
            EventAdapter.Instance.Execute(EventKey.Victory);
        }
    }

    public void SubtractTaskCounter(int taskIndex)
    {
        if (missionTasks[taskIndex].IsCounterZero) return;
        
        if (missionTasks[taskIndex].IsDone)
        {
            tasksDoneAmount--;
        }
        
        missionTasks[taskIndex].SubtractCounter();
    }
}