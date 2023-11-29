using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows;
using GameEntitiesLibrary;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;

namespace Arkanoid;

public partial class StartWindow : Window
{
    private readonly IServiceProvider _serviceProvider;

    public StartWindow(IServiceProvider serviceProvider)
    {
        InitializeComponent();
        _serviceProvider = serviceProvider;
        UpdateRecords();
    }

    private void UpdateRecords()
    {
        var fileName = "records.json";
        List<Record> records;

        if (File.Exists(fileName))
        {
            var json = File.ReadAllText(fileName);
            records = JsonConvert.DeserializeObject<List<Record>>(json) ?? new List<Record>();

            if (records.Count > 0)
            {
                foreach (var record in records)
                {
                    RecordsListViwe.Items.Add(record);
                }
            }
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
}