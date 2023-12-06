using GameEntitiesLibrary;
using Newtonsoft.Json;
using Record = GameEntitiesLibrary.Record;

namespace Tests;

public class FacadeLoadRecordsTests
{
    [Fact]
    public void LoadRecords_FileDoesNotExist_ReturnsEmptyList()
    {
        var facade = new JsonFacade();
        var result = facade.LoadRecords("nonexistentfile.json");
        Assert.Empty(result);
    }

    [Fact]
    public void LoadRecords_FileExistsButEmpty_ReturnsEmptyList()
    {
        var facade = new JsonFacade();
        File.WriteAllText("emptyfile1.json", "");
        var result = facade.LoadRecords("emptyfile1.json");
        File.Delete("emptyfile1.json");
        Assert.Empty(result);
    }

    [Fact]
    public void LoadRecords_FileExistsAndValid_ReturnsRecordList()
    {
        var facade = new JsonFacade();
        var records = new List<Record> { new Record() };
        File.WriteAllText("validfile.json", JsonConvert.SerializeObject(records));
        var result = facade.LoadRecords("validfile.json");
        File.Delete("validfile.json");
        Assert.Equal(records.Count, result.Count);
    }
}