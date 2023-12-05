namespace GameEntitiesLibrary;

public class User
{
    public User(string name)
    {
        Name = name;
    }

    public string Name { get; }
    public int Record { get; set; }
    public int LevelNumber { get; set; } = 0;

    public Dictionary<int, int> LevelScores { get; set; } = new();

    public void IncreaseLevel()
    {
        LevelNumber++;
    }

    public void CalculateRecord()
    {
        Record = 0;
        foreach (var value in LevelScores.Values) Record += value;
    }
}