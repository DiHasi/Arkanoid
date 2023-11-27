using System;
using System.Collections.Generic;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using LevelLibrary;
using Newtonsoft.Json;

namespace Arkanoid;

public partial class LevelSelectorWindow : Window
{
    private List<Button> buttons = new List<Button>();
    private List<Level>? levels = new List<Level>();
    public LevelSelectorWindow()
    {
        InitializeComponent();

        List<BlockConfiguration> blockConfigurations1 = new List<BlockConfiguration>();
        int startX = 30;
        int startY = 30;
        int blockWidth = 60;
        int blockHeight = 50;

        for (int row = 0; row < 6; row++)
        {
            for (int col = 0; col < 12; col++)
            {
                int positionX = startX + col * blockWidth;
                int positionY = startY + row * blockHeight;

                blockConfigurations1.Add(new BlockConfiguration(new Block(BlockType.Ordinary), positionX, positionY));
            }
        }
        
        Level level1 = new Level
        (
            blockConfigurations1,
            new AdditionalParameters
            {
                MagnetBonus = 0.5,
                PlatformEnlargementBonus = 0,
                BallAccelerationBonus = 0.5
            }
        );
        levels.Add(level1);
        
        var json = JsonConvert.SerializeObject(levels, Formatting.Indented);
        File.WriteAllText("levels.json", json);
        
        
        // var json = File.ReadAllText("levels.json");
        // levels = JsonConvert.DeserializeObject<List<Level>>(json);
        

        foreach (UIElement element in Grid.Children)
        {
            if (element is Button button)
            {
                button.Click += Button_Click;
                buttons.Add(button);
            }
        }

        if (levels != null)
            for (int i = 0; i < levels.Count; i++)
            {
                buttons[i].IsEnabled = true;
            }
        
        
    }
    private void Button_Click(object sender, RoutedEventArgs e)
    {
        Button clickedButton = (Button)sender;
        Level levelNumber = levels?[buttons.IndexOf(clickedButton)]!;
        
        GameWindow gameWindow = new GameWindow(levelNumber);
        gameWindow.Show();
        
        Window levelSelectorWindow = Window.GetWindow(clickedButton);
        levelSelectorWindow.Close();
    }
}