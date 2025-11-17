using Godot;
using System;

public partial class CardState 
{
    public virtual void Enter(BaseCardTemplate card, ref CardState? optionalState) { }
    public virtual void Exit(BaseCardTemplate card, ref CardState? optionalState) { }
    public virtual void Update(double delta) { }
}
