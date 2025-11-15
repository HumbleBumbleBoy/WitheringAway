using Godot;
using System;

public partial class AttackManager : Node
{
    public int Attack;
    public int HowManyAttacks;
    public int CurrentAttack;
    public int CurrentHowManyAttacks;

    public void Initialize()
    {
        CurrentAttack = Attack;
        CurrentHowManyAttacks = HowManyAttacks;
    }
}
