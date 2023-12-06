using GameEntitiesLibrary;
using Newtonsoft.Json;

namespace Tests;

public class FacadeUpdateUsersTests
{
    [Fact]
    public void UpdateUsers_WhenFileDoesNotExist_CreatesFileAndWritesUsers()
    {
        var facade = new JsonFacade();
        var users = new List<User> { new User("Alice") , new User("Bob") };
        var fileName = "newfile.json";

        facade.UpdateUsers(fileName, users);

        Assert.True(File.Exists(fileName));

        var serializedUsers = File.ReadAllText(fileName);
        var result = JsonConvert.DeserializeObject<List<User>>(serializedUsers);

        File.Delete(fileName);

        Assert.Equal(users.Count, result!.Count);
        Assert.Equal(users[0].Name, result[0].Name);
        Assert.Equal(users[1].Name, result[1].Name);
    }

    [Fact]
    public void UpdateUsers_WhenFileExists_OverwritesExistingUsers()
    {
        var facade = new JsonFacade();
        var initialUsers = new List<User> { new User("Alice")  };
        var newUsers = new List<User> { new User("Bob"), new User("Charlie")  };
        var fileName = "existingfile.json";

        File.WriteAllText(fileName, JsonConvert.SerializeObject(initialUsers));

        facade.UpdateUsers(fileName, newUsers);

        var serializedUsers = File.ReadAllText(fileName);
        var result = JsonConvert.DeserializeObject<List<User>>(serializedUsers);

        File.Delete(fileName);

        Assert.Equal(newUsers.Count, result!.Count);
        Assert.Equal(newUsers[0].Name, result[0].Name);
        Assert.Equal(newUsers[1].Name, result[1].Name);
    }
}