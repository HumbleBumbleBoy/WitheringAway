using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Godot;
using Witheringaway.Game_elements.lib;

using Witheringaway.Game_elements.Cards.Units.BaseCardTemplate;

public class CardDied(bool isPlayer) : IState<BaseCardTemplate>
{
    public IState<BaseCardTemplate>? OnEnter(BaseCardTemplate card, IState<BaseCardTemplate>? previousState)
    {
        GD.Print(card.Name + " died");
        
        var fieldData = FieldData.Instance;
        string nameOfPosition = card.GetParent().Name;
        string positionNumber = Regex.Replace(nameOfPosition, @"[^\d]", "");
        int indexOfLane = int.Parse(positionNumber) - 1;
        fieldData.RemoveCardOnSpecificLane(indexOfLane, isPlayer);
        
        var friendlies = fieldData.GetCardsOnField(isPlayer);
        for (var lane = 0; lane < friendlies.Length; lane++)
        {
            var friendly = friendlies[lane];
            friendly?.OnFriendlyExitField(card, lane);
        }
        
        var enemies = fieldData.GetCardsOnField(!isPlayer);
        for (var lane = 0; lane < enemies.Length; lane++)
        {
            var enemy = enemies[lane];
            enemy?.OnEnemyExitField(card, lane);
        }

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

        await card.Kill();
    }
}