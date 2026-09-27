public class LocalizationGameLevelHint : LocalizationText
{
    protected override void Start()
    {
        file = LevelsManager.IsGameLevel() 
            ? $"{LevelsManager.GetActiveLevel().ToString()}/{file}"
            : "Interface";
        
        base.Start();
    }
}