using Witheringaway.Game_elements.Cards.Units.BaseCardTemplate;

public partial class Drunkyard : BaseCardTemplate
{
    // +1ATK every time it deals damage and survives

    public override void OnAttackLanded(BaseCardTemplate? target, bool isPlayer)
    {
        base.OnAttackLanded(target, isPlayer);
        
        BuffAttackDamage(1);
    }
}
