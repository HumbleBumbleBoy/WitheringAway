using Godot;
using System;
using System.Linq;

public partial class GameScene : Node2D
{
    [Export] public HBoxContainer? inGameMenu;
    [Export] private TurnManager? turnManager;

    private EnemyHandManager? playerHandManager;
    private PlayerDeckManager? playerDeckManager;

    public override void _Ready()
    {
        playerHandManager = GetNode<Node>("Player").GetNode<EnemyHandManager>("PlayerHandManager");
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

    private void onSpawnDeckButtonPressed()
    {
        int randomDeckIndex = new Random().Next(0, playerDeckManager?.Decks.Length??0);
        PackedScene[] selectedDeckCards = playerDeckManager?.Decks[randomDeckIndex].Cards??[];
        
        foreach (PackedScene packedScene in selectedDeckCards)
        {
            playerDeckManager?.addCardToDeck(packedScene);
            playerDeckManager?.addCardToDeckVisually(packedScene);
        }
    }

    private void onKillCardOnFieldButton()
    {
        FieldData fieldData = GetNode<FieldData>("/root/GameScene/FieldData");
    
        // Get all non-null cards on field
        var aliveCards = fieldData.playerCardsOnField.Where(card => card != null).ToList();
        
        if (aliveCards.Count > 0)
        {
            // Pick a random card and kill it
            var randomCard = aliveCards[new Random().Next(0, aliveCards.Count)];
            randomCard?.ChangeState(new CardDied());
            GD.Print("Killed: " + randomCard?.Name);
        }
        else
        {
            GD.Print("No cards on field to kill");
        }
    }

    private void OnPassTurnButtonPressed()
    {
        GetNode<AudioStreamPlayer>("Click").Play();
        turnManager?.ChangeState(new EnemyTurn());
    }
}
