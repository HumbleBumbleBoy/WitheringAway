using System;
using Godot;
using Witheringaway.Game_elements.Cards.Units.BaseCardTemplate;

namespace Witheringaway.Game_elements.lib.manager;

public partial class HandManager : Control
{
    
    public static HandManager PlayerHandManager = null!;
    public static HandManager EnemyHandManager = null!;
    
    public static HandManager GetHandManager(bool isPlayer)
    {
        return isPlayer ? PlayerHandManager : EnemyHandManager;
    }
    
    public virtual bool HasMoreCards()
    {
        return false;
    }
    
    public virtual void GetTopCard()
    {
        
    }
    
    public virtual void RemoveCardFromHand(BaseCardTemplate card)
    {
        
    }

    public virtual BaseCardTemplate? FindCard(int availableSouls, Predicate<BaseCardTemplate>? additionalCondition = null)
    {
        return null;
    }
    
    
}