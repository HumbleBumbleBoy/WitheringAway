using System;
using Witheringaway.Game_elements.Cards.Units.BaseCardTemplate;

public partial class Surgeon : BaseCardTemplate
{
    // heal RANDOM FRIENDLY for 3HP on enter field, but take 1ATK from that unit

    public override void OnSelfEnterField(bool isPlayer)
    {
        base.OnSelfEnterField(isPlayer);

        var targetCard = FieldData.Instance.RandomCardOnField(isPlayer, this);

        targetCard!.Heal(3);
        targetCard.DebuffAttackDamage(1);
    }
}
