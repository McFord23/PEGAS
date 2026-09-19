using UnityEditor;

[CustomEditor(typeof(MissionManager), true)]
[CanEditMultipleObjects]
public class MissionManagerEditor : Editor
{
    private MissionManager missionManager;
    private SerializedProperty submenu;
    private SerializedProperty initializeMode;
    private SerializedProperty tasksTargetCounter;
    private SerializedProperty tasksTargetCounterFromObject;
    
    private void OnEnable()
    {
        missionManager = (MissionManager)target;
        submenu = serializedObject.FindProperty( nameof( missionManager.submenu ));
        initializeMode = serializedObject.FindProperty( nameof( missionManager.initializeMode ));
        tasksTargetCounter = serializedObject.FindProperty( nameof( missionManager.tasksTargetCount ));
        tasksTargetCounterFromObject = serializedObject.FindProperty( nameof( missionManager.tasksTargetCountFromObject ));
    }

    public override void OnInspectorGUI()
    {
        serializedObject.Update();
        
        EditorGUILayout.PropertyField(submenu);
        EditorGUILayout.PropertyField(initializeMode);
        
        switch (missionManager.initializeMode)
        {
            case MissionManager.InitializeMode.FromNumber:
                EditorGUILayout.PropertyField(tasksTargetCounter);
                break;
            
            case MissionManager.InitializeMode.FromObject:
                EditorGUILayout.PropertyField(tasksTargetCounterFromObject);
                break;
        }
        
        serializedObject.ApplyModifiedProperties();
    }
}