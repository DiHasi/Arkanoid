using GameEntitiesLibrary;
using Newtonsoft.Json;
using Record = GameEntitiesLibrary.Record;

namespace Tests;

public class FacadeUpdateRecordsTests
{
    [Fact]
    public void UpdateRecords_WhenNewUser_AddsUserRecordToFile()
    {
        var facade = new JsonFacade();
        var fileName = "records1.json";
        var currentUser = new User("Alice") { Record = 100 };
        var records = new List<Record>(); 

        facade.UpdateRecords(fileName, records, currentUser);

        var serializedRecords = File.ReadAllText(fileName);
        var result = JsonConvert.DeserializeObject<List<Record>>(serializedRecords);

        File.Delete(fileName);

        Assert.Single(result!);
        Assert.Equal(currentUser.Name, result?[0].PlayerName);
        Assert.Equal(currentUser.Record, result?[0].RecordValue);
    }

    [Fact]
    public void UpdateRecords_WhenExistingUser_UpdatesUserRecordAndOrderRecords()
    {
        var facade = new JsonFacade();
        var fileName = "records2.json";
        var currentUser = new User("Alice") { Record = 150 };
        var records = new List<Record>
        {
            new Record { PlayerName = "Bob", RecordValue = 200 },
            new Record { PlayerName = "Charlie", RecordValue = 100 }
        };

        facade.UpdateRecords(fileName, records, currentUser);

        var serializedRecords = File.ReadAllText(fileName);
        var result = JsonConvert.DeserializeObject<List<Record>>(serializedRecords);

        File.Delete(fileName);

        Assert.Equal(3, result?.Count);
        Assert.Equal(currentUser.Name, result?[1].PlayerName);
        Assert.Equal(currentUser.Record, result?[1].RecordValue);
        Assert.Equal("Bob", result?[0].PlayerName);
        Assert.Equal(200, result?[0].RecordValue);
        Assert.Equal("Charlie", result?[2].PlayerName);
        Assert.Equal(100, result?[2].RecordValue);
    }
}