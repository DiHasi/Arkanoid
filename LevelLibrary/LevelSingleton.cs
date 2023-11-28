using System.Windows.Shapes;

namespace LevelLibrary;

public class LevelSingleton
{
    private static LevelSingleton _instance;
    public Dictionary<Rectangle, Block> BlockDictionary = new();
    public Level CurrentLevel;
    public List<Level>? Levels = new();

    private LevelSingleton()
    {
    }

    public static LevelSingleton Instance
    {
        get
        {
            if (_instance == null) _instance = new LevelSingleton();
            return _instance;
        }
    }
}