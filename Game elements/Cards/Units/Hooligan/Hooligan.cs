using Witheringaway.Game_elements.Cards.Units.BaseCardTemplate;

public partial class Hooligan : BaseCardTemplate
{
    
    // +1/+1 for every other hooligan on field

    public override void OnSelfEnterField(bool isPlayer)
    {
        base.OnSelfEnterField(isPlayer);

        foreach (var card in FieldData.Instance.GetCardsOnField(isPlayer))
        {
            if (card == this) continue;
            if (card is not Hooligan) continue;
            
            TempBuffAttackDamage(1);
            TempBuffHealth(1);
        }
    }

    public override void OnFriendlyEnterField(BaseCardTemplate card, int laneIndex)
    {
        base.OnFriendlyEnterField(card, laneIndex);

        if (card == this) return;
        if (card is not Hooligan) return;
        
        TempBuffAttackDamage(1);
        TempBuffHealth(1);
    }

    public override void OnFriendlyExitField(BaseCardTemplate card, int laneIndex)
    {
        base.OnFriendlyExitField(card, laneIndex);

        if (card == this) return;
        if (card is not Hooligan) return;
        
        TempDebuffAttackDamage(1);
        TempDebuffHealth(1);
    }
}
