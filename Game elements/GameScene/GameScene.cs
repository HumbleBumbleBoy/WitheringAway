using Godot;
using System;
using System.Linq;

public partial class GameScene : Node2D
{
    [Export] public HBoxContainer? inGameMenu;
    [Export] private TurnManager? turnManager;
    [Export] private Sprite2D? winScreen;

    private PlayerHandManager? playerHandManager;
    private PlayerDeckManager? playerDeckManager;

    public override void _Ready()
    {
        playerHandManager = GetNode<Node>("Player").GetNode<PlayerHandManager>("PlayerHandManager");
        playerDeckManager = GetNode<Node>("Player").GetNode<PlayerDeckManager>("PlayerDeckManager");
    }

    private void OnSettingsButtonPressed()
    {
        GetNode<AudioStreamPlayer>("Click").Play();
        inGameMenu.Visible = !inGameMenu.Visible;  
    }

    private void onDrawCardButtonPressed()
    {
        playerHandManager?.GetTopCard();
    }


    private void OnPassTurnButtonPressed()
    {
        GetNode<AudioStreamPlayer>("Click").Play();
        turnManager?.StateMachine.ChangeState(new EnemyTurn());
    }

    private void OnBackToMainMenuButtonPressed()
    {
        winScreen?.Hide();
        GetTree().ChangeSceneToFile("res://Game elements/UI/Main menu/main_menu.tscn");
    }
}
