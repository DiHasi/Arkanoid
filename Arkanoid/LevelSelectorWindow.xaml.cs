using System;
using System.Collections.Generic;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using GameEntitiesLibrary;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;

namespace Arkanoid;

public partial class LevelSelectorWindow : Window
{
    private readonly List<Button> buttons = new();
    private readonly IServiceProvider _serviceProvider;

    public LevelSelectorWindow(IServiceProvider serviceProvider)
    {
        InitializeComponent();
        _serviceProvider = serviceProvider;

        LoadLevels();
        LoadButtons();
    }

    private void LoadLevels()
    {
        var json = File.ReadAllText("levels.json");
        var levels = _serviceProvider.GetRequiredService<List<Level>>();
        levels.AddRange(JsonConvert.DeserializeObject<List<Level>>(json)!);
    }

    private void ActivateButtons()
    {
        var levels = _serviceProvider.GetRequiredService<List<Level>>();
        for (var i = 0; i < levels.Count; i++)
            buttons[i].IsEnabled = true;
    }

    private void LoadButtons()
    {
        foreach (UIElement element in Grid.Children)
            if (element is Button button)
            {
                button.Click += Button_Click;
                buttons.Add(button);
            }

        ActivateButtons();
    }

    private void Button_Click(object sender, RoutedEventArgs e)
    {
        var levelSingleton = _serviceProvider.GetRequiredService<LevelSingleton>();
        var levels = _serviceProvider.GetRequiredService<List<Level>>();
        var clickedButton = (Button)sender;
        levelSingleton.CurrentLevel = levels[buttons.IndexOf(clickedButton)]!;

        var gameWindow = new GameWindow(_serviceProvider);
        gameWindow.Show();

        var levelSelectorWindow = GetWindow(clickedButton);
        levelSelectorWindow!.Hide();
    }
}