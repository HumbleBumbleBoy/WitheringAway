using Godot;

[GlobalClass]
public partial class BaseCard : Resource
{
    [Export] public Texture2D? Art { get; set; }
    [Export] public string? Name { get; set; }
    [Export] public string? Description { get; set; }
    [Export] public int TimeLeftOnField { get; set; }
    [Export] public int Cost { get; set; }
    [Export] public int Health { get; set; }
    [Export] public int Defense { get; set; }
    [Export] public int Attack { get; set; }
    [Export] public int HowManyAttacks { get; set; }
}