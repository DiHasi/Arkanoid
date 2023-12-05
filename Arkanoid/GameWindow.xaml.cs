using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Shapes;
using System.Windows.Threading;
using GameEntitiesLibrary;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;

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

    private static readonly SolidColorBrush OrdinaryBlockColor = new(Colors.DarkOrange);
    private static readonly SolidColorBrush StrongBlockColor = new(Colors.MediumOrchid);
    private static readonly SolidColorBrush DamagedStrongBlockColor = new(Colors.DodgerBlue);
    private static readonly SolidColorBrush BonusBlockColor = new(Colors.CornflowerBlue);


    private readonly IServiceProvider _serviceProvider;
    private readonly JsonFacade _jsonFacade;
    private readonly LevelState _levelState;
    private readonly List<Level> _levels;
    private readonly List<User> _users;
    private readonly List<Record> _records;

    public GameWindow(IServiceProvider serviceProvider)
    {
        InitializeComponent();
        _serviceProvider = serviceProvider;
        _levelState = _serviceProvider.GetRequiredService<LevelState>();
        _levels = _serviceProvider.GetRequiredService<List<Level>>();
        _users = _serviceProvider.GetRequiredService<List<User>>();
        _records = _serviceProvider.GetRequiredService<List<Record>>();
        _jsonFacade = _serviceProvider.GetRequiredService<JsonFacade>();
        LoadLevel();
        CompositionTarget.Rendering += CompositionTarget_Rendering;
    }

    private double PlatformPositionX { get; set; }
    private Stopwatch? Stopwatch { get; set; }
    private double PreviousTime { get; set; }
    private double BallPositionX { get; set; }
    private double BallPositionY { get; set; }
    private double BallSpeedX { get; set; }
    private double BallSpeedY { get; set; }

    private bool IsFinished { get; set; } = false;

    private void ConfigureElements()
    {
        PlatformPositionX = DefaultPlatformPositionX;
        BallPositionX = DefaultBallPositionX;
        BallPositionY = DefaultBallPositionY;
        BallSpeedX = DefaultBallSpeedX;
        BallSpeedY = DefaultBallSpeedY;
    }


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
        foreach (var child in Canvas.Children.Cast<UIElement>().ToList())
            if (child is Rectangle { Tag : int })
                Canvas.Children.Remove(child);
        
        foreach (var blockConfig in _levelState.CurrentLevel?.BlockConfigurations!)
        {
            _levelState.CurrentLevel.BlockConfigurations.ForEach(b => b.Block.RestoreHealth());
            var block = blockConfig.Block;

            var blockRectangle = new Rectangle
            {
                Width = 50,
                Height = 30,
                Tag = _levelState.CurrentLevel.BlockConfigurations.IndexOf(blockConfig)
            };

            blockRectangle.Fill = block.Type switch
            {
                BlockType.Ordinary => OrdinaryBlockColor,
                BlockType.Strong => StrongBlockColor,
                BlockType.Bonus => BonusBlockColor,
                _ => blockRectangle.Fill
            };

            _levelState.BlockDictionary[blockRectangle] = blockConfig.Block;

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
        else if (Keyboard.IsKeyDown(Key.R))
        {
            LoadLevel();
        }

        Canvas.SetLeft(Platform, PlatformPositionX);
    }

    private void MoveBall(double deltaTime)
    {
        BallPositionX += BallSpeedX * deltaTime * BallSpeed;
        BallPositionY += BallSpeedY * deltaTime * BallSpeed;
        
        // Проверка границ
        if (BallPositionX < 10) // Левая граница
        {
            BallPositionX = 10;
            BallSpeedX *= -1;
        }
        else if (BallPositionX + BallHeight > 774) // Правая граница
        {
            BallPositionX = 774 - BallHeight;
            BallSpeedX *= -1;
        }

        if (BallPositionY < 10) // Верхняя граница
        {
            BallPositionY = 10;
            BallSpeedY *= -1;
        }


        
        HitTestBounce();

        Canvas.SetLeft(Ball, BallPositionX);
        Canvas.SetTop(Ball, BallPositionY);

    }

    private bool IsMessageBoxShown { get; set; }

    private void CompositionTarget_Rendering(object? sender, EventArgs e)
    {
        var currentTime = Stopwatch!.Elapsed.TotalSeconds;
        var deltaTime = currentTime - PreviousTime;
        PreviousTime = currentTime;
        
        ShowMessageBox();
        if(!IsFinished)
            MovePlatform(deltaTime);
        MoveBall(deltaTime);
    }
    
    private void ShowMessageBox(bool isLevelFinished = false)
    {
        if (IsFinished && !IsMessageBoxShown)
        {
            BallSpeedX = 0;
            BallSpeedY = 0;
            
            IsMessageBoxShown = true;
            Dispatcher.BeginInvoke((Action)(() =>
            {
                var res = MessageBox.Show("Начать заново?", "Вы Молодец.", MessageBoxButton.YesNo, MessageBoxImage.Asterisk);
                if (res == MessageBoxResult.Yes)
                {
                    IsMessageBoxShown = false;
                    IsFinished = false;
                    LoadLevel();
                }
                else
                    FinishLevel(isLevelFinished);
            }));
             
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

    private void CalculateAngle(Rectangle platform, out double ballSpeedXNew, out double ballSpeedYNew)
    {
        var platformCenterX = Canvas.GetLeft(platform) + platform.Width / 2;
        var distance = Math.Abs(BallPositionX + BallHeight / 2 - platformCenterX);
        var maxDistance = platform.Width / 2;
        var angle = Math.PI / 2 * (distance / maxDistance);
        var maxAngle = Math.PI / 4;
        angle = Math.Max(-maxAngle, Math.Min(maxAngle, angle));
        if (BallPositionX + BallHeight / 2 < platformCenterX) angle = -angle;

        var ballSpeedMagnitude = Math.Sqrt(BallSpeedX * BallSpeedX + BallSpeedY * BallSpeedY);
        ballSpeedXNew = ballSpeedMagnitude * Math.Sin(angle);
        ballSpeedYNew = -ballSpeedMagnitude * Math.Cos(angle);
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
                    IsFinished = true;
                    break;
                case "Platform":
                    CalculateAngle(rectangle, out var ballSpeedXNew, out var ballSpeedYNew);
                    BallSpeedX = ballSpeedXNew;
                    BallSpeedY = ballSpeedYNew;
                    Console.WriteLine(ballSpeedXNew + " " + ballSpeedYNew);
                    break;
            }
    }


    private bool CheckCollisionWithBlocks(HitTestResult result)
    {
        if (result.VisualHit is Rectangle { Tag: int } hitBlock)
        {
            if (Canvas.GetTop(hitBlock) + 30 <= Canvas.GetTop(Ball) ||
                Canvas.GetTop(hitBlock) >= Canvas.GetTop(Ball) + BallHeight)
            {
                BallSpeedY *= -1;
                Console.WriteLine(Canvas.GetTop(hitBlock) + " " + Canvas.GetTop(Ball) + " " + BallSpeedY + " " + BallSpeedX);
            }
            else
                BallSpeedX *= -1;

            var levelState = _serviceProvider.GetRequiredService<LevelState>();
            if (levelState.BlockDictionary.TryGetValue(hitBlock, out var block))
            {
                block.Hit();
                if (block.IsDestroyed()) Canvas.Children.Remove(hitBlock);
                //instance.BlockDictionary.Remove(hitBlock);
                if (block.Type == BlockType.Strong && !block.IsDestroyed())
                    hitBlock.Fill = DamagedStrongBlockColor;
            }
            
            
            if (!Canvas.Children.OfType<Rectangle>().Any(rect => rect.Tag is int))
            {
                IsFinished = true;
                ShowMessageBox(true);
            }
            
            return true;
        }

        return false;
    }

    private HitTestResultBehavior HitTestCallback(HitTestResult result)
    {
        CheckCollisionWithWall(result);
        if (CheckCollisionWithBlocks(result))
        {
            return HitTestResultBehavior.Stop;
        }

        return HitTestResultBehavior.Continue;
    }

    private void FinishLevel(bool isCompleted = false)
    {
        if (isCompleted)
        {
            if (_levels.IndexOf(_levelState.CurrentLevel!)  == _levelState.CurrentUser!.LevelNumber)
            {
                _levelState.CurrentUser.IncreaseLevel();
                _jsonFacade.UpdateUsers("users.json", _users);
            }

            _levelState.CurrentUser.LevelScores[_levels.IndexOf(_levelState.CurrentLevel!)] =
                _levelState.CurrentUser.LevelScores.TryGetValue(_levels.IndexOf(_levelState.CurrentLevel!), out var score)
                    ? Math.Max(score, 100)
                    : 100;
            _levelState.CurrentUser.CalculateRecord();
            _jsonFacade.UpdateRecords("records.json", _records, _levelState.CurrentUser);
        }

        _jsonFacade.UpdateUsers("users.json", _users);
        new LevelSelectorWindow(_serviceProvider).Show();
        Close();
    }
}