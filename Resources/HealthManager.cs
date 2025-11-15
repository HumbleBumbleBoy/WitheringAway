using Godot;
using System;

public partial class HealthManager : Node
{
    public int Health;
    public int Defense;
    public int CurrentHealth;
    public int CurrentDefense;
    public int TimeLeftOnField;

    public void Initialize()
    {
        CurrentHealth = Health;
        CurrentDefense = Defense;
    }
}
