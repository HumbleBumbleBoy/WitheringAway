using Godot;
using System;

public partial class CardDied : CardState
{
    public override void Enter(BaseCardTemplate card, ref CardState? optionalState)
    {
        GD.Print(card.Name + " died");
        
        FieldData fieldData = card.GetNode<FieldData>("/root/GameScene/FieldData");
        string nameOfPosition = card.GetParent().Name;
        string positionNumber = System.Text.RegularExpressions.Regex.Replace(nameOfPosition, @"[^\d]", "");
        int indexOfLane = int.Parse(positionNumber)-1;
        fieldData.removeCardOnSpecificLane(indexOfLane, true);

        card.GetParent().RemoveChild(card);
        card.QueueFree();
        /* 
        if ( has on death abbility )
        {
            do the death abbility
            card.QueueFree();
        } else card.QueueFree();       // proabably should play like a deat hanimation
        */
    }
}
