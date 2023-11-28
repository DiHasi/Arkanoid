namespace LevelLibrary;

public enum BlockType
{
    Ordinary,
    Strong,
    Bonus
}

public class Block
{
    private int Health { get; set; }
    public Block(BlockType type)
    {
        Type = type;
        Health = type switch
        {
            BlockType.Ordinary => 1,
            BlockType.Strong => 2,
            BlockType.Bonus => 1,
            _ => Health
        };
    }

    public void Hit()
    {
        Health--;
    }
    
    public bool IsDestroyed()
    {
        return Health <= 0;
    }
    
    public BlockType Type { get; set; }
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