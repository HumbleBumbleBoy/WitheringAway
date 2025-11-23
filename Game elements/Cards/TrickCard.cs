using System.Linq;
using Godot;
using Witheringaway.Game_elements.Cards.Units.BaseCardTemplate;
using Witheringaway.Game_elements.lib;
using Witheringaway.Game_elements.lib.manager;

namespace Witheringaway.Game_elements.Cards;

public partial class TrickCard : BaseCardTemplate
{
    [Export] public bool CanPlaceOnFriendlyField;
    [Export] public bool CanPlaceOnEnemyField;
    
    [Export] public bool CanPlaceOnPlayer;
    [Export] public bool CanPlaceOnEnemy;
    
    [Export] public bool IsGeneralEffect;
    
    [Export] public TrickEffect[] TrickEffects = [];

    public override void _Ready()
    {
        base._Ready();

        GetNodeOrNull<Sprite2D>("Trick")?.Show();
    }

    public bool ApplyEffect(Duelist duelist, int round)
    {
        switch (duelist.IsPlayer)
        {
            case true when !CanPlaceOnPlayer:
            case false when !CanPlaceOnEnemy:
                return false;
            default:
            {
                var effect = FindEffect(round);
                effect?.Apply(duelist);
                break;
            }
        }

        return true;
    }
    
    public bool ApplyEffect(BaseCardTemplate card, int round, bool isPlayer)
    {
        switch (isPlayer)
        {
            case true when !CanPlaceOnFriendlyField:
            case false when !CanPlaceOnEnemyField:
                return false;
            default:
            {
                var effect = FindEffect(round);
                effect?.Apply(card);
                break;
            }
        }

        return true;
    }

    public bool ApplyGeneralEffect(int round, bool isPlayer)
    {
        if (!IsGeneralEffect)
        {
            return false;
        }
        
        var effect = FindEffect(round);
        
        var handManager = isPlayer
            ? GetTree().GetFirstNodeInGroup("PlayerHandManager") as HandManager
            : GetTree().GetFirstNodeInGroup("EnemyHandManager") as HandManager;

        for (var i = 0; i < (effect?.ExtraCardsToDraw ?? 0); i++)
        {
            handManager?.GetTopCard();
        }

        return true;
    }

    protected override bool IsValidDropPosition()
    {
        return false; // Trick cards cannot be placed on the field
    }

    protected override void DropOnCard(BaseCardTemplate? card, bool isPlayer, int laneIndex)
    {
        GD.Print("Dropped on card: " + (card?.Name ?? "null"));
        if (card is null)
        {
            base.DropOnCard(card, isPlayer, laneIndex);
            return;
        }

        if (ApplyEffect(card, GetNode<TurnManager>("/root/GameScene/TurnManager").CurrentRound, isPlayer))
        {
            Kill();
            return;
        }
        
        base.DropOnCard(card, isPlayer, laneIndex);
    }

    protected override void DropOnDuelist(Duelist duelist)
    {
        if (ApplyEffect(duelist, GetNode<TurnManager>("/root/GameScene/TurnManager").CurrentRound))
        {
            Kill();
            return;
        }
        
        base.DropOnDuelist(duelist);
    }

    protected override void Drop(bool isPlayer)
    {
        if (ApplyGeneralEffect(GetNode<TurnManager>("/root/GameScene/TurnManager").CurrentRound, isPlayer))
        {
            Kill();
            return;
        }
        
        base.Drop(isPlayer);
    }

    private TrickEffect? FindEffect(int round)
    {
        return TrickEffects.Where(effect => effect.MinRound < round).MaxBy(effect => effect.MinRound);
    }
}