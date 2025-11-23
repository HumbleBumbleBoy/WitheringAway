using Witheringaway.Game_elements.Cards.Units.BaseCardTemplate;

public partial class Baker : BaseCardTemplate
{
    // on end combat give +1/+1 to a random friendly unit

    public override void OnCombatEnd(int lane, bool isPlayer)
    {
        base.OnCombatEnd(lane, isPlayer);
        
        var targetCard = FieldData.Instance.RandomCardOnField(isPlayer, this);
        targetCard!.BuffAttackDamage(1);
        targetCard.BuffHealth(1);
    }
}
