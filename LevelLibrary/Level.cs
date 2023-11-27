namespace LevelLibrary;

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
