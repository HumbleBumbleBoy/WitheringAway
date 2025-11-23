using System.Collections.Generic;
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

    [Export] public bool CanPlaceOnEnemy;
    [Export] public bool CanPlaceOnPlayer;

    [Export] public TrickEffect[] TrickEffects = [];
    
    public void ApplyEffect(Duelist duelist, int round)
    {
        switch (duelist.IsPlayer)
        {
            case true when !CanPlaceOnPlayer:
            case false when !CanPlaceOnEnemy:
                return;
            default:
            {
                var effect = FindEffect(round);
                effect?.Apply(duelist);
                break;
            }
        }
    }
    
    public void ApplyEffect(BaseCardTemplate card, int round, bool isPlayer)
    {
        switch (isPlayer)
        {
            case true when !CanPlaceOnFriendlyField:
            case false when !CanPlaceOnEnemyField:
                return;
            default:
            {
                var effect = FindEffect(round);
                effect?.Apply(card);
                break;
            }
        }
    }

    public void ApplyGeneralEffect(int round, bool isPlayer)
    {
        var effect = FindEffect(round);
        
        var handManager = isPlayer
            ? GetTree().GetFirstNodeInGroup("PlayerHandManager") as HandManager
            : GetTree().GetFirstNodeInGroup("EnemyHandManager") as HandManager;

        for (var i = 0; i < (effect?.ExtraCardsToDraw ?? 0); i++)
        {
            handManager?.GetTopCard();
        }
    }

    protected override void DropOnCard(BaseCardTemplate? card, bool isPlayer, int laneIndex)
    {
        if (card is null)
        {
            base.DropOnCard(card, isPlayer, laneIndex);
            return;
        }
        
        ApplyEffect(card, GetNode<TurnManager>("/root/GameScene/TurnManager").CurrentRound, isPlayer);
        
        Kill();
    }

    protected override void DropOnDuelist(Duelist duelist)
    {
        ApplyEffect(duelist, GetNode<TurnManager>("/root/GameScene/TurnManager").CurrentRound);

        Kill();
    }

    protected override void Drop(bool isPlayer)
    {
        ApplyGeneralEffect(GetNode<TurnManager>("/root/GameScene/TurnManager").CurrentRound, isPlayer);

        Kill();
    }

    private TrickEffect? FindEffect(int round)
    {
        return TrickEffects.Where(effect => effect.MinRound < round).MaxBy(effect => effect.MinRound);
    }
}