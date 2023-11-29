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

    public GameWindow(IServiceProvider serviceProvider)
    {
        InitializeComponent();
        _serviceProvider = serviceProvider;
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

        var levelState = _serviceProvider.GetRequiredService<LevelState>();
        foreach (var blockConfig in levelState.CurrentLevel?.BlockConfigurations!)
        {
            levelState.CurrentLevel.BlockConfigurations.ForEach(b => b.Block.RestoreHealth());
            var block = blockConfig.Block;

            var blockRectangle = new Rectangle
            {
                Width = 50,
                Height = 30,
                Tag = levelState.CurrentLevel.BlockConfigurations.IndexOf(blockConfig)
            };

            blockRectangle.Fill = block.Type switch
            {
                BlockType.Ordinary => OrdinaryBlockColor,
                BlockType.Strong => StrongBlockColor,
                BlockType.Bonus => BonusBlockColor,
                _ => blockRectangle.Fill
            };

            levelState.BlockDictionary[blockRectangle] = blockConfig.Block;

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

        HitTestBounce();

        Canvas.SetLeft(Ball, BallPositionX);
        Canvas.SetTop(Ball, BallPositionY);
    }

    private void CompositionTarget_Rendering(object? sender, EventArgs e)
    {
        var currentTime = Stopwatch!.Elapsed.TotalSeconds;
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
                    var res = MessageBox.Show("Начать заново?", "Вы проиграли.", MessageBoxButton.YesNo);
                    if (res == MessageBoxResult.Yes)
                        LoadLevel();
                    else
                        FinishLevel();
                    break;
                case "Platform":
                    CalculateAngle(rectangle, out var ballSpeedXNew, out var ballSpeedYNew);
                    BallSpeedX = ballSpeedXNew;
                    BallSpeedY = ballSpeedYNew;
                    Console.WriteLine(ballSpeedXNew + " " + ballSpeedYNew);
                    break;
            }
    }


    private void CheckCollisionWithBlocks(HitTestResult result)
    {
        if (result.VisualHit is Rectangle { Tag: int } hitBlock)
        {
            if (Canvas.GetTop(hitBlock) + 30 <= Canvas.GetTop(Ball) ||
                Canvas.GetTop(hitBlock) >= Canvas.GetTop(Ball) + BallHeight)
                BallSpeedY *= -1;
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
                BallPositionY = DefaultBallPositionY;
                var res = MessageBox.Show("Начать заново?", "Вы молодец!!", MessageBoxButton.YesNo);
                if (res == MessageBoxResult.Yes)
                    LoadLevel();
                else
                    FinishLevel(true);
            }
        }
    }

    private HitTestResultBehavior HitTestCallback(HitTestResult result)
    {
        CheckCollisionWithWall(result);
        CheckCollisionWithBlocks(result);

        return HitTestResultBehavior.Continue;
    }

    private void FinishLevel(bool isCompleted = false)
    {
        var levelState = _serviceProvider.GetRequiredService<LevelState>();
        var levels = _serviceProvider.GetRequiredService<List<Level>>();


        if (isCompleted)
        {
            if (levels.IndexOf(levelState.CurrentLevel!)  == levelState.CurrentUser!.LevelNumber)
            {
                levelState.CurrentUser.IncreaseLevel();
                UpdateUsers();
            }

            levelState.CurrentUser.LevelScores[levels.IndexOf(levelState.CurrentLevel!)] =
                levelState.CurrentUser.LevelScores.TryGetValue(levels.IndexOf(levelState.CurrentLevel), out var score)
                    ? Math.Max(score, 100)
                    : 100;
            levelState.CurrentUser.CalculateRecord();
            UpdateRecords();
        }

        UpdateUsers();
        new LevelSelectorWindow(_serviceProvider).Show();
        Close();
    }

    private void UpdateUsers()
    {
        var users = _serviceProvider.GetRequiredService<List<User>>();
        var fileName = "users.json";
        var serializedUsers = JsonConvert.SerializeObject(users);
        File.WriteAllText(fileName, serializedUsers);
    }

    private void UpdateRecords()
    {
        var levelState = _serviceProvider.GetRequiredService<LevelState>();
        var fileName = "records.json";
        List<Record> records;

        try
        {
            var json = File.ReadAllText(fileName);
            records = JsonConvert.DeserializeObject<List<Record>>(json) ?? new List<Record>();
        }
        catch (Exception)
        {
            records = new List<Record>();
        }

        var userRecord = records.FirstOrDefault(r => r.PlayerName == levelState.CurrentUser?.Name);
        if (userRecord == null)
        {
            userRecord = new Record
                { PlayerName = levelState.CurrentUser?.Name!, RecordValue = levelState.CurrentUser?.Record };
            records.Add(userRecord);
        }
        else
        {
            userRecord.RecordValue = levelState.CurrentUser?.Record!;
        }

        records = records.OrderByDescending(r => r.RecordValue).ToList();

        var serializedRecords = JsonConvert.SerializeObject(records);
        File.WriteAllText(fileName, serializedRecords);
    }

    private void UpdateUser()
    {
        var levelState = _serviceProvider.GetRequiredService<LevelState>();
        var levels = _serviceProvider.GetRequiredService<List<Level>>();
        var users = _serviceProvider.GetRequiredService<List<User>>();
        
        var serializedUsers = JsonConvert.SerializeObject(users);
    }
}