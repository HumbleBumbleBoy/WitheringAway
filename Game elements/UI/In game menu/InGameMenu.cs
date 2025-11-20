using Godot;
using System;

public partial class InGameMenu : HBoxContainer
{
    private void OnBackButtonPressed()
    {
        GetNode<AudioStreamPlayer>("Click").Play();
        this.Hide();
    }

    private void OnExitButtonPressed()
    {
        GetNode<AudioStreamPlayer>("Click").Play();
        GetTree().Quit();
    }
}
