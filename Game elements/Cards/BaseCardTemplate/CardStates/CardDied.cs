using Godot;
using System;

public partial class CardDied : CardState
{
    public override void Enter(BaseCardTemplate card, ref CardState? optionalState)
    {
        GD.Print(card + " died");
        /* 
        if ( has on death abbility )
        {
            do the death abbility
            card.QueueFree();
        } else card.QueueFree();       // proabably should play like a deat hanimation
        */
    }
}
