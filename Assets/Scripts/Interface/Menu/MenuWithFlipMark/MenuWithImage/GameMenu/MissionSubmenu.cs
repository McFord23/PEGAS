using UnityEngine;
using UnityEngine.UI;

public class MissionSubmenu : MonoBehaviour
{
    [Header("Mission")]
    [SerializeField] private Sprite missionSprite;
    [SerializeField] private Image missionImage;
    [SerializeField] private Vector2 missionImageSize;
    [SerializeField] private bool isFlipImage;
    
    [SerializeField] private MissionTaskLabel[] missionTasks;
    
    [Header("Descriptions")]
    [SerializeField] private LocalizationBase pauseDescription;
    [SerializeField] private LocalizationBase failedDescription;
    [SerializeField] private LocalizationBase passedDescription;

    public void Initialize(int tasksDoneTarget)
    {
        missionImage.sprite = missionSprite;
        missionImage.rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, missionImageSize.x);
        missionImage.rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, missionImageSize.y);
        missionImage.rectTransform.localScale = isFlipImage 
            ? new Vector3(-1, 1, 1) 
            : new Vector3(1, 1, 1);
        
        var level = SceneManagerAdapter.GetActiveScene().ToString();

        for (var i = 0; i < tasksDoneTarget; i++)
        {
            missionTasks[i].Initialized($"{level}/Mission", i.ToString());
        }
        
        var infoFile = $"{level}/Info";
        pauseDescription.UpdatePhrase(infoFile, "description");
        failedDescription.UpdatePhrase(infoFile, "failedDescription");
        passedDescription.UpdatePhrase(infoFile, "passedDescription");
    }

    public MissionTaskLabel[] GetTasksLabels()
    {
        return missionTasks;
    }
}