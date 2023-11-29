using System.Collections.Generic;
using System.IO;
using System.Windows;
using GameEntitiesLibrary;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;

namespace Arkanoid;

/// <summary>
///     Interaction logic for App.xaml
/// </summary>
public partial class App : Application
{
    protected override void OnStartup(StartupEventArgs e)
    {
        base.OnStartup(e);
        var services = new ServiceCollection();
        services.AddSingleton(new List<Level>());
        services.AddSingleton(new LevelState());

        var userList = new List<User>();

        if (File.Exists("users.json"))
        {
            var jsonData = File.ReadAllText("users.json");
            userList = JsonConvert.DeserializeObject<List<User>>(jsonData);
        }

        if (userList != null) services.AddSingleton(userList);

        var serviceProvider = services.BuildServiceProvider();

        var startWindow = new StartWindow(serviceProvider);
        startWindow.Show();
    }
}