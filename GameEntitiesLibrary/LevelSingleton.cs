using System.Windows.Shapes;

namespace GameEntitiesLibrary;

public class LevelSingleton 
{
    public Dictionary<Rectangle, Block> BlockDictionary = new();
    public Level? CurrentLevel;

    public LevelSingleton()
    {
    }
}