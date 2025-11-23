using Witheringaway.Game_elements.Cards.Units.BaseCardTemplate;

public partial class Reaper : BaseCardTemplate
{
    // before combat/on new turn take away 1 time left on field from a random enemy card

    public override void OnCombatStart(int lane, bool isPlayer)
    {
        var targetCard = FieldData.Instance.RandomCardOnField(!isPlayer, this);
        if (targetCard is null) return;
        
        targetCard.SubtractTimeOnField(1);
    }
}
