using Godot;
using System;

public partial class GameScene : Node2D
{
    private HandManager handManager;
    private DeckManager deckManager;

    public override void _Ready()
    {
        handManager = GetNode<HandManager>("HandManager");
        deckManager = GetNode<DeckManager>("DeckManager");
    }
}
