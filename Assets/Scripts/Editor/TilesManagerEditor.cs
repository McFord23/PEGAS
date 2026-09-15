using UnityEngine;
using UnityEditor;
using UnityEditor.SceneManagement;

[CustomEditor(typeof(TilesManager), true)]
[CanEditMultipleObjects]
public class TilesManagerEditor : Editor
{
    private TilesManager tilesManager;
    
    private void OnEnable()
    {
        tilesManager = (TilesManager)target;
    }

    public override void OnInspectorGUI()
    {
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