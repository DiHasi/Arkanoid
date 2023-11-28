using System;
using System.Diagnostics;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Shapes;
using LevelLibrary;

namespace Arkanoid;

public partial class GameWindow
{
    private const double PlatformSpeed = 500;


    private const double PlatformWidth = 100;
    private const double DefaultPlatformPositionX = 350;


    private const double DefaultBallPositionX = 430;
    private const double DefaultBallPositionY = 500;

    private const double BallSpeed = 200;
    private const double BallHeight = 20;
    private const double DefaultBallSpeedX = -1;
    private const double DefaultBallSpeedY = 2;

    private readonly LevelSingleton instance;

    public GameWindow()
    {
        InitializeComponent();
        instance = LevelSingleton.Instance;
        LoadLevel();
        CompositionTarget.Rendering += CompositionTarget_Rendering;
    }

    private void ConfigureElements()
    {
        PlatformPositionX = DefaultPlatformPositionX;
        BallPositionX = DefaultBallPositionX;
        BallPositionY = DefaultBallPositionY;
        BallSpeedX = DefaultBallSpeedX;
        BallSpeedY = DefaultBallSpeedY;
    }

    private double PlatformPositionX { get; set; }
    private Stopwatch Stopwatch { get; set; }
    private double PreviousTime { get; set; }
    private double BallPositionX { get; set; }
    private double BallPositionY { get; set; }
    private double BallSpeedX { get; set; }
    private double BallSpeedY { get; set; }

    private void StartStopwatch()
    {
        Stopwatch = new Stopwatch();
        Stopwatch.Restart();
        PreviousTime = 0;
    }

    private void LoadLevel()
    {
        StartStopwatch();
        LoadBlocks();
        ConfigureElements();
    }
    

    private void LoadBlocks()
    {
        foreach (UIElement child in Canvas.Children.Cast<UIElement>().ToList())
        {
            if(child is Rectangle {Tag : int} rectangle)
            {
                Canvas.Children.Remove(child);
            }
        }
        foreach (var blockConfig in instance.CurrentLevel.BlockConfigurations)
        {
            instance.CurrentLevel.BlockConfigurations.ForEach(b => b.Block.RestoreHealth());
            var block = blockConfig.Block;

            var blockRectangle = new Rectangle
            {
                Width = 50,
                Height = 30,
                Tag = LevelSingleton.Instance.CurrentLevel.BlockConfigurations.IndexOf(blockConfig)
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
        PreviousTime = currentTime;
        
        MovePlatform(deltaTime);
        MoveBall(deltaTime);

    }
    private void HitTestBounce()
    {
        var ballBounds = new Rect(BallPositionX, BallPositionY, BallHeight, BallHeight);

        var ballGeometry = new EllipseGeometry(ballBounds);
        var hitTestParams = new GeometryHitTestParameters(ballGeometry);

        HitTestResultCallback hitTestCallback = HitTestCallback;

        VisualTreeHelper.HitTest(Canvas, null, hitTestCallback, hitTestParams);
    }



    private void CheckCollisionWithWall(HitTestResult result)
    {
        if (result.VisualHit is Rectangle { Tag: "base" } rectangle)
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
                    var res = MessageBox.Show("Начать заново?", "Вы проиграли.", MessageBoxButton.YesNo);
                    if (res == MessageBoxResult.Yes)
                    {
                        LoadLevel();
                    }
                    else
                    {
                        Application.Current.Shutdown();
                    }

                    break;
                case "Platform":
                    BallSpeedY *= -1;
                    break;
            }
    }

    private void CheckCollisionWithBlocks(HitTestResult result)
    {
        if (result.VisualHit is Rectangle { Tag: int } hitBlock)
        {
            if (Canvas.GetTop(hitBlock) + 30 <= Canvas.GetTop(Ball) ||
                Canvas.GetTop(hitBlock) >= Canvas.GetTop(Ball) + 20)
                BallSpeedY *= -1;
            else
                BallSpeedX *= -1;

            if (instance.BlockDictionary.TryGetValue(hitBlock, out var block))
            {
                block.Hit();
                if (block.IsDestroyed())
                {
                    Canvas.Children.Remove(hitBlock);
                    //instance.BlockDictionary.Remove(hitBlock);
                }

                if (block.Type == BlockType.Strong && !block.IsDestroyed())
                    hitBlock.Fill = new SolidColorBrush(Colors.DodgerBlue);
            }
        }
    }

    private HitTestResultBehavior HitTestCallback(HitTestResult result)
    {
        CheckCollisionWithWall(result);
        CheckCollisionWithBlocks(result);

        return HitTestResultBehavior.Continue;
    }
}