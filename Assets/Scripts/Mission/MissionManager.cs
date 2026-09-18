using System.Collections.Generic;
using UnityEngine;

public class MissionManager : MonoBehaviour
{
    public MissionSubmenu submenu;
    [Tooltip("Максимум 4 задания, ибо больше не помещается в подменю миссии\n\n1 - одноразовое задание\n>1 - задание со счётчиком")]
    public List<int> tasksTargetCounter;
    [Tooltip("Максимум 4 задания, ибо больше не помещается в подменю миссии")]
    public List<Transform> tasksTargetCounterFromChildCount;

    public InitializeMode initializeMode = InitializeMode.Standard;
    
    public enum InitializeMode
    {
        Standard,
        ChildCount
    }
    
    private readonly List<MissionTask> missionTasks = new();
    private int tasksDoneTarget;
    private int tasksDoneAmount;
    
    private void Start()
    {
        if (!SceneManagerAdapter.IsGameScene()) return;

        if (initializeMode is InitializeMode.ChildCount)
        {
            tasksTargetCounter.Clear();
            foreach (var targetTransform in tasksTargetCounterFromChildCount)
            {
                tasksTargetCounter.Add(targetTransform.childCount);
            }
        }
        
        tasksDoneTarget = tasksTargetCounter.Count;
        
        submenu.Initialize(tasksDoneTarget);
        var labels = submenu.GetTasksLabels();
        
        for (var i = 0; i < tasksDoneTarget; i++)
        {
            var missionTask = new MissionTask(tasksTargetCounter[i], labels[i]);
            missionTasks.Add(missionTask);
        }
    }

    public void OnReset()
    {
        tasksDoneAmount = 0;
        
        foreach (var missionTask in missionTasks)
        {
            missionTask.OnReset();
        }
    }
    
    public void IncreaseTaskCounter(int taskIndex)
    {
        if (missionTasks[taskIndex].IsDone) return;
        
        missionTasks[taskIndex].IncreaseCounter();

        if (missionTasks[taskIndex].IsDone)
        {
            tasksDoneAmount++;
        }
        
        if (tasksDoneAmount == tasksDoneTarget)
        {
            EventAdapter.Instance.Execute(EventKey.Victory);
        }
    }

    public void DecreaseTaskCounter(int taskIndex)
    {
        if (missionTasks[taskIndex].IsCounterZero) return;
        
        if (missionTasks[taskIndex].IsDone)
        {
            tasksDoneAmount--;
        }
        
        missionTasks[taskIndex].DecreaseCounter();
    }
}