using Godot;
using System;

public partial class MainMenu : CanvasLayer
{
    [Export] private HBoxContainer? mainMenu;
    [Export] private HBoxContainer? settings;
    [Export] private HBoxContainer? help;

    private void OnPlayButtonPressed()
    {
        GetNode<AudioStreamPlayer>("Click").Play();
        GetTree().ChangeSceneToFile("res://Game elements/GameScene/game_scene.tscn");
    }

    private void OnSettingsButtonPressed()
    {
        GetNode<AudioStreamPlayer>("Click").Play();
        mainMenu?.Hide();
        settings?.Show();
    }

    private void OnBackButtonPressed()
    {
        GetNode<AudioStreamPlayer>("Click").Play();
        settings?.Hide();
        mainMenu?.Show();
    }

    private void OnExitButtonPressed()
    {
        GetNode<AudioStreamPlayer>("Click").Play();
        GetTree().Quit();
    }

    private void OnHelpButtonPressed()
    {
        settings?.Hide();
        help?.Show();
    }

    private void OnHelpBackButtonPressed()
    {
        settings?.Show();
        help?.Hide();
    }
}
