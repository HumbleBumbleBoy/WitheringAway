using Godot;

namespace Witheringaway.Game_elements.components;

[GlobalClass]
public partial class AttackComponent : Component
{
    [Export] public int AttackDamage { get; private set; }
    [Export] public int AttackCount { get; private set; }

    public void SetAttackDamage(int amount)
    {
        AttackDamage = Mathf.Clamp(amount, 0, int.MaxValue);
    }

    public void SetAttackCount(int amount)
    {
        AttackCount = Mathf.Clamp(amount, 1, int.MaxValue);
    }
}