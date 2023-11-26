using System;
using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
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
        BallRadius = BallHeight / 2;
        BallPositionX = Canvas.GetLeft(Ball);
        BallPositionY = Canvas.GetBottom(Ball) + BallHeight;
        BallSpeedX = -1;
        BallSpeedY = -2;
        BallSpeed = 200;
        CompositionTarget.Rendering += CompositionTarget_Rendering;
    }

    private void CompositionTarget_Rendering(object? sender, EventArgs e)
    {
        var currentTime = Stopwatch.Elapsed.TotalSeconds;
        var deltaTime = currentTime - PreviousTime;

        BallPositionX += BallSpeedX * deltaTime * BallSpeed;
        BallPositionY += BallSpeedY * deltaTime * BallSpeed;

        Canvas.SetLeft(Ball, BallPositionX);
        Canvas.SetBottom(Ball, BallPositionY);

        var ballBounds = new Rect(BallPositionX, BallPositionY, BallHeight, BallHeight);
        var platformBounds = new Rect(PlatformPositionX, Canvas.GetBottom(Platform), PlatformWidth,
            PlatformHeight);
        

        if (ballBounds.IntersectsWith(platformBounds))
        {
            // // Calculate the collision point on the platform
            // double collisionPointX = BallPositionX + BallHeight / 2;
            // double platformCenterX = PlatformPositionX + PlatformWidth / 2;
            // double distanceFromCenter = collisionPointX - platformCenterX;
            //
            // // Calculate the bounce angle based on the distance from the center
            // double maxDistanceFromCenter = PlatformWidth / 2;
            // double maxBounceAngle = 60; // Maximum bounce angle in degrees
            //
            // double bounceAngle = maxBounceAngle * (distanceFromCenter / maxDistanceFromCenter);
            // double bounceAngleRadians = bounceAngle * (Math.PI / 180); // Convert to radians
            //
            // Console.WriteLine(bounceAngle + " " + bounceAngleRadians);
            //
            // // Calculate the new ball speed and direction based on the bounce angle
            // double ballSpeedMagnitude = Math.Sqrt(BallSpeedX * BallSpeedX + BallSpeedY * BallSpeedY);
            // double ballDirectionRadians = Math.Atan2(BallSpeedY, BallSpeedX);
            //
            // double newBallDirectionRadians = Math.PI + bounceAngleRadians - ballDirectionRadians; // Change the sign of the bounce angle
            // double newBallSpeedX = Math.Cos(newBallDirectionRadians) * ballSpeedMagnitude;
            // double newBallSpeedY = -Math.Sin(newBallDirectionRadians) * ballSpeedMagnitude;
            //
            // // Update the ball speed and direction
            // BallSpeedX = newBallSpeedX;
            // BallSpeedY = newBallSpeedY;

            // double collisionPointX = BallPositionX + BallHeight / 2;
            // double platformCenterX = PlatformPositionX + PlatformWidth / 2;
            // double distanceFromCenter = collisionPointX - platformCenterX;
            //
            // // Calculate the relative collision point (-0.5 to 0.5, where 0 is the center of the platform)
            // double relativeCollisionPoint = distanceFromCenter / (PlatformWidth / 2);
            //
            // // Calculate the bounce angle based on the relative collision point (from -60 to 60 degrees)
            // double bounceAngleDegrees = relativeCollisionPoint * 60;
            // double bounceAngleRadians = bounceAngleDegrees * (Math.PI / 180); // Convert to radians
            //
            // // Calculate the new ball speed and direction based on the bounce angle
            // double ballSpeedMagnitude = Math.Sqrt(BallSpeedX * BallSpeedX + BallSpeedY * BallSpeedY);
            // BallSpeedX = Math.Cos(bounceAngleRadians) * ballSpeedMagnitude;
            // BallSpeedY = -Math.Sin(bounceAngleRadians) * ballSpeedMagnitude;
            BallSpeedY *= -1;
        }

        if (BallPositionX <= 0 || BallPositionX + BallHeight >= Canvas.ActualWidth)
        {
            BallSpeedX *= -1;
        }

        if (BallPositionY <= 0 || BallPositionY + BallHeight >= Canvas.ActualHeight)
        {
            BallSpeedY *= -1;
        }

        if (Keyboard.IsKeyDown(Key.A))
        {
            if (PlatformPositionX - PlatformSpeed * deltaTime < 1)
            {
                PlatformPositionX = 1;
            }
            else
            {
                PlatformPositionX -= PlatformSpeed * deltaTime;
            }
        }
        else if (Keyboard.IsKeyDown(Key.D))
        {
            if (PlatformPositionX + PlatformWidth + PlatformSpeed * deltaTime > 764)
            {
                PlatformPositionX = 764 - PlatformWidth;
            }
            else
            {
                PlatformPositionX += PlatformSpeed * deltaTime;
            }
        }

        AnimatePlatform(PlatformPositionX);

        PreviousTime = currentTime;
    }


    private void AnimatePlatform(double newLeft)
    {
        Canvas.SetLeft(Platform, newLeft);
    }
}