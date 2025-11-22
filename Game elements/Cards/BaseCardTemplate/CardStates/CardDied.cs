using System.Text.RegularExpressions;
using Godot;
using Witheringaway.Game_elements.lib;

public class CardDied : IState<BaseCardTemplate>
{
    public IState<BaseCardTemplate>? OnEnter(BaseCardTemplate card, IState<BaseCardTemplate>? previousState)
    {
        GD.Print(card.Name + " died");
        
        FieldData fieldData = card.GetNode<FieldData>("/root/GameScene/FieldData");
        string nameOfPosition = card.GetParent().Name;
        string positionNumber = Regex.Replace(nameOfPosition, @"[^\d]", "");
        int indexOfLane = int.Parse(positionNumber) - 1;
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

        return null;
    }
}