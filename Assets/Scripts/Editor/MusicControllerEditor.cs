using UnityEngine;
using UnityEditor;
using UnityEditor.SceneManagement;

[CustomEditor(typeof(MusicController), true)]
[CanEditMultipleObjects]
public class MusicControllerEditor : Editor
{
    private MusicController musicController;
    private bool isGameScene;

    private void OnEnable()
    {
        musicController = (MusicController)target;
        isGameScene = SceneManagerAdapter.IsGameScene();
    }
    
    public override void OnInspectorGUI()
    {
        EditorGUILayout.LabelField("Celestia Theme", EditorStyles.boldLabel);
        MusicThemeField(musicController.celestiaTheme);
        
        EditorGUILayout.Space();
        EditorGUILayout.LabelField("Luna Theme", EditorStyles.boldLabel);
        MusicThemeField(musicController.lunaTheme);
        
        EditorGUILayout.Space();
        EditorGUILayout.LabelField("COOP Theme", EditorStyles.boldLabel);
        MusicThemeField(musicController.coopTheme);
        
        if (GUI.changed) SetObjectDirty();
    }
    
    private void MusicThemeField(MusicController.MusicTheme musicTheme)
    {
        MusicField("Main", ref musicTheme.main.hasIntro, ref musicTheme.main.intro, ref musicTheme.main.loop);
        
        if (!isGameScene) return;
        
        EditorGUILayout.Space();
        
        MusicField("Pause", ref musicTheme.pause.hasIntro, ref musicTheme.pause.intro, ref musicTheme.pause.loop);
        
        EditorGUILayout.Space();
        
        MusicField("Victory", ref musicTheme.victory.hasIntro, ref musicTheme.victory.intro, ref musicTheme.victory.loop);
    }

    private void MusicField(string label, ref bool hasIntro, ref AudioClip intro, ref AudioClip loop)
    {
        hasIntro = EditorGUILayout.Toggle($"Has {label} Intro", hasIntro);
        
        if (hasIntro)
        {
            intro = AudioClipField($"{label} Music (intro)", ref intro);
        }
        
        loop = AudioClipField($"{label} Music", ref loop);
    }
    
    private AudioClip AudioClipField(string label, ref AudioClip audioClip)
    {
        return (AudioClip)EditorGUILayout.ObjectField(label, audioClip, typeof(AudioClip), false);
    }
    
    private void SetObjectDirty()
    {
        EditorUtility.SetDirty(target);
        EditorSceneManager.MarkSceneDirty(musicController.gameObject.scene);
    }
}
