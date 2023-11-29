using System.Windows.Shapes;

namespace GameEntitiesLibrary;

public class LevelState
{
    public Dictionary<Rectangle, Block> BlockDictionary = new();
    public Level? CurrentLevel;
    public User CurrentUser;
}