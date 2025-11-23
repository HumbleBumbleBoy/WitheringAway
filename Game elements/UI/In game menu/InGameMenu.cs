using Godot;

public partial class InGameMenu : HBoxContainer
{
    private void OnBackButtonPressed()
    {
        GetNode<AudioStreamPlayer>("Click").Play();
        Hide();
    }

    private void OnExitButtonPressed()
    {
        GetNode<AudioStreamPlayer>("Click").Play();
        GetTree().Quit();
    }

    private void OnBackToMainMenuButtonPressed()
    {
        GetTree().ChangeSceneToFile("res://Game elements/UI/Main menu/main_menu.tscn");
    }
}
