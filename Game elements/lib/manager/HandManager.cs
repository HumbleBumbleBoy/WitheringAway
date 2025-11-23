using Godot;

namespace Witheringaway.Game_elements.lib.manager;

public partial class HandManager : Control
{
    
    public virtual bool HasMoreCards()
    {
        return false;
    }
    
    public virtual void GetTopCard()
    {
        
    }
    
    public virtual void RemoveCardFromHand(Cards.BaseCardTemplate.BaseCardTemplate card)
    {
        
    }

    public virtual Cards.BaseCardTemplate.BaseCardTemplate? FindCard(int availableSouls)
    {
        return null;
    }
    
}