namespace GameEntitiesLibrary;
using System.Collections.Generic;
using System.IO;
using Newtonsoft.Json;

public class JsonFacade
{
    public List<User> LoadUsers(string fileName)
    {
        if (!File.Exists(fileName)) return new List<User>();
        var json = File.ReadAllText(fileName);
        return JsonConvert.DeserializeObject<List<User>>(json) ?? new List<User>();

    }
    public List<Level> LoadLevels(string fileName)
    {
        if (!File.Exists(fileName)) return new List<Level>();
        var json = File.ReadAllText(fileName);
        return JsonConvert.DeserializeObject<List<Level>>(json) ?? new List<Level>();
    }

    
    public List<Record> LoadRecords(string fileName)
    {
        if(!File.Exists(fileName)) return new List<Record>();
        var json = File.ReadAllText(fileName);
        return JsonConvert.DeserializeObject<List<Record>>(json) ?? new List<Record>();
    }

    public void UpdateUsers(string fileName, List<User> users)
    {
        var serializedUsers = JsonConvert.SerializeObject(users);
        File.WriteAllText(fileName, serializedUsers);
    }
    public void UpdateRecords(string fileName, List<Record> records, User currentUser)
    {
        var userRecord = records.FirstOrDefault(r => r.PlayerName == currentUser.Name);
        if (userRecord == null)
        {
            userRecord = new Record
                { PlayerName = currentUser.Name, RecordValue = currentUser.Record };
            records.Add(userRecord);
        }
        else
        {
            userRecord.RecordValue = currentUser.Record;
        }

        records = records.OrderByDescending(r => r.RecordValue).ToList();

        var serializedRecords = JsonConvert.SerializeObject(records);
        File.WriteAllText(fileName, serializedRecords);
    }
    
}