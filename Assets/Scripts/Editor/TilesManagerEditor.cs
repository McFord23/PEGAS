using UnityEngine;
using UnityEditor;
using UnityEditor.SceneManagement;

[CustomEditor(typeof(TilesManager), true)]
[CanEditMultipleObjects]
public class TilesManagerEditor : Editor
{
    private TilesManager tilesManager;

    private SerializedProperty progressObjects;
    private SerializedProperty progressIncreased;
    private SerializedProperty progressDecreased;

    private SerializedProperty floor;
    private SerializedProperty wallPrefab;
    private SerializedProperty wallsParent;
    
    private void OnEnable()
    {
        tilesManager = (TilesManager)target;

        progressObjects = serializedObject.FindProperty(TilesManager.Fields.PROGRESS_OBJECTS);
        progressIncreased = serializedObject.FindProperty(TilesManager.Fields.PROGRESS_INCREASE_EVENT);
        progressDecreased = serializedObject.FindProperty(TilesManager.Fields.PROGRESS_DECREASE_EVENT);
        
        floor = serializedObject.FindProperty(TilesManager.Fields.FLOOR);
        wallPrefab = serializedObject.FindProperty(TilesManager.Fields.WALL_PREFAB);
        wallsParent = serializedObject.FindProperty(TilesManager.Fields.WALLS_PARENT);
    }

    public override void OnInspectorGUI()
    {
        serializedObject.Update();
        EditorGUILayout.PropertyField(progressObjects);
        EditorGUILayout.PropertyField(progressIncreased);
        EditorGUILayout.PropertyField(progressDecreased);
        EditorGUILayout.Space();
        EditorGUILayout.PropertyField(floor);
        EditorGUILayout.PropertyField(wallPrefab);
        EditorGUILayout.PropertyField(wallsParent);
        serializedObject.ApplyModifiedProperties();
        
        EditorGUILayout.Space();
        
        if (GUILayout.Button("Assign Tiles"))
        {
            tilesManager.AssignTiles();
            EditorUtility.SetDirty(target);
            EditorSceneManager.MarkSceneDirty(tilesManager.gameObject.scene);
        }
        
        if (GUILayout.Button("Generate Wall Colliders"))
        {
            tilesManager.GenerateWalls();
            EditorUtility.SetDirty(target);
            EditorSceneManager.MarkSceneDirty(tilesManager.gameObject.scene);
        }
    }
}