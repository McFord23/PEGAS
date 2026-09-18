using UnityEditor;

[CustomEditor(typeof(MissionManager), true)]
[CanEditMultipleObjects]
public class MissionManagerEditor : Editor
{
    private MissionManager missionManager;
    private SerializedProperty submenu;
    private SerializedProperty initializeMode;
    private SerializedProperty tasksTargetCounter;
    private SerializedProperty tasksTargetCounterFromChildCount;
    
    private void OnEnable()
    {
        missionManager = (MissionManager)target;
        submenu = serializedObject.FindProperty( nameof( missionManager.submenu ));
        initializeMode = serializedObject.FindProperty( nameof( missionManager.initializeMode ));
        tasksTargetCounter = serializedObject.FindProperty( nameof( missionManager.tasksTargetCounter ));
        tasksTargetCounterFromChildCount = serializedObject.FindProperty( nameof( missionManager.tasksTargetCounterFromChildCount ));
    }

    public override void OnInspectorGUI()
    {
        serializedObject.Update();
        
        EditorGUILayout.PropertyField(submenu);
        EditorGUILayout.PropertyField(initializeMode);
        
        switch (missionManager.initializeMode)
        {
            case MissionManager.InitializeMode.Standard:
                EditorGUILayout.PropertyField(tasksTargetCounter);
                break;
            
            case MissionManager.InitializeMode.ChildCount:
                EditorGUILayout.PropertyField(tasksTargetCounterFromChildCount);
                break;
        }
        
        serializedObject.ApplyModifiedProperties();
    }
}