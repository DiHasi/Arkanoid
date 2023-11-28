using System.Windows.Shapes;


namespace LevelLibrary;

public class LevelFacade
{
    private static LevelFacade _instance;
    public List<Level>? Levels = new List<Level>();
    public Dictionary<Rectangle, Block> BlockDictionary = new Dictionary<Rectangle, Block>();
    public Level CurrentLevel;

    private LevelFacade() {}

    public static LevelFacade Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = new LevelFacade();
            }
            return _instance;
        }
    }
}