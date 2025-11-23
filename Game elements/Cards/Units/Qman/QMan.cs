using Godot;

public partial class QMan : Witheringaway.Game_elements.Cards.Units.BaseCardTemplate.BaseCardTemplate
{
    // choose a lane to spawn a bird in (2HP/3ATK)

    public override void OnSelfEnterField(bool isPlayer)
    {
        base.OnSelfEnterField(isPlayer);
        
        var beeSwarm = GD.Load<PackedScene>("res://Game elements/Cards/Units/Bird/bird.tscn");
        FieldData.Instance.SpawnOnRandomLane(beeSwarm.Instantiate<Bird>(), isPlayer);
    }
}
