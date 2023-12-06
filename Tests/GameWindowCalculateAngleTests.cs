using System.Windows.Controls;
using System.Windows.Shapes;
using Arkanoid;

namespace Tests;

public class GameWindowCalculateAngleTests
{
    
    [StaFact]
    public void CalculateAngle_WhenBallPositionXIsLessThanPlatformCenterX_CalculatesNegativeAngle()
    {
        var gameWindow = new GameWindow();

        var platform = new Rectangle();
        Canvas.SetLeft(platform, 100);
        Canvas.SetTop(platform, 100);
        platform.Width = 200;

        const double ballHeight = 20; 
        const double ballSpeedX = 1;
        const double ballSpeedY = 1;

        const double ballPositionX = 150; 

        gameWindow.CalculateAngle(platform, ballPositionX, ballHeight, ballSpeedX, ballSpeedY, out var ballSpeedXNew, out var ballSpeedYNew);

        Assert.True(ballSpeedXNew < 0);
        Assert.True(ballSpeedYNew < 0);
    }
    [StaFact]
    public void CalculateAngle_WhenBallPositionXIsGreaterThanPlatformCenterX_CalculatesPositiveAngle()
    {
        var gameWindow = new GameWindow();

        var platform = new Rectangle();
        Canvas.SetLeft(platform, 100);
        Canvas.SetTop(platform, 100);
        platform.Width = 200;

        const double ballHeight = 20; 
        const double ballSpeedX = 1;
        const double ballSpeedY = 1;

        const double ballPositionX = 250; 

        gameWindow.CalculateAngle(platform, ballPositionX, ballHeight, ballSpeedX, ballSpeedY, out var ballSpeedXNew, out var ballSpeedYNew);
        
        Assert.True(ballSpeedXNew > 0);
        Assert.True(ballSpeedYNew < 0);
    }
    
    [StaFact]
    public void CalculateAngle_WhenBallPositionXIsEqualToPlatformCenterX_CalculatesZeroAngle()
    {
        var gameWindow = new GameWindow();

        var platform = new Rectangle();
        Canvas.SetLeft(platform, 100);
        Canvas.SetTop(platform, 100);
        platform.Width = 200;

        const double ballHeight = 20;
        const double ballSpeedX = 1;
        const double ballSpeedY = 1;

        const double ballPositionX = 190; 

        gameWindow.CalculateAngle(platform, ballPositionX, ballHeight, ballSpeedX, ballSpeedY, out var ballSpeedXNew, out var ballSpeedYNew);

        Assert.Equal(0, ballSpeedXNew); 
        Assert.True(ballSpeedYNew < 0);
    }
}