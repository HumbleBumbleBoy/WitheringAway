using Godot;

public partial class EnemyDeckManager : Node
{
    [Export] private Godot.Collections.Array<PackedScene> cardScenes = new();
    private EnemyHandManager handManager;

    public override void _Ready()
    {
        handManager = GetParent().GetNode<EnemyHandManager>("EnemyHandManager");

        foreach (PackedScene cardScene in cardScenes)
        {
            handManager.AddCardToHand(cardScene);
        }
    }
}