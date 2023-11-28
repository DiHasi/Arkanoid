using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Shapes;
using LevelLibrary;

namespace Arkanoid;

public partial class GameWindow : Window
{
    private const double PlatformSpeed = 500;

    public GameWindow()
    {
        InitializeComponent();
        instance = LevelFacade.Instance;
        LoadLevel();
        Stopwatch = new Stopwatch();
        Stopwatch.Start();
        PreviousTime = 0;
        PlatformPositionX = DefaultPlatformPositionX;
        BallHeight = 20;
        BallPositionX = Canvas.GetLeft(Ball);
        BallPositionY = Canvas.GetTop(Ball);
        BallSpeedX = -1;
        BallSpeedY = 2;
        BallSpeed = 200;
        CompositionTarget.Rendering += CompositionTarget_Rendering;

    }

    private double DefaultPlatformPositionX { get; } = 350;
    private double PlatformWidth { get; } = 100;
    private Stopwatch Stopwatch { get; }
    private double PreviousTime { get; set; }
    private double PlatformPositionX { get; set; }

    private double BallSpeed { get; set; }
    private double BallHeight { get; }
    private double BallPositionX { get; set; }
    private double BallPositionY { get; set; }
    private double BallSpeedX { get; set; }
    private double BallSpeedY { get; set; }

    private LevelFacade instance;
    

    private void LoadLevel()
    {
        foreach (var blockConfig in instance.CurrentLevel.BlockConfigurations)
        {
            var block = blockConfig.Block;
            
            var blockRectangle = new Rectangle
            {
                Width = 50,
                Height = 30,
                Tag = LevelFacade.Instance.CurrentLevel.BlockConfigurations.IndexOf(blockConfig)
            };
            
            // эти switch просто 🔥🔥🔥🔥
            blockRectangle.Fill = block.Type switch
            {
                BlockType.Ordinary => new SolidColorBrush(Colors.DarkOrange),
                BlockType.Strong => new SolidColorBrush(Colors.MediumOrchid),
                BlockType.Bonus => new SolidColorBrush(Colors.CornflowerBlue),
                _ => blockRectangle.Fill
            };

            instance.BlockDictionary[blockRectangle] = blockConfig.Block;

            Canvas.SetLeft(blockRectangle, blockConfig.PositionX);
            Canvas.SetTop(blockRectangle, blockConfig.PositionY);

            Canvas.Children.Add(blockRectangle);
        }
    }

    private void HitTestBounce()
    {
        var ballBounds = new Rect(BallPositionX, BallPositionY, BallHeight, BallHeight);

        var ballGeometry = new EllipseGeometry(ballBounds);
        var hitTestParams = new GeometryHitTestParameters(ballGeometry);

        HitTestResultCallback hitTestCallback = HitTestCallback;

        VisualTreeHelper.HitTest(Canvas, null, hitTestCallback, hitTestParams);
    }

    private void MovePlatform(double deltaTime)
    {
        if (Keyboard.IsKeyDown(Key.A))
        {
            if (PlatformPositionX - PlatformSpeed * deltaTime < 10)
                PlatformPositionX = 10;
            else
                PlatformPositionX -= PlatformSpeed * deltaTime;
        }
        else if (Keyboard.IsKeyDown(Key.D))
        {
            if (PlatformPositionX + PlatformWidth + PlatformSpeed * deltaTime > 774)
                PlatformPositionX = 774 - PlatformWidth;
            else
                PlatformPositionX += PlatformSpeed * deltaTime;
        }

        Canvas.SetLeft(Platform, PlatformPositionX);
    }

    private void MoveBall(double deltaTime)
    {
        BallPositionX += BallSpeedX * deltaTime * BallSpeed;
        BallPositionY += BallSpeedY * deltaTime * BallSpeed;

        HitTestBounce();

        Canvas.SetLeft(Ball, BallPositionX);
        Canvas.SetTop(Ball, BallPositionY);
    }

    private void CompositionTarget_Rendering(object? sender, EventArgs e)
    {
        var currentTime = Stopwatch.Elapsed.TotalSeconds;
        var deltaTime = currentTime - PreviousTime;

        MovePlatform(deltaTime);
        MoveBall(deltaTime);

        PreviousTime = currentTime;
    }

    private HitTestResultBehavior HitTestCallback(HitTestResult result)
    {
        if (result.VisualHit is Rectangle { Tag: "base" } rectangle)
        {
            switch (rectangle.Name)
            {
                case "LeftWall":
                case "RightWall":
                    BallSpeedX *= -1;
                    break;
                case "TopWall":
                    BallSpeedY *= -1;
                    break;
                case "BottomWall":
                    Application.Current.Shutdown();
                    break;
                case "Platform":
                    BallSpeedY *= -1;
                    break;
            }
        }
        else if (result.VisualHit is Rectangle { Tag: int } hitBlock)
        {
            if (Canvas.GetTop(hitBlock) + 30 <= Canvas.GetTop(Ball) || Canvas.GetTop(hitBlock) >= Canvas.GetTop(Ball) + 20)
                BallSpeedY *= -1;
            else
                BallSpeedX *= -1;
            
            if (instance.BlockDictionary.TryGetValue(hitBlock, out var block))
            {
                block.Hit();
                if (block.IsDestroyed())
                {
                    Canvas.Children.Remove(hitBlock);
                    instance.BlockDictionary.Remove(hitBlock);
                }

                if (block.Type == BlockType.Strong && !block.IsDestroyed())
                {
                    hitBlock.Fill = new SolidColorBrush(Colors.DodgerBlue);
                }
            }
        }

        return HitTestResultBehavior.Continue;
    }
}