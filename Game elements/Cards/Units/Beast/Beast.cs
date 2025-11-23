using Witheringaway.Game_elements.Cards.Units.BaseCardTemplate;

public partial class Beast : BaseCardTemplate
{
    // heal for 2 when killing a unit, it has to surive for that buff to happen

    private bool hasKilled;

    public override void OnAttackLanded(BaseCardTemplate? target, bool isPlayer)
    {
        base.OnAttackLanded(target, isPlayer);

        if (target?.GetCurrentHealth() <= 0)
        {
            hasKilled = true;
        }
    }

    public override void OnCombatEnd(int lane, bool isPlayer)
    {
        base.OnCombatEnd(lane, isPlayer);

        if (!hasKilled) return;
        
        Heal(2);
        hasKilled = false;
    }
}
