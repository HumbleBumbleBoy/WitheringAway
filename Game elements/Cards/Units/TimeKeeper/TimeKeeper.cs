using Witheringaway.Game_elements.Cards.Units.BaseCardTemplate;

public partial class TimeKeeper : BaseCardTemplate
{
    // on place, give +2 turns of life to all friendly characters

    public override void OnSelfEnterField(bool isPlayer)
    {
        base.OnSelfEnterField(isPlayer);

        var fieldData = FieldData.Instance;

        for (var lane = 0; lane < 5; lane++)
        {
            var card = fieldData.GetCardOnSpecificLane(lane, isPlayer);
            if (card == null) continue;

            if (!IsInstanceValid(card)) continue;

            card.AddTimeOnField(2);
        }
    }
}
