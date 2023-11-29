namespace GameEntitiesLibrary;

public enum BlockType
{
    Ordinary,
    Strong,
    Bonus
}

public class Block
{
    public Block(BlockType type)
    {
        Type = type;
        MaxHealth = type switch
        {
            BlockType.Ordinary => 1,
            BlockType.Strong => 2,
            BlockType.Bonus => 1,
            _ => MaxHealth
        };
    }

    private int Health { get; set; }
    private int MaxHealth { get; }

    public BlockType Type { get; set; }

    public void Hit()
    {
        Health--;
    }

    public bool IsDestroyed()
    {
        return Health <= 0;
    }

    public void RestoreHealth()
    {
        Health = MaxHealth;
    }
}

public class BlockConfiguration
{
    public BlockConfiguration(Block block, int positionX, int positionY)
    {
        Block = block;
        PositionX = positionX;
        PositionY = positionY;
    }

    public Block Block { get; set; }
    public int PositionX { get; set; }
    public int PositionY { get; set; }
}

public class AdditionalParameters
{
    public double MagnetBonus { get; set; }
    public double PlatformEnlargementBonus { get; set; }
    public double BallAccelerationBonus { get; set; }
}

public class Level
{
    public Level(List<BlockConfiguration> blockConfiguration, AdditionalParameters additionalParameters)
    {
        BlockConfigurations = blockConfiguration;
        AdditionalParameters = additionalParameters;
    }
    

    public List<BlockConfiguration> BlockConfigurations { get; set; }
    public AdditionalParameters AdditionalParameters { get; set; }
}