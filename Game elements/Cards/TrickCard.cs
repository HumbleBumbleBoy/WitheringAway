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

    private bool _shouldKeepDragging = true;

    protected override void CheckAppearance()
    {
        base.CheckAppearance();

        if (IsCardInField || GetCurrentHealth() <= 0)
        {
            return;
        }
        
        if (IsFlipped)
        {
            GetNodeOrNull<Sprite2D>("Trick")?.Hide();   
        }
        else
        {
            GetNodeOrNull<Sprite2D>("Trick")?.Show();
        }
    }

    public override bool ShouldKeepDragging()
    {
        return _shouldKeepDragging;
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
        if (Duelist.GetDuelist(isPlayer).CurrentSouls < GetCost(isPlayer) || card is null)
        {
            base.DropOnCard(card, isPlayer, laneIndex);
            return;
        }

        if (ApplyEffect(card, GetNode<TurnManager>("/root/GameScene/TurnManager").CurrentRound, isPlayer))
        {
            _shouldKeepDragging = false;
            Duelist.GetDuelist(isPlayer).TakeSouls(GetCost(isPlayer));
            Kill();
            return;
        }
        
        base.DropOnCard(card, isPlayer, laneIndex);
    }

    protected override void DropOnDuelist(Duelist duelist, bool isPlayer)
    {
        if (Duelist.GetDuelist(isPlayer).CurrentSouls < GetCost(isPlayer))
        {
            base.DropOnDuelist(duelist, isPlayer);
            return;
        }
        
        if (ApplyEffect(duelist, GetNode<TurnManager>("/root/GameScene/TurnManager").CurrentRound))
        {
            _shouldKeepDragging = false;
            Duelist.GetDuelist(isPlayer).TakeSouls(GetCost(isPlayer));
            Kill();
            return;
        }
        
        base.DropOnDuelist(duelist, isPlayer);
    }

    protected override void Drop(bool isPlayer)
    {
        GD.Print("Dropped on empty space");
        if (Duelist.GetDuelist(isPlayer).CurrentSouls < GetCost(isPlayer))
        {
            base.Drop(isPlayer);
            return;
        }
        
        if (ApplyGeneralEffect(GetNode<TurnManager>("/root/GameScene/TurnManager").CurrentRound, isPlayer))
        {
            _shouldKeepDragging = false;
            Duelist.GetDuelist(isPlayer).TakeSouls(GetCost(isPlayer));
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