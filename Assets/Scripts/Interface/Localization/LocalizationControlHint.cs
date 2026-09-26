public class LocalizationControlHint : LocalizationText
{
    protected override void Start()
    {
        file = LevelManager.IsGameLevel() 
            ? $"{LevelManager.GetActiveLevel().ToString()}/Info"
            : "Interface";
        
        base.Start();
    }
}