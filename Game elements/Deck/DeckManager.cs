using Godot;

public partial class DeckManager : Node
{
    [Export] private Godot.Collections.Array<PackedScene> cardScenes = new();
    private HandManager handManager;

    public override void _Ready()
    {
        handManager = GetParent().GetNode<HandManager>("HandManager");

        foreach (PackedScene cardScene in cardScenes)
        {
            handManager.AddCardToHand(cardScene);
            GD.Print("Card added");
        }
    }
}