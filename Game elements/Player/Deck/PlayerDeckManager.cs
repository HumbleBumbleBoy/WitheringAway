using Godot;

public partial class PlayerDeckManager : Node
{
    [Export] private Godot.Collections.Array<PackedScene> cardScenes = new();
    private PlayerHandManager handManager;

    public override void _Ready()
    {
        handManager = GetParent().GetNode<PlayerHandManager>("PlayerHandManager");

        foreach (PackedScene cardScene in cardScenes)
        {
            handManager.AddCardToHand(cardScene);
            GD.Print("Card added");
        }
    }
}