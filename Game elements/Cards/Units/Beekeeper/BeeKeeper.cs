using Godot;

using Witheringaway.Game_elements.Cards.Units.BaseCardTemplate;

public partial class BeeKeeper : BaseCardTemplate
{
    // on enter spawn 2 bee swarms in random spots

    public override void OnSelfEnterField(bool isPlayer)
    {
        base.OnSelfEnterField(isPlayer);

        var beeSwarm = GD.Load<PackedScene>("res://Game elements/Cards/Units/BeeSwarm/bee_swarm.tscn");
        FieldData.Instance.SpawnOnRandomLane(beeSwarm.Instantiate<BeeSwarm>(), isPlayer);
        FieldData.Instance.SpawnOnRandomLane(beeSwarm.Instantiate<BeeSwarm>(), isPlayer);
    }
}
