using System;
using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Media3D;
using System.Windows.Shapes;

namespace Arkanoid;

public partial class GameWindow : Window
{
    private const double PlatformSpeed = 500;
    private double DefaultPlatformPositionX { get; set; } = 350;
    private double PlatformWidth { get; set; } = 100;
    private double PlatformHeight { get; set; } = 20;
    private Stopwatch Stopwatch { get; set; }
    private double PreviousTime { get; set; }
    private double PlatformPositionX { get; set; }

    private double BallSpeed { get; set; }
    private double BallHeight { get; set; }
    private double BallPositionX { get; set; }
    private double BallPositionY { get; set; }

    private double BallRadius { get; set; }
    private double BallPositionCenter { get; set; }
    private double BallSpeedX { get; set; }
    private double BallSpeedY { get; set; }


    public GameWindow()
    {
        InitializeComponent();
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

    private void CompositionTarget_Rendering(object? sender, EventArgs e)
    {
        var currentTime = Stopwatch.Elapsed.TotalSeconds;
        var deltaTime = currentTime - PreviousTime;

        BallPositionX += BallSpeedX * deltaTime * BallSpeed;
        BallPositionY += BallSpeedY * deltaTime * BallSpeed;

        Canvas.SetLeft(Ball, BallPositionX);
        Canvas.SetTop(Ball, BallPositionY);

        var ballBounds = new Rect(BallPositionX, BallPositionY, BallHeight, BallHeight);
        
        var ballGeometry = new EllipseGeometry(ballBounds);
        var hitTestParams = new GeometryHitTestParameters(ballGeometry);
        
        HitTestResultCallback hitTestCallback = HitTestCallback;
        
        VisualTreeHelper.HitTest(Canvas, null, hitTestCallback, hitTestParams);
        

        if (Keyboard.IsKeyDown(Key.A))
        {
            if (PlatformPositionX - PlatformSpeed * deltaTime < 10)
            {
                PlatformPositionX = 10;
            }
            else
            {
                PlatformPositionX -= PlatformSpeed * deltaTime;
            }
        }
        else if (Keyboard.IsKeyDown(Key.D))
        {
            if (PlatformPositionX + PlatformWidth + PlatformSpeed * deltaTime > 774)
            {
                PlatformPositionX = 774 - PlatformWidth;
            }
            else
            {
                PlatformPositionX += PlatformSpeed * deltaTime;
            }
        }

        AnimatePlatform(PlatformPositionX);

        PreviousTime = currentTime;
    }

    
    private HitTestResultBehavior HitTestCallback(HitTestResult result)
    {
        if (result.VisualHit is Rectangle wall)
        {
            if (wall.Name == "LeftWall")
            {
                BallSpeedX *= -1;
            }
            else if (wall.Name == "RightWall")
            {
                BallSpeedX *= -1;
            }
            else if (wall.Name == "TopWall")
            {
                BallSpeedY *= -1;
            }
            else if (wall.Name == "BottomWall")
            {
                BallSpeedY = 0;
                BallSpeedX = 0;
            }
            else if (wall.Name == "Platform")
            {
                BallSpeedY *= -1;
            }
        }
        return HitTestResultBehavior.Continue;
    }

    private void AnimatePlatform(double newLeft)
    {
        Canvas.SetLeft(Platform, newLeft);
    }
}