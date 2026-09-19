using UnityEngine;
using UnityEditor;
using UnityEditor.SceneManagement;

[CustomEditor(typeof(TilesManager), true)]
[CanEditMultipleObjects]
public class TilesManagerEditor : Editor
{
    private TilesManager tilesManager;
    
    private SerializedProperty progressIncreased;
    private SerializedProperty progressDecreased;

    private SerializedProperty floor;
    private SerializedProperty wallPrefab;
    private SerializedProperty wallsParent;
    private SerializedProperty tilesClusters;
    
    private void OnEnable()
    {
        tilesManager = (TilesManager)target;
        
        progressIncreased = serializedObject.FindProperty( nameof( tilesManager.progressIncreaseEvent ));
        progressDecreased = serializedObject.FindProperty( nameof( tilesManager.progressDecreaseEvent ));
        
        floor = serializedObject.FindProperty( nameof( tilesManager.floor ));
        wallPrefab = serializedObject.FindProperty( nameof( tilesManager.wallPrefab ));
        wallsParent = serializedObject.FindProperty( nameof( tilesManager.wallsParent ));
        tilesClusters = serializedObject.FindProperty( nameof( tilesManager.tilesClusters ));
    }

    public override void OnInspectorGUI()
    {
        serializedObject.Update();
        EditorGUILayout.PropertyField(progressIncreased);
        EditorGUILayout.PropertyField(progressDecreased);
        EditorGUILayout.Space();
        EditorGUILayout.PropertyField(floor);
        EditorGUILayout.PropertyField(wallPrefab);
        EditorGUILayout.PropertyField(wallsParent);
        EditorGUILayout.PropertyField(tilesClusters);
        serializedObject.ApplyModifiedProperties();
        
        EditorGUILayout.Space();
        if (GUILayout.Button("Check Walls"))
        {
            tilesManager.GenerateWalls();
            EditorUtility.SetDirty(target);
            EditorSceneManager.MarkSceneDirty(tilesManager.gameObject.scene);
        }
    }
}