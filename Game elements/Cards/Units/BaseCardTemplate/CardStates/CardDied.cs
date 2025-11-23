using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Godot;
using Witheringaway.Game_elements.lib;

using Witheringaway.Game_elements.Cards.BaseCardTemplate;

public class CardDied(bool isPlayer) : IState<BaseCardTemplate>
{
    public IState<BaseCardTemplate>? OnEnter(BaseCardTemplate card, IState<BaseCardTemplate>? previousState)
    {
        GD.Print(card.Name + " died");
        
        FieldData fieldData = card.GetNode<FieldData>("/root/GameScene/FieldData");
        string nameOfPosition = card.GetParent().Name;
        string positionNumber = Regex.Replace(nameOfPosition, @"[^\d]", "");
        int indexOfLane = int.Parse(positionNumber) - 1;
        fieldData.RemoveCardOnSpecificLane(indexOfLane, isPlayer);
        
        DeathSequence(card);

        /*
    if ( has on death abbility )
    {
        do the death abbility
        card.QueueFree();
    } else card.QueueFree();       // proabably should play like a deat hanimation
    */

        return null;
    }
    
    private static async void DeathSequence(BaseCardTemplate card)
    {
        card.Wait(0.2f).ContinueWith(_ => card.CallDeferred(nameof(BaseCardTemplate.DisableArt)));

        await Task.WhenAll(
            card.PlayAnimation("Dying", 0.2f),
            card.PlaySound("Death")
        );
        
        card.GetParent().RemoveChild(card);
        card.QueueFree();
    }
}