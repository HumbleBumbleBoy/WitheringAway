using Godot;
using System;

public partial class MainMenu : CanvasLayer
{
    [Export] private HBoxContainer? mainMenu;
    [Export] private HBoxContainer? settings;

    private void OnPlayButtonPressed()
    {
        GetTree().ChangeSceneToFile("res://Game elements/GameScene/game_scene.tscn");
    }

    private void OnSettingsButtonPressed()
    {
        mainMenu?.Hide();
        settings?.Show();
    }

    private void OnBackButtonPressed()
    {
        settings?.Hide();
        mainMenu?.Show();
    }

    private void OnExitButtonPressed()
    {
        GetTree().Quit();
    }
}
