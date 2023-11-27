using System;
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
    private double DefaultPlatformPositionX { get; } = 350;
    private double PlatformWidth { get; } = 100;
    private Stopwatch Stopwatch { get; }
    private double PreviousTime { get; set; }
    private double PlatformPositionX { get; set; }

    private double BallSpeed { get; }
    private double BallHeight { get; }
    private double BallPositionX { get; set; }
    private double BallPositionY { get; set; }
    private double BallSpeedX { get; set; }
    private double BallSpeedY { get; set; }

    public GameWindow(Level level)
    {
        InitializeComponent();
        LoadLevel(level);
        Stopwatch = new Stopwatch();
        Stopwatch.Start();
        PreviousTime = 0;
        PlatformPositionX = DefaultPlatformPositionX;
        BallHeight = 20;
        BallPositionX = Canvas.GetLeft(Ball);
        BallPositionY = Canvas.GetTop(Ball);
        BallSpeedX = -1;
        BallSpeedY = 2;
        BallSpeed = 300;
        CompositionTarget.Rendering += CompositionTarget_Rendering;
    }

    private void LoadLevel(Level level)
    {
        foreach (BlockConfiguration blockConfig in level.BlockConfigurations)
        {
            Block block = blockConfig.Block;
            int x = blockConfig.PositionX;
            int y = blockConfig.PositionY;
            
            Rectangle blockRectangle = new Rectangle
            {
                Width = 50,
                Height = 30,
                Fill = new SolidColorBrush(Colors.Orange),
                Tag = "block"
            };
            
            Canvas.SetLeft(blockRectangle, x);
            Canvas.SetTop(blockRectangle, y);
            
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
                    BallSpeedY = 0;
                    BallSpeedX = 0;
                    break;
                case "Platform":
                    BallSpeedY *= -1;
                    break;
            }
        }
        else if (result.VisualHit is Rectangle { Tag: "block" } block)
        {
            if (Canvas.GetTop(block) + 30 <= Canvas.GetTop(Ball) || Canvas.GetTop(block) >= Canvas.GetTop(Ball) + 20)
            {
                BallSpeedY *= -1;
            }
            else
            {
                BallSpeedX *= -1;
            }
            Canvas.Children.Remove(block);
        }

        return HitTestResultBehavior.Continue;
    }
}