using GameEntitiesLibrary;
using Newtonsoft.Json;

namespace Tests;

public class TestsFacadeLoadUsers
{
    [Fact]
    public void LoadUsers_ShouldReturnEmptyList_WhenFileDoesNotExist()
    {
        var jsonFacade = new JsonFacade();
        var fileName = "nonexistent-file.json";

        var result = jsonFacade.LoadUsers(fileName);

        Assert.Empty(result);
    }

    [Fact]
    public void LoadUsers_ShouldReturnDeserializedList_WhenFileExists()
    {
        var jsonFacade = new JsonFacade();
        var fileName = "existing-file.json";
        var users = new List<User> { new User("John")  , new User("Jane") };
        var json = JsonConvert.SerializeObject(users);
        File.WriteAllText(fileName, json);

        var result = jsonFacade.LoadUsers(fileName);

        Assert.Equal(users.Count, result.Count);
        Assert.Equal(users[0].Name, result[0].Name);
        Assert.Equal(users[1].Name, result[1].Name);

        File.Delete(fileName);
    }

    [Fact]
    public void LoadUsers_ShouldReturnEmptyList_WhenFileIsEmpty()
    {
        var jsonFacade = new JsonFacade();
        var fileName = "empty-file.json";
        File.WriteAllText(fileName, "");
        
        var result = jsonFacade.LoadUsers(fileName);
        
        Assert.Empty(result);
        
        File.Delete(fileName);
    }
}