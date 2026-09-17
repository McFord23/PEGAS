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
    
    private void OnEnable()
    {
        tilesManager = (TilesManager)target;
        progressIncreased = serializedObject.FindProperty( nameof( tilesManager.progressIncreaseEvent ));
        progressDecreased = serializedObject.FindProperty( nameof( tilesManager.progressDecreaseEvent ));
    }

    public override void OnInspectorGUI()
    {
        serializedObject.Update();
        EditorGUILayout.PropertyField(progressIncreased);
        EditorGUILayout.PropertyField(progressDecreased);
        serializedObject.ApplyModifiedProperties();
        
        if (GUILayout.Button("Check Walls"))
        {
            for (int i = 0; i < tilesManager.transform.childCount; i++)
            {
                var tile = tilesManager.transform.GetChild(i).GetComponent<Tile>();
                tile.CheckWalls();
            }
            
            EditorUtility.SetDirty(target);
            EditorSceneManager.MarkSceneDirty(tilesManager.gameObject.scene);
        }
    }
}