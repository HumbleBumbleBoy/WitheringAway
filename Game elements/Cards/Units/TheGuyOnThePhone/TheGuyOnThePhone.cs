using Witheringaway.Game_elements.Cards.Units.BaseCardTemplate;
using Witheringaway.Game_elements.lib.manager;

public partial class TheGuyOnThePhone : BaseCardTemplate
{
    // on successful attack, draw a card

    public override void OnAttackLanded(BaseCardTemplate? target, bool isPlayer)
    {
        base.OnAttackLanded(target, isPlayer);

        var handManager = HandManager.GetHandManager(isPlayer);
        handManager.GetTopCard();
    }
    
}
