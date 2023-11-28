using System;
using System.Collections.Generic;
using System.Windows;
using GameEntitiesLibrary;
using Microsoft.Extensions.DependencyInjection;


namespace Arkanoid;

/// <summary>
///     Interaction logic for App.xaml
/// </summary>
public partial class App : Application
{
    private List<Level> _levels = new();
    protected override void OnStartup(StartupEventArgs e)
    {
        base.OnStartup(e);
        var services = new ServiceCollection();
        services.AddSingleton(new List<Level>());
        services.AddSingleton(new LevelSingleton());

        var serviceProvider = services.BuildServiceProvider();
        
        var levelSelectorWindow = new LevelSelectorWindow(serviceProvider);
        levelSelectorWindow.Closed += (sender, e) =>
        {
            Current.Shutdown();
        };
        Current.MainWindow!.Hide();
        levelSelectorWindow.Show();
    }

}