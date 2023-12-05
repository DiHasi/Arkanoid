using System.Windows;
using GameEntitiesLibrary;
using Microsoft.Extensions.DependencyInjection;

namespace Arkanoid;

/// <summary>
///     Interaction logic for App.xaml
/// </summary>
public partial class App
{
    private readonly JsonFacade _jsonFacade = new();
    protected override void OnStartup(StartupEventArgs e)
    {
        base.OnStartup(e);
        var services = new ServiceCollection();
        services.AddSingleton(new LevelState());
        services.AddSingleton(_jsonFacade);
        services.AddSingleton(_jsonFacade.LoadUsers("users.json"));
        services.AddSingleton(_jsonFacade.LoadLevels("levels.json"));
        services.AddSingleton(_jsonFacade.LoadRecords("records.json"));

        var serviceProvider = services.BuildServiceProvider();

        var startWindow = new StartWindow(serviceProvider);
        startWindow.Show();
    }
}