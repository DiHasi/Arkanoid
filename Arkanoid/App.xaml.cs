using System.Windows;
using GameEntitiesLibrary;
using Microsoft.Extensions.DependencyInjection;


namespace Arkanoid;

/// <summary>
///     Interaction logic for App.xaml
/// </summary>
public partial class App : Application
{
    protected override void OnStartup(StartupEventArgs e)
    {
        base.OnStartup(e);
        // var serviceProvider = new ServiceCollection()
        //     .AddSingleton<LevelSingleton>(instance)
        //     .BuildServiceProvider();
        // // Создание экземпляра LevelSingleton
        // var levelSingleton = new LevelSingleton();
        //
        // // Регистрация LevelSingleton в ресурсах приложения
        // Current.Resources.Add("LevelSingleton", levelSingleton);
        //
        // // Запуск главного окна
        // var levelSelectorWindow = new LevelSelectorWindow();
        // levelSelectorWindow.Show();
    }
}