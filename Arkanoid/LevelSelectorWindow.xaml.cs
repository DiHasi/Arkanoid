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
    private readonly IServiceProvider _serviceProvider;
    private readonly List<Level> _levels;
    private readonly LevelState _levelState;
    private readonly List<Button> _buttons = new();

    public LevelSelectorWindow(IServiceProvider serviceProvider)
    {
        InitializeComponent();
        _serviceProvider = serviceProvider;
        _levels = _serviceProvider.GetRequiredService<List<Level>>();
        _levelState = _serviceProvider.GetRequiredService<LevelState>();
        
        LoadButtons();
    }
    
    private void ActivateButtons()
    {
        var currentUser = _levelState.CurrentUser;

        //var maxLevel = _levels.Count;
        var currentLevel = currentUser.LevelNumber;
        for (var i = 0; i <= currentLevel; i++)
            _buttons[i].IsEnabled = true;
    }

    private void LoadButtons()
    {
        foreach (UIElement element in Grid.Children)
            if (element is Button button)
            {
                button.Click += Button_Click;
                _buttons.Add(button);
            }

        ActivateButtons();
    }

    private void Button_Click(object sender, RoutedEventArgs e)
    {
        var clickedButton = (Button)sender;
        _levelState.CurrentLevel = _levels[_buttons.IndexOf(clickedButton)]!;

        var gameWindow = new GameWindow(_serviceProvider);
        gameWindow.Show();
        Close();
    }

    private void BackButton_OnClick(object sender, RoutedEventArgs e)
    {
        new StartWindow(_serviceProvider).Show();
        Close();
    }

    private void QuitButton_OnClick(object sender, RoutedEventArgs e)
    {
        Close();
    }
}