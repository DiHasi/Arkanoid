using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using GameEntitiesLibrary;
using Microsoft.Extensions.DependencyInjection;

namespace Arkanoid;

public partial class StartWindow
{
    private readonly JsonFacade _jsonFacade;
    private readonly List<Record> _records;
    private readonly IServiceProvider _serviceProvider;

    public StartWindow(IServiceProvider serviceProvider)
    {
        InitializeComponent();
        _serviceProvider = serviceProvider;
        _jsonFacade = _serviceProvider.GetRequiredService<JsonFacade>();
        _records = _serviceProvider.GetRequiredService<List<Record>>();
        LoadRecords();
    }

    private void LoadRecords()
    {
        if (_records.Count <= 0) return;
        {
            foreach (var record in _records)
                RecordsListViwe.Items.Add(record);
        }
    }

    private void Button_Click(object sender, RoutedEventArgs e)
    {
        if (PlayerName.Text == "")
        {
            MessageBox.Show("Введите не пустое имя", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            return;
        }

        var users = _serviceProvider.GetRequiredService<List<User>>();
        var levelState = _serviceProvider.GetRequiredService<LevelState>();
        if (users.Any(u => u.Name == PlayerName.Text))
        {
            levelState.CurrentUser = users.First(u => u.Name == PlayerName.Text);
        }
        else
        {
            users.Add(new User(PlayerName.Text));
            levelState.CurrentUser = users.First(u => u.Name == PlayerName.Text);
        }

        new LevelSelectorWindow(_serviceProvider).Show();
        Close();
    }
    
    private void ListViewItem_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
    {
        if (sender is ListViewItem item && item.DataContext is Record record)
        {
            PlayerName.Text = record.PlayerName;
        }
    }
}