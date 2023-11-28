﻿using System.Collections.Generic;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using LevelLibrary;
using Newtonsoft.Json;

namespace Arkanoid;

public partial class LevelSelectorWindow : Window
{
    LevelFacade instance = LevelFacade.Instance;
    public LevelSelectorWindow()
    {
        InitializeComponent();
        
        LoadLevels();
        LoadButtons();
    }
    
    private List<Button> buttons = new();
    private List<Level>? levels = new();
    
    private void LoadLevels()
    {
        var json = File.ReadAllText("levels.json");
        
        instance.Levels = JsonConvert.DeserializeObject<List<Level>>(json);
    }

    private void ActivateButtons()
    {
        if (instance.Levels != null)
            for (var i = 0; i < instance.Levels.Count; i++)
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
        var clickedButton = (Button)sender;
        instance.CurrentLevel = instance.Levels?[buttons.IndexOf(clickedButton)]!;

        var gameWindow = new GameWindow();
        gameWindow.Show();

        var levelSelectorWindow = GetWindow(clickedButton);
        levelSelectorWindow?.Close();
    }
}