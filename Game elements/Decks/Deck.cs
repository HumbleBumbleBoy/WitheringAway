using Godot;

[GlobalClass]
public partial class Deck : Resource
{
    [Export] public PackedScene[] Cards = [];
}
