public class LocalizationControlHint : LocalizationText
{
    protected override void Start()
    {
        file = SceneManagerAdapter.IsGameScene() 
            ? $"{SceneManagerAdapter.GetActiveScene().ToString()}/Info"
            : "Interface";
        
        base.Start();
    }
}