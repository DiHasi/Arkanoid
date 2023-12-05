using GameEntitiesLibrary;
using Newtonsoft.Json;

namespace Tests;

public class TestsFacadeLoadLevels
{
    [Fact]
    public void LoadLevels_FileDoesNotExist_ReturnsEmptyList()
    {
        var facade = new JsonFacade();
        var result = facade.LoadLevels("nonexistentfile.json");
        Assert.Empty(result);
    }

    [Fact]
    public void LoadLevels_FileExistsButEmpty_ReturnsEmptyList()
    {
        var facade = new JsonFacade();
        File.WriteAllText("emptyfile.json", "");
        var result = facade.LoadLevels("emptyfile.json");
        File.Delete("emptyfile.json");
        Assert.Empty(result);
    }

    [Fact]
    public void LoadLevels_FileExistsAndValid_ReturnsLevelList()
    {
        var facade = new JsonFacade();
        var blockConfigurations = new List<BlockConfiguration> 
        { 
            new BlockConfiguration(new Block(BlockType.Ordinary), 0, 0) 
        };
        var additionalParameters = new AdditionalParameters
        {
            MagnetBonus = 1,
            PlatformEnlargementBonus = 0,
            BallAccelerationBonus = 0
        };
        var levels = new List<Level> 
        { 
            new Level(blockConfigurations, additionalParameters) 
        };
        File.WriteAllText("validfile.json", JsonConvert.SerializeObject(levels));
        var result = facade.LoadLevels("validfile.json");
        File.Delete("validfile.json");
        Assert.Equal(levels.Count, result.Count);
    }
}